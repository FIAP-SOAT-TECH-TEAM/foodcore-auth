package repository

import (
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/domain"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/exceptions"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/repository"
	"gorm.io/gorm"
	"strings"
)

type userRepositoryGorm struct {
	db *gorm.DB
}

func NewUserRepositoryGorm(db *gorm.DB) repository.UserRepository {
	return &userRepositoryGorm{db}
}

func (r *userRepositoryGorm) Create(user *domain.User) error {
	if err := r.db.Create(user).Error; err != nil {
		if isPostgresUniqueViolation(err) {
			if strings.Contains(err.Error(), "ux_users_document") {
				return exceptions.ErrDocumentAlreadyExists
			}
			if strings.Contains(err.Error(), "ux_users_email") {
				return exceptions.ErrEmailAlreadyExists
			}
		}
		return err
	}
	return nil
}

func (r *userRepositoryGorm) FindByEmail(email string) (*domain.User, error) {
	var user domain.User
	result := r.db.Preload("Role").Where("email = ?", email).First(&user)
	if result.Error != nil {
		return nil, result.Error
	}
	return &user, nil
}

func (r *userRepositoryGorm) FindByCPF(cpf string) (*domain.User, error) {
	var user domain.User
	result := r.db.Preload("Role").Where("document = ?", cpf).First(&user)
	if result.Error != nil {
		return nil, result.Error
	}
	return &user, nil
}

func (r *userRepositoryGorm) FindFirstByGuestTrue() (*domain.User, error) {
	var user domain.User
	result := r.db.Preload("Role").Where("guest = ?", true).First(&user)
	if result.Error != nil {
		return nil, result.Error
	}
	return &user, nil
}

func isPostgresUniqueViolation(err error) bool {
	return strings.Contains(err.Error(), "23505")
}

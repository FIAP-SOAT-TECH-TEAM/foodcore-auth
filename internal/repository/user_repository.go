package repository

import (
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/domain"
)

type UserRepository interface {
	Create(user *domain.User) error
	FindByEmail(email string) (*domain.User, error)
	FindByCPF(cpf string) (*domain.User, error)
	FindFirstByGuestTrue() (*domain.User, error)
}

package usecase

import (
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/domain"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/repository"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/token"
	"golang.org/x/crypto/bcrypt"
)

type AuthUsecase struct {
	repo repository.UserRepository
}

func NewAuthUsecase(r repository.UserRepository) *AuthUsecase {
	return &AuthUsecase{repo: r}
}

func (u *AuthUsecase) Register(name, email, password, role string) (string, *domain.User, error) {
	hashed, _ := bcrypt.GenerateFromPassword([]byte(password), bcrypt.DefaultCost)

	user := &domain.User{Name: name, Email: email, Password: string(hashed), Role: role}
	if err := u.repo.Create(user); err != nil {
		return "", nil, err
	}

	jwt, err := token.GenerateJWT(user.ID, user.Role)
	if err != nil {
		return "", nil, err
	}

	return jwt, user, nil
}

func (u *AuthUsecase) Login(email, password string) (string, *domain.User, error) {
	user, err := u.repo.FindByEmail(email)
	if err != nil {
		return "", nil, err
	}

	if bcrypt.CompareHashAndPassword([]byte(user.Password), []byte(password)) != nil {
		return "", nil, err
	}

	jwt, err := token.GenerateJWT(user.ID, user.Role)
	if err != nil {
		return "", nil, err
	}

	return jwt, user, nil
}

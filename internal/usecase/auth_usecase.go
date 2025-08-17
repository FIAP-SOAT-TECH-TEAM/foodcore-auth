package usecase

import (
	"fmt"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/domain"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/dto"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/repository"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/token"
	"golang.org/x/crypto/bcrypt"
	"gorm.io/gorm"
)

type AuthUsecase struct {
	repo repository.UserRepository
}

func NewAuthUsecase(r repository.UserRepository) *AuthUsecase {
	return &AuthUsecase{repo: r}
}

func (u *AuthUsecase) Register(req dto.RegisterUserInput) (string, *domain.User, error) {
	var existingUser *domain.User
	var err error

	if domain.IsGuest(req.Email, req.Document, req.Guest) {
		existingUser, err = u.repo.FindFirstByGuestTrue()
	} else if req.Document != "" {
		existingUser, err = u.repo.FindByCPF(req.Document)
	} else if req.Email != "" {
		existingUser, err = u.repo.FindByEmail(req.Email)
	}

	// Handle unexpected errors from the repository.
	if err != nil && err != gorm.ErrRecordNotFound {
		return "", nil, err
	}

	// If an existing user was found, perform login and return.
	if existingUser != nil {
		fmt.Println("Usuário já cadastrado, realizando login.")
		jwt, err := token.GenerateJWT(existingUser.ID, existingUser.Role.Name)
		if err != nil {
			return "", nil, err
		}
		return jwt, existingUser, nil
	}

	user, err := domain.NewUserFromInput(
		req.Name,
		req.Email,
		req.Password,
		req.Document,
		req.Guest,
	)
	fmt.Println(user)
	if err := u.repo.Create(user); err != nil {
		return "", nil, err
	}

	jwt, err := token.GenerateJWT(user.ID, user.Role.Name)
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

	jwt, err := token.GenerateJWT(user.ID, user.Role.Name)
	if err != nil {
		return "", nil, err
	}

	return jwt, user, nil
}

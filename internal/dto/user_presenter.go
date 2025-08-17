package dto

import (
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/domain"
	"time"
)

// UserPresenter representa os dados do usuário para a API de resposta.
type UserPresenter struct {
	ID        uint      `json:"id"`
	Name      string    `json:"name"`
	Username  string    `json:"username"`
	Email     string    `json:"email"`
	Document  string    `json:"document"`
	Active    bool      `json:"active"`
	RoleId    *uint     `json:"role_id"`
	Guest     bool      `json:"guest"`
	CreatedAt time.Time `json:"created_at"`
	UpdatedAt time.Time `json:"updated_at"`
}

// AuthPresenter representa a estrutura completa da resposta de login/registro.
type AuthPresenter struct {
	User  *UserPresenter
	Token string `json:"token"`
}

// NewUserPresenter converte a entidade de domínio User para um UserPresenter.
func NewUserPresenter(user *domain.User) *UserPresenter {
	if user == nil {
		return nil
	}

	return &UserPresenter{
		ID:        user.ID,
		Name:      user.Name,
		Username:  user.Username,
		Email:     user.Email,
		Document:  user.Document,
		Active:    user.Active,
		RoleId:    user.RoleID,
		Guest:     user.Guest,
		CreatedAt: user.AuditInfo.CreatedAt,
		UpdatedAt: user.AuditInfo.UpdatedAt,
	}
}

package domain

import (
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/shared"
	"time"
)

// Role representa a entidade de domínio de um papel de usuário.
type Role struct {
	ID          uint   `gorm:"primaryKey"`
	Name        string `gorm:"uniqueIndex"`
	Description string
	AuditInfo   shared.AuditInfo `gorm:"embedded"`
}

// RoleType define os papéis de usuário possíveis.
type RoleType int

const (
	ADMIN RoleType = iota + 1
	USER
	GUEST
)

func AdminRole() *Role {
	return &Role{
		ID:        uint(ADMIN),
		Name:      "ADMIN",
		AuditInfo: shared.AuditInfo{CreatedAt: time.Now()},
	}
}

func DefaultRole() *Role {
	return &Role{
		ID:        uint(USER),
		Name:      "USER",
		AuditInfo: shared.AuditInfo{CreatedAt: time.Now()},
	}
}

// GuestRole cria e retorna uma instância de Role para um usuário convidado.
func GuestRole() *Role {
	return &Role{
		ID:        uint(GUEST),
		Name:      "GUEST",
		AuditInfo: shared.AuditInfo{CreatedAt: time.Now()},
	}
}

// Função de apoio que cria a role com base em um ID.
func getRoleByID(id int) *Role {
	switch RoleType(id) {
	case ADMIN:
		return AdminRole()
	case USER:
		return DefaultRole()
	case GUEST:
		return GuestRole()
	default:
		return DefaultRole()
	}
}

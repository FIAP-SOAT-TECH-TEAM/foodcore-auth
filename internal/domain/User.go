package domain

import (
	"fmt"
	userErrors "github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/exceptions"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/shared"
	"regexp"
	"strings"
	"time"
)

type User struct {
	ID        uint `gorm:"primaryKey"`
	Name      string
	Username  string `gorm:"uniqueIndex"`
	Email     string `gorm:"uniqueIndex"`
	Password  string `json:"-"`
	Document  string
	Active    bool
	Guest     bool
	RoleID    *uint
	Role      *Role
	LastLogin time.Time
	AuditInfo shared.AuditInfo `gorm:"embedded"`
}

// NewUserFromInput cria um novo usuário a partir de um DTO de entrada.
func NewUserFromInput(
	name, email, password, document string,
	guest bool,
) (*User, error) {
	u := &User{
		Name:      name,
		Email:     email,
		Document:  document,
		Active:    true,
		Guest:     IsGuest(email, document, guest),
		AuditInfo: shared.AuditInfo{CreatedAt: time.Now()},
	}

	// Lógica para definir a role com base nos dados.
	if u.Guest {
		u.Role = GuestRole()
	} else {
		// Se não for convidado e nenhum RoleID for fornecido, a role padrão é USER.
		u.Role = DefaultRole()
	}

	// A validação deve ocorrer APÓS a definição dos campos.
	if err := u.ValidateInternalState(); err != nil {
		return nil, err
	}

	return u, nil
}

// isGuest verifica se o usuário deve ser considerado um convidado.
func IsGuest(email, document string, isGuest bool) bool {
	return email == "" || document == "" || isGuest
}

// NewUser é um construtor que valida e cria uma nova instância de User.
func NewUser(guest bool, name, username, email, password, document string, role *Role) (*User, error) {
	u := &User{
		Guest:    guest,
		Name:     name,
		Username: username,
		Email:    email,
		Password: password,
		Document: document,
		Role:     role,
		Active:   true,
	}

	if err := u.ValidateInternalState(); err != nil {
		return nil, err
	}

	return u, nil
}

// ValidateInternalState valida o estado interno do usuário.
func (u *User) ValidateInternalState() error {
	if u.hasDocument() && !u.isValidDocument() {
		return userErrors.ErrInvalidDocument
	}
	if u.hasDocument() && len(u.Document) < 11 {
		return userErrors.ErrDocumentLength
	}
	if u.hasEmail() {
		if !strings.Contains(u.Email, "@") {
			return userErrors.ErrInvalidEmail
		}
		if !u.hasName() {
			return userErrors.ErrNameRequired
		}
	}
	if u.hasUsername() && len(u.Username) < 3 {
		return userErrors.ErrUsernameLength
	}
	if u.hasPassword() && len(u.Password) < 8 {
		return userErrors.ErrPasswordLength
	}
	return nil
}

// isGuest verifica se o usuário é um convidado.
func (u *User) IsGuest() bool {
	return (u.Document == "" || u.Email == "" || u.Username == "") || u.Guest
}

// activate ativa o usuário.
func (u *User) Activate() {
	u.Active = true
}

// deactivate desativa o usuário.
func (u *User) Deactivate() {
	u.Active = false
}

var nonNumericRegex = regexp.MustCompile(`\D`)

// isValidDocument verifica se o documento é válido (implementação simples).
func (u *User) isValidDocument() bool {
	if !u.hasDocument() {
		return false
	}
	numericDocument := nonNumericRegex.ReplaceAllString(u.Document, "")
	fmt.Printf("documento: %s", numericDocument)
	return len(numericDocument) == 11
}

// hasName verifica se o usuário tem um nome.
func (u *User) hasName() bool {
	return u.Name != ""
}

// hasUsername verifica se o usuário tem um username.
func (u *User) hasUsername() bool {
	return u.Username != ""
}

// hasEmail verifica se o usuário tem um email.
func (u *User) hasEmail() bool {
	return u.Email != ""
}

// hasPassword verifica se o usuário tem uma senha.
func (u *User) hasPassword() bool {
	return u.Password != ""
}

// hasDocument verifica se o usuário tem um documento.
func (u *User) hasDocument() bool {
	return u.Document != ""
}

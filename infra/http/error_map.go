package http

import (
	"net/http"

	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/exceptions"
)

// MapDomainError mapeia erros de domínio para códigos de status HTTP.
func MapDomainError(err error) (int, string) {
	switch err {
	// Erros de validação
	case exceptions.ErrInvalidDocument, exceptions.ErrDocumentLength,
		exceptions.ErrInvalidEmail, exceptions.ErrNameRequired,
		exceptions.ErrUsernameLength, exceptions.ErrPasswordLength:
		return http.StatusUnprocessableEntity, err.Error()
	// Erros de unicidade
	case exceptions.ErrDocumentAlreadyExists, exceptions.ErrEmailAlreadyExists:
		return http.StatusConflict, err.Error()
	default:
		// Para erros inesperados, retorna um erro genérico.
		return http.StatusInternalServerError, "um erro inesperado ocorreu"
	}
}

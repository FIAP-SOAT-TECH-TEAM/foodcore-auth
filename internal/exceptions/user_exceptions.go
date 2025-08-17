package exceptions

import "errors"

// UserException é um erro personalizado para a lógica de domínio do usuário.
var (
	ErrInvalidDocument       = errors.New("documento inválido")
	ErrDocumentLength        = errors.New("o documento deve ter pelo menos 11 caracteres")
	ErrInvalidEmail          = errors.New("email inválido")
	ErrNameRequired          = errors.New("nome é obrigatório")
	ErrUsernameLength        = errors.New("o username deve ter pelo menos 3 caracteres")
	ErrPasswordLength        = errors.New("a senha deve ter pelo menos 8 caracteres")
	ErrEmailAlreadyExists    = errors.New("email já cadastrado")
	ErrDocumentAlreadyExists = errors.New("documento já cadastrado")
	ErrUserAlreadyExists     = errors.New("usuário já cadastrado")
)

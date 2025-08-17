package dto

// RegisterUserInput representa os dados de entrada para o registro.
type RegisterUserInput struct {
	Name     string `json:"name"`
	Email    string `json:"email"`
	Password string `json:"password"`
	Document string `json:"document"`
	Guest    bool   `json:"guest"`
}

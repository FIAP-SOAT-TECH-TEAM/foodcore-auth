# remote states
  variable "foodcore-backend-resource-group" {
    type        = string
    description = "Nome do resource group onde o backend está armazenado"
  }

  variable "foodcore-backend-storage-account" {
    type        = string
    description = "Nome da conta de armazenamento onde o backend está armazenado"
  }

  variable "foodcore-backend-container" {
    type        = string
    description = "Nome do contêiner onde o backend está armazenado"
  }

  variable "foodcore-backend-infra-key" {
    type        = string
    description = "Chave do arquivo tfstate do foodcore-infra"
  }

variable "swagger_path" {
  description = "Caminho do arquivo swagger.json"
  type        = string
}

variable "apim_api_auth_name" {
  description = "Nome da API de autenticação no API Management"
  type        = string
}

variable "apim_api_auth_version" {
  description = "Versão da API de autenticação no API Management"
  type        = string
}

variable "apim_api_auth_display_name" {
  description = "Nome exibido da API de autenticação no API Management"
  type        = string
}

variable "auth_api_path" {
  type        = string
  description = "Caminho da API de autenticação."
}
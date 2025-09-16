output "azfunc_resource_group_name_from_remote" {
  value       = data.terraform_remote_state.infra.outputs.resource_group_name
  description = "O nome do resource group do backend do FoodCore"
}

output "azfunc_name_from_remote" {
  value       = data.terraform_remote_state.infra.outputs.azfunc_name
  description = "O nome da Azure Function App do backend do FoodCore"
}

output "auth_api_validate_endpoint" {
  value = module.apim.auth_api_validate_endpoint
  description = "A URL do endpoint de validação da API de autenticação"
}
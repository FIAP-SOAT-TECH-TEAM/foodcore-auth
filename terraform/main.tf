module "apim" {
  source = "./modules/apim"

  swagger_path                      = var.swagger_path
  foodcore-backend-container        = var.foodcore-backend-container
  foodcore-backend-infra-key        = var.foodcore-backend-infra-key
  foodcore-backend-resource-group   = var.foodcore-backend-resource-group
  foodcore-backend-storage-account  = var.foodcore-backend-storage-account
  apim_api_auth_name                = var.apim_api_auth_name
  apim_api_auth_version             = var.apim_api_auth_version
  auth_api_path                     = var.auth_api_path
  apim_api_auth_display_name        = var.apim_api_auth_display_name

}
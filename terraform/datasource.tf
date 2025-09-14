data "terraform_remote_state" "infra" {
  backend = "azurerm"
  config = {
    resource_group_name  = var.foodcore-backend-resource-group
    storage_account_name = var.foodcore-backend-storage-account
    container_name       = var.foodcore-backend-container
    key                  = var.foodcore-backend-infra-key
  }
}
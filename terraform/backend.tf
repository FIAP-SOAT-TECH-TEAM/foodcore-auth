terraform {
  backend "azurerm" {
    resource_group_name  = "replace_via_secret"
    storage_account_name = "replace_via_secret"
    container_name       = "replace_via_secret"
    subscription_id      = "replace_via_secret"
    key                  = "replace_via_secret"
  }
}
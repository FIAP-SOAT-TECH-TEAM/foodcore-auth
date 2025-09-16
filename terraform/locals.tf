locals {
  auth_api_path_without_slash = replace(var.auth_api_path, "/", "")
}
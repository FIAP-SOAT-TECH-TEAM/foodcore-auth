locals {
  auth_api_backend = "https://${data.terraform_remote_state.infra.outputs.azfunc_private_dns_fqdn}/api"
}
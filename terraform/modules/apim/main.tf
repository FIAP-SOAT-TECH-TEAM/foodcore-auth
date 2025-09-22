resource "azurerm_api_management_api" "foodcoreauth_apim" {
  name                = var.apim_api_auth_name
  resource_group_name = data.terraform_remote_state.infra.outputs.apim_resource_group
  api_management_name = data.terraform_remote_state.infra.outputs.apim_name
  revision            = var.apim_api_auth_version
  display_name        = var.apim_api_auth_display_name
  path                = var.auth_api_path
  protocols           = ["https"]

  import {
    content_format = "openapi+json"
    content_value  = file(var.swagger_path)
  }
}

resource "azurerm_api_management_api_policy" "set_backend_api" {
  api_name            = azurerm_api_management_api.foodcoreauth_apim.name
  api_management_name = data.terraform_remote_state.infra.outputs.apim_name
  resource_group_name = data.terraform_remote_state.infra.outputs.apim_resource_group

  xml_content = <<XML
  <policies>
    <inbound>
      <base />
      <!-- Define backend global -->
      <set-backend-service base-url="${local.auth_api_backend}" />
    </inbound>
    <backend>
      <base />
    </backend>
    <outbound>
      <base />
    </outbound>
    <on-error>
      <!-- Normaliza Path (adiciona / se não existir) -->
      <set-variable name="normalizedPath" value="@{
          var path = context.Request?.Url?.Path ?? "";
          var normalizedPath = path.StartsWith("/") ? path : $"/{path}";
          return normalizedPath;
      }" />
      <choose>
        <when condition="@(context.LastError != null)">
          <return-response>
            <set-status code="@(context.Response?.StatusCode ?? 500)" reason="Other errors" />
            <set-header name="Content-Type" exists-action="override">
              <value>application/json</value>
            </set-header>
            <set-body>@{
                var error = new JObject();
                error["timestamp"] = DateTime.UtcNow.ToString("o"); // ISO 8601
                error["status"]    = context.Response?.StatusCode ?? 500;
                error["message"]   = context.LastError.Message;
                error["path"]      = context.Variables.GetValueOrDefault<string>("normalizedPath");
                return error.ToString(Newtonsoft.Json.Formatting.Indented);
            }</set-body>
          </return-response>
        </when>
      </choose>
    </on-error>
  </policies>
XML
}

resource "azurerm_api_management_product_api" "foodcoreauth_start_product_assoc" {
  api_name            = azurerm_api_management_api.foodcoreauth_apim.name
  product_id          = data.terraform_remote_state.infra.outputs.apim_foodcore_start_productid
  api_management_name = data.terraform_remote_state.infra.outputs.apim_name
  resource_group_name = data.terraform_remote_state.infra.outputs.apim_resource_group
}
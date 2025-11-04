output "container_app_fqdn" {
  description = "Fully Qualified Domain Name (FQDN) of the Container App"
  value       = azurerm_container_app.main.ingress[0].fqdn
}

output "container_app_id" {
  description = "Resource ID of the Container App"
  value       = azurerm_container_app.main.id
}

output "container_app_name" {
  description = "Name of the Container App"
  value       = azurerm_container_app.main.name
}

output "container_app_url" {
  description = "Public URL of the Container App"
  value       = "https://${azurerm_container_app.main.ingress[0].fqdn}"
}

output "container_registry_id" {
  description = "Resource ID of the Container Registry"
  value       = azurerm_container_registry.main.id
}

output "container_registry_login_server" {
  description = "Login server URL of the Container Registry"
  value       = azurerm_container_registry.main.login_server
}

output "container_registry_name" {
  description = "Name of the Container Registry"
  value       = azurerm_container_registry.main.name
}

output "resource_group_id" {
  description = "Resource ID of the Resource Group"
  value       = azurerm_resource_group.main.id
}

output "resource_group_name" {
  description = "Name of the Resource Group"
  value       = azurerm_resource_group.main.name
}

output "user_assigned_identity_client_id" {
  description = "Client ID of the User Assigned Managed Identity"
  value       = azurerm_user_assigned_identity.main.client_id
}

output "user_assigned_identity_id" {
  description = "Resource ID of the User Assigned Managed Identity"
  value       = azurerm_user_assigned_identity.main.id
}

output "user_assigned_identity_principal_id" {
  description = "Principal ID of the User Assigned Managed Identity"
  value       = azurerm_user_assigned_identity.main.principal_id
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location

  tags = var.tags
}

# User Assigned Managed Identity
resource "azurerm_user_assigned_identity" "main" {
  name                = var.identity_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name

  tags = var.tags
}

# Container Registry
resource "azurerm_container_registry" "main" {
  name                = var.registry_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = var.registry_sku

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.main.id]
  }

  tags = var.tags
}

# Role assignment for ACR pull
resource "azurerm_role_assignment" "acr_pull" {
  scope                = azurerm_container_registry.main.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.main.principal_id
}

# Container Apps Environment
resource "azurerm_container_app_environment" "main" {
  name                = var.environment_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name

  tags = var.tags
}

# Container App
resource "azurerm_container_app" "main" {
  name                         = var.container_app_name
  container_app_environment_id = azurerm_container_app_environment.main.id
  resource_group_name          = azurerm_resource_group.main.name
  revision_mode                = "Single"

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.main.id]
  }

  registry {
    server   = azurerm_container_registry.main.login_server
    identity = azurerm_user_assigned_identity.main.id
  }

  template {
    container {
      name   = "aitour2025"
      image  = "${azurerm_container_registry.main.login_server}/${var.container_image_name}:${var.container_image_tag}"
      cpu    = var.container_cpu
      memory = var.container_memory

      env {
        name  = "ASPNETCORE_URLS"
        value = "http://+:8080"
      }

      env {
        name  = "SemanticKernelModel_DeploymentName"
        value = var.semantic_kernel_deployment_name
      }

      env {
        name        = "SemanticKernelModel_ApiKey"
        secret_name = "semantic-kernel-api-key"
      }

      env {
        name  = "SemanticKernelModel_Endpoint"
        value = var.semantic_kernel_endpoint
      }

      env {
        name  = "SemanticKernelModel_ApiVersion"
        value = var.semantic_kernel_api_version
      }

      env {
        name  = "SemanticKernelModel_Research_DeploymentName"
        value = var.semantic_kernel_research_deployment_name
      }

      env {
        name        = "SemanticKernelModel_Research_ApiKey"
        secret_name = "semantic-kernel-research-api-key"
      }

      env {
        name  = "SemanticKernelModel_Research_Endpoint"
        value = var.semantic_kernel_research_endpoint
      }

      env {
        name  = "SemanticKernelModel_Research_ApiVersion"
        value = var.semantic_kernel_research_api_version
      }

      env {
        name        = "AppInsights"
        secret_name = "app-insights-connection-string"
      }
    }

    min_replicas = var.min_replicas
    max_replicas = var.max_replicas
  }

  ingress {
    external_enabled = true
    target_port      = 8080

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  secret {
    name  = "semantic-kernel-api-key"
    value = var.semantic_kernel_api_key
  }

  secret {
    name  = "semantic-kernel-research-api-key"
    value = var.semantic_kernel_research_api_key
  }

  secret {
    name  = "app-insights-connection-string"
    value = var.app_insights_connection_string
  }

  tags = var.tags

  depends_on = [
    azurerm_role_assignment.acr_pull
  ]
}

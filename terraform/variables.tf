variable "app_insights_connection_string" {
  description = "Application Insights connection string for monitoring"
  type        = string
  sensitive   = true
}

variable "container_app_name" {
  description = "Name of the Azure Container App"
  type        = string
  default     = "aitour2025-app"
}

variable "container_cpu" {
  description = "CPU allocation for the container (in cores)"
  type        = number
  default     = 0.5
}

variable "container_image_name" {
  description = "Name of the container image"
  type        = string
  default     = "aitour2025"
}

variable "container_image_tag" {
  description = "Tag of the container image"
  type        = string
  default     = "latest"
}

variable "container_memory" {
  description = "Memory allocation for the container (e.g., '1Gi')"
  type        = string
  default     = "1Gi"
}

variable "environment_name" {
  description = "Name of the Azure Container Apps Environment"
  type        = string
  default     = "aitour2025-environment"
}

variable "identity_name" {
  description = "Name of the User Assigned Managed Identity"
  type        = string
  default     = "aitour2025-identity"
}

variable "location" {
  description = "Azure region where resources will be deployed"
  type        = string
  default     = "swedencentral"
}

variable "max_replicas" {
  description = "Maximum number of container replicas"
  type        = number
  default     = 10
}

variable "min_replicas" {
  description = "Minimum number of container replicas"
  type        = number
  default     = 1
}

variable "registry_name" {
  description = "Name of the Azure Container Registry (must be globally unique and alphanumeric)"
  type        = string
  default     = "aitour2025registry"
}

variable "registry_sku" {
  description = "SKU of the Azure Container Registry"
  type        = string
  default     = "Basic"

  validation {
    condition     = contains(["Basic", "Standard", "Premium"], var.registry_sku)
    error_message = "Registry SKU must be Basic, Standard, or Premium."
  }
}

variable "resource_group_name" {
  description = "Name of the Azure Resource Group"
  type        = string
  default     = "aitour2025"
}

variable "semantic_kernel_api_key" {
  description = "API key for Semantic Kernel model"
  type        = string
  sensitive   = true
}

variable "semantic_kernel_api_version" {
  description = "API version for Semantic Kernel model"
  type        = string
  default     = "2024-08-01-preview"
}

variable "semantic_kernel_deployment_name" {
  description = "Deployment name for Semantic Kernel model"
  type        = string
  default     = "gpt-4o"
}

variable "semantic_kernel_endpoint" {
  description = "Endpoint URL for Semantic Kernel model"
  type        = string
}

variable "semantic_kernel_research_api_key" {
  description = "API key for Semantic Kernel research model"
  type        = string
  sensitive   = true
}

variable "semantic_kernel_research_api_version" {
  description = "API version for Semantic Kernel research model"
  type        = string
  default     = "2024-08-01-preview"
}

variable "semantic_kernel_research_deployment_name" {
  description = "Deployment name for Semantic Kernel research model"
  type        = string
  default     = "gpt-4o-mini"
}

variable "semantic_kernel_research_endpoint" {
  description = "Endpoint URL for Semantic Kernel research model"
  type        = string
}

variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default = {
    Environment = "Development"
    Project     = "AITour2025"
    ManagedBy   = "Terraform"
  }
}

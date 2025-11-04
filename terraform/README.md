# Terraform Configuration for AI Tour 2025 Azure Container Apps

This Terraform configuration deploys the AI Tour 2025 application infrastructure to Azure Container Apps.

## Infrastructure Components

The configuration creates the following Azure resources:

- **Resource Group**: Container for all Azure resources
- **User Assigned Managed Identity**: Identity for secure authentication between services
- **Azure Container Registry (ACR)**: Private Docker image registry
- **Container Apps Environment**: Hosting environment for container apps
- **Container App**: The deployed application with ingress, scaling, and secrets management

## Prerequisites

Before deploying this infrastructure, ensure you have:

1. **Azure CLI** installed and authenticated
   ```bash
   az login
   ```

2. **Terraform** installed (version >= 1.5.0)
   ```bash
   terraform --version
   ```

3. **Docker** installed (for building and pushing images)
   ```bash
   docker --version
   ```

4. **Azure Subscription** with appropriate permissions to create resources

5. **HCP Terraform Account** (optional, for remote state management)
   - Create an account at https://app.terraform.io/
   - Update `backend.tf` with your organization name

## Configuration

### 1. Backend Configuration

Edit `backend.tf` and replace the organization name with your HCP Terraform organization:

```hcl
terraform {
  cloud {
    organization = "your-org-name"  # Replace this
    
    workspaces {
      name = "aitour2025"
    }
  }
}
```

**Alternative**: To use local state instead of HCP Terraform, delete or comment out the `backend.tf` file.

### 2. Required Variables

Create a `terraform.tfvars` file with your configuration:

```hcl
# Azure Configuration
location            = "swedencentral"
resource_group_name = "aitour2025"

# Container Registry
registry_name = "aitour2025registry"  # Must be globally unique and alphanumeric

# AI/ML Configuration
semantic_kernel_endpoint          = "https://YOUR-ENDPOINT.openai.azure.com"
semantic_kernel_api_key           = "your-api-key"
semantic_kernel_deployment_name   = "gpt-4o"

semantic_kernel_research_endpoint        = "https://YOUR-ENDPOINT.openai.azure.com"
semantic_kernel_research_api_key         = "your-research-api-key"
semantic_kernel_research_deployment_name = "gpt-4o-mini"

# Monitoring
app_insights_connection_string = "your-app-insights-connection-string"

# Container Configuration (optional overrides)
container_image_tag = "v1"
container_cpu       = 0.5
container_memory    = "1Gi"
min_replicas        = 1
max_replicas        = 10
```

**Security Note**: Never commit `terraform.tfvars` to version control. Add it to `.gitignore`.

### 3. Alternative: Use Environment Variables

Instead of a tfvars file, you can use environment variables:

```bash
export TF_VAR_semantic_kernel_api_key="your-api-key"
export TF_VAR_semantic_kernel_endpoint="https://YOUR-ENDPOINT.openai.azure.com"
export TF_VAR_semantic_kernel_research_api_key="your-research-api-key"
export TF_VAR_semantic_kernel_research_endpoint="https://YOUR-ENDPOINT.openai.azure.com"
export TF_VAR_app_insights_connection_string="your-connection-string"
```

## Deployment Steps

### Step 1: Initialize Terraform

```bash
cd terraform
terraform init
```

This will:
- Download the Azure provider
- Configure the backend (HCP Terraform or local)

### Step 2: Build and Push Docker Image

Before applying Terraform, build and push your Docker image to ACR:

```bash
# Navigate to the application directory
cd ../aitour2025

# Build the Docker image
docker build -t aitour2025:v1 -f Dockerfile .

# Note: You'll need to push the image after the ACR is created
# The ACR login server will be: <registry_name>.azurecr.io
```

**Important**: The Container App expects the image to already exist in ACR. You have two options:

**Option A - Initial Deployment with Placeholder**:
1. First, deploy with a placeholder image or comment out the container app resource
2. After ACR is created, build and push your image
3. Uncomment and apply again

**Option B - Pre-create ACR**:
1. Deploy only the ACR first
2. Push your image
3. Deploy the Container App

### Step 3: Review the Execution Plan

```bash
terraform plan
```

Review the planned changes carefully. You should see:
- 1 resource group to be created
- 1 managed identity to be created
- 1 container registry to be created
- 1 role assignment to be created
- 1 container apps environment to be created
- 1 container app to be created

### Step 4: Apply the Configuration

```bash
terraform apply
```

Type `yes` when prompted to confirm the deployment.

The deployment typically takes 5-10 minutes.

### Step 5: Push Docker Image to ACR

After the infrastructure is deployed:

```bash
# Get the ACR login server from Terraform output
ACR_LOGIN_SERVER=$(terraform output -raw container_registry_login_server)

# Get the ACR name
ACR_NAME=$(terraform output -raw container_registry_name)

# Login to ACR
az acr login --name $ACR_NAME

# Tag the image
docker tag aitour2025:v1 $ACR_LOGIN_SERVER/aitour2025:v1

# Push the image
docker push $ACR_LOGIN_SERVER/aitour2025:v1
```

### Step 6: Update Container App (if needed)

If you deployed before pushing the image, update the container app:

```bash
terraform apply -refresh-only
```

## Accessing the Application

After successful deployment, get the application URL:

```bash
terraform output container_app_url
```

Test the endpoints:

```bash
# Get the FQDN
APP_URL=$(terraform output -raw container_app_url)

# Test the AnalyzeImage endpoint
curl -X POST "$APP_URL/api/CosentinoAI/AnalyzeImage" \
  -H "Content-Type: application/json" \
  -d @../imagerequest.json

# Test the GetCustomerContext endpoint
curl -X POST "$APP_URL/api/CosentinoAI/GetCustomerContext" \
  -H "Content-Type: application/json" \
  -d @../customerContextRequest.json

# Test the CreateSupportEmail endpoint
curl -X POST "$APP_URL/api/CosentinoAI/CreateSupportEmail" \
  -H "Content-Type: application/json" \
  -d @../emailrequest.json
```

## Viewing Outputs

To see all outputs:

```bash
terraform output
```

Key outputs:
- `container_app_url`: Public URL of your application
- `container_registry_login_server`: ACR login server for pushing images
- `container_app_fqdn`: Fully qualified domain name

## Updating the Infrastructure

To update the infrastructure:

1. Modify the Terraform files or variables
2. Run `terraform plan` to review changes
3. Run `terraform apply` to apply changes

### Update Container Image

To deploy a new version of your application:

```bash
# Build new version
docker build -t aitour2025:v2 -f ../aitour2025/Dockerfile ../aitour2025

# Tag and push
docker tag aitour2025:v2 $ACR_LOGIN_SERVER/aitour2025:v2
docker push $ACR_LOGIN_SERVER/aitour2025:v2

# Update the variable and apply
terraform apply -var="container_image_tag=v2"
```

## Destroying the Infrastructure

To remove all resources:

```bash
terraform destroy
```

Type `yes` when prompted to confirm deletion.

**Warning**: This will delete all resources including the Container Registry and any images stored in it.

## Troubleshooting

### Container App Not Starting

Check the logs:

```bash
az containerapp logs show \
  --name $(terraform output -raw container_app_name) \
  --resource-group $(terraform output -raw resource_group_name)
```

### Image Pull Failures

Verify the managed identity has ACR pull permissions:

```bash
az role assignment list \
  --assignee $(terraform output -raw user_assigned_identity_principal_id) \
  --scope $(terraform output -raw container_registry_id)
```

### Missing Environment Variables

Verify secrets are properly configured:

```bash
az containerapp show \
  --name $(terraform output -raw container_app_name) \
  --resource-group $(terraform output -raw resource_group_name) \
  --query "properties.configuration.secrets"
```

## Security Best Practices

1. **Secrets Management**: 
   - Never commit `terraform.tfvars` with sensitive values
   - Consider using Azure Key Vault for production secrets
   - Use HCP Terraform variable sets for sensitive values

2. **Network Security**:
   - Consider using private endpoints for production
   - Implement Azure Front Door or Application Gateway for DDoS protection

3. **Identity and Access**:
   - Use managed identities instead of connection strings where possible
   - Implement Azure AD authentication for the application

4. **Monitoring**:
   - Configure alerts in Application Insights
   - Set up budget alerts for cost management

## Cost Optimization

- **Container Apps**: Charged per vCore-second and GB-second
- **Container Registry**: Basic SKU is $5/month (as of 2024)
- **Managed Identity**: Free
- **Application Insights**: Charged per GB of data ingested

To reduce costs:
- Adjust `min_replicas` to 0 for non-production environments
- Use smaller CPU/memory allocations if sufficient
- Implement appropriate monitoring retention policies

## Production Considerations

This configuration is suitable for development/testing. For production:

1. **Upgrade ACR SKU**: Use Standard or Premium for better performance
2. **Enable Zone Redundancy**: For Container Apps Environment
3. **Implement Private Networking**: Use VNET integration
4. **Add Azure Key Vault**: For secrets management
5. **Configure Custom Domains**: With TLS certificates
6. **Implement CI/CD**: Automate image builds and deployments
7. **Enable Diagnostic Settings**: For all resources
8. **Configure Backup/DR**: For Container Registry

## Additional Resources

- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Terraform Azure Provider Documentation](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- [HCP Terraform Documentation](https://developer.hashicorp.com/terraform/cloud-docs)
- [Application Project README](../README.md)

## Support

For issues with:
- **Infrastructure/Terraform**: Check the troubleshooting section above
- **Application Code**: See the main project [README](../README.md)
- **Azure Services**: Consult Azure documentation or support

## License

This infrastructure configuration is provided as-is for the AI Tour 2025 project.

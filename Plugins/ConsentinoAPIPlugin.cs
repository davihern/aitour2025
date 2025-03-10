using Microsoft.SemanticKernel;


namespace ChatApp.WebApi.Plugins
{
    public class ConsentinoAPIPlugin()
    {
        [KernelFunction("get_cosentino_products")]
        public async Task<List<Product>> GetCosentinoProductsAsync()
        {
            //build a variable to hold the result of this method, which is a list of products
            List<Product> consentinoProducts = new List<Product>();

            //create some lines of code to insert sample values in the consentinoProducts list
            consentinoProducts.Add(new Product { Name = "Counter", Price = 1000, Description = "A beautiful brown counter", Type = Product.ProductType.Counter });
            consentinoProducts.Add(new Product { Name = "Sink", Price = 500, Description = "A beautiful yellow sink", Type = Product.ProductType.Sink });
            consentinoProducts.Add(new Product { Name = "Faucet", Price = 200, Description = "A beautiful pink faucet", Type = Product.ProductType.Faucet });
            consentinoProducts.Add(new Product { Name = "Cabinet", Price = 1500, Description = "A beautiful brown cabinet", Type = Product.ProductType.Cabinet });

            //return the list of products
            return consentinoProducts;

        }

        [KernelFunction("get_customer_information_by_dni")]
        public async Task<CustomerInfo> GetCustomerInformationByDNIAsync(string DNI)
        {
           
            if(DNI == "12345678A")
            {
               return new CustomerInfo("John", "Doe", "New York", "123 Main St", "123456789", "", "12345678A");
            }
            else
            {
                throw new Exception("Customer not found");
            }

        }

        [KernelFunction("get_cosentino_supportcases")]
        public async Task<List<SupportCase>> GetCosentinoSupportCasesAsync()
        {
            //build a variable to hold the result of this method, which is a list of products
            List<SupportCase> consentinoSupportCases = new List<SupportCase>();

            //create some lines of code to insert sample values in the consentinoProducts list
            consentinoSupportCases.Add(new SupportCase("1", "Encimera rota", "Closed", true, "9945678B", "Contactar con distribuidor mas cercano para reparacion"));
            consentinoSupportCases.Add(new SupportCase("2", "Encimera manchada con pintura", "Closed", true, "88345678C", "Limpiar la encimera con acetona."));

            //return the list of products
            return consentinoSupportCases;

        }

    }

}
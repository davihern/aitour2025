using Microsoft.SemanticKernel;


namespace ChatApp.WebApi.Plugins
{
    public class ConsentinoAPIPlugin()
    {
   

        [KernelFunction("get_customer_information_by_dni")]
        public async Task<CustomerInfo> GetCustomerInformationByDNIAsync(string DNI)
        {
           
            if(DNI == "12345678A")
            {
               return new CustomerInfo("Maria", "Perez", "Salamanca", "Calle larga", "123456789", "", "12345678A");
            }
            else if(DNI == "12345678B")
            {
                return new CustomerInfo("Juan", "Garcia", "Madrid", "Calle corta", "123456789", "", "12345678B");
            }
            else if(DNI == "12345678C")
            {
                return new CustomerInfo("Pedro", "Gomez", "Barcelona", "Calle ancha", "123456789", "", "12345678C");
            }
            else if(DNI == "12345678D")
            {
                return new CustomerInfo("Luis", "Gonzalez", "Valencia", "Calle estrecha", "123456789", "", "12345678D");
            }
            else
            {
                throw new Exception("Cliente con DNI no encontrado:" +  DNI);
            }

        }

        [KernelFunction("get_cosentino_supportcases")]
        public async Task<List<SupportCase>> GetCosentinoSupportCasesAsync()
        {
            //build a variable to hold the result of this method, which is a list of products
            List<SupportCase> consentinoSupportCases = new List<SupportCase>();

            //create some lines of code to insert sample values in the consentinoProducts list
            consentinoSupportCases.Add(new SupportCase("1", "Encimera rota", "Closed", true, "9945678B", "Contactar con distribuidor mas cercano para reparacion"));
            consentinoSupportCases.Add(new SupportCase("2", "Encimera manchada con pintura", "Closed", true, "88345678C", @"Cómo limpiar una encimera Dekton
Dekton es una superficie ultracompacta, conocida por su durabilidad y resistencia. Aunque es muy resistente a manchas y arañazos, también necesita un cuidado adecuado para mantener su belleza y funcionalidad. La limpieza regular es sencilla:
1. Para la limpieza diaria: utiliza un paño suave o una esponja con agua y jabón neutro. Evita usar estropajos o limpiadores abrasivos que puedan dañar la superficie.
2. Para manchas difíciles: en caso de manchas de vino, café o productos de maquillaje, usa un poco de bicarbonato de sodio y agua para formar una pasta. Aplícala sobre la mancha, deja actuar unos minutos y luego limpia con un paño suave. Para manchas de grasa, un limpiador específico para superficies de piedra puede ser efectivo.
3. Precauciones: a pesar de su resistencia, es recomendable evitar el contacto directo con ollas y sartenes calientes, así como el uso de productos químicos agresivos como lejía o amoníaco."));
            consentinoSupportCases.Add(new SupportCase("3", "Encimera ", "Closed", true, "9945678B", "Contactar con distribuidor mas cercano para reparacion"));


            //return the list of products
            return consentinoSupportCases;

        }

             [KernelFunction("get_cosentino_products")]
        public async Task<List<Product>> GetCosentinoProductsAsync()
        {
            //build a variable to hold the result of this method, which is a list of products
            List<Product> consentinoProducts = new List<Product>();

            //create some lines of code to insert sample values in the consentinoProducts list
            consentinoProducts.Add(new Product { Name = "Encimera", Price = 1000, Description = "Una hermosa encimera marrón", Type = Product.ProductType.Counter });
            consentinoProducts.Add(new Product { Name = "Fregadero", Price = 500, Description = "Un hermoso fregadero amarillo", Type = Product.ProductType.Sink });
            consentinoProducts.Add(new Product { Name = "Grifo", Price = 200, Description = "Un hermoso grifo rosa", Type = Product.ProductType.Faucet });
           

            //return the list of products
            return consentinoProducts;

        }

    }

}
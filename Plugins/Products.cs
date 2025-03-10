//Create a new class called Products.cs that contains Consentino producs, which are products that sell this company and are used to build the kitchen. Such as Counters, Sinks, Faucets, and Cabinets.
//Make this a generic product with fields such as Name, Price, and Description, and also an enum of product type.

using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.WebApi.Plugins
{
    public class Product
    {
        public enum ProductType
        {
            Counter,
            Sink,
            Faucet,
            Cabinet
        }

        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public ProductType Type { get; set; }
    }
}
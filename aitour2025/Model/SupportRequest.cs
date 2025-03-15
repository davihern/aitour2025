//Create a class to perform a Support Request, it should have the following properties: DNI, description and Photo
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class SupportRequest
    {
        public string DNI { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
    }
}
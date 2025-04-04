
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.WebApi.Plugins
{
    public class CustomerInfo
    {

       
        public CustomerInfo(string name, string surname, string city, string address, string phoneNumber, string email, string dNI)
        {
            Name = name;
            Surname = surname;
            City = city;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
            DNI = dNI;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string DNI { get; set; }
    }
}
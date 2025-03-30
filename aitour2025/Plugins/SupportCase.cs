using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.WebApi.Plugins
{
    public class SupportCase
    {
        public SupportCase(string caseNumber, string caseDescription, string caseStatus, bool isSolved, string customerDNI, string solution)
        {
            CaseNumber = caseNumber;
            CaseDescription = caseDescription;
            CaseStatus = caseStatus;
            IsSolved = isSolved;
            CustomerDNI = customerDNI;
            Solution = solution;
        }

        public string CaseNumber { get; set; }
        public string CaseDescription { get; set; }
        public string CaseStatus { get; set; }
        public bool IsSolved { get; set; }
        public string CustomerDNI { get; set; }
        public string Solution { get; set; }
    }
}
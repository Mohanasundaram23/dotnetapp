using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace dotnetapp.Models
{
    public class Loan
    {
        [Key]
        public int LoanId {get; set;}
        public string LoanType {get; set;}
        public string Description {get; set;}
        public decimal InterestRate {get; set;}
        public decimal MaximumAmount {get; set;}
    }
}
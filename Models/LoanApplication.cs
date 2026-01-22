using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetapp.Models
{
    public class LoanApplication
    {
        public int LoanApplicationId {get; set;}
        public int? UserId {get; set;}
        public User? User {get; set;}
        public int? LoanId {get; set;}
        public Loan? Loan {get; set;}
        public DateTime SubmissionDate {get; set;}
        public decimal Income {get; set;}
        public DateTime Model {get; set;}
        public decimal PurchasePrice {get; set;}
        public int LoanStatus {get; set;}
        public string Address {get; set;}
        public string File {get; set;}
    }
}
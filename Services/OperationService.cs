using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Services
{
    public class OperationService
    {
        public static List<Operation> retrieveFinancialOperation(ApplicationDbContext context, Customer customer)
        {
            string[] opts = {"Deposit", "Withdraw"};
            List<Operation> financialOperations = context.Operations.Where(x => x.customer == customer && opts.Contains(x.operationType)).ToList();
            return financialOperations;
        }
        public static List<Operation> retrieveManagerialOperation(ApplicationDbContext context, Customer customer)
        {
            string[] opts = { "SignIn", "SignUp", "SignOut", "ListOperations", "ListTransactions", "DepositInquiry" };
            List<Operation> managerialOperations = context.Operations.Where(x => x.customer == customer && opts.Contains(x.operationType)).ToList();
            return managerialOperations;
        }
  

    }
}

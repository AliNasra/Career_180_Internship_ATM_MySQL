using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Services
{
    public class TransactionService
    {
        public static List<Transaction> GetPendingTransactions(ApplicationDbContext context, Customer recipient)
        {
            List<Transaction> transactions = context.Transactions.Where(x => x.recipientCustomer == recipient && x.IsCompleteTransfer == false).ToList();
            return transactions;
        }
        public static Transaction GetTransaction(ApplicationDbContext context,Customer sender, Customer recipient)
        {
            Transaction transaction = context.Transactions.SingleOrDefault(x => x.senderCustomer == sender && x.recipientCustomer == recipient);
            return transaction;
        }

        public static Transaction GetTransaction(ApplicationDbContext context, int id)
        {
            Transaction transaction = context.Transactions.SingleOrDefault(x => x.transactionID == id);
            return transaction;
        }

        public static Transaction GetTransaction(ApplicationDbContext context, Customer recipient, int id)
        {
            Transaction transaction = context.Transactions.SingleOrDefault(x => x.transactionID == id && x.recipientCustomer == recipient);
            return transaction;
        }

        public static void resolveTransactions(ApplicationDbContext context, Customer deletedCustomer)
        {
            context.Customers.Attach(deletedCustomer);
            List<Transaction> transactionsToBeResolved = context.Transactions.Where(x=>x.recipientCustomer == deletedCustomer && x.IsCompleteTransfer == false).ToList();
            foreach (Transaction transaction in transactionsToBeResolved)
            {
                Customer customer                         = CustomerService.retrieveCustomer(context, transaction.senderCustomerID);
                customer.bankDeposit                      = customer.bankDeposit + transaction.transferredMoney;
                transaction.recipientPosttransferDeposit  = transaction.recipientCustomer.bankDeposit;
                transaction.senderPosttransferDeposit     = customer.bankDeposit;
                transaction.conclusionTime                = DateTime.Now;
                transaction.transactionState              = "Refunded";
                transaction.IsCompleteTransfer            = true;
            }
            deletedCustomer.activityStatus = false;
            context.SaveChanges();          
        }

    }
}

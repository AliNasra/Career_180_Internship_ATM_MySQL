using ConsoleApp1.Helpers;
using ConsoleApp1.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Services
{
    public class CustomerService
    {
        public static List<Customer> getOrdinaryCustomers(ApplicationDbContext context)
        {
            List <Customer> ordinaryCustomers = context.Customers.Where(x => x.customerType == "O").ToList();
            return ordinaryCustomers; 
        }
        public static List<Customer> getVIPCustomers(ApplicationDbContext context)
        {
            List<Customer> VIPCustomers = context.Customers.Where(x => x.customerType == "V").ToList();
            return VIPCustomers;
        }
    
        public static Customer retrieveCustomer(ApplicationDbContext context, String username)
        {
            Customer? customer = context.Customers.SingleOrDefault(u => u.userName == username);
            return customer;
        }
        public static Customer retrieveCustomer(ApplicationDbContext context, int userID)
        {
            Customer? customer = context.Customers.SingleOrDefault(u => u.customerID == userID);
            return customer;
        }
        public static Customer checkCustomer(ApplicationDbContext context, string username, string password)
        {

            Customer? customer = context.Customers.SingleOrDefault(u => u.userName == username);
            if (customer == null) {
                return null;
            }
            else
            {
                bool checkPassword = EncryptionServices.DecryptPassword(customer.password) == password ? true : false;
                return (checkPassword? customer : null);
            }
        }
        public static bool checkCustomer(ApplicationDbContext context, string username)
        {
            bool checkCustomerExistence = context.Customers.Any(u => u.userName == username && u.activityStatus == true);
            return !checkCustomerExistence;
        }
        public static void registerVIPUser(ApplicationDbContext context, string userName, string password, string email, string birthdate)
        {
            string encryptedPassword = EncryptionServices.EncryptPassword(password);
            DateTime birthDate       = DateTime.Parse(birthdate);
            Customer VIPCustomer     = new Customer
            {
                userName             = userName,
                password             = encryptedPassword,
                email                = email,
                birthDate            = birthDate,
                accountDate          = DateTime.Now,
                operationCounter     = 0,
                accountTimer         = DateTime.Now.AddDays(1),
                customerType         = "V",
                activityStatus       = true,
                operations           = new List<Operation>(),
                sentTransactions     = new List<Transaction>(),
                receivedTransactions = new List<Transaction>()
            };
            Operation signUpOperation = new Operation
            {
                operationDate = DateTime.Now,
                customer      = VIPCustomer,
                operationType = "SignUp",
                successStatus = true
            };
            context.Customers.Add(VIPCustomer);
            context.Operations.Add(signUpOperation);
            context.SaveChanges();
            Console.WriteLine("Registration Completed Successfully");
        }
        public static void registerOrdinaryUser(ApplicationDbContext context, string userName, string password, string email, string birthdate)
        {
            string encryptedPassword  = EncryptionServices.EncryptPassword(password);
            DateTime birthDate        = DateTime.Parse(birthdate);
            Customer ordinaryCustomer = new Customer
            {
                userName         = userName,
                password         = encryptedPassword,
                email            = email,
                birthDate        = birthDate,
                accountDate      = DateTime.Now,
                operationCounter = 0,
                accountTimer     = DateTime.Now.AddDays(1),
                customerType     = "O",
                activityStatus   = true
            };
            Operation signUpOperation = new Operation
            {
                operationDate = DateTime.Now,
                customer      = ordinaryCustomer,
                operationType = "SignUp",
                successStatus = true
            };
            context.Customers.Add(ordinaryCustomer);
            context.Operations.Add(signUpOperation);
            context.SaveChanges();
        }
        public static void signIn(ApplicationDbContext context, Customer customer)
        {
            Operation signInOperation = new Operation
            {
                operationDate = DateTime.Now,
                customerID    = customer.customerID,
                operationType = "SignIn",
                successStatus = true
            };
            context.Operations.Add(signInOperation);
            context.SaveChanges();
            Console.WriteLine("Welcome On Board!");
        }
        public static void signOut(ApplicationDbContext context, Customer customer)
        {
            Operation signOutOperation = new Operation
            {
                operationDate = DateTime.Now,
                customerID    = customer.customerID,
                operationType = "SignOut",
                successStatus = true
            };
            context.Operations.Add(signOutOperation);
            context.SaveChanges();
        }
        public static double getDepositInfo(ApplicationDbContext context,Customer customer)
        {
            Operation DepositOperation = new Operation
            {
                operationDate = DateTime.Now,
                customerID    = customer.customerID,
                operationType = "DepositInquiry",
                successStatus = true
            };
            context.Operations.Add(DepositOperation);
            context.SaveChanges();
            return customer.bankDeposit;
        }
        public static void depositMoney(ApplicationDbContext context, Customer customer, double amount)
        {
            if (!canPerformOperation(context,customer))
            {
                Console.WriteLine($"You have reached your operation limit. You can perform financial operations again after {customer.accountTimer:dddd, MMMM d, yyyy h:mm tt}");
                return;
            }
            context.Customers.Attach(customer);
            customer.bankDeposit       = customer.bankDeposit + amount;
            Operation DepositOperation = new Operation
            {
                operationDate = DateTime.Now,
                customerID    = customer.customerID,
                moneyAmount   = amount,
                operationType = "Deposit",
                successStatus = true
            };
            context.Operations.Add(DepositOperation);
            customer.operationCounter += 1;
            context.SaveChanges();
            Console.WriteLine("Operation Deposited Successfully!");
        }
        public static void withdrawMoney(ApplicationDbContext context, Customer customer, double amount)
        {
            if (!canPerformOperation(context, customer))
            {
                Console.WriteLine($"You have reached your operation limit. You can perform financial operations again after {customer.accountTimer:dddd, MMMM d, yyyy h:mm tt}");
                return;
            }
            context.Customers.Attach(customer);
            customer.bankDeposit       = customer.bankDeposit - amount;
            Operation DepositOperation = new Operation
            {
                operationDate = DateTime.Now,
                customerID    = customer.customerID,
                moneyAmount   = amount,
                operationType = "Withdraw",
                successStatus = true
            };
            context.Operations.Add(DepositOperation);
            customer.operationCounter += 1;
            context.SaveChanges();
            Console.WriteLine("Operation Withdrawn Successfully!");
        }
        public static bool verifyIdentity(ApplicationDbContext context, string userName, string password)
        {
            bool checkCustomer = context.Customers.Any(u => u.userName == userName && EncryptionServices.DecryptPassword(u.password) == password && u.activityStatus == true);
            return checkCustomer;
        }
        public static void makeTransaction(ApplicationDbContext context,Customer customer ,Customer recipient, double amount)
        {
            if (!canPerformOperation(context, customer))
            {
                Console.WriteLine($"You have reached your operation limit. You can perform financial operations again after {customer.accountTimer:dddd, MMMM d, yyyy h:mm tt}");
                return;
            }
            context.Customers.Attach(customer);
            customer.bankDeposit    = customer.bankDeposit - amount;
            Transaction transaction = new Transaction
            {
            senderCustomerID             = customer.customerID,
            recipientCustomerID          = recipient.customerID,
            transferredMoney             = amount,
            senderPretransferDeposit     = customer.bankDeposit,
            recipientPretransferDeposit  = recipient.bankDeposit,
            transactionState             = "Pending",
            transactionTime              = DateTime.Now,
            successStatus                = true,
            IsCompleteTransfer           = false,
            };
            context.Transactions.Add(transaction);
            customer.operationCounter += 1;
            context.SaveChanges();
            Console.WriteLine("Transaction Completed Successfully!");
        }
        public static void acceptTransaction(ApplicationDbContext context, Customer customer, Transaction transaction)
        {
            if (!canPerformOperation(context, customer))
            {
                Console.WriteLine($"You have reached your operation limit. You can perform financial operations again after {customer.accountTimer:dddd, MMMM d, yyyy h:mm tt}");
                return;
            }
            context.Customers.Attach(customer);
            context.Transactions.Attach(transaction);
            if (transaction.recipientCustomer == customer)
            {
                customer.bankDeposit                     = customer.bankDeposit + transaction.transferredMoney;
                transaction.transactionState             = "Accepted";
                transaction.IsCompleteTransfer           = true;
                transaction.senderPosttransferDeposit    = transaction.senderPretransferDeposit;
                transaction.recipientPosttransferDeposit = customer.bankDeposit;
                transaction.conclusionTime               = DateTime.Now;
                customer.operationCounter                = customer.operationCounter + 1;
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Please Check Your Input!");
            }
        }
        public static void rejectTransaction(ApplicationDbContext context, Customer customer, Transaction transaction)
        {
            if (!canPerformOperation(context, customer))
            {
                Console.WriteLine($"You have reached your operation limit. You can perform financial operations again after {customer.accountTimer:dddd, MMMM d, yyyy h:mm tt}");
                return;
            }
            context.Customers.Attach(customer);
            context.Transactions.Attach(transaction);
            if (transaction.recipientCustomer == customer)
            {
                Customer senderCustomer                  = retrieveCustomer(context, transaction.senderCustomerID);
                senderCustomer.bankDeposit               = senderCustomer.bankDeposit + transaction.transferredMoney;
                transaction.transactionState             = "Rejected";
                transaction.IsCompleteTransfer           = true;
                transaction.senderPosttransferDeposit    = senderCustomer.bankDeposit;
                transaction.recipientPosttransferDeposit = customer.bankDeposit;
                transaction.conclusionTime               = DateTime.Now;
                customer.operationCounter                = customer.operationCounter + 1;
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Please Check Your Input!");
            }
        }
        public static void ViewRecentTransactions(ApplicationDbContext context, Customer customer)
        {
            List<Transaction> pendingTransactions = context.Transactions.Where(x=>x.recipientCustomer == customer && x.IsCompleteTransfer == false).ToList();
            if (pendingTransactions.Count == 0)
            {
                Console.WriteLine("No Pending Transactions Found!");
            }
            else
            {
                foreach (Transaction transaction in pendingTransactions)
                {
                    Console.WriteLine(transaction.getPendingTransactionString());
                }
            }    
            Operation ListOperation = new Operation
            {
                operationDate = DateTime.Now,
                customerID    = customer.customerID,
                operationType = "ListTransactions",
                successStatus = true
            };
            context.Operations.Add(ListOperation);
            context.SaveChanges();
        }


        public static async Task<bool> handleVIPUserDeletion(ApplicationDbContext context, Customer customer)
        {
            context.Customers.Attach(customer);
            int pendingTransactions = TransactionService.GetPendingTransactions(context, customer).Count;
            int waitSeconds = 30;
            string? userResponse = null;
            if (pendingTransactions > 0)
            {
                Console.WriteLine($"You have {pendingTransactions} pending transaction(s). Do you want to proceed? y/n");
                Console.WriteLine($"Please answer within {waitSeconds} seconds!");
                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                userResponse = await ATM.ReadInputAsync(cts.Token);
                if (userResponse == null)
                {
                    Console.WriteLine($"Time expired! You didn't enter anything within {waitSeconds} seconds.");
                    return false;
                }
                else if (userResponse == "y")
                {
                }
                else if (userResponse == "n")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please type either y or n to declare your decision");
                    return false;
                }
            }
            TransactionService.resolveTransactions(context, customer);
            customer.activityStatus = false;
            context.SaveChanges();
            Console.WriteLine("Account Deleted Successfully");
            return true;
        }

        public static void listAllOperations(ApplicationDbContext context, Customer customer)
        {
            List<Operation> fO = OperationService.retrieveFinancialOperation(context, customer);
            List<Operation> mO = OperationService.retrieveManagerialOperation(context, customer);
            Console.WriteLine("Financial Operations:");
            if (fO.Count == 0)
            {
                Console.WriteLine("No Financial Operations Were Found!");
            }
            else
            {
                foreach (Operation op in fO)
                {
                    Console.WriteLine(op.getFinancialOperationString());
                }
            }
            Console.WriteLine("Managerial Operations:");
            if (mO.Count == 0)
            {
                Console.WriteLine("No Managerial Operations Were Found!");
            }
            else
            {
                foreach (Operation op in mO)
                {
                Console.WriteLine(op.getManagerialOperationString());
                }
            }
            Console.WriteLine("Transaction-related Operations:");
            List<Transaction> refundedTransactions = context.Transactions.Where(x => x.senderCustomer    == customer && x.transactionState == "Refunded").ToList();
            List<Transaction> settledTransactions  = context.Transactions.Where(x => x.recipientCustomer == customer && x.IsCompleteTransfer == true).ToList();
            List<Transaction> madeTransactions     = context.Transactions.Where(x => x.senderCustomer == customer).ToList();
            refundedTransactions.AddRange(settledTransactions);
            refundedTransactions.AddRange(madeTransactions);
            if (refundedTransactions.Count == 0)
            {
                Console.WriteLine("No Transaction-related Operations Were Found!");
            }
            else
            {
                foreach (Transaction transaction in refundedTransactions)
                {
                    Console.WriteLine(transaction.getTransactionString());
                }
            }         
            Operation ListOperation = new Operation
            {
                operationDate = DateTime.Now,
                customerID    = customer.customerID,
                operationType = "ListOperations",
                successStatus = true
            };
            context.Operations.Add(ListOperation);
            context.SaveChanges();
        }
        public static bool canPerformOperation(ApplicationDbContext context, Customer customer)
        {
            context.Customers.Attach(customer);
            int operationLimit = 10;
            if (DateTime.Now > customer.accountTimer)
            {
                DateTime newTime = customer.accountTimer;
                while (true)
                {
                    newTime = newTime.AddDays(1);
                    if (newTime > customer.accountTimer)
                    {
                        break;
                    }
                }
                customer.accountTimer     = newTime;
                customer.operationCounter = 0;
                return true;
            }
            else
            {
                if (customer.operationCounter < operationLimit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}

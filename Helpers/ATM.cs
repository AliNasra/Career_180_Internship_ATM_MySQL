﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Services;
using ConsoleApp1.Models;

namespace ConsoleApp1.Helpers
{
    class ATM
    {
        
        private string[] OrdinaryoperationList = { "Inquire about your account's deposit", "Deposit Money", "Withdraw Money", "Make a Transaction", "View Pending Transactions", "Accept Transaction", "Reject Transaction", "List Operations", "Exit" };
        private string[] VIPoperationList      = { "Inquire about your account's deposit", "Deposit Money", "Withdraw Money", "Make a Transaction", "View Pending Transactions", "Accept Transaction", "Reject Transaction", "List Operations", "Delete Account", "Exit" };

        public void printOrdinaryOptionList()
        {
            for (int i = 1; i <= this.OrdinaryoperationList.Length; i++)
            {
                Console.WriteLine($"{i}-> {OrdinaryoperationList[i - 1]}");
            }

        }

        public void printVIPOptionList()
        {
            for (int i = 1; i <= this.VIPoperationList.Length; i++)
            {
                Console.WriteLine($"{i}-> {VIPoperationList[i - 1]}");
            }

        }

        string[] readUserCredentials()
        {
            Console.WriteLine("Please Enter your credentials");
            Console.WriteLine("Username:");
            string userName = Console.ReadLine();
            Console.WriteLine("Password:");
            string password = Console.ReadLine();
            string[] userData = { userName, password };
            return userData;
        }

        string[] readUserRegistrationCredentials()
        {
            Console.WriteLine("Please Enter your credentials!");
            Console.WriteLine("Username:");
            string userName = Console.ReadLine().Trim();
            if (!VerificationService.isValidUserName(userName))
            {
                Console.WriteLine("The username should be a single word!");
                return null;
            }
            Console.WriteLine("Password:");
            string password = Console.ReadLine().Trim();
            if (!VerificationService.isValidPassword(password))
            {
                Console.WriteLine("Invalid Password!");
                return null;
            }
            Console.WriteLine("Email:");
            string email = Console.ReadLine().Trim();
            if (!VerificationService.isValidEmail(email))
            {
                Console.WriteLine("Invalid Email!");
                return null;
            }
            Console.WriteLine("Birthdate in MM/DD/YYYY format:");
            string birthdate = Console.ReadLine().Trim();
            if (!VerificationService.isValidBirthDate(birthdate))
            {
                Console.WriteLine("Invalid Birth Date Entry");
                return null;
            }
            Console.WriteLine("User Category\n  VIP-> 1:\n  Ordinary->2");
            string userCategory = Console.ReadLine().Trim();
            if (!VerificationService.isValidCategory(userCategory))
            {
                Console.WriteLine("Invalid User Category");
                return null;
            }
            string[] userData = { userName, password, email, birthdate, userCategory };
            return userData;
        }
        int handleMainMenuOptions()
        {
            string inputedOption = Console.ReadLine();
            if (inputedOption == "1")
            {
                return 1;
            }
            else if (inputedOption == "2")
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
        

        async Task handleLogin()
        {
            string[] loginData = readUserCredentials();
            using (var context = new ApplicationDbContext())//connectionString
            {
                Customer loggedInCustomer = CustomerService.checkCustomer(context,loginData[0], loginData[1]);

                if (loggedInCustomer != null)
                {
                    if (loggedInCustomer.customerType == "V")
                    {
                        await processVIPUser(loggedInCustomer);
                    }
                    else
                    {
                        processOrdinaryUser(loggedInCustomer);
                    }
                }
                else
                {
                    Console.WriteLine("Check Your Credentials");
                }
            }           
        }

        void handleSignUp()
        {
            string[] registeredData = readUserRegistrationCredentials();
            if (registeredData == null)
            {
                Console.WriteLine("Registration Failed!");
                return;
            }
            using (var context = new ApplicationDbContext())//connectionString
            {
                if (CustomerService.checkCustomer(context, registeredData[0]))
                {
                    if (registeredData[4] == "1")
                    {
                        CustomerService.registerVIPUser(context,registeredData[0], registeredData[1], registeredData[2], registeredData[3]);
                    }
                    else
                    {
                        CustomerService.registerOrdinaryUser(context,registeredData[0], registeredData[1], registeredData[2], registeredData[3]);
                    }

                }
                else
                {
                    Console.WriteLine("Please pick a unique username");
                }
            }         
        }

        public async Task handleUser()
        {
            while (true)
            {
                Console.WriteLine("Select Operation\n1- Register\n2- Login");
                int selectedOption = handleMainMenuOptions();
                if (selectedOption == 2)
                {
                    await handleLogin();
                }
                else if (selectedOption == 1)
                {

                    handleSignUp();
                }
                else
                {
                    Console.WriteLine("Please insert a proper input");
                }
            }
        }
  
        #region Processing Ordinary Users
        void processOrdinaryUser(Customer loggedinCustomer)
        {
            using (var context = new ApplicationDbContext())//connectionString
            {
                CustomerService.signIn(context, loggedinCustomer);
                while (true)
                {
                    printOrdinaryOptionList();
                    string option = Console.ReadLine();
                    if (option == "1")
                    {
                        double amount = CustomerService.getDepositInfo(context, loggedinCustomer);
                        Console.WriteLine($"Your Account Has {amount}$");
                    }
                    else if (option == "2")
                    {
                        Console.WriteLine("Enter The Amount To Deposit");
                        try
                        {
                            double amount = Convert.ToDouble(Console.ReadLine());
                            if (amount > 0)
                            {
                                CustomerService.depositMoney(context, loggedinCustomer,amount);
                            }
                            else
                            {
                                Console.WriteLine("Don't Enter A Negative Number");
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                    else if (option == "3")
                    {
                        Console.WriteLine("Enter The Amount To be Withdrawn");
                        try
                        {
                            double amount = Convert.ToDouble(Console.ReadLine());
                            if (amount > 0)
                            {
                                CustomerService.withdrawMoney(context, loggedinCustomer, amount);
                            }
                            else
                            {
                                Console.WriteLine("Don't Enter A Negative Number");
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                    else if (option == "4")
                    {
                        Console.WriteLine("Enter The Username Of The Recipient:");
                        string recipientUsername = Console.ReadLine();
                        Customer recipient = CustomerService.retrieveCustomer(context, recipientUsername);
                        if (recipient == null)
                        {
                            Console.WriteLine("This Username Isn't Valid");
                            continue;
                        }
                        Console.WriteLine("Enter The Amount To Be Remitted:");
                        try
                        {
                            double amount = Convert.ToDouble(Console.ReadLine());
                            CustomerService.makeTransaction(context,loggedinCustomer,recipient, amount);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Please Type A Valid Numerical Input");
                        }
                    }
                    else if (option == "5")
                    {
                        CustomerService.ViewRecentTransactions(context, loggedinCustomer);
                    }
                    else if (option == "6")
                    {
                        Console.WriteLine("Enter The ID Of The Transaction:");
                        string transactionID = Console.ReadLine();
                        int numericalTransactionID;
                        if (int.TryParse(transactionID, out numericalTransactionID))
                        {
                            Transaction transaction = TransactionService.GetTransaction(context,loggedinCustomer, numericalTransactionID);
                            if (transaction != null)
                            {
                                CustomerService.acceptTransaction(context, loggedinCustomer, transaction);
                            }
                            else
                            {
                                Console.WriteLine("Please Insert A Valid Transaction ID");
                            }
                            
                        }
                        else
                        {
                            Console.WriteLine("Invalid integer format.");
                        }                
                    }
                    else if (option == "7")
                    {
                        Console.WriteLine("Enter The ID Of The Transaction:");
                        string transactionID = Console.ReadLine();
                        int numericalTransactionID;
                        if (int.TryParse(transactionID, out numericalTransactionID))
                        {
                            Transaction transaction = TransactionService.GetTransaction(context, loggedinCustomer, numericalTransactionID);
                            if (transaction != null)
                            {
                                CustomerService.rejectTransaction(context, loggedinCustomer, transaction);
                            }
                            else
                            {
                                Console.WriteLine("Please Insert A Valid Transaction ID");
                            }

                        }
                        else
                        {
                            Console.WriteLine("Invalid integer format.");
                        }
                    }
                    else if (option == "8")
                    {
                        CustomerService.listAllOperations(context, loggedinCustomer);
                    }
                    else if (option == "9")
                    {
                        CustomerService.signOut(context,loggedinCustomer);
                        Console.WriteLine("Terminating ...");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Enter An Integer From 1 To {this.OrdinaryoperationList.Length}");
                        continue;
                    }
                }
            }
        }
        #endregion
        #region Processing VIP Users
        async Task processVIPUser(Customer loggedinCustomer)
        {
            using (var context = new ApplicationDbContext())//connectionString
            {
                CustomerService.signIn(context, loggedinCustomer);
                while (true)
                {
                    printVIPOptionList();
                    string option = Console.ReadLine();
                    if (option == "1")
                    {
                        double amount = CustomerService.getDepositInfo(context, loggedinCustomer);
                        Console.WriteLine($"Your Account Has {amount}$");
                    }
                    else if (option == "2")
                    {
                        Console.WriteLine("Enter The Amount To Deposit");
                        try
                        {
                            double amount = Convert.ToDouble(Console.ReadLine());
                            if (amount > 0)
                            {
                                CustomerService.depositMoney(context, loggedinCustomer, amount);
                            }
                            else
                            {
                                Console.WriteLine("Don't Enter A Negative Number");
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                    else if (option == "3")
                    {
                        Console.WriteLine("Enter The Amount To be Withdrawn");
                        try
                        {
                            double amount = Convert.ToDouble(Console.ReadLine());
                            if (amount > 0)
                            {
                                CustomerService.withdrawMoney(context, loggedinCustomer, amount);
                            }
                            else
                            {
                                Console.WriteLine("Don't Enter A Negative Number");
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                    else if (option == "4")
                    {
                        Console.WriteLine("Enter The Username Of The Recipient:");
                        string recipientUsername = Console.ReadLine();
                        Customer recipient = CustomerService.retrieveCustomer(context, recipientUsername);
                        if (recipient == null)
                        {
                            Console.WriteLine("This Username Isn't Valid");
                            continue;
                        }
                        Console.WriteLine("Enter The Amount To Be Remitted:");
                        try
                        {
                            double amount = Convert.ToDouble(Console.ReadLine());
                            CustomerService.makeTransaction(context, loggedinCustomer, recipient, amount);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Please Type A Valid Numerical Input");
                        }
                    }
                    else if (option == "5")
                    {
                        CustomerService.ViewRecentTransactions(context, loggedinCustomer); 
                    }
                    else if (option == "6")
                    {
                        Console.WriteLine("Enter The ID Of The Transaction:");
                        string transactionID = Console.ReadLine();
                        int numericalTransactionID;
                        if (int.TryParse(transactionID, out numericalTransactionID))
                        {
                            Transaction transaction = TransactionService.GetTransaction(context, loggedinCustomer, numericalTransactionID);
                            if (transaction != null)
                            {
                                CustomerService.acceptTransaction(context, loggedinCustomer, transaction);
                            }
                            else
                            {
                                Console.WriteLine("Please Insert A Valid Transaction ID");
                            }

                        }
                        else
                        {
                            Console.WriteLine("Invalid integer format.");
                        }
                    }
                    else if (option == "7")
                    {
                        Console.WriteLine("Enter The ID Of The Transaction:");
                        string transactionID = Console.ReadLine();
                        int numericalTransactionID;
                        if (int.TryParse(transactionID, out numericalTransactionID))
                        {
                            Transaction transaction = TransactionService.GetTransaction(context, loggedinCustomer, numericalTransactionID);
                            if (transaction != null)
                            {
                                CustomerService.rejectTransaction(context, loggedinCustomer, transaction);
                            }
                            else
                            {
                                Console.WriteLine("Please Insert A Valid Transaction ID");
                            }

                        }
                        else
                        {
                            Console.WriteLine("Invalid integer format.");
                        }
                    }
                    else if (option == "8")
                    {
                        CustomerService.listAllOperations(context, loggedinCustomer);
                    }
                    else if (option == "9")
                    {
                        bool deletionResult = await CustomerService.handleVIPUserDeletion(context,loggedinCustomer);
                        if (deletionResult == true)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (option == "10")
                    {
                        Console.WriteLine("Terminating ...");
                        CustomerService.signOut(context, loggedinCustomer);
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Enter An Integer From 1 To {this.VIPoperationList.Length}");
                        continue;
                    }
                }
            }
        }
        #endregion
        public static async Task<string?> ReadInputAsync(CancellationToken token)
        {
            return await Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        return Console.ReadLine();
                    }
                    Thread.Sleep(50);
                }
                return null;
            }, token);
        }


    }
}

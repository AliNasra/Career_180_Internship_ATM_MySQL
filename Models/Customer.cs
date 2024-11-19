using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class Customer
    {
        public int        customerID;
        public string     userName;
        public string     password;
        public double     bankDeposit;
        public string     email;
        public DateTime   birthDate;
        public DateTime   accountDate;
        public DateTime   accountTimer;
        public int        operationCounter;
        public string     customerType;
        public bool       activityStatus;
        public List<Operation>         operations            = new List<Operation>();
        public List<Transaction>       sentTransactions      = new List<Transaction>();
        public List<Transaction>       receivedTransactions  = new List<Transaction>();
    }
}

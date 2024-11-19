﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class Operation : IComparable<Operation>

    {
        public int      operationID   { get; set; }
        public DateTime operationDate { get; set; }
        public int      customerID    { get; set; }
        public Customer customer      { get; set; }    
        public string   operationType { get; set; }
        public double?  moneyAmount   { get; set; }
        public bool     successStatus { get; set; }

        public int CompareTo(Operation  other)
        {
            return this.operationDate.CompareTo(other.operationDate);
        }
        public string getManagerialOperationString()
        {
            return $"{this.operationDate:dddd, MMMM d, yyyy h:mm tt} - Operation Type: {this.operationType}";
        }
        public string getFinancialOperationString()
        {
            return $"{this.operationDate:dddd, MMMM d, yyyy h:mm tt} - Operation Type: {this.operationType} - Money: {this.moneyAmount}";
        }
    }
}

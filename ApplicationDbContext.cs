using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Models;
using ConsoleApp1.Configurations;
using ConsoleApp1.Helpers;
using ConsoleApp1.Services;


namespace ConsoleApp1
{
    public class ApplicationDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATM;Integrated Security=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new CustomerEntityTypeConfiguration().Configure(modelBuilder.Entity<Customer>());
            new OperationEntityTypeConfiguration().Configure(modelBuilder.Entity<Operation>());
            new TransactionEntityTypeConfiguration().Configure(modelBuilder.Entity<Transaction>());
        }
        public DbSet<Customer>    Customers    { get; set; }
        public DbSet<Operation>   Operations   { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}

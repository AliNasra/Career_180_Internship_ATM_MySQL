using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Configurations
{
    public class CustomerEntityTypeConfiguration:IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(x => x.userName).IsClustered(false);
            builder.HasKey(x => x.customerID).HasName("customerID");
            builder.Property(b => b.userName).IsRequired();
            builder.Property(b => b.password).IsRequired();
            builder.Property(b => b.bankDeposit).IsRequired();
            builder.Property(b => b.email).IsRequired();
            builder.Property(b => b.birthDate).IsRequired(); 
            builder.Property(b => b.accountTimer).IsRequired();
            builder.Property(b => b.accountDate).IsRequired();
            builder.Property(b => b.operationCounter).IsRequired();
            builder.Property(b => b.customerType).IsRequired();
            builder.Property(b => b.activityStatus).IsRequired();
            builder.Property(b => b.bankDeposit).HasDefaultValue(0);
            builder.Property(b => b.operationCounter).HasDefaultValue(0);
            builder.Property(b => b.activityStatus).HasDefaultValue(true);
        }
    }
}

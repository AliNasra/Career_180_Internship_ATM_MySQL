using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Models;

namespace ConsoleApp1.Configurations
{
    public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");
            builder.HasKey(x => x.transactionID).HasName("transactionID");
            builder.Property(b => b.transferredMoney).IsRequired();
            builder.Property(b => b.senderPretransferDeposit).IsRequired();
            builder.Property(b => b.recipientPretransferDeposit).IsRequired();
            builder.Property(b => b.transactionState).IsRequired();
            builder.Property(b => b.transactionTime).IsRequired();
            builder.Property(b => b.successStatus).IsRequired();
            builder.Property(b => b.IsCompleteTransfer).IsRequired();
            builder.Property(b => b.IsCompleteTransfer).HasDefaultValue(false);
            builder.Property(b=>b.senderCustomerID).HasColumnType("int").HasColumnName("senderID");
            builder.Property(b=>b.recipientCustomerID).HasColumnType("int").HasColumnName("recipientID");
            builder.HasOne(p=>p.senderCustomer).WithMany(p=>p.sentTransactions).HasForeignKey(p => p.senderCustomerID).OnDelete(DeleteBehavior.ClientSetNull).IsRequired(); 
            builder.HasOne(p=>p.recipientCustomer).WithMany(p=>p.receivedTransactions).HasForeignKey(p => p.recipientCustomerID).OnDelete(DeleteBehavior.ClientSetNull).IsRequired(); 
        }
    }
}

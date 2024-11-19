using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Models;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsoleApp1.Configurations
{
    public class OperationEntityTypeConfiguration:IEntityTypeConfiguration<Operation>
    {
        public void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder.ToTable("Operations");
            builder.HasKey(x => x.operationID).HasName("operationID");
            builder.HasOne(p => p.customer).WithMany(p => p.operations).HasForeignKey(p => p.customerID).OnDelete(DeleteBehavior.ClientSetNull).IsRequired();
            builder.Property(b => b.operationDate).IsRequired();
            builder.Property(b => b.operationType).IsRequired();
            builder.Property(b => b.successStatus).IsRequired();
        }
    }
}

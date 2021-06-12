using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Mapping
{
  public class DefaultConvention : Convention
  {
    public DefaultConvention()
    {
      this.Types()
          .Where(t => typeof(BaseEntity).IsAssignableFrom(t))
          .Configure(t =>
          {
            t.ToTable($"{t.ClrType.Name}")
            .HasKey("Id");
          });

      this.Properties<string>()
          .Configure(c =>
          {
            c.HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsOptional();
          });

      this.Properties<DateTime>()
          .Configure(c =>
          {
            c.HasColumnType("datetime")
            .IsRequired();
          });
      this.Properties<decimal>()
          .Configure(c =>
          {
            c.HasPrecision(10, 2);
            c.IsRequired();
          });
    }
  }
}

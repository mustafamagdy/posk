using Geeky.POSK.ORM.Contect.Core.Mapping;
using Geeky.POSK.Models;

namespace Geeky.POSK.ORM.Mapping
{
  public class SessionPaymentMap : BaseMap<SessionPayment>
  {
    public SessionPaymentMap()
      //: base("SessionPayment")
    {
      Property(p => p.PaymentRefNumber).HasColumnType("varchar").HasMaxLength(255).IsRequired();
      Property(p => p.StackedAmount).IsRequired();
      Property(p => p.IsJammed).IsRequired();
      Property(p => p.CashCodeStatus).HasColumnType("varchar").HasMaxLength(255).IsOptional();

      HasRequired(x => x.Session).WithMany(x=>x.Payments).WillCascadeOnDelete(false);
    }
  }
}

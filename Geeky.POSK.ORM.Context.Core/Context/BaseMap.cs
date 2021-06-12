using Geeky.POSK.Infrastructore.Core;
using System;
using System.Data.Entity.ModelConfiguration;

namespace Geeky.POSK.ORM.Contect.Core.Mapping
{
  public abstract class BaseMap<TEntity> : EntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
  {
  }
}

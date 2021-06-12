using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.UnitOfWork.Exceptions;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Infrastructore.Helpers;
using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.ORM.Contect.Core.DbInitializers;
//using Geeky.POSK.ORM.Contect.Core.IdGenerator;
using Geeky.POSK.ORM.Context.EF.Migrations;
using Geeky.POSK.ORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Geeky.POSK.ORM.Context.EF
{
  public class AppDbContext : DbContext, IAppDbContext
  {
    public AppDbContext()
      : this(new CustomDatabaseMigrationToLatestVersion<AppDbContext, Configuration>())
    {
    }

    public AppDbContext(IDatabaseInitializer<AppDbContext> initializer)
      : base("DefaultConnection")
    {
      Database.SetInitializer(initializer);
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Conventions.AddFromAssembly(typeof(DefaultConvention).Assembly);
      modelBuilder.Configurations.AddFromAssembly(typeof(VendorMap).Assembly);
      base.OnModelCreating(modelBuilder);
    }


    //public virtual DbSet<Terminal> Terminals { get; set; }
    //public virtual DbSet<Pin> Pins { get; set; }
    //public virtual DbSet<Product> Products { get; set; }
    //public virtual DbSet<Vendor> Vendors { get; set; }
    //public virtual DbSet<TransferTrx> TransferTrxs { get; set; }
    //public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
    //public virtual DbSet<ClientSession> ClientSessions { get; set; }
    //public virtual DbSet<SessionPayment> SessionPayments { get; set; }

  }
}

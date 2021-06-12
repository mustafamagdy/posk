using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Contect.Core.DbInitializers
{
  public class CustomDatabaseMigrationToLatestVersion<Context, Configuration> : 
    MigrateDatabaseToLatestVersion<Context, Configuration>
    where Context : DbContext
    where Configuration : DbMigrationsConfiguration<Context>, new()
  {
    public override void InitializeDatabase(Context context)
    {
      base.InitializeDatabase(context);
    }

  }
}

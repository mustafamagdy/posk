using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Helpers;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.ORM.Contect.Core.DbInitializers;
//using Geeky.POSK.ORM.Contect.Core.IdGenerator;
using Geeky.POSK.ORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;

namespace Geeky.POSK.ORM.Context.EF
{


  //public class BaseContext : DbContext, IDataContext
  //{
  //  public BaseContext()
  //    : this("name=DefaultConnection")
  //  { }
  //  public BaseContext(string connectionString)
  //    : this(connectionString, new CustomCreateIfNotExist<BaseContext>())
  //  { }
  //  public BaseContext(string connectionString, IDatabaseInitializer<BaseContext> initializer)
  //    : base(connectionString)
  //  {
  //    Database.SetInitializer(initializer);
  //    initializer.InitializeDatabase(this);
  //  }

  //  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  //  {
  //    base.OnModelCreating(modelBuilder);
  //    modelBuilder.Conventions.Remove<StoreGeneratedIdentityKeyConvention>();
  //    var mappingAssembly = typeof(PinMap).Assembly;
  //    modelBuilder.Configurations.AddFromAssembly(mappingAssembly);

  //  }
  //  public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
  //  {
  //    return Database.BeginTransaction(isolationLevel);
  //  }

  //  public IQueryable<TEntity> Query<TEntity>() where TEntity : class
  //  {
  //    return Set<TEntity>().AsQueryable();
  //  }
  //}


  //public abstract class BaseDbContext<TEntity, TKey> : IDataContext<TEntity, TKey>
  //where TEntity : class, IBaseEntity<TKey>
  //{
  //  protected DbContext Context;
  //  public BaseDbContext(BaseContext context) { Context = context; }

  //  private IIdentityGenerator<TEntity, TKey> _generator;
  //  IIdentityGenerator<TEntity, TKey> generator { get { return (_generator != null) ? _generator : _generator = new SeqHiloGenerator<TEntity, TKey>(Context); } }
  //  public TEntity FindById(TKey key)
  //  {
  //    var result = Context.Set<TEntity>().Find(key);
  //    return result;
  //  }
  //  public IEnumerable<TEntity> FindAllBy(Expression<Func<TEntity, bool>> query)
  //  {
  //    return Context.Set<TEntity>().Where(query).ToList();
  //  }
  //  public TEntity Save(TEntity entity)
  //  {
  //    var seqHiloEntity = entity as TEntity;
  //    if (seqHiloEntity != null)
  //      IdGeneratorHelper<TEntity, TKey>.TrySetId(seqHiloEntity, generator);
  //    Context.Set<TEntity>().Attach(entity);
  //    Entry(entity).State = EntityState.Added;
  //    return entity;
  //  }

  //  public TEntity Load(TKey key)
  //  {
  //    return FindById(key);
  //  }
  //  public void Remove(TEntity entity)
  //  {
  //    new CascadeDelete<TKey>(Context).Cascade(entity);
  //  }
  //  public void Remove(TKey key)
  //  {
  //    Remove(FindById(key));
  //  }
  //  public IQueryable<TEntity> Query()
  //  {
  //    return Context.Set<TEntity>().AsQueryable();
  //  }
  //  public DbEntityEntry<TEntity> Entry(TEntity entity)
  //  {
  //    return Context.Entry(entity);
  //  }
  //  public TEntity Attach(TEntity entity)
  //  {
  //    return Context.Set<TEntity>().Attach(entity);
  //  }
  //  public bool Any()
  //  {
  //    return Context.Set<TEntity>().Any();
  //  }
  //  public bool Any(Expression<Func<TEntity, bool>> predicate)
  //  {
  //    return Context.Set<TEntity>().Any(predicate);
  //  }

  //  public virtual int SaveChanges()
  //  {
  //    return Context.SaveChanges();
  //  }

  //  private class ConcurrentPropertyWrapper : IConcurrentEntity
  //  {
  //    public ConcurrentPropertyWrapper(byte[] rowVersion)
  //    {
  //      RowVersion = rowVersion;
  //    }

  //    public byte[] RowVersion { get; }

  //    public static readonly string _rowVerionPropertyName = ReflectionHelpers.GetPropertyName<ConcurrentPropertyWrapper>(x => x.RowVersion);

  //    public static string RowVersionPropertyName
  //    {
  //      get
  //      { return _rowVerionPropertyName; }
  //    }
  //  }

  //}

}

using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using System.Linq.Expressions;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Repository.Core.Interface.Base;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Infrastructore.Helpers;
//using Geeky.POSK.ORM.Context.Core.Extensions;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace Geeky.POSK.Repository.Core.Base
{

  public abstract class BaseRepository<TEntity, TKey, TContext> : IRepository<TEntity, TKey>
  where TContext : DbContext  /*IDataContext<TEntity, TKey>*/
  where TEntity : class, IBaseEntity<TKey>, new()
  {
    public abstract TEntity Add(TEntity entity);
    public abstract IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
    public abstract IEnumerable<TEntity> FindAll();
    public abstract TEntity Get(TKey id);
    public abstract void Remove(TEntity entity);
    public abstract TEntity Update(TEntity entity);
    public abstract IQueryable<TEntity> Query();
  }

  public abstract class BaseRepository<TEntity, TKey> : BaseRepository<TEntity, TKey,DbContext /*IDataContext<TEntity, TKey>*/>
  where TEntity : class, IBaseEntity<TKey>, new()
  {
    //IDataContext<TEntity, TKey> _context;
    DbContext _context;
    public BaseRepository(DbContext context)
    //public BaseRepository(IDataContext<TEntity, TKey> context)
    {
      _context = context;
    }
    public override TEntity Add(TEntity entity)
    {
      var saved = _context.Set<TEntity>().Add(entity);
      _context.SaveChanges();
      return saved;
    }
    public override IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
    {
      return _context.Set<TEntity>().Where(predicate);
    }
    public override TEntity Get(TKey id)
    {
      return _context.Set<TEntity>().Find(id);
    }
    public override IEnumerable<TEntity> FindAll()
    {
      return _context.Set<TEntity>().AsEnumerable();
    }
    public bool Any()
    {
      return _context.Set<TEntity>().Any();
    }
    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
      return _context.Set<TEntity>().Any(predicate);
    }
    public override void Remove(TEntity entity)
    {
      _context.Set<TEntity>().Remove(entity);
      _context.SaveChanges();
    }
    public override TEntity Update(TEntity entity)
    {
      // _context.Update(entity);
      _context.Set<TEntity>().Attach(entity);
      _context.Entry(entity).State = EntityState.Modified;
      _context.SaveChanges();
      return entity;
    }
    public override IQueryable<TEntity> Query()
    {
      return _context.Set<TEntity>().AsQueryable();
    }
  }

}

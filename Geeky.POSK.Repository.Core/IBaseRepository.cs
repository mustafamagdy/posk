using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.Linq.Expressions;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.ORM.Contect.Core;
using System.Data.Entity;

namespace Geeky.POSK.Repository.Core.Interface.Base
{
  public interface IRepository
  { }

  public interface IRepository<TEntity> : IRepository<TEntity, int> { }
  public interface IRepository<TEntity, TKey> : IRepository
  {
    TEntity Add(TEntity entity);
    void Remove(TEntity entity);
    TEntity Get(TKey id);
    TEntity Update(TEntity entity);
    IEnumerable<TEntity> FindAll();
    IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> Query();
  }

  public interface IBaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
   where TEntity : BaseEntity<TKey>, new()
  {

  }

  //public interface IBaseRepository<TEntity, TKey, TContext> : IRepository<TEntity, TKey>
  // where TContext :DbContext  /*IDataContext<TEntity, TKey>*/
  // where TEntity : BaseEntity<TKey>, new()
  //{

  //}

}

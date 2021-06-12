using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.UnitOfWork.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Contect.Core
{
  /// <summary>
  /// Handles delete and cascading related records
  /// Use with caution calling Cascade method will trigger lazy loading on all 
  /// navigation properties that have WillCascadeOnDelete set to true in mapping
  /// [This Class Is Not Thread Safe]
  /// </summary>
  public class CascadeDelete<TKey>
  {
    private HashSet<IBaseEntity<TKey>> _checked = new HashSet<IBaseEntity<TKey>>();
    DbContext _dbContext;
    ObjectContext _internalContext;

    //TODO: change this to depend on either DbContext directly or on IDbContext interface
    public CascadeDelete(DbContext context)
    {
      _dbContext = context;
      _internalContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
    }
    /// <summary>
    /// Mark an entity is remove in the db context and makes sure to cascade delete 
    /// based on entity mapping info
    /// </summary>
    /// <param name="entity">entity to be remove and cascaded</param>
    public void Cascade(IBaseEntity<TKey> entity)
    {
      Remove(entity);

      _checked.Clear();
    }
    /// <summary>
    /// Marks an entity as deleted along all its cascade delete enabled relations
    /// </summary>
    /// <param name="entity">entity to be deleted</param>
    private void Remove(IBaseEntity<TKey> entity)
    {
      if (entity == null)
        return;
      //if (IsChecked(entity))
      //    return;
      var entry = _dbContext.Entry(entity);
      IsChecked(entity);
      //run delete interceptor
      //ServiceLocator.Current.GetAllInstances<ITransactionObserver>()
      //  .ForEach(e => e.OnDeleteEntity(entity, new OriginalValueRetriever(entry)));
      RemoveRelations(entry);

      //Remove the entity from the internal DbSet
      _dbContext.Set(entity.GetType()).Remove(entity);

    }
    /// <summary>
    /// From a DbEntityEntry instance lazy loaded enabled properties 
    /// will be triggered and each item or item in collection will be removed as well
    /// </summary>
    /// <param name="entry">Context entry of the entity that its relatiions needs to be removed</param>
    private void RemoveRelations(DbEntityEntry entry)
    {
      //TODO: please review this (a bug may be raised from the filter u.FromEndMember.DeleteBehaviour || u.ToEndMember.DeleteBehvaiour => (shoule cascade happens if any of the ends has cacsde or if just the FromEndMeber????)

      ObjectStateEntry oState;
      _internalContext.ObjectStateManager.TryGetObjectStateEntry(entry.Entity, out oState);
      var navigations = ((EntityType)oState.EntitySet.ElementType)
          .NavigationProperties.Where(u =>
            u.FromEndMember.RelationshipMultiplicity != RelationshipMultiplicity.Many //&& 
                                                                                      //(u.FromEndMember.DeleteBehavior == OperationAction.Cascade
                                                                                      //|| u.ToEndMember.DeleteBehavior == OperationAction.Cascade)
            );
      foreach (var nav in navigations)
      {
        //get the value
        var temp = entry.Entity.GetType()
            .GetProperty(nav.Name)
            .GetValue(entry.Entity);
        if (temp == null)
          continue;

        //if not cascading it should be null or empty
        if
          (nav.FromEndMember.DeleteBehavior == OperationAction.None && nav.ToEndMember.DeleteBehavior == OperationAction.None)
        {
          if (temp != null)
          {

            if (temp is BaseEntity<int>)
            {
              if (_dbContext.Entry(temp).State == EntityState.Deleted)
                continue;
              throw new RelationExistException(entry.Entity, temp);
            }
            var col = (temp as IEnumerable<BaseEntity<int>>);
            if (col != null && col.Any())
            {
              if (_dbContext.Entry(col.First()).State == EntityState.Deleted)
                continue;
              throw new RelationExistException(entry.Entity, col.First());
            }

          }
        }
        else //cascade
        {
          //TODO: Potential performance impact
          /*
          possible fix to create and compile an expression that access 
          this navigatio property and cach it in 
          a hashtable using the navigatio property as a key
          */
          if (nav.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
          {
            var col = (temp as IEnumerable<BaseEntity<TKey>>).ToArray();
            foreach (var related in col)
            {

              if (!IsChecked(related))
                Remove(related);

            }
          }
          else if (nav.ToEndMember.RelationshipMultiplicity == nav.ToEndMember.RelationshipMultiplicity && nav.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
          {
            var related = (BaseEntity<TKey>)temp;
            if (!IsChecked(related))
              Remove(related);

          }
          else if (nav.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One && nav.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne)
          {

            var related = (BaseEntity<TKey>)temp;
            if (!IsChecked(related))
              Remove(related);

          }
        }

      }
    }
    /// <summary>
    /// Returns true if entity has been processed before or already has been marked as delted somewhere else
    /// </summary>
    /// <param name="obj">entity instance</param>
    /// <returns>[True] Or [False] based on whether entity instance has been deleted or processed before or not </returns>
    bool IsChecked(IBaseEntity<TKey> obj)
    {
      if (_checked.Contains(obj) ||
          _dbContext.Entry(obj).State == EntityState.Deleted)
        return true;
      _checked.Add(obj);
      return false;
    }
  }

  //public class RelationExistException : ApplicationException
  //{

  //  private readonly object _source;
  //  private readonly object _related;
  //  public RelationExistException(object source, object relation, Exception innerException = null)
  //  {
  //    _source = source;
  //    _related = relation;
  //  }

  //  public object SourceEntity { get { return _source; } }
  //  public object RelatedEntity { get { return _related; } }
  //}

}

using Geeky.POSK.Infrastructore.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Contect.Core.Context.Bulk
{
  public class MetaData
  {
    public MetaData()
    {
      Tables = new HashSet<Table>();
    }

    public static MetaData Discover(IObjectContextAdapter objectContextAdapter)
    {
      var efMetadata = objectContextAdapter.ObjectContext.MetadataWorkspace;
      var objectCollection = (ObjectItemCollection)efMetadata.GetItemCollection(DataSpace.OSpace);
      var metaData = new MetaData();

      //get clr class
      var entityTypes = efMetadata.GetItems<EntityType>(DataSpace.OSpace);
      //get associations 
      var associations = efMetadata.GetItems<AssociationType>(DataSpace.SSpace);

      foreach (var entity in entityTypes)
      {
        //get sets of it in db context
        var entitySet = efMetadata.GetItems<EntityContainer>(DataSpace.CSpace)
                                .Single()
                                .EntitySets
                                .Single(x => x.ElementType.Name == entity.Name);

        //get mapping of it (between conceptual and storage spaces)
        var mappingOfIt = efMetadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                                  .Single()
                                  .EntitySetMappings
                                  .Single(x => x.EntitySet == entitySet);

        var entityMapping = mappingOfIt.EntityTypeMappings.Single();
        var mappingFragment = entityMapping.Fragments.Single();

        var tbl = mappingFragment.StoreEntitySet;
        var tableName = tbl.Table;
        var properies = mappingFragment.PropertyMappings
                                       .OfType<ScalarPropertyMapping>();
        //((System.Data.Entity.Core.Metadata.Edm.ClrEntityType)(new System.Collections.Generic.Mscorlib_CollectionDebugView<System.Data.Entity.Core.Metadata.Edm.EntityType>(entityTypes).Items[0])).ClrType
        var fullClrName = $"{mappingOfIt.EntitySet.ElementType.FullName}";
        metaData.AddTable(tbl.Schema, tableName, tbl.Name, fullClrName);
        foreach (var prop in properies)
        {
          metaData.AddColumnToTable(tableName, new Column
          {
            ColumnName = prop.Column.Name,
            Type = prop.Property.UnderlyingPrimitiveType.ClrEquivalentType,
            IsDbGenerated = prop.Column.IsStoreGeneratedIdentity
          });
        }

        var navigationProperties = mappingOfIt.EntityTypeMappings
                                              .Single().EntityType.NavigationProperties
                                              .Where(x => x.RelationshipType is AssociationType)
                                              .ToArray();

        var relations = new List<Relation>();
        foreach (var navProp in navigationProperties)
        {
          var navPropName = navProp.Name;
          var relType = (AssociationType)navProp.RelationshipType;
          var association = associations.Single(x => x.Name == relType.Name);

          if (!relations.Any(m => m.Name == association.Name))
          {
            var relationMapping = new Relation
            {
              NavigationPropertyName = navProp.Name,
              BuiltInTypeKind = navProp.TypeUsage.EdmType.BuiltInTypeKind,
              Name = relType.Name,
              FromType = association.Constraint.FromProperties.Single().DeclaringType.Name,
              FromProperty = association.Constraint.FromProperties.Single().Name,
              ToType = association.Constraint.ToProperties.Single().DeclaringType.Name,
              ToProperty = association.Constraint.ToProperties.Single().Name,
              RelationUnderlyingType = association.Constraint.ToProperties.Single().UnderlyingPrimitiveType.ClrEquivalentType
            };
            relations.Add(relationMapping);
          }
        }

        var relationTo = relations.Where(m => m.ToType == entity.Name).ToArray();
        var relationFrom = relations.Where(m => m.FromType == entity.Name).ToArray();

        metaData.SetRelations(tableName, relationTo, relationFrom);
      }
      return metaData;
    }
    public static DataTable CreateDataTableStructure<T>(IObjectContextAdapter objectContextAdapter)
    {
      MetaData metaData = Discover(objectContextAdapter);
      var dt = new DataTable();
      var tblMetaData = metaData.Entity<T>();
      typeof(T).GetProperties().ForEach(p =>
      {
        Type pType = null;
        if (Nullable.GetUnderlyingType(p.GetType()) != null)
          pType = Nullable.GetUnderlyingType(p.GetType());
        else
          pType = p.PropertyType;

        if (primitives.Contains(pType) && p.Name != "RowVersion")
          dt.Columns.Add(p.Name, p.PropertyType);
        else
        {
          var relationTo = tblMetaData.RelationTo.FirstOrDefault(x => x.NavigationPropertyName == p.Name);
          if (relationTo != null)
            dt.Columns.Add(relationTo.ToProperty, relationTo.RelationUnderlyingType);
        }
      });
      return dt;
    }

    private void SetRelations(string tableName, Relation[] toForeignKeyMappings, Relation[] fromForeignKeyMappings)
    {
      Tables.Single(x => x.TableName == tableName)
            .RelationTo = toForeignKeyMappings;
      Tables.Single(x => x.TableName == tableName)
            .RelationFrom = fromForeignKeyMappings;

      //foreach (var relationFrom in fromForeignKeyMappings)
      //{
      //  Tables.Single(x => x.TableName == tableName)
      //    .AddColumn(relationFrom.ToProperty, typeof(int), false);
      //}

    }

    private static Type GetClrType(string fullName)
    {
      return Type.GetType(fullName);
      return AppDomain.CurrentDomain
                      .GetAssemblies()
                      .SelectMany(x => x.GetTypes())
                      .Where(x => x.FullName == fullName)
                      .Single();
    }
    public Table Entity<T>()
    {
      var t = typeof(T);
      return Tables.Single(x => x.ClrName == t.Name);
    }
    public Table Entity(Type t)
    {
      return Tables.Single(x => x.ClrName == t.Name);
    }
    public Table Entity(string clrName)
    {
      return Tables.Single(x => x.ClrName == clrName);
    }
    public HashSet<Table> Tables { get; private set; }
    private void AddTable(string schema, string tableName, string clrName, string fullClrName)
    {
      Tables.Add(new Table { Schema = schema, TableName = tableName, ClrName = clrName, FullClrName = fullClrName /*Type = type*/ });
    }
    private void AddColumnToTable(string tableName, Column column)
    {
      Tables.Single(x => x.TableName == tableName)
            .AddColumn(column.ColumnName, column.Type, column.IsDbGenerated);
    }

    public class Table
    {
      public Table()
      {
        Columns = new HashSet<Column>();
      }
      public string Schema { get; set; }
      public string TableName { get; set; }
      public string ClrName { get; set; }
      //public Type Type { get; set; }
      public string FullClrName { get; set; }
      public HashSet<Column> Columns { get; private set; }
      public Relation[] RelationTo { get; internal set; }
      public Relation[] RelationFrom { get; internal set; }

      public void AddColumn(string columnName, Type type, bool isDbGenerated = false)
      {
        Columns.Add(new Column { ColumnName = columnName, Type = type, IsDbGenerated = isDbGenerated });
      }

    }
    public class Column
    {
      public bool IsDbGenerated { get; set; }
      public string ColumnName { get; set; }
      public Type Type { get; set; }
    }
    public class Relation
    {
      public BuiltInTypeKind BuiltInTypeKind { get; set; }
      public string NavigationPropertyName { get; set; }
      public string Name { get; set; }
      public string FromType { get; set; }
      public string FromProperty { get; set; }
      public string ToType { get; set; }
      public string ToProperty { get; set; }
      public Type RelationUnderlyingType { get; set; }
    }


    static Type[] primitives = new[]
                  {
                      typeof (Enum),typeof (String),typeof (Char),typeof (Int16),
                      typeof (Guid),typeof (Boolean), typeof (Byte), typeof (Double),
                      typeof (Int32), typeof (Int64), typeof (Single),
                      typeof (Decimal), typeof (SByte), typeof (UInt16),
                      typeof (UInt32), typeof (UInt64), typeof (DateTime), 
                      typeof (byte), typeof (byte[]), 
                      //typeof (TimeSpan),typeof (DateTimeOffset),
                };
  }
}

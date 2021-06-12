using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.ORM.Contect.Core.Context.Bulk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Contect.Core.Context
{
  public static class BulkInsertExtension
  {
    private static MetaData metaData = null;

    private static void Batch<T>(this IEnumerable<T> source, int batchSize, Action<IEnumerable<T>> action)
    {
      while (source.Any()) { action(source.Take(batchSize)); source = source.Skip(batchSize); }
    }

    public static void BulkInsert<T>(this DbContext _context, DataTable readyToBeInsert, SqlTransaction transaction = null)
    {
      var _cnn = transaction == null ? (_context.Database.Connection as SqlConnection) : transaction.Connection;
      if (_cnn.State == ConnectionState.Closed)
        _cnn.Open();

      metaData = MetaData.Discover(_context);
      var iT = typeof(T);
      var tbl = metaData.Entity(iT);
      bool selfCommit = transaction == null;
      transaction = transaction ?? _cnn.BeginTransaction();

      SqlBulkCopy bulk = new SqlBulkCopy(_cnn, SqlBulkCopyOptions.Default, transaction);
      foreach (DataColumn col in readyToBeInsert.Columns)
      {
        bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(col.ColumnName, col.ColumnName));
      }

      bulk.BulkCopyTimeout = 0;
      bulk.BatchSize = readyToBeInsert.Rows.Count;
      bulk.DestinationTableName = tbl.Schema + "." + tbl.TableName;
      bulk.WriteToServer(readyToBeInsert);

      if (selfCommit)
        transaction.Commit();
    }

    public static void BulkInsert(this DbContext _context, IList entities, SqlTransaction transaction = null)
    {
      if (entities.Count == 0) return;
      Stopwatch sw = new Stopwatch();
      sw.Start();
      var _cnn = (_context.Database.Connection as SqlConnection);
      if (_cnn.State == ConnectionState.Closed)
        _cnn.Open();

      metaData = MetaData.Discover(_context);
      var iT = entities[0].GetType();
      var tbl = metaData.Entity(iT);
      transaction = transaction ?? _cnn.BeginTransaction();

      BulkInsertInternal(tbl, entities, _cnn, transaction);

      transaction.Commit();

      sw.Stop();
      var e = sw.Elapsed;
      Trace.WriteLine($@"Total time {e.Hours} : {e.Minutes} : {e.Seconds} : {e.Milliseconds}");
    }

    private static void BulkInsertInternal(MetaData.Table tblMapping, IList entities, SqlConnection cnn, SqlTransaction trx)
    {
      if (entities.Count == 0) return;

      SqlBulkCopy bulk = new SqlBulkCopy(cnn, SqlBulkCopyOptions.Default, trx);
      bulk.BulkCopyTimeout = 0;
      bulk.BatchSize = entities.Count;

      var dt = new DataTable(tblMapping.Schema + "." + tblMapping.TableName);
      bulk.DestinationTableName = dt.TableName;
      foreach (var p in tblMapping.Columns)
      {
        dt.Columns.Add(p.ColumnName, p.Type);
        bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping
        {
          DestinationColumn = p.ColumnName,
          SourceColumn = p.ColumnName
        });
      }


      dt.Clear();
      entities.ForEach(entity =>
      {
        Type tEntity = entity.GetType();
        DataRow row = dt.NewRow();
        foreach (var p in tblMapping.Columns)
        {
          if (p.IsDbGenerated) continue;
          var pValue = tEntity.GetProperty(p.ColumnName).GetValue(entity);
          row[p.ColumnName] = pValue == null ? DBNull.Value : pValue;
        }
        dt.Rows.Add(row);
      });

      bulk.WriteToServer(dt);

      var relatedItems = new List<object>();
      var iT = entities[0].GetType();
      foreach (var relationFrom in tblMapping.RelationFrom)
      {
        var tRelationToType = metaData.Entity(relationFrom.ToType);
        //rule: no identity and graph insertion
        if (tRelationToType.Columns.Any(x => x.IsDbGenerated)) continue;

        foreach (var entity in entities)
        {
          relatedItems.AddRange((IEnumerable<object>)iT.GetProperty(relationFrom.NavigationPropertyName).GetValue(entity));
        }

        BulkInsertInternal(tRelationToType, relatedItems, cnn, trx);

      }

    }

  }
}

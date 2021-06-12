using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.UnitOfWork.Exceptions
{

  public class RelationExistException : BaseUnitOfWorkException
  {
    public RelationExistException() : this(null)
    {

    }

    public RelationExistException(Exception innerException)
      : base(DataAccessErrorType.RelationExist, "Data conflict error\r\nPossibaly data has been changed since ", innerException)
    {

    }
    private readonly object _source;
    private readonly object _related;
    public RelationExistException(object source, object relation, Exception innerException = null) 
      : base(DataAccessErrorType.RelationExist, "Data conflict error\r\nPossibaly data has been changed since ", innerException)
    {
      _source = source;
      _related = relation;
    }

    public object SourceEntity { get { return _source; } }
    public object RelatedEntity { get { return _related; } }
  }
}

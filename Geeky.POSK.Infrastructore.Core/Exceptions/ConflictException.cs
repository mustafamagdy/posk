using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.UnitOfWork.Exceptions
{

  public class ConflictException : BaseUnitOfWorkException
  {
    
    private object _source;
    private string _op;
    public ConflictException(object source, string op, Exception innerException = null) 
      : base(DataAccessErrorType.Conflict, innerException)
    {
      _source = source;
    }
    public object SourceEntity
    {
      get { return _source; }
    }
    public string Operation
    {
      get
      {
        return _op;
      }
    }
  }
}

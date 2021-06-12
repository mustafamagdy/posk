using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.UnitOfWork.Exceptions
{

  public abstract class BaseUnitOfWorkException : ApplicationException
  {
    private DataAccessErrorType _errorType;
    public DataAccessErrorType ErrorType
    {
      get
      {
        return _errorType;
      }
    }
    public BaseUnitOfWorkException(DataAccessErrorType errorType):this(errorType, string.Empty,null)
    {

    }
    public BaseUnitOfWorkException(DataAccessErrorType errorType, Exception innerException) : this(errorType, string.Empty, innerException)
    {

    }

    public BaseUnitOfWorkException(DataAccessErrorType errorType, string message):this(errorType, message,null)
    {

    }
    
    public BaseUnitOfWorkException(DataAccessErrorType errorType,string message, Exception innerException) : base(string.Empty, innerException)
    {
      _errorType = errorType;
    }

  }
}

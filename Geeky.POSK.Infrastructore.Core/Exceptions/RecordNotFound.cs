using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.UnitOfWork.Exceptions
{

  public class RecordNotFoundException : BaseUnitOfWorkException
  {
    
    public RecordNotFoundException(object id):this(id,null)
    {

    }
    
    public RecordNotFoundException(object id, Exception innerException) 
      : base(DataAccessErrorType.Conflict, $"Record with id: {id} doesn't exist", innerException)
    {

    }
  }
}

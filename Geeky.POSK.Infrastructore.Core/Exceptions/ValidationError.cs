using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.UnitOfWork.Exceptions
{
  public class EntityValidationError
  {
    
    public string PropertyName { get; set; }
    public string Message { get; set; }
    public override string ToString()
    {
      return $"{PropertyName}: {Message}";
    }
  }
  public class DataValidationResult
  {
    public object Entity { get; set; }
    public ICollection<EntityValidationError> Errors { get; set; } = new List<EntityValidationError>();
    public override string ToString()
    {
      return $"Data Validation Error for [{Entity.GetType().Name}] \r\n\t" + string.Join("\r\n\t", Errors);
    }
      
  }
  public class DataValidationException : BaseUnitOfWorkException
  {
    private IEnumerable<DataValidationResult> _result;

    
    public DataValidationException(IEnumerable<DataValidationResult> result):this(result, null)
    {

    }
    
    public DataValidationException(IEnumerable<DataValidationResult> result, Exception innerException) 
      : base(DataAccessErrorType.ValidationError, $"Data validation Error", innerException)
    {
      _result = result;
    }
    public IEnumerable<DataValidationResult> ValidationResult
    {
      get
      {
        return _result;
      }
    }
    public override string ToString()
    {
      return  string.Join("\r\n", _result);
    }
  }
}

using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core.Enums;
using Geeky.POSK.Infrastructore.Core.Exceptions;
using Geeky.POSK.Infrastructore.Core.Logging;
using Geeky.POSK.Infrastructore.Core.UnitOfWork.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Services.Exceptions
{
  public class ServerFaultsFactory : IServerFaultsFactory
  {
    private ILogger _logger
    {
      get
      {
        return ServiceLocator.Current.GetInstance<ILogger>();
      }
    }
    public ServerFaultsFactory()
    {

    }

    public void Error(ServerFaults faultType)
    {
      var fault = GetFaultException(GetExceptionCodeForFault(faultType));
      Throw(fault);
    }


    public void Error(ServerFaults faultType, Exception exception)
    {
      var fault = GetFaultException(GetExceptionCodeForFault(faultType), exception.Message, exception);
      Throw(fault);
    }

    public void Error(ServerFaults faultType, string message)
    {
      var ex = new Exception(GetExceptionMessageForFault(faultType));
      var fault = GetFaultException(GetExceptionCodeForFault(faultType), message, ex.StackTrace);
      Throw(fault);
    }
    public void Error(ServerFaults faultType, string message, Exception exception)
    {
      var fault = GetFaultException(GetExceptionCodeForFault(faultType), message, exception);
      Throw(fault);
    }
    public void Error(string message)
    {
      var ex = new Exception(GetExceptionMessageForFault(message));
      var fault = GetFaultException(GetExceptionCodeForFault(message), message, ex);
      Throw(fault);
    }
    FaultException GetFaultException(string code)
    {
      var faultCode = FaultCode.CreateSenderFaultCode(code, code);
      var faultMessage = MessageFault.CreateFault(faultCode, code);
      return new FaultException(faultMessage);
    }
    FaultException GetFaultException(Exception ex)
    {
      return new FaultException<Exception>(ex);
    }

    FaultException GetFaultException(string code, string message, string stackTrace)
    {
      var faultCode = FaultCode.CreateSenderFaultCode(code, message);
      var faultMessage = MessageFault.CreateFault(faultCode, stackTrace);
      return new FaultException(faultMessage);
    }

    FaultException GetFaultException(string code, string message, Exception ex)
    {
      return new FaultException<Exception>(ex, message, FaultCode.CreateSenderFaultCode(code, message));
    }

    public void Error(Exception exception)
    {
      if (exception is DataValidationException)
      {
        var ex = exception as DataValidationException;
        this.Error(ServerFaults.DATA_VALIDATION_ERROR, ex.Message, new DataValidationException(ex.ValidationResult));
      }
      else if (exception is AggregateException)
      {
        this.Error((exception as AggregateException).InnerExceptions.FirstOrDefault());
      }
      else
      {
        this.Error(ServerFaults.INTERNAL_ERROR, "Unexpected internal error", exception);
      }
    }
    private string GetExceptionCodeForFault(ServerFaults faultType)
    {
      return faultType.ToString();
    }
    private string GetExceptionCodeForFault(string message)
    {
      return message;
    }
    private string GetExceptionMessageForFault(ServerFaults faultType)
    {
      return faultType.ToString();
    }
    private string GetExceptionMessageForFault(string errorMessage)
    {
      return errorMessage;
    }
    void Throw(FaultException fault)
    {
      _logger.Debug($"Error: [{fault.Code}], {fault.Message}, Details: {fault.StackTrace}");
      throw fault;
    }


  }
}


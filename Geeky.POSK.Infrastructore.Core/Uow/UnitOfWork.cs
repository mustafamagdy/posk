using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.Uow
{
  public interface IUnitOfWork : IDisposable
  { }

  public class IocChildScopUnitOfWork : IUnitOfWork
  {
    ILifetimeScope _scope;
    public IocChildScopUnitOfWork(ILifetimeScope scope, Action onDispose)
    {
      _scope = scope;
      _onDispose = onDispose;
    }

    public Action _onDispose;
    public void Dispose()
    {
      _onDispose?.Invoke();
    }
  }
}

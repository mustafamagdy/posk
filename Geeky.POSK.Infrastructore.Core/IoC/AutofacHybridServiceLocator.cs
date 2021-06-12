using Autofac;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;

namespace Geeky.POSK.Infrastructore.Core
{
  /*
  - this class is the backbone of service locator and dependency injector.
  - it follows the composite design patter as most of modern IOC containers do.
  - there is BaseContainer and nested/child container each container can create another nested/child container from itself.
  - the current implementation does support only one level of child containers. and it will throw exception if child container tried to create another child.
  - in web application its recommended to create child container per request  preferably when request start and dispose it when request end (this task will be done automatically by autofac through Autofac.Integration.WebApi and Autofac.MVC assemblies ).
  - within web context if you create nested/child container from base container then creating another nested/child container from the base container will have no effect , you must dispose the old nested/child container first.
  - within thread context  if you create nested/child container from base container then creating another nested/child container from the base container will have no effect , you must dispose the old nested/child container first or create the nested/child container in another thread.
  - this class support both Thread and Web contexts that's from where the [Hybrid] name come from , 
    if this class found out web context it will preserve nested/child container in the HttpContext items otherwise it will preserve in [ThreadStatic] field.
  - the above note is obselete now this class does not preseve in HtpContext 
  */
  public sealed class AutofacHybridServiceLocator : ServiceLocatorImplBase,
  IDependencyResolver, IDependencyScope
  {
    /*
     important noate : as i have read the .net does not copy ThreadStatic to the thread state !!
     this mean if async operation is resumed in another thread by .net this varible values is not copied to the state of that other thread !!
     the only place that we have such methods in this project is in the web api middleware .
     however i tried to make multiple concurrent requests for this project but no problems happened .
     if later ThreadStatic caused any problems please find another way to preseve this varible in some where which will be copied to the thread state 
     please don't rely on HttpContext items data because we should remove the depdendency in System.Web as we run the project in Owin context
      */
    [ThreadStatic]
    private static ILifetimeScope _threadLifetimeScope;

    private const string nestedScopeKey = "autofac_nestedScope";
    private readonly IContainer _baseContainer;

    [ThreadStatic]
    private static Stack<ILifetimeScope> _nestedScopes_;
    private Stack<ILifetimeScope> _nestedScopes
    {
      get
      {
        //should be attached to HttpContext if exist like _threadLifetimeScope??
        if (_nestedScopes_ == null)
          _nestedScopes_ = new Stack<ILifetimeScope>();
        return _nestedScopes_;
      }
    }


    #region Singleton

    private static AutofacHybridServiceLocator _instance;
    private AutofacHybridServiceLocator(IContainer baseContainer)
    {
      _baseContainer = baseContainer;
      _baseContainer.ChildLifetimeScopeBeginning += _baseContainer_ChildLifetimeScopeBeginning;

    }
    public static AutofacHybridServiceLocator Instance
    {
      get
      {
        return _instance;
      }
    }

    //if the baseContainer has been updated this method must be called , any child life time scope must be disposed
    public static AutofacHybridServiceLocator Initialize(IContainer baseContainer)
    {
      if (_instance != null)
      {
        _instance.Dispose(!_instance._baseContainer.Equals(baseContainer));
      }
      _instance = new AutofacHybridServiceLocator(baseContainer);
      return _instance;
    }

    #endregion

    #region Public_Methods

    public static IContainer GetBaseContainer()
    {
      return Instance._baseContainer;
    }
    public void DisposeCurrentChildScope()
    {
      var scope = LifetimeScope;
      if (scope != null)
      {
        var fromScope = _nestedScopes.Count == 0 ? null : _nestedScopes.Peek();
        if (fromScope != null)
        {

          LifetimeScope = _nestedScopes.Pop();

        }
        else
          LifetimeScope = null;
        scope.Dispose();
        //LifetimeScope = null;
      }


    }
    public ILifetimeScope CreateChildScope()
    {

      return _baseContainer.BeginLifetimeScope();
    }

    public static bool IsRegistered<T>()
    {
      return Instance.LifetimeScope.IsRegistered<T>();

    }


    public static bool IsRegistered(Type type)
    {

      return Instance.LifetimeScope.IsRegistered(type);
    }

    public static bool IsRegisteredWithKey<T>(object key)
    {
      return Instance.LifetimeScope.IsRegisteredWithKey<T>(key);
    }

    #endregion

    #region Private_Properties
    private ILifetimeScope LifetimeScope
    {
      get { return GetLifetimeScope(); }
      set { SetLifetimeScope(value); }
    }

    #endregion

    #region Private_Methods
    private ILifetimeScope GetLifetimeScope()
    {
      //if (HttpContext.Current != null)
      //{
      //    return (ILifetimeScope)HttpContext.Current.Items[nestedScopeKey];
      //}
      //else
      //{
      return _threadLifetimeScope;
      // }
    }
    private void SetLifetimeScope(ILifetimeScope lifetimeScope)
    {
      //if (HttpContext.Current != null)
      //{
      //    HttpContext.Current.Items[nestedScopeKey] = lifetimeScope;
      //}
      //else
      //{
      _threadLifetimeScope = lifetimeScope;
      // }
    }
    private void _baseContainer_ChildLifetimeScopeBeginning(object sender, Autofac.Core.Lifetime.LifetimeScopeBeginningEventArgs e)
    {

      var newLifetimeScope = (e.LifetimeScope as Autofac.Core.Lifetime.LifetimeScope);
      if (newLifetimeScope.ParentLifetimeScope != newLifetimeScope.RootLifetimeScope)
      {
        throw new NotSupportedException("nested lifetime scopes should be nested from root only");
      }
      if (LifetimeScope != null && !IsDisposed(LifetimeScope))
        _nestedScopes.Push(LifetimeScope); //push current to stack
                                           //if (LifetimeScope == null || IsDisposed(LifetimeScope))
      {
        LifetimeScope = e.LifetimeScope;
      }
    }
    private bool IsDisposed(ILifetimeScope lifetimeScope)
    {
      return (bool)lifetimeScope.GetType()
           .GetProperty("IsDisposed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
           .GetValue(lifetimeScope);
    }
    private void Dispose(bool disposeBaseContainer = false)
    {
      //cleanup : dispose nested container and optionally the base container

      //_instance._baseContainer.ChildLifetimeScopeBeginning -= _baseContainer_ChildLifetimeScopeBeginning;

      if (disposeBaseContainer)
        _instance._baseContainer.Dispose();

      //var currentHttpContext = HttpContext.Current;
      //if (currentHttpContext != null && currentHttpContext.Items.Contains(nestedScopeKey))
      //{
      //    ((ILifetimeScope)HttpContext.Current.Items[nestedScopeKey]).Dispose();
      //    HttpContext.Current.Items.Remove(nestedScopeKey);
      //}

      if (_threadLifetimeScope != null)
      {
        _threadLifetimeScope.Dispose();
        _threadLifetimeScope = null;
      }

      _instance = null;
    }

    #endregion

    #region override_ServiceLocatorImplBase
    protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
    {
      if (serviceType == null)
      {
        throw new ArgumentNullException("serviceType");
      }
      var currentScope = LifetimeScope ?? _baseContainer;

      if (!currentScope.IsRegistered(serviceType))
        return Enumerable.Empty<object>();


      Type type = typeof(IEnumerable<>).MakeGenericType(new Type[] { serviceType });
      return Enumerable.Cast<object>((System.Collections.IEnumerable)ResolutionExtensions.Resolve(currentScope, type));
    }

    protected override object DoGetInstance(Type serviceType, string key)
    {



      if (serviceType == null)
      {
        throw new ArgumentNullException("serviceType");
      }
      var currentScope = LifetimeScope ?? _baseContainer;
      if (key == null)
      {


        return currentScope.ResolveOptional(serviceType);
      }
      return currentScope.ResolveNamed(key, serviceType);
    }

    public IDependencyScope BeginScope()
    {
      return this;
    }

    public override object GetService(Type serviceType)
    {
      return DoGetInstance(serviceType, null);
    }


    public IEnumerable<object> GetServices(Type serviceType)
    {
      return DoGetAllInstances(serviceType);
    }

    public void Dispose()
    {
      //do nothing let the caller control disposing child scope
    }


    #endregion
  }
}
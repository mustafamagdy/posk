using NLog;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace POSK.ClientApp
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

    protected void ApplicationStartup(object sender, StartupEventArgs e)
    {
      Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

      AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      DispatcherUnhandledException += App_DispatcherUnhandledException;
      TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

      AppStartup.ExecuteAsync();

      FrameworkElement.LanguageProperty.OverrideMetadata(
         typeof(FrameworkElement),
         new FrameworkPropertyMetadata(
         XmlLanguage.GetLanguage(
         CultureInfo.CurrentCulture.IetfLanguageTag)));

    }
    

    void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
      logger.Error("[UNHANDLEDED]: " + e.Exception.Message);
      //ProcessError(e.Exception);   - This could be used here to log ALL errors, even those caught by a Try/Catch block 
    }

    void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
      //log.ProcessError(e.Exception);
      e.Handled = true;
    }

    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      //var exception = e.ExceptionObject as Exception;
      //log.ProcessError(exception);
      //if (e.IsTerminating)
      //{
      //  //Now is a good time to write that critical error file! 
      //}
    }

    void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
      //log.ProcessError(e.Exception);
      //e.SetObserved();
    }

    void WinFormApplication_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      //log.ProcessError(e.Exception);
    }

  }
}

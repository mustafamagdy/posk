using Geeky.POSK.WPF.Core.Base;
using Geeky.POSK.Infrastructore.Core.Enums;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Geeky.POSK.Infrastructore.Core;
using System.Timers;

namespace POSK.Client.ViewModels
{
  public class SelectLanguageViewModel : BindableBaseViewModel
  {
    private string _content;
    public string Content { get { return _content; } set { SetProperty(ref _content, value); } }
    private string[] _allLanguageContent;

    //timer to switch content between languages
    private Timer tmr;
    int i = 1;

    public DelegateCommand<string> SelectLanguage { get; private set; }

    public SelectLanguageViewModel()
    {
      _allLanguageContent = new string[] { "Welcome", "اهلا وسهلا", "خوش آمدید", "स्वागत" };
      Content = _allLanguageContent[i % 4];

      tmr = new Timer();
      tmr.Interval = 1500;
      tmr.Elapsed += Tmr_Elapsed;
      tmr.Start();

      SelectLanguage = new DelegateCommand<string>(OnLanguageSelected);
    }

    private void Tmr_Elapsed(object sender, ElapsedEventArgs e)
    {
      Content = _allLanguageContent[i % 4];
      i++;
      if (i > 1000) i = 0;
    }

    public event Action<string> LanguageSelected = delegate { };
    private void OnLanguageSelected(string cultureCode)
    {
      LanguageSelected(string.IsNullOrEmpty(cultureCode) ? UILanguage.Language1 : cultureCode);
    }
  }
}

using AutoMapper;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using System;

namespace Geeky.POSK.Server.ViewModels
{
  public class TerminalViewModel : BindableBaseViewModel
  {
    private TerminalDto _dto;
    public TerminalDto Dto { get { return _dto; } set { SetProperty(ref _dto, value); } }

    public DelegateCommand SaveItem { get; private set; }
    public DelegateCommand CancelSave { get; private set; }

    public event Action Close = delegate { };
    public TerminalViewModel(TerminalDto dto)
    {
      Dto = dto ?? new TerminalDto();
      SaveItem = new DelegateCommand(OnSaveItem);
      CancelSave = new DelegateCommand(OnCancelSave);

    }

    private void OnSaveItem()
    {
      //todo logic to save
      var repo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      if (Dto.Id == Guid.Empty)
      {
        Dto.Id = Guid.NewGuid();
        repo.Add(Mapper.Map<Terminal>(Dto));
      }
      else
      {
        var _Terminal = repo.Get(Dto.Id);
        Mapper.Map(Dto, _Terminal);
        repo.Update(_Terminal);
      }

      Close();
    }
    private void OnCancelSave()
    {
      if (Dto != null && Dto.Id != Guid.Empty)
      {
        var repo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
        var _Terminal = repo.Get(Dto.Id);
        Dto = null;
        Dto = Mapper.Map<TerminalDto>(_Terminal);
      }

      Close();
    }

  }
}

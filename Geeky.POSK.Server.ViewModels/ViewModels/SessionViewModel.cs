using AutoMapper;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Geeky.POSK.Server.ViewModels
{
  public class SessionViewModel : BindableBaseViewModel
  {
    private ClientSessionDto _dto;
    public ClientSessionDto Dto { get { return _dto; } set { SetProperty(ref _dto, value); } }

    private ObservableCollection<DecryptedPinDto> _sessionPins;
    public ObservableCollection<DecryptedPinDto> SessionPins { get { return _sessionPins; } set { SetProperty(ref _sessionPins, value); } }

    public DelegateCommand CloseDlg { get; private set; }

    public event Action Close = delegate { };
    public SessionViewModel(ClientSessionDto dto, ObservableCollection<DecryptedPinDto> sessionPins)
    {
      CloseDlg = new DelegateCommand(OnCancelSave);
    }

    private void OnCancelSave()
    {
      Close();
    }
  }
}

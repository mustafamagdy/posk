using CommonServiceLocator;
using POSK.Client.ApplicationService.Interface;
using System;
using System.Transactions;
using Geeky.POSK.Models;
using System.Collections.Generic;
using System.Linq;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.DataContracts;
using System.Collections.ObjectModel;
using NLog;
using Geeky.POSK.Infrastructore.Core;

namespace POSK.Client.ViewModels
{
  public sealed class UserCart
  {
    private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
    private Action<ClientSession, string> LogSession = (session, s) => { logger.Trace($"Session: {session?.Id} -> {s}"); };
    private Action<string> Error = (string s) => { logger.Error(s); };

    //not accessable, use create factory method
    private UserCart()
    {
      _clientSvc = ServiceLocator.Current.GetInstance<IClientService>();
    }

    //item added to cart
    public event Action CartItemAdded = delegate { };
    //item removed from cart
    public event Action CartItemRemoved = delegate { };
    //amount paid to cart
    public event Action AmountPaid = delegate { };
    //Cart has been disposed
    public event Action CartDisposed = delegate { };
    //print pins
    public event Action<List<DecryptedPinDto>> Print = delegate { };

    private IClientService _clientSvc;

    public static UserCart Create()
    {
      var clientSvc = ServiceLocator.Current.GetInstance<IClientService>();
      var session = clientSvc.CreateSession(MainViewModel.CurrentTerminalId);

      var _cart = new UserCart
      {
        Session = session,
        _items = new List<EncryptedPinDto>(),
        _amounts = new List<PaymentValueDto>(),
        _logs = new List<Tuple<DateTime, LogTypeEnum, string>>(),
        _isDisposed = false,
        //IsTimeoutWarningShown = false
      };

      _cart.IsReceivingMoney = false;

      return _cart;
    }

    public void AddItem(ProductItemViewModel item)
    {
      //reserve pin first from db
      try
      {
        var pin = _clientSvc.ReserveItem(Session.Id, item.ProductId);
        //add it to cart items
        _items.Add(pin);
        //notify subscribers, that a new item has been added
        CartItemAdded();
      }
      catch (Exception ex)
      {
        DisposeCart(releaseReservedItems: true);
      }
    }

    public void AddTerminalLog(LogTypeEnum logType, string message)
    {

    }

    public void RemoveItem(Guid pinId)
    {
      //release the item from db
      _clientSvc.ReleaseItem(pinId);
      //remove item from cart items
      _items.Remove(x => x.Id == pinId);
      //notify subscribers that an item has been removed
      CartItemRemoved();
    }


    public void DisposeCart(bool releaseReservedItems)
    {
      lock (_locker)
      {
        if (_isDisposed) // avoid call dispose again from diffrent place
          return;

        //Save everything here
        foreach (var _amount in _amounts)
        {
          _clientSvc.AddPaymentToSession(Session.Id, _amount);
        }

        foreach (var _log in _logs)
        {
          _clientSvc.LogTerminalStatus(TerminalId, Session.Id, _log.Item2, _log.Item3, _log.Item1);
        }

        _clientSvc.EndSession(Session.Id, releaseItms: releaseReservedItems);

        _items = new List<EncryptedPinDto>();
        _amounts = new List<PaymentValueDto>();
        _logs = new List<Tuple<DateTime, LogTypeEnum, string>>();

        Session = null;
        OrderFinished = true;
        _isDisposed = true;

        CartDisposed();
      }
    }

    public void AddPaidValue(PaymentValueDto value)
    {
      LogSession(Session, $"[Cart] -> Add Payment {value.StackedAmount}");

      //add payment to cart payments 
      _amounts.Add(value);

      LogSession(Session, $"[Cart] -> Amount {value.StackedAmount} Added, total paid is {TotalPaid}");

      //notify subscribers that a payment has been added to cart payments
      AmountPaid();
    }

    public void Checkout()
    {
      var soldItems = new List<DecryptedPinDto>();

      foreach (var item in _items)
      {
        //mark item as sold
        var soldItem = _clientSvc.SellItem(item.Id, Transaction.Current, item.Id, Session.Id);
        // addit to sold items to print
        soldItems.Add(soldItem);
      }

      //print
      Print(soldItems);

      OrderFinished = true;
      //IsTimeoutWarningShown = false;
    }

    /// <summary>
    /// Clear items in cart, and release them
    /// </summary>
    public void ClearItems()
    {
      var itemsToRemove = (_items ?? new List<EncryptedPinDto>()).ToList();
      itemsToRemove?.ForEach(i =>
      {
        RemoveItem(i.Id);
      });
    }

    private bool _isDisposed = false;
    public bool OrderFinished { get; private set; }
    //public bool IsTimeoutWarningShown { get; private set; }

    public ClientSession Session { get; private set; }
    private List<EncryptedPinDto> _items;
    private List<PaymentValueDto> _amounts;
    private List<Tuple<DateTime, LogTypeEnum, String>> _logs;

    private object _locker = new object();

    /// <summary>
    /// Only ready if payment has been fullfilled
    /// </summary>
    public bool Ready
    {
      get
      {
        lock (_locker)
        {
          return TotalValue > 0 && TotalPaid >= TotalValue;
        }
      }
    }
    //public bool Ready { get { return TotalValue > 0 && Math.Abs(TotalPaid - TotalValue) <= 0.01m; } }
    /// <summary>
    /// Total amount for added items
    /// </summary>
    public decimal TotalValue
    {
      get
      {
        lock (_locker)
        {
          return _items.Sum(x => x.PriceAfterTax);
        }
      }
    }
    /// <summary>
    /// Only sum stacked amount
    /// </summary>
    public decimal TotalPaid
    {
      get
      {
        lock (_locker)
        {
          return _amounts.Sum(x => x.StackedAmount);
        }
      }
    }
    /// <summary>
    /// Reminaing value 
    /// </summary>
    public decimal Remaining { get { return TotalValue - TotalPaid; } }
    /// <summary>
    /// Item count in cart
    /// </summary>
    public int ItemCount
    {
      get
      {
        lock (_locker)
        {
          return _items.Count();
        }
      }
    }

    private bool _isReceivingMoney;
    public bool IsReceivingMoney
    {
      get { return _isReceivingMoney; }
      set
      {
        lock (_locker)
        {
          _isReceivingMoney = value;
        }
      }
    }

    public Guid TerminalId { get; internal set; }

    /// <summary>
    /// Used to check if user asked to extend timeout or not
    /// </summary>
    //public bool ExtendTimeout { get; set; }
    /// <summary>
    /// Items in the cart (read only)
    /// </summary>
    public ReadOnlyCollection<EncryptedPinDto> Items { get { return _items.AsReadOnly(); } }

    //todo

    //public Task SetTimeoutExpired()
    //{
    //  //todo show time count down
    //  return Task.Delay(0);
    //}
    public void ShowError(string errorMessage)
    {
      //todo
    }

    //public void SetShowTimeoutWarning()
    //{
    //  IsTimeoutWarningShown = true;
    //}

    //public void ClearTimeoutWarning()
    //{
    //  IsTimeoutWarningShown = false;
    //}
  }
}

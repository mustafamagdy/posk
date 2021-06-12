using Geeky.POSK.DataContracts.Base;
using Geeky.POSK.Infrastructore.Core;
using System;
using System.Runtime.Serialization;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public abstract class BasePinDto : BaseDto
  {
    private string _serialNumber;
    private string _refNumber;
    private DateTime _expiryDate;
    private DateTime _createDate;
    private DateTime? _soldDate;
    private string _productCode;
    private string _vendorCode;
    private string _terminalCode;
    private bool _sold;
    private bool _hold;
    private Guid _terminalId;
    private Guid _vendorId;
    private Guid _productId;
    private decimal _price;
    private decimal _priceAfterTax;
    private string _productInstructions;
    private byte[] _vendorLogo;
    private byte[] _printedLogo;
    private ProductTypeEnum _productType;

    [DataMember] public string SerialNumber { get { return _serialNumber; } set { SetProperty(ref _serialNumber, value); } }
    [DataMember] public string RefNumber { get { return _refNumber; } set { SetProperty(ref _refNumber, value); } }
    [DataMember] public DateTime ExpiryDate { get { return _expiryDate; } set { SetProperty(ref _expiryDate, value); } }
    [DataMember] public DateTime CreateDate { get { return _createDate; } set { SetProperty(ref _createDate, value); } }
    [DataMember] public DateTime? SoldDate { get { return _soldDate; } set { SetProperty(ref _soldDate, value); } }
    [DataMember] public Guid TerminalId { get { return _terminalId; } set { SetProperty(ref _terminalId, value); } }
    [DataMember] public Guid VendorId { get { return _vendorId; } set { SetProperty(ref _vendorId, value); } }
    [DataMember] public Guid ProductId { get { return _productId; } set { SetProperty(ref _productId, value); } }
    [DataMember] public string TerminalCode { get { return _terminalCode; } set { SetProperty(ref _terminalCode, value); } }
    [DataMember] public string ProductCode { get { return _productCode; } set { SetProperty(ref _productCode, value); } }
    [DataMember] public string VendorCode { get { return _vendorCode; } set { SetProperty(ref _vendorCode, value); } }
    [DataMember] public bool Sold { get { return _sold; } set { SetProperty(ref _sold, value); } }
    [DataMember] public bool Hold { get { return _hold; } set { SetProperty(ref _hold, value); } }
    [DataMember] public decimal Price { get { return _price; } set { SetProperty(ref _price, value); } }
    [DataMember] public decimal PriceAfterTax { get { return _priceAfterTax; } set { SetProperty(ref _priceAfterTax, value); } }
    [DataMember] public byte[] VendorLogo { get { return _vendorLogo; } set { SetProperty(ref _vendorLogo, value); } }
    [DataMember] public byte[] PrintedLogo { get { return _printedLogo; } set { SetProperty(ref _printedLogo, value); } }
    [DataMember] public string ProductInstructions { get { return _productInstructions; } set { SetProperty(ref _productInstructions, value); } }
    [DataMember] public ProductTypeEnum ProductType { get { return _productType; } set { SetProperty(ref _productType, value); } }

  }

  [DataContract]
  public class EncryptedPinDto : BasePinDto
  {
    private byte[] _pin;
    private string[] _productNames;
    private string[] _vendorNames;

    [DataMember] public byte[] Pin { get { return _pin; } set { SetProperty(ref _pin, value); } }
    [DataMember] public string[] ProductNames { get { return _productNames; } set { SetProperty(ref _productNames, value); } }
    [DataMember] public string[] VendorNames { get { return _vendorNames; } set { SetProperty(ref _vendorNames, value); } }

  }

  [DataContract]
  public class DecryptedPinDto : BasePinDto
  {
    private string _pin;
    [DataMember] public string Pin { get { return _pin; } set { SetProperty(ref _pin, value); } }
  }
}

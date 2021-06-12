using Geeky.POSK.DataContracts.Base;
using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public class ProductDto : BaseDto
  {
    private string _code;
    private string _language1Name;
    private string _language2Name;
    private string _language3Name;
    private string _language4Name;
    private bool _isActive;
    private decimal _price;
    private decimal _priceAftertax;
    private bool _soldOut;
    private Guid _vendorId;
    private string _vendorCode;
    private ProductTypeEnum _productType;
    [DataMember] public string Code { get { return _code; } set { SetProperty(ref _code, value); } }
    [DataMember] public string Language1Name { get { return _language1Name; } set { SetProperty(ref _language1Name, value); } }
    [DataMember] public string Language2Name { get { return _language2Name; } set { SetProperty(ref _language2Name, value); } }
    [DataMember] public string Language3Name { get { return _language3Name; } set { SetProperty(ref _language3Name, value); } }
    [DataMember] public string Language4Name { get { return _language4Name; } set { SetProperty(ref _language4Name, value); } }
    [DataMember] public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value); } }
    [DataMember] public decimal Price { get { return _price; } set { SetProperty(ref _price, value); } }
    [DataMember] public decimal PriceAfterTax { get { return _priceAftertax; } set { SetProperty(ref _priceAftertax, value); } }
    [DataMember] public bool SoldOut { get { return _soldOut; } set { SetProperty(ref _soldOut, value); } }
    [DataMember] public Guid VendorId { get { return _vendorId; } set { SetProperty(ref _vendorId, value); } }
    [DataMember] public string VendorCode { get { return _vendorCode; } set { SetProperty(ref _vendorCode, value); } }
    [DataMember] public ProductTypeEnum ProductType { get { return _productType; } set { SetProperty(ref _productType, value); } }


  }
}

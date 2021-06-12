using Geeky.POSK.DataContracts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.DataContracts
{

  [DataContract]
  public class VendorDto : BaseDto
  {
    private string _code;
    private string _language1Name;
    private string _language2Name;
    private string _language3Name;
    private string _language4Name;
    private bool _isActive;
    private bool _soldOut;
    private byte[] _logo;
    private byte[] _printedLogo;
    private string _instructions;
    private ICollection<ProductDto> _products;
    [DataMember] public string Code { get { return _code; } set { SetProperty(ref _code, value); } }
    [DataMember] public string Language1Name { get { return _language1Name; } set { SetProperty(ref _language1Name, value); } }
    [DataMember] public string Language2Name { get { return _language2Name; } set { SetProperty(ref _language2Name, value); } }
    [DataMember] public string Language3Name { get { return _language3Name; } set { SetProperty(ref _language3Name, value); } }
    [DataMember] public string Language4Name { get { return _language4Name; } set { SetProperty(ref _language4Name, value); } }
    [DataMember] public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value); } }
    [DataMember] public bool SoldOut { get { return _soldOut; } set { SetProperty(ref _soldOut, value); } }
    [DataMember] public byte[] Logo { get { return _logo; } set { SetProperty(ref _logo, value); } }
    [DataMember] public byte[] PrintedLogo { get { return _printedLogo; } set { SetProperty(ref _printedLogo, value); } }
    [DataMember] public ICollection<ProductDto> Products { get { return _products; } set { SetProperty(ref _products, value); } }
    [DataMember] public string Instructions { get { return _instructions; } set { SetProperty(ref _instructions, value); } }
  }
}

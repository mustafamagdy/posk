using Geeky.POSK.DataContracts;
using Geeky.POSK.Models;
using Geeky.POSK.WPF.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Filters
{
  [DataContract]
  public class FilterCriteria : BindableBaseViewModel
  {
    private DateTime? _fromDate;
    private DateTime? _toDate;
    [DataMember] public DateTime? FromDate { get { return _fromDate; } set { SetProperty(ref _fromDate, value); } }
    [DataMember] public DateTime? ToDate { get { return _toDate; } set { SetProperty(ref _toDate, value); } }
  }

  [DataContract]
  public class SalesReportFilterCriteria : FilterCriteria
  {
    private VendorDto _vendor;
    [DataMember] public VendorDto Vendor { get { return _vendor; } set { SetProperty(ref _vendor, value); } }
  }
}

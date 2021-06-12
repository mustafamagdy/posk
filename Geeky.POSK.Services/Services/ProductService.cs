using AutoMapper;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Exceptions;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using Geeky.POSK.Filters;
using Geeky.POSK.Views;

namespace Geeky.POSK.Services
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  public partial class ProductService : IProductService, IStatisticsService
  {
  }
}

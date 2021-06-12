using AutoMapper;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.DataContracts.MappingProfile
{
  public class ContractProfile : Profile
  {
    public ContractProfile()
    {
      CreateMap<Pin, EncryptedPinDto>()
             .ForMember(x => x.VendorCode, cfg => cfg.MapFrom(x => x.Product.Vendor.Code))
             .ForMember(x => x.VendorNames, cfg => cfg.MapFrom(x => new string[] { x.Product.Vendor.Language1Name, x.Product.Vendor.Language2Name, x.Product.Vendor.Language3Name, x.Product.Vendor.Language4Name }))
             .ForMember(x => x.ProductCode, cfg => cfg.MapFrom(x => x.Product.Code))
             .ForMember(x => x.ProductNames, cfg => cfg.MapFrom(x => new string[] { x.Product.Language1Name, x.Product.Language2Name, x.Product.Language3Name, x.Product.Language4Name }))
             .ForMember(x => x.VendorLogo, cfg => cfg.MapFrom(x => x.Product.Vendor.Logo))
             .ForMember(x => x.PrintedLogo, cfg => cfg.MapFrom(x => x.Product.Vendor.PrintedLogo))
             .ForMember(x => x.Price, cfg => cfg.MapFrom(x => x.Product.SellingPrice))
             .ForMember(x => x.PriceAfterTax, cfg => cfg.MapFrom(x => x.Product.PriceAfterTax))
             .ForMember(x => x.TerminalId, cfg => cfg.MapFrom(x => x.Terminal.Id))
             .ForMember(x => x.VendorId, cfg => cfg.MapFrom(x => x.Product.Vendor.Id))
             .ForMember(x => x.ProductId, cfg => cfg.MapFrom(x => x.Product.Id))
             .ForMember(x => x.ProductType, cfg => cfg.MapFrom(x => x.Product.ProductType))
             .ForMember(x => x.TerminalCode, cfg => cfg.MapFrom(x => x.Terminal.TerminalKey))
             .ReverseMap();

      CreateMap<Product, ProductDto>()
             .ForMember(x => x.Price, cfg => cfg.MapFrom(x => x.SellingPrice))
             .ForMember(x => x.SoldOut, cfg => cfg.MapFrom(x => x.Pins.Any() == false || x.Pins.All(a => a.Sold == true || a.Hold == true)))
             .ForMember(x => x.VendorId, cfg => cfg.MapFrom(x => x.Vendor.Id))
             .ForMember(x => x.VendorCode, cfg => cfg.MapFrom(x => x.Vendor.Code));

      CreateMap<ProductDto, Product>()
             .ForMember(x => x.SellingPrice, cfg => cfg.MapFrom(x => x.Price))
             .ForMember(x => x.Vendor, cfg => cfg.Ignore());

      CreateMap<Pin, DecryptedPinDto>()
             .ForMember(x => x.Pin, cfg => cfg.MapFrom(x => AesEncyHelper.Decyrpt(x.PIN, x.Terminal.Id.ToByteArray(), x.Terminal.Id.ToByteArray())))
             .ForMember(x => x.VendorCode, cfg => cfg.MapFrom(x => x.Product.Vendor.Code))
             .ForMember(x => x.Price, cfg => cfg.MapFrom(x => x.Product.SellingPrice))
             .ForMember(x => x.PrintedLogo, cfg => cfg.MapFrom(x => x.Product.Vendor.PrintedLogo))
             .ForMember(x => x.PriceAfterTax, cfg => cfg.MapFrom(x => x.Product.PriceAfterTax))
             .ForMember(x => x.ProductCode, cfg => cfg.MapFrom(x => x.Product.Code))
             .ForMember(x => x.TerminalId, cfg => cfg.MapFrom(x => x.Terminal.Id))
             .ForMember(x => x.VendorId, cfg => cfg.MapFrom(x => x.Product.Vendor.Id))
             .ForMember(x => x.ProductId, cfg => cfg.MapFrom(x => x.Product.Id))
             .ForMember(x => x.TerminalCode, cfg => cfg.MapFrom(x => x.Terminal.TerminalKey))
             .ForMember(x => x.ProductType, cfg => cfg.MapFrom(x => x.Product.ProductType))
             .ForMember(x => x.ProductInstructions, cfg => cfg.MapFrom(x => x.Product.Vendor.Instructions));


      CreateMap<Vendor, VendorDto>()
        .ForMember(x => x.Products, cfg => cfg.Ignore())
        .ForMember(x => x.SoldOut, cfg => cfg.MapFrom(x => x.Products.Any() == false || x.Products.All(p => p.Pins.Any() == false || p.Pins.All(a => a.Sold == true || a.Hold == true))))
        .ForMember(x => x.Logo, cfg => cfg.MapFrom(x => x.Logo))
        .ForMember(x => x.PrintedLogo, cfg => cfg.MapFrom(x => x.PrintedLogo))
        .ReverseMap()
        .ForMember(x => x.Products, cfg => cfg.Ignore());

      CreateMap<Terminal, TerminalDto>().ReverseMap();
      CreateMap<Terminal, TerminalPingStatusDto>()
        .ForMember(x => x.TerminalId, cfg => cfg.MapFrom(x => x.Id))
        .ForMember(x => x.TerminalKey, cfg => cfg.MapFrom(x => x.TerminalKey));

      CreateMap<PaymentMethod, PaymentMethodDto>().ReverseMap();

      CreateMap<SessionPayment, PaymentValueDto>()
             .ForMember(x => x.PayMethod, cfg => cfg.Ignore())
             .ForMember(x => x.StackedAmount, cfg => cfg.MapFrom(x => x.StackedAmount))
             .ForMember(x => x.IsJammed, cfg => cfg.MapFrom(x => x.IsJammed))
             .ForMember(x => x.CashCodeStatus, cfg => cfg.MapFrom(x => x.CashCodeStatus))
             .ForMember(x => x.CashAmount, cfg => cfg.MapFrom(x => x.CashAmount))
             .ForMember(x => x.RejectionReason, cfg => cfg.MapFrom(x => x.RejectionReason))
             .ForMember(x => x.PaymentRefNumber, cfg => cfg.MapFrom(x => x.PaymentRefNumber))
        .ReverseMap();


      CreateMap<TransferTrx, TransferTrxDto>()
        .ForMember(x => x.DestTerminalCode, cfg => cfg.MapFrom(x => x.DestTerminal.Code))
        .ForMember(x => x.DestTerminalId, cfg => cfg.MapFrom(x => x.DestTerminal.Id))
        .ForMember(x => x.SourceTerminalCode, cfg => cfg.MapFrom(x => x.SourceTerminal.Code))
        .ForMember(x => x.SourceTerminalId, cfg => cfg.MapFrom(x => x.SourceTerminal.Id))
        .ReverseMap();

      CreateMap<TerminalLog, TerminalLogDto>()
      .ForMember(x => x.TerminalId, cfg => cfg.MapFrom(x => x.Terminal.Id))
      .ForMember(x => x.TerminalKey, cfg => cfg.MapFrom(x => x.Terminal.TerminalKey))
      .ForMember(x => x.SessionId, cfg => cfg.MapFrom(x => x.Session.Id));

      CreateMap<TerminalLogDto, TerminalLog>()
        .ForMember(x => x.RowVersion, cfg => cfg.Ignore());



      CreateMap<ClientSession, ClientSessionDto>()
        .ForMember(x => x.Payments, cfg => cfg.MapFrom(x => x.Payments))
        .ReverseMap();
    }
  }
}

using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Extensions;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.ORM.Contect.Core.DbInitializers;
using Geeky.POSK.ORM.Context.EF;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace UnitTesting
{
  [TestFixture]
  public class ValidData : BaseUnitTest
  {
    public override void SetupTest()
    {
      base.SetupTest();

      InitDependancies();
    }

    private void InitDependancies()
    {

    }


    ITerminalRepository _terminalRepo = null;
    IPinRepository _pinRepo = null;
    IProductRepository _productRepo = null;
    IVendorRepository _vendorRepo = null;
    private List<string> reservedPins = new List<string>();
    private List<string> reservedSerials = new List<string>();

    public static byte[] ImageToByte(Image img)
    {
      ImageConverter converter = new ImageConverter();
      return (byte[])converter.ConvertTo(img, typeof(byte[]));
    }

    [Test]
    public void GenerateValidData()
    {

      _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      _productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
      _vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();

      Database.SetInitializer(new CustomDropCreateDatabaseAlways<AppDbContext>());

      var serverTerminal = _terminalRepo.GetServerTerminal();
      if (serverTerminal == null)
        serverTerminal = _terminalRepo.CreateSererTerminal();

      var terminals = new List<Terminal>();

      for (int t = 3; t <= 5; t++)
      {
        var terminal = new Terminal
        {
          Code = $"T{t}",
          TerminalKey = $"t{t}",
          Address = "",
          IP = "100.0.0.1",
          MachineName = t == 5 ? Environment.MachineName : $"VEND0{t}-PC",
          State = TerminalStateEnum.Active//t == 2 ? TerminalStateEnum.NotActive : 
        };
        terminal = _terminalRepo.Add(terminal);
        terminals.Add(terminal);
      }

      var stc = _vendorRepo.Add(new Vendor
      {
        Code = "STC",
        IsActive = true,
        Order = 1,
        Language1Name = "STC",
        Language2Name = "STC",
        Language3Name = "STC",
        Language4Name = "STC",
        PrintedLogo = ImageToByte(Properties.Resources.stc_printed),
        Instructions = @"Recharge via MySTC للشحن عبر تطبيق
Tap on recharge    اضغط على شحن
Tap on Camera icon اضغط على الكاميرا
Scan the Barcode امسح الباركود
Tap on recharge اضغط على شحن
Or enter او أدخل
From left to right  من اليسار الى اليمين
*155*Activation No# *155*رقم التفعيل#
For other ways لاعادة الشحن
Of recharge send   بطرق مختلفة أرسل
Recharge to   900  كلمة شحن الى"
      });

      var mobily = _vendorRepo.Add(new Vendor
      {
        Code = "Mobily",
        IsActive = true,
        Order = 2,
        Language1Name = "موبايلي",
        Language2Name = "Mobily",
        Language3Name = "Mobily",
        Language4Name = "Mobily",
        PrintedLogo = ImageToByte(Properties.Resources.mobily_printed),
        Instructions = @"Recharge via Mobily للشحن عبر تطبيق
Tap on recharge    اضغط على شحن
Or enter او أدخل
From left to right  من اليسار الى اليمين
*996*Activation No# *996*رقم التفعيل#
For other ways لاعادة الشحن
Of recharge send   بطرق مختلفة أرسل
Recharge to   800  كلمة شحن الى"
      });
      var zain = _vendorRepo.Add(new Vendor
      {
        Code = "Zain",
        IsActive = true,
        Order = 2,
        Language1Name = "زين",
        Language2Name = "Zain",
        Language3Name = "Zain",
        Language4Name = "Zain",
        PrintedLogo = ImageToByte(Properties.Resources.zain_printed),
      });

      var virgin = _vendorRepo.Add(new Vendor
      {
        Code = "Virgin",
        IsActive = true,
        Order = 2,
        Language1Name = "فيرجن",
        Language2Name = "Virgin",
        Language3Name = "Virgin",
        Language4Name = "Virgin",
        PrintedLogo = ImageToByte(Properties.Resources.virgin_printed),
      });
      var jawwy = _vendorRepo.Add(new Vendor
      {
        Code = "Jawwy",
        IsActive = true,
        Order = 2,
        Language1Name = "جوي",
        Language2Name = "Jawwy",
        Language3Name = "Jawwy",
        Language4Name = "Jawwy",
        PrintedLogo = ImageToByte(Properties.Resources.jawwy_printed),
      });

      var other = _vendorRepo.Add(new Vendor
      {
        Code = "OTHER",
        IsActive = false,
        Order = 4,
        Language1Name = "أخرى",
        Language2Name = "OTHER",
        Language3Name = "دیگر",
        Language4Name = "अन्य",
      });

      var stc_p10 = _productRepo.Add(new Product { IsActive = true, Vendor = stc, Code = "STC_10", SellingPrice = 10.0M, PriceAfterTax = 10.0m * 1.05M, Order = 1, Language1Name = "سوا 10", Language2Name = "SAWA 10", Language3Name = "SAWA 10", Language4Name = "SAWA 10", ProductType = ProductTypeEnum.Voice });
      var stc_p20 = _productRepo.Add(new Product { IsActive = true, Vendor = stc, Code = "STC_20", SellingPrice = 20.0M, PriceAfterTax = 20.0M * 1.05M, Order = 2, Language1Name = "سوا 20", Language2Name = "SAWA 20", Language3Name = "SAWA 20", Language4Name = "SAWA 20", ProductType = ProductTypeEnum.Voice });
      var stc_p50 = _productRepo.Add(new Product { IsActive = true, Vendor = stc, Code = "STC_50", SellingPrice = 50.0M, PriceAfterTax = 50.0M * 1.05M, Order = 3, Language1Name = "سوا 50", Language2Name = "SAWA 50", Language3Name = "SAWA 50", Language4Name = "SAWA 50", ProductType = ProductTypeEnum.Voice });
      var stc_p100 = _productRepo.Add(new Product { IsActive = true, Vendor = stc, Code = "STC_100", SellingPrice = 100.0M, PriceAfterTax = 100.0M * 1.05M, Order = 4, Language1Name = "سوا 100", Language2Name = "SAWA 100", Language3Name = "SAWA 100", Language4Name = "SAWA 100", ProductType = ProductTypeEnum.Data });

      var mobily_p10 = _productRepo.Add(new Product { IsActive = true, Vendor = mobily, Code = "MOBILY_10", SellingPrice = 10.0M, PriceAfterTax = 10.0m * 1.05M, Order = 1, Language1Name = "موبايلي 10", Language2Name = "Mobily 10", Language3Name = "Mobily 10", Language4Name = "Mobily 10", ProductType = ProductTypeEnum.Voice });
      var mobily_p20 = _productRepo.Add(new Product { IsActive = true, Vendor = mobily, Code = "MOBILY_20", SellingPrice = 20.0M, PriceAfterTax = 20.0M * 1.05M, Order = 2, Language1Name = "موبايلي 20", Language2Name = "Mobily 20", Language3Name = "Mobily 20", Language4Name = "Mobily 20", ProductType = ProductTypeEnum.Voice });
      var mobily_p50 = _productRepo.Add(new Product { IsActive = true, Vendor = mobily, Code = "MOBILY_50", SellingPrice = 50.0M, PriceAfterTax = 50.0M * 1.05M, Order = 3, Language1Name = "موبايلي 50", Language2Name = "Mobily 50", Language3Name = "Mobily 50", Language4Name = "Mobily 50", ProductType = ProductTypeEnum.Voice });
      var mobily_p100 = _productRepo.Add(new Product { IsActive = true, Vendor = mobily, Code = "MOBILY_100", SellingPrice = 100.0M, PriceAfterTax = 100.0M * 1.05M, Order = 4, Language1Name = "موبايلي 100", Language2Name = "Mobily 100", Language3Name = "Mobily 100", Language4Name = "Mobily 100", ProductType = ProductTypeEnum.Data });


      var jawwy_p100 = _productRepo.Add(new Product { IsActive = true, Vendor = jawwy, Code = "JAWWY_100", SellingPrice = 100.0M, PriceAfterTax = 100.0m * 1.05M, Order = 1, Language1Name = "جوي 100", Language2Name = "Jawwy 100", Language3Name = "Jawwy 100", Language4Name = "Jawwy 100", ProductType = ProductTypeEnum.Voice });
      var jawwy_p200 = _productRepo.Add(new Product { IsActive = true, Vendor = jawwy, Code = "JAWWY_200", SellingPrice = 200.0M, PriceAfterTax = 200.0M * 1.05M, Order = 2, Language1Name = "جوي 200", Language2Name = "Jawwy 200", Language3Name = "Jawwy 200", Language4Name = "Jawwy 200", ProductType = ProductTypeEnum.Voice });
      var jawwy_p300 = _productRepo.Add(new Product { IsActive = true, Vendor = jawwy, Code = "JAWWY_300", SellingPrice = 300.0M, PriceAfterTax = 300.0M * 1.05M, Order = 3, Language1Name = "جوي 300", Language2Name = "Jawwy 300", Language3Name = "Jawwy 300", Language4Name = "Jawwy 300", ProductType = ProductTypeEnum.Data });
      var jawwy_p500 = _productRepo.Add(new Product { IsActive = true, Vendor = jawwy, Code = "JAWWY_500", SellingPrice = 500.0M, PriceAfterTax = 500.0M * 1.05M, Order = 4, Language1Name = "جوي 500", Language2Name = "Jawwy 500", Language3Name = "Jawwy 500", Language4Name = "Jawwy 500", ProductType = ProductTypeEnum.Data });


      //stc
      AddPins(500, stc_p10, serverTerminal);
      AddPins(200, stc_p20, serverTerminal);
      AddPins(500, stc_p50, serverTerminal);
      AddPins(300, stc_p100, serverTerminal);

      //mobily
      AddPins(600, mobily_p10, serverTerminal);
      AddPins(340, mobily_p20, serverTerminal);
      AddPins(220, mobily_p50, serverTerminal);
      AddPins(400, mobily_p100, serverTerminal);

      //jawwy
      AddPins(50, jawwy_p100, serverTerminal);
      AddPins(120, jawwy_p200, serverTerminal);
      AddPins(70, jawwy_p300, serverTerminal);
      AddPins(90, jawwy_p500, serverTerminal);


      var svcProductManager = ServiceLocator.Current.GetInstance<IProductManagementService>();
      var pins = _pinRepo.FindAll().OrderBy(x => x.Id).ToList();//randomize them by guid

      var t3 = terminals[0];
      var t4 = terminals[1];
      var t5 = terminals[2];


      var t3Pins = pins.Take(400);
      var t4Pins = pins.Skip(400).Take(200);
      var t5Pins = pins.Skip(600);

      t3Pins.ForEach(p =>
      {
        p.Terminal = t5;
        var pin = AesEncyHelper.Decyrpt(p.PIN, serverTerminal.Id.ToByteArray(), serverTerminal.Id.ToByteArray());
        p.PIN = AesEncyHelper.Encyrpt(pin, t5.Id.ToByteArray(), t5.Id.ToByteArray());

        _pinRepo.Update(p);
      });

      t4Pins.ForEach(p =>
      {
        p.Terminal = t4;
        var pin = AesEncyHelper.Decyrpt(p.PIN, serverTerminal.Id.ToByteArray(), serverTerminal.Id.ToByteArray());
        p.PIN = AesEncyHelper.Encyrpt(pin, t4.Id.ToByteArray(), t4.Id.ToByteArray());

        _pinRepo.Update(p);
      });

      t5Pins.ForEach(p =>
      {
        p.Terminal = t3;
        var pin = AesEncyHelper.Decyrpt(p.PIN, serverTerminal.Id.ToByteArray(), serverTerminal.Id.ToByteArray());
        p.PIN = AesEncyHelper.Encyrpt(pin, t3.Id.ToByteArray(), t3.Id.ToByteArray());

        _pinRepo.Update(p);
      });
    }

    void AddPins(int count, Product product, Terminal serverTerminal)
    {
      Random r = new Random();
      count.Loop(i =>
      {
        var pin = Randomz.GetRandomNumber(15, reservedPins).ToString();
        var serial = Randomz.GetRandomNumber(11, reservedSerials).ToString();
        reservedPins.Add(pin);
        reservedSerials.Add(serial);

        var p = new Pin
        {

          Product = product,
          Terminal = serverTerminal,
          ExpiryDate = DateTime.Now.AddMonths(r.Next(1, 10)),
          PIN = AesEncyHelper.Encyrpt(pin, serverTerminal.Id.ToByteArray(), serverTerminal.Id.ToByteArray()),
          RefNumber = "",
          SerialNumber = serial,
          Sold = false,
        };
        _pinRepo.Add(p);

      });
    }
  }
}

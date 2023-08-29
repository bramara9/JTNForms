﻿using ClosedXML.Excel;
using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Mvc;

namespace JTNForms.Controllers
{
    public class SalesOrderController : Controller
    {

        public dapperDbContext _dapperDbContext = new dapperDbContext();
        public SalesOrderController(dapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }
        // GET: InvoiceController
        public ActionResult Index(int customerId)
        {
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");
            var details = GetRoomDetails(customerId);
            ViewBag.CutomerId = customerId;
            return View(details);
            //return View();
        }

        // GET: InvoiceController/Details/5
        public ActionResult DownloadInvoice(int customerId)
        {
            var details = GetRoomDetails(customerId);
            var CustomerName = HttpContext.Session.GetString("username");
            var filePath = Path.GetFullPath(@"ExcelTemplate\\SAMPLE.xlsx");
            //string filePath = "C:\\Users\\dell\\source\\repos\\GitJTF\\ExcelTemplate\\JTN Form Changes.xlsx";
            var wbook = new XLWorkbook(filePath);
            var dataStartVal = 14;
            var ws = wbook.Worksheet(1);
            var InsexVal = 1;
            ws.Cell("AA3").Value = CustomerName;
            ws.Cell("G3").Value = DateTime.Now.ToString("dd/MM/yyyy");
            foreach (var windows in details)
            {
                ws.Cell("A" + dataStartVal).Value = InsexVal;
                ws.Cell("B" + dataStartVal).Value = windows.RoomName;
                ws.Cell("C" + dataStartVal).Value = windows.BlindType;
                ws.Cell("D" + dataStartVal).Value = windows.FabricName;
                ws.Cell("E" + dataStartVal).Value = windows.WindowName;
                ws.Cell("F" + dataStartVal).Value = Math.Truncate((Decimal)windows.OrderedWidth).ToString();
                ws.Cell("G" + dataStartVal).Value = Math.Truncate((Decimal)windows.OrderedHeight).ToString();
                ws.Cell("H" + dataStartVal).Value = "Blind Size";
                ws.Cell("I" + dataStartVal).Value = 1;
                ws.Cell("L" + dataStartVal).Value = windows.TotalPrice;
                ws.Cell("M" + dataStartVal).Value = (windows.IsNoValance ? "CLASSIC" : "EVO");
                ws.Cell("N" + dataStartVal).Value = (windows.ControlType.Contains("Stainless Steel Beaded Loop") || windows.ControlType.Contains("Cordless")) ? windows.ControlPosition : windows.ControlType;
                ws.Cell("O" + dataStartVal).Value = "child safety";
                ws.Cell("AA" + dataStartVal).Value = windows.NoOfPanels == 0 ? "" : windows.NoOfPanels;
                ws.Cell("AB" + dataStartVal).Value = windows.StackType;
                ws.Cell("AD" + dataStartVal).Value = (windows.Is2In1 ? "2In1 Blind" : "-");
                dataStartVal++;
                InsexVal++;
            }
            //wbook.SaveAs("C:\\Users\\dell\\source\\repos\\GitJTF\\ExcelTemplate\\JTN Form Changes2.xlsx");
            System.IO.Stream spreadsheetStream = new System.IO.MemoryStream();
            wbook.SaveAs(spreadsheetStream);
            spreadsheetStream.Position = 0;

            return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = String.Concat(CustomerName.Where(c => !Char.IsWhiteSpace(c))) + ".xlsx" };
        }

        [HttpPost]
        public IActionResult InvoiceDetails(Int32 customerId)
        {

            return RedirectToAction("Index", "Invoice", new { customerId = customerId });

        }


        private void SetUserName(int customerId)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                var customer = _dapperDbContext.Customers.FirstOrDefault(x => x.Id == customerId);
                //TempData["username"] = customer.FirstName +" "+ customer.LastName;
                HttpContext.Session.SetString("username", customer.FirstName + " " + customer.LastName);
            }
        }

        private List<WindowDetails> GetRoomDetails(int customerId)
        {
            List<WindowDetails> lstRoomDetails;

            using (var Db = _dapperDbContext)
            {
                var customer = _dapperDbContext.Customers.FirstOrDefault(x => x.Id == customerId);

                lstRoomDetails = (from y in Db.Windows.Where(a => a.CustomerId == customerId && a.IsItemSelected == true)
                                  select new
                                  {
                                      BasePrice = y.BasePrice,
                                      BlindType = y.BlindType,
                                      RoomName = y.RoomName,
                                      Id = y.Id,
                                      FabricName = y.FabricName,
                                      WindowName = y.WindowName,
                                      Height = y.Height,
                                      Width = y.Width,
                                      ControlType = y.ControlType,
                                      ControlPosition = y.Option,
                                      TotalPrice = y.TotalPrice,
                                      IsItemSelected = y.IsItemSelected,
                                      NoOfPanels = y.NoOfPanels,
                                      IsNoValance = y.IsNoValance,
                                      Notes = y.Notes,
                                      Is2In1 = y.Is2In1,
                                      IsNeedExtension = y.IsNeedExtension,
                                      StackType = y.StackType,
                                      OrderedWidth = y.OrderedWidth,
                                      OrderedHeight = y.OrderedHeight

                                  }).AsEnumerable().Select(y => new WindowDetails
                                  {

                                      BasePrice = y.BasePrice,
                                      BlindType = y.BlindType,
                                      RoomName = y.RoomName,
                                      Id = y.Id,
                                      FabricName = y.FabricName,
                                      WindowName = y.WindowName,
                                      Height = y.Height,
                                      Width = y.Width,
                                      ControlType = y.ControlType ?? "",
                                      ControlPosition = y.ControlPosition ?? "",
                                      TotalPrice = y.TotalPrice,
                                      IsItemSelection = y.IsItemSelected ?? false,
                                      NoOfPanels = y.NoOfPanels ?? 0,
                                      IsNoValance = y.IsNoValance ?? false,
                                      Notes = y.Notes,
                                      Is2In1 = y.Is2In1 ?? false,
                                      IsNeedExtension = y.IsNeedExtension ?? false,
                                      StackType = y.StackType ?? "",
                                      OrderedHeight = (double)y.OrderedHeight,
                                      OrderedWidth = (double)y.OrderedWidth,
                                  }).ToList();
            }

            return lstRoomDetails;
        }

        [HttpPost]
        public IActionResult SaveSalesOrderedData(WindowDetails windowDtls)
        {
            using (var Db = _dapperDbContext)
            {

                var result = Db.Windows.Where(a => a.Id == windowDtls.Id).FirstOrDefault();
                if (result != null)
                {

                    result.OrderedWidth = (decimal)(windowDtls.OrderedWidth ?? 0);
                    result.OrderedHeight = (decimal)(windowDtls.OrderedHeight ?? 0);

                    Db.Windows.Update(result);
                }

                Db.SaveChanges();
            }

            return Ok();

        }

        //private double GetOrderedHeight(bool? isInchOrMm, decimal? height)
        //{
        //    double millimeter;
        //    if (isInchOrMm ?? false)
        //    {

        //        millimeter = 25.4 * (double)(height ?? 0);
        //    }
        //    millimeter = (double)(height ?? 0);
        //    return millimeter - 4;
        //}

        //private double GetOrderedWidth(bool? isInchOrMm, decimal? width)
        //{
        //    double millimeter;
        //    if (isInchOrMm ?? false)
        //    {

        //        millimeter = 25.4 * (double)(width ?? 0);
        //    }
        //    millimeter = (double)(width ?? 0);
        //    millimeter = Math.Round(millimeter) switch
        //    {
        //        < 900 => millimeter - 6,
        //        > 900 and < 1500 => millimeter - 7,
        //        > 900 and < 1800 => millimeter - 8,
        //        > 1800 => millimeter - 9,
        //        _ => 0
        //    };

        //    return millimeter;
        //}
    }
}

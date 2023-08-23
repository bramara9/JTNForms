using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.Text.RegularExpressions;

namespace JTNForms.Controllers
{
    public class InvoiceController : Controller
    {
        public dapperDbContext _dapperDbContext = new dapperDbContext();
        public InvoiceController(dapperDbContext dapperDbContext)
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
            var filePath = Path.GetFullPath(@"ExcelTemplate\\Hemanth.xlsx");
            //string filePath = "C:\\Users\\dell\\source\\repos\\GitJTF\\ExcelTemplate\\JTN Form Changes.xlsx";
            var wbook = new XLWorkbook(filePath);
            var dataStartVal = 11;
            var ws = wbook.Worksheet(1);
            var InsexVal = 1;
            ws.Cell("D3").Value = CustomerName;
            //ws.Cell("G3").Value = DateTime.Now.ToString("dd/MM/yyyy");
            foreach (var windows in details)
            {
                ws.Cell("B" + dataStartVal).Value = InsexVal;
                ws.Cell("C" + dataStartVal).Value = windows.RoomName;
                ws.Cell("D" + dataStartVal).Value = windows.WindowName;
                ws.Cell("E" + dataStartVal).Value = windows.FabricName;
                ws.Cell("F" + dataStartVal).Value = windows.NoOfPanels;
                ws.Cell("G" + dataStartVal).Value = windows.ControlType;
              
                ws.Cell("H" + dataStartVal).Value = windows.ControlPosition; 
                ws.Cell("I" + dataStartVal).Value = windows.Area;
               dataStartVal++;
                InsexVal++;
            }
            ws.Cell("B" + dataStartVal).Value = "Totals";
            ws.Cell("C" + dataStartVal).Value = "INVENTORY ITEMS:";
            ws.Cell("F" + dataStartVal).Value = 10;
            ws.Cell("C" + dataStartVal+1).Value = "Installation";
            ws.Cell("C" + dataStartVal + 2).Value = "Total";
            //wbook.SaveAs("C:\\Users\\dell\\source\\repos\\GitJTF\\ExcelTemplate\\JTN Form Changes2.xlsx");
            System.IO.Stream spreadsheetStream = new System.IO.MemoryStream();
            wbook.SaveAs(spreadsheetStream);
            spreadsheetStream.Position = 0;

            return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = String.Concat(CustomerName.Where(c => !Char.IsWhiteSpace(c))) + "_Invoice.xlsx" };
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

            using ( var Db = _dapperDbContext)
            {
                lstRoomDetails = (from y in Db.Windows.Where(a => a.CustomerId == customerId && a.IsItemSelected==true)
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
                                      OrderedHeight = y.OrderedHeight,
                                      OrderedWidth = y.OrderedWidth

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
                                      NoOfPanels = y.NoOfPanels??0,
                                      IsNoValance = y.IsNoValance ?? false,
                                      Notes = y.Notes,
                                      Is2In1 = y.Is2In1 ?? false,
                                      IsNeedExtension = y.IsNeedExtension ?? false,
                                      StackType = y.StackType?? "",
                                      OrderedHeight= (double)y.OrderedHeight,
                                      OrderedWidth= (double)y.OrderedWidth,
                                      Area =getArea((double)y.Width, (double)y.Height)
                                  }).ToList();
            }

            return lstRoomDetails;
        }

        private double? getArea(double? width, double? height)
        {
            return width * height;
        }
    }
}

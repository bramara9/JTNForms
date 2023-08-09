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
                ws.Cell("F" + dataStartVal).Value = windows.Width;
                ws.Cell("G" + dataStartVal).Value = windows.Height;
                ws.Cell("H" + dataStartVal).Value = "Blind Size";
                ws.Cell("I" + dataStartVal).Value = 1;
                ws.Cell("L" + dataStartVal).Value = windows.TotalPrice;
                ws.Cell("M" + dataStartVal).Value = (windows.IsNoValance ? "CLASSIC" : "EVO");
                ws.Cell("N" + dataStartVal).Value = windows.ControlPosition;
                ws.Cell("O" + dataStartVal).Value = "child safety";
                ws.Cell("AA" + dataStartVal).Value = windows.NoOfPanels;
                ws.Cell("AB" + dataStartVal).Value = windows.StackType;
                ws.Cell("AD" + dataStartVal).Value = windows.ControlType + " And 2In1 Blind : " + (windows.Is2In1?"Yes":"No") ;
                dataStartVal++;
                InsexVal++;
            }
            //wbook.SaveAs("C:\\Users\\dell\\source\\repos\\GitJTF\\ExcelTemplate\\JTN Form Changes2.xlsx");
            System.IO.Stream spreadsheetStream = new System.IO.MemoryStream();
            wbook.SaveAs(spreadsheetStream);
            spreadsheetStream.Position = 0;

            return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = String.Concat(CustomerName.Where(c => !Char.IsWhiteSpace(c))) + ".xlsx" };
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
                                      ControlPosition = y.ControlType,
                                      TotalPrice = y.TotalPrice,
                                      IsItemSelected = y.IsItemSelected,
                                      NoOfPanels = y.NoOfPanels,
                                      IsNoValance = y.IsNoValance,
                                      Notes = y.Notes,
                                      Is2In1 = y.Is2In1,
                                      IsNeedExtension = y.IsNeedExtension,
                                      StackType = y.StackType

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
                                      ControlType = y.ControlType,
                                      ControlPosition = y.ControlType,
                                      TotalPrice = y.TotalPrice,
                                      IsItemSelection = y.IsItemSelected ?? false,
                                      NoOfPanels = y.NoOfPanels??0,
                                      IsNoValance = y.IsNoValance ?? false,
                                      Notes = y.Notes,
                                      Is2In1 = y.Is2In1 ?? false,
                                      IsNeedExtension = y.IsNeedExtension ?? false,
                                      StackType = y.StackType
                                  }).ToList();
            }

            return lstRoomDetails;
        }

    }
}

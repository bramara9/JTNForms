using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;

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
            var filePath = Path.GetFullPath(@"ExcelTemplate\\salesorderTemplate.xlsx");
            //string filePath = "C:\\Users\\dell\\source\\repos\\GitJTF\\ExcelTemplate\\JTN Form Changes.xlsx";
            var wbook = new XLWorkbook(filePath);

            var ws = wbook.Worksheet(1);
            ws.Cell("A14").Value = "Srini vasu";
            //wbook.SaveAs("C:\\Users\\dell\\source\\repos\\GitJTF\\ExcelTemplate\\JTN Form Changes2.xlsx");
            System.IO.Stream spreadsheetStream = new System.IO.MemoryStream();
            wbook.SaveAs(spreadsheetStream);
            spreadsheetStream.Position = 0;

            return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = "salesorder.xlsx" };
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
                lstRoomDetails = (from y in Db.Windows.Where(a => a.CustomerId == customerId)
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

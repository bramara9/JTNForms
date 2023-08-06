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
            var details = GetRoomDetails(customerId);
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

        private List<RoomDetails> GetRoomDetails(int customerId)
        {
            List<RoomDetails> lstRoomDetails;

            using ( var Db = _dapperDbContext)
            {
                lstRoomDetails = (from room in Db.Rooms.Where(a => a.CustomerId == customerId)
                                  select new
                                  {
                                      BasePrice = room.BasePrice,
                                      BlindType = room.BlindType,
                                      RoomName = room.RoomName,
                                      Id = room.Id,
                                      Fabric = room.FabricName

                                  }).AsEnumerable().Select(room => new RoomDetails
                                  {

                                      BasePrice = room.BasePrice,
                                      BlindType = room.BlindType,
                                      RoomName = room.RoomName,
                                      Id = room.Id,
                                      Fabric = room.Fabric,
                                      WindowDetails = Db.Windows.Where(x => x.RoomId == room.Id).Select(y => new WindowDetails()
                                      {
                                          Id = y.Id,
                                          WindowName = y.WindowName,
                                          Height = y.Height,
                                          Width = y.Width,
                                          ControlType = y.ControlType,
                                          ControlPosition = y.ControlType,
                                          TotalPrice = y.TotalPrice

                                      }).ToList()
                                  }).ToList();
            }

            return lstRoomDetails;
        }

    }
}

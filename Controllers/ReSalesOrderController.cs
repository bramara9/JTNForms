using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Spire.Xls;
using Issue = JTNForms.DataModels.Issue;

namespace JTNForms.Controllers
{
    public class ReSalesOrderController : Controller
    {
        public static List<IssuesModel> details = new List<IssuesModel> { new IssuesModel { } };
        public readonly dapperDbContext _dapperDbContext;

        public ReSalesOrderController(dapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }

        // GET: ReSalesOrderController
        public IActionResult Index(int customerId)
        {
            ViewBag.CutomerId = customerId;
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");
            List<WindowDetails> lstRoomDetails;

            var customer = _dapperDbContext.Customers.FirstOrDefault(x => x.Id == customerId);

            lstRoomDetails = (from y in _dapperDbContext.Windows
                              join
                              x in _dapperDbContext.Issues on y.Id equals x.WindowId
                              where y.CustomerId == customerId && x.Resolution == "ReOrder"
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
                                  Notes = x.Notes,
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

            ViewBag.BlindTypes = _dapperDbContext.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
            {
                Value = y.Name,
                Text = y.Name
            }).ToList();
            ViewBag.ControlTypes = _dapperDbContext.LookUps.Where(x => x.Type.Trim() == "ControlType").Select(y => new SelectListItem()
            {
                Value = y.Name,
                Text = y.Name
            }).ToList();
            ViewBag.ControlPositions = _dapperDbContext.LookUps.Where(x => x.Type.Trim() == "ControlPosition").Select(y => new SelectListItem()
            {
                Value = y.Name,
                Text = y.Name
            }).ToList();
            ViewBag.StackTypes = _dapperDbContext.LookUps.Where(x => x.Type.Trim() == "Stack").Select(y => new SelectListItem()
            {
                Value = y.Name,
                Text = y.Name
            }).ToList();



            return View(lstRoomDetails);
        }

        public IActionResult AddIssues(int customerId, string coming = null)
        {
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");
            ViewBag.CutomerId = customerId;

            var RoomDetails = (from y in _dapperDbContext.Windows.Where(a => a.CustomerId == customerId)
                               select new SelectListItem
                               { Value = y.RoomName, Text = y.RoomName }).Distinct().ToList();
            ViewBag.RoomTypes = RoomDetails;
            List<SelectListItem> IssueTypes = new List<SelectListItem> {
                new SelectListItem{ Text="Repair",Value="Repair"},
                new SelectListItem{ Text="ReOrder",Value="ReOrder"},
                };
            ViewBag.IssueTypes = IssueTypes;


            if (coming == "Add")
            {
                var ResaleOrderDtls = JsonConvert.DeserializeObject<List<IssuesModel>>(TempData["IssueDetails"].ToString());
                ResaleOrderDtls.ForEach((room) =>
                 {
                     room.lstWindowsName = (from y in _dapperDbContext.Windows.Where(a => a.RoomName == room.RoomName)
                                            select new SelectListItem
                                            { Value = Convert.ToString(y.Id), Text = y.WindowName }).ToList();

                 });
                ViewBag.IssueDetails = ResaleOrderDtls;
            }
            else
            {
                var dbIssues = _dapperDbContext.Issues.Where(a => a.CustomerId == customerId).ToList();
                if (dbIssues != null && dbIssues.Any())
                {
                    details = (from issues in dbIssues
                               select new
                               {
                                   Description = issues.Description,
                                   RepairId = issues.Id,
                                   IssueType = issues.Resolution,
                                   Notes = issues.Notes,
                                   WindowId = issues.WindowId


                               }).AsEnumerable().Select(issues => new IssuesModel()
                               {

                                   Description = issues.Description,
                                   RepairId = issues.RepairId,
                                   IssueType = issues.IssueType,
                                   Notes = issues.Notes,
                                   WindowId = issues.WindowId,
                                   RoomName = _dapperDbContext.Windows.Where(a => a.Id == issues.WindowId).FirstOrDefault()?.RoomName
                               }).ToList();
                    details.ToList().ForEach(a =>
                    {
                        a.lstWindowsName = (from y in _dapperDbContext.Windows.Where(x => x.RoomName == a.RoomName)
                                            select new SelectListItem
                                            { Value = Convert.ToString(y.Id), Text = y.WindowName }).ToList();
                    });



                }

                ViewBag.IssueDetails = details;

            }


            ViewBag.CutomerId = customerId;
            return View();
        }

        [HttpPost]
        public IActionResult Add(List<IssuesModel> details, int customerId)
        {
            ViewBag.userName = HttpContext.Session.GetString("username");
            if (!ModelState.IsValid)
            {
                ViewBag.IssueDetails = details;
                return RedirectToAction("AddIssues", new { customerId = customerId });
            }
            details.Add(new IssuesModel { });
            TempData["IssueDetails"] = JsonConvert.SerializeObject(details);
            return RedirectToAction("AddIssues", new { customerId = customerId, coming = "Add" });
        }
        [HttpPost]
        public IActionResult Delete(List<IssuesModel> details, Int32 id, int customerId)
        {
            ViewBag.userName = HttpContext.Session.GetString("username");

            if (details.Any())
            {
                details.Remove(details.FirstOrDefault(a => a.IndexVal == id));
            }
            TempData["IssueDetails"] = JsonConvert.SerializeObject(details);
            //return RedirectToAction("Index");
            return RedirectToAction("AddIssues", new { customerId = customerId, coming = "Add" });
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

        [HttpPost]
        public IActionResult GetWindowsDataByWindowsId(string roomName)
        {

            var RoomDetails = (from y in _dapperDbContext.Windows.Where(a => a.RoomName == roomName)
                               select new SelectListItem
                               { Value = Convert.ToString(y.Id), Text = y.WindowName }).ToList();

            return Json(RoomDetails);

        }

        [HttpPost]
        public IActionResult Save(List<IssuesModel> details, int customerId)
        {

            if (details != null && details.Any())
            {
                var excludedissues = details.Select(x => x.RepairId).Distinct();
                var deletedRows = _dapperDbContext.Issues.Where(b => !excludedissues.Contains(b.Id) && b.CustomerId == customerId);
                _dapperDbContext.Issues.RemoveRange(deletedRows);
                foreach (var reOrder in details)
                {

                    var dbIssues = _dapperDbContext.Issues.Where(a => a.Id == reOrder.RepairId).FirstOrDefault();
                    if (dbIssues != null)
                    {
                        dbIssues.WindowId = reOrder.WindowId;
                        dbIssues.Description = reOrder.Description;
                        dbIssues.Notes = reOrder.Notes;
                        dbIssues.Resolution = reOrder.IssueType;
                        dbIssues.CustomerId = customerId;
                        _dapperDbContext.Issues.Update(dbIssues);
                    }
                    else
                    {
                        Issue Issues = new Issue();
                        Issues.Description = reOrder.Description;
                        Issues.WindowId = reOrder.WindowId;
                        Issues.Notes = reOrder.Notes;
                        Issues.Resolution = reOrder.IssueType;
                        Issues.CustomerId = customerId;
                        _dapperDbContext.Issues.Add(Issues);
                    }


                }
                _dapperDbContext.SaveChanges();
            }
            if (details.Where(a => a.IssueType == "ReOrder").Count() > 0)
            {
                return RedirectToAction("Index", new { customerId = customerId });
            }
            else
            {
                return RedirectToAction("Customer", "Customer", new { id = customerId });
            }

        }

        [HttpPost]
        public IActionResult SaveReOrderDetails(List<WindowDetails> details, int customerId)
        {
            FileStreamResult result = null;
            var customer = _dapperDbContext.Customers.First(c => c.Id == customerId);
            if (details != null && details.Any())
            {

                foreach (var reOrder in details)
                {

                    var dbWindow = _dapperDbContext.Windows.Where(a => a.Id == reOrder.Id).FirstOrDefault();
                    if (dbWindow != null)
                    {
                        dbWindow.ReOrdered = true;
                        _dapperDbContext.Windows.Update(dbWindow);
                    }

                    ReorderWindow tblWindow = new ReorderWindow();
                    tblWindow.WindowId = reOrder.Id;
                    tblWindow.RoomName = reOrder.RoomName;
                    tblWindow.FabricName = reOrder.FabricName;
                    tblWindow.BlindType = reOrder.BlindType;
                    tblWindow.NoOfPanels = reOrder.NoOfPanels;
                    tblWindow.ControlType = reOrder.ControlType;
                    tblWindow.Option = reOrder.ControlPosition;
                    tblWindow.StackType = reOrder.StackType;
                    tblWindow.Is2In1 = reOrder.Is2In1;
                    tblWindow.IsNoValance = reOrder.IsNoValance;
                    tblWindow.IsNeedExtension = reOrder.IsNeedExtension;
                    tblWindow.OrderedWidth = (decimal)GetOrderedWidth(customer.IsInchOrMm, reOrder.Width);
                    tblWindow.OrderedHeight = (decimal)GetOrderedHeight(customer.IsInchOrMm, reOrder.Height);
                    tblWindow.Notes = reOrder.Notes;
                    _dapperDbContext.ReorderWindows.Add(tblWindow);

                }
                _dapperDbContext.SaveChanges();
                result = downloadRESalesOrder(customerId);

            }

            if (result != null)
                return result;
            return RedirectToAction("Customer", "Customer", new { id = customerId });


        }

        public FileStreamResult downloadRESalesOrder(int customerId)
        {
            using (var Db = _dapperDbContext)
            {
                var details = (from y in Db.Windows
                               join
                               x in Db.ReorderWindows on y.Id equals x.WindowId
                               where y.CustomerId == customerId && y.ReOrdered == true
                               select new
                               {
                                   BasePrice = y.BasePrice,
                                   BlindType = x.BlindType,
                                   RoomName = y.RoomName,
                                   Id = y.Id,
                                   FabricName = x.FabricName,
                                   WindowName = y.WindowName,
                                   Height = y.Height,
                                   Width = y.Width,
                                   ControlType = x.ControlType,
                                   ControlPosition = x.Option,
                                   TotalPrice = y.TotalPrice,
                                   IsItemSelected = y.IsItemSelected,
                                   NoOfPanels = x.NoOfPanels,
                                   IsNoValance = x.IsNoValance,
                                   Notes = x.Notes,
                                   Is2In1 = x.Is2In1,
                                   IsNeedExtension = x.IsNeedExtension,
                                   StackType = x.StackType,
                                   OrderedWidth = x.OrderedWidth,
                                   OrderedHeight = x.OrderedHeight

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
                return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = String.Concat(CustomerName.Where(c => !Char.IsWhiteSpace(c))) + "_ReOrder.xlsx" };

            }
        }

        private double GetOrderedHeight(bool? isInchOrMm, decimal? height)
        {
            double millimeter;
            if (isInchOrMm ?? false)
            {

                millimeter = 25.4 * (double)(height ?? 0);
            }
            millimeter = (double)(height ?? 0);
            return millimeter - 4;
        }

        private double GetOrderedWidth(bool? isInchOrMm, decimal? width)
        {
            double millimeter;
            if (isInchOrMm ?? false)
            {

                millimeter = 25.4 * (double)(width ?? 0);
            }
            millimeter = (double)(width ?? 0);
            millimeter = Math.Round(millimeter) switch
            {
                < 900 => millimeter - 6,
                > 900 and < 1500 => millimeter - 7,
                > 900 and < 1800 => millimeter - 8,
                > 1800 => millimeter - 9,
                _ => 0
            };

            return millimeter;
        }
    }
}

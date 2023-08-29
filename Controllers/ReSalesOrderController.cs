using DocumentFormat.OpenXml.Presentation;
using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace JTNForms.Controllers
{
    public class ReSalesOrderController : Controller
    {
        public static List<IssuesModel> details = new List<IssuesModel> { new IssuesModel { } };
        public dapperDbContext _dapperPocDbContext = new dapperDbContext();

        public ReSalesOrderController(dapperDbContext dapperDbContext)
        {
            _dapperPocDbContext = dapperDbContext;
        }

        // GET: ReSalesOrderController
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddIssues(int customerId, string coming = null)
        {
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");


            using (var Db = _dapperPocDbContext)
            {
                var RoomDetails = (from y in Db.Windows.Where(a => a.CustomerId == customerId)
                                   select new SelectListItem
                                   { Value = Convert.ToString(y.Id), Text = y.RoomName }).ToList();
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
                         room.lstWindowsName = (from y in Db.Windows.Where(a => a.Id == room.WindowId)
                                                select new SelectListItem
                                                { Value = y.WindowName, Text = y.WindowName }).ToList();

                     });
                    ViewBag.IssueDetails = ResaleOrderDtls;
                }
                else
                {
                    ViewBag.IssueDetails = details;
                }
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
                return View("Index");
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
                var customer = _dapperPocDbContext.Customers.FirstOrDefault(x => x.Id == customerId);
                //TempData["username"] = customer.FirstName +" "+ customer.LastName;
                HttpContext.Session.SetString("username", customer.FirstName + " " + customer.LastName);
            }
        }

        [HttpPost]
        public IActionResult GetWindowsDataByWindowsId(int windowId)
        {
            using (var Db = _dapperPocDbContext)
            {
                var RoomDetails = (from y in Db.Windows.Where(a => a.Id == windowId)
                                   select new SelectListItem
                                   { Value = y.WindowName, Text = y.WindowName }).ToList();

                return Json(RoomDetails);
            }
        }

        [HttpPost]
        public IActionResult Save(List<IssuesModel> details, int customerId)
        {

            if (details != null && details.Any())
            {

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
    }
}

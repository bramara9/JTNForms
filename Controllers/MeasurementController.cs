using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Linq;

namespace JTNForms.Controllers
{
    public class MeasurementController : Controller
    {

        public static List<WindowDetails> details = new List<WindowDetails> { new WindowDetails { } };
        public dapperDbContext _dapperPocDbContext = new dapperDbContext();

        public static Measurement tmpWindow = new Measurement
        {
            lstWindowDetails = details
        };

        public MeasurementController(dapperDbContext dapperDbContext)
        {
            _dapperPocDbContext = dapperDbContext;
        }

        public IActionResult Index(int customerId)
        {
            ViewBag.userName = HttpContext.Session.GetString("username");
            if (customerId > 0)
            {
                tmpWindow.customerId = customerId;
            }
            ViewBag.JNTFDetails = tmpWindow;

            return View("Index");
        }
        [HttpPost]
        public IActionResult Add(Measurement window)
        {
            //var userId = TempData["username"]?.ToString();
            //TempData.Keep();
            //var customerId = Convert.ToInt32(TempData["CustomerId"]??0);
            ViewBag.userName = HttpContext.Session.GetString("username");
            if (!ModelState.IsValid)
            {
                ViewBag.JNTFDetails = tmpWindow;
                return View("Index");
            }
            window.lstWindowDetails.Add(new WindowDetails { });
            tmpWindow = window;
            return RedirectToAction("Index", new { customerId = window.customerId });
        }


        [HttpPost]
        public IActionResult Copy(Measurement window, Int32 id)
        {
            //var userId = TempData["username"]?.ToString();
            //TempData.Keep();
            ViewBag.userName = HttpContext.Session.GetString("username");
            if (!ModelState.IsValid)
            {
                ViewBag.JNTFDetails = tmpWindow;
                return RedirectToAction("Index");
            }
            if (window.lstWindowDetails.Any())
            {
                window.lstWindowDetails.Add(window.lstWindowDetails.FirstOrDefault(a => a.IndexVal == id));
            }
            else
            {
                window.lstWindowDetails.Add(new WindowDetails { });
            }
            tmpWindow = window;
            return RedirectToAction("Index", new { customerId = window.customerId });
        }

        [HttpPost]
        public IActionResult Delete(Measurement window, Int32 id)
        {
            ViewBag.userName = HttpContext.Session.GetString("username");

            if (window.lstWindowDetails.Any())
            {
                window.lstWindowDetails.Remove(window.lstWindowDetails.FirstOrDefault(a => a.IndexVal == id));
            }
            tmpWindow = window;
            //return RedirectToAction("Index");
            return RedirectToAction("Index", new { customerId = window.customerId });
        }

        [HttpPost]
        public IActionResult AutoCompleteData(string prefix)
        {

            var names = _dapperPocDbContext.LookUps.Where(x => x.Type.Trim() == "Room" && x.Name.ToLower().StartsWith(prefix.ToLower())).Select(y => y.Name).ToList();
            return Json(names);
        }

        [HttpPost]
        public IActionResult Save(Measurement jntfModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.JNTFDetails = tmpWindow;
                return RedirectToAction("Index");
            }
            var mapper = MapperConfig.InitializeAutomapper();
            List<Window> windows = new List<Window>();
            foreach (var x in jntfModel.lstWindowDetails)
            {
                windows.Add(mapper.Map<WindowDetails, Window>(x));
            }
            if (windows.Count > 0)
            {
                var rooms = windows.Select(x => x.RoomName.Trim()).Distinct();
                var res = rooms.Select(y => new Room() { RoomName = y, Windows = windows.Where(u => u.RoomName == y).ToList() });
                var customer = _dapperPocDbContext.Customers.First(c => c.Id == jntfModel.customerId);
                customer.Rooms.AddRange<Room>((IEnumerable<Room>)res);
                _dapperPocDbContext.SaveChanges();
            }
            return Json("Ok");

        }

    }
}

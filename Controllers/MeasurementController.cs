using JTNForms.DataModels;
using JTNForms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace JTNForms.Controllers
{
    public class MeasurementController : Controller
    {

        public static List<WindowDetails> details = new List<WindowDetails> { new WindowDetails { } };
        public static Measurement tmpWindow = new Measurement
        {
            lstWindowDetails = details
        };
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
        public IActionResult Copy(Measurement window,Int32 id )
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
        public IActionResult AutoCompleteData(string Prefix)
        {
            //Note : you can bind same list from database  
            List<City> ObjList = new List<City>()
            {

                new City {Id=1,Name="Latur" },
                new City {Id=2,Name="Mumbai" },
                new City {Id=3,Name="Pune" },
                new City {Id=4,Name="Delhi" },
                new City {Id=5,Name="Dehradun" },
                new City {Id=6,Name="Noida" },
                new City {Id=7,Name="New Delhi" },
                  new City {Id=8,Name="New Mumbai" }

        };
            //Searching records from list using LINQ query  
            var Name = (from N in ObjList
                        where N.Name.StartsWith(Prefix)
                        select new { N.Name });
            return Json(Name);
        }

        [HttpPost]
        public IActionResult Save(Measurement jntfModel)
        {
            return Json("Ok");
           
        }


    }

    internal class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

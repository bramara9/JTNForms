using JTNForms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JTNForms.Controllers
{
    public class MeasurementController : Controller
    {

        public static List<WindowDetails> details = new List<WindowDetails> { new WindowDetails { } };
        public static Measurement tmpWindow = new Measurement
        {
            lstWindowDetails = details
        };
        public IActionResult Index()
        {
            return View("Index");
        }
        [HttpPost]
        public IActionResult Add(Measurement window)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.JNTFDetails = tmpWindow;
                return View("Index");
            }
            tmpWindow.lstWindowDetails.Add(new WindowDetails { });
            tmpWindow = window;
            return RedirectToAction("Index");
        }
        public IActionResult Add(int customerId)
        {          
            tmpWindow.lstWindowDetails.Add(new WindowDetails { });
            return View("Index");
        }
        [HttpPost]
        public IActionResult Save(Measurement jntfModel)
        {
            return Json("Ok");
           
        }


    }
}

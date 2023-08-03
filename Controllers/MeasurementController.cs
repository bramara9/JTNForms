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
using Microsoft.AspNetCore.Mvc.Rendering;

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
            if (customerId > 0 && tmpWindow.customerId <= 0)
            {
                tmpWindow.customerId = customerId;

                var result = (from c in _dapperPocDbContext.Customers.Where(a => a.Id == customerId)
                              orderby c.Id
                              select new Measurement
                              {
                                  customerId = c.Id,
                                  IsInchOrMM = true,

                                  lstWindowDetails =
                                  (from r in _dapperPocDbContext.Rooms
                                   join w in _dapperPocDbContext.Windows
                                   on r.Id equals w.RoomId
                                   where r.CustomerId == c.Id
                                   select new WindowDetails
                                   {
                                       Height = w.Height,
                                       Width = w.Width,
                                       Notes = w.Notes,
                                       RoomName = r.RoomName,
                                       WindowName = w.WindowName,


                                   })
                                   .ToList()
                              }).FirstOrDefault();

                ViewBag.JNTFDetails = result;

            }
            else
            {
                ViewBag.JNTFDetails = tmpWindow;
            }


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
                return RedirectToAction("Index", new { customerId = jntfModel.customerId });
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
            return RedirectToAction("RoomDetails", new { customerId = jntfModel.customerId });

        }

        [HttpGet]
        public IActionResult RoomDetails(int customerId)
        {
            ViewBag.userName = HttpContext.Session.GetString("username");
            List<RoomDetails> lstRoomDetails = new List<RoomDetails>();
            lstRoomDetails = GetRoomDetails(customerId);
            ViewBag.CutomerId = customerId;
            return View(lstRoomDetails);

        }
        [HttpPost]
        public IActionResult SaveRoomDetails(List<RoomDetails> roomDetails, Int32 customerId)
        {
            using (var Db = _dapperPocDbContext)
            {
                foreach (var roomDtls in roomDetails)
                {
                    var result = Db.Rooms.Where(a => a.Id == roomDtls.Id).FirstOrDefault();
                    if (result != null)
                    {

                        result.FabricName = roomDtls.Fabric;
                        result.BasePrice = roomDtls.BasePrice;
                        result.BlindType = roomDtls.BlindType;
                    }
                }
                Db.SaveChanges();
            }

            return RedirectToAction("WindowDetails", new { customerId = customerId });

        }

        [HttpGet]
        public IActionResult WindowDetails(int customerId)
        {
            ViewBag.userName = HttpContext.Session.GetString("username");
            List<RoomDetails> lstRoomDetails = new List<RoomDetails>();
            lstRoomDetails = GetRoomDetails(customerId);
            ViewBag.CutomerId = customerId;
            return View("WindowDetails", lstRoomDetails);
        }
        [HttpPost]
        public IActionResult SaveWindowDetails(List<RoomDetails> roomDetails, Int32 customerId)
        {
            using (var Db = _dapperPocDbContext)
            {
                foreach (var roomDtls in roomDetails)
                {
                    foreach (var windowDtls in roomDtls.WindowDetails)
                    {
                        var result = Db.Windows.Where(a => a.Id == windowDtls.Id).FirstOrDefault();
                        if (result != null)
                        {

                            //result.TotalPrice = windowDtls.TotalPrice;
                            result.ControlType = windowDtls.ControlType;
                            result.Option = windowDtls.ControlPosition;
                            result.Width = windowDtls.Width;
                            result.Height = windowDtls.Height;
                            result.Notes = windowDtls.Notes;
                        }
                    }
                }
                Db.SaveChanges();
            }

            return RedirectToAction("Index", "Invoice", new { customerId = customerId });

        }

        private List<RoomDetails> GetRoomDetails(int customerId)
        {
            List<RoomDetails> lstRoomDetails;
            using (var Db = _dapperPocDbContext)
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
                                      BlindTypes = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                                      {

                                          Value = y.Name,
                                          Text = y.Name,
                                          Selected = (y.Name == room.BlindType)
                                      }).ToList(),
                                      WindowDetails = Db.Windows.Where(x => x.RoomId == room.Id).Select(y => new WindowDetails()

                                      {
                                          Id = y.Id,
                                          WindowName = y.WindowName,
                                          Height = y.Height,
                                          Width = y.Width,
                                          ControlType = y.ControlType,
                                          ControlPosition = y.ControlType,
                                          TotalPrice = y.TotalPrice,


                                      }).ToList()
                                  }).ToList();

                if (lstRoomDetails != null && lstRoomDetails.Any())
                {
                    foreach (var room in lstRoomDetails)
                    {
                        foreach (var window in room.WindowDetails)
                        {
                            window.ControlTypes = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                            {

                                Value = y.Name,
                                Text = y.Name,
                                Selected = (y.Name == window.ControlType)
                            }).ToList();
                            window.ControlPositions = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                            {

                                Value = y.Name,
                                Text = y.Name,
                                Selected = (y.Name == window.ControlPosition)
                            }).ToList();
                            window.StackTypes = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                            {

                                Value = y.Name,
                                Text = y.Name,
                                Selected = (y.Name == window.StackType)
                            }).ToList();
                        }
                    }
                }
            }
            return lstRoomDetails;
        }

    }
}

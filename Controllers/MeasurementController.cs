﻿using JTNForms.DataModels;
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

        public static List<RWDetails> details = new List<RWDetails> { new RWDetails { } };
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
            if (tmpWindow.customerId <= 0 || customerId != tmpWindow.customerId)
            {
               //tmpWindow.customerId = customerId;

                var result = (from c in _dapperPocDbContext.Customers.Where(a => a.Id == customerId)
                              orderby c.Id
                              select new Measurement
                              {
                                  customerId = c.Id,
                                  IsInchOrMM = c.IsInchOrMm,

                                  lstWindowDetails =
                                  (from r in _dapperPocDbContext.Rooms
                                   join w in _dapperPocDbContext.Windows
                                   on r.Id equals w.RoomId
                                   where r.CustomerId == c.Id
                                   select new RWDetails
                                   {
                                       RoomId = r.Id,
                                       WindowId   = w.Id,
                                       Height = w.Height,
                                       Width = w.Width,
                                       Notes = w.Notes,
                                       RoomName = r.RoomName,
                                       WindowName = w.WindowName,


                                   })
                                   .ToList()
                              }).FirstOrDefault();
                if (result == null)
                {
                    result = new Measurement();
                    result.customerId=customerId;
                }
                if (result.lstWindowDetails == null || result.lstWindowDetails.Count()==0)
                {
                    result.lstWindowDetails.Add(new RWDetails { });
                }
                tmpWindow = result;
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
            window.lstWindowDetails.Add(new RWDetails { });
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
                window.lstWindowDetails.Add(new RWDetails { });
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
            if (jntfModel!=null && jntfModel.lstWindowDetails.Any())
            {
                 var customer = _dapperPocDbContext.Customers.First(c => c.Id == jntfModel.customerId);
                customer.IsInchOrMm = jntfModel.IsInchOrMM;
                var rooms = jntfModel.lstWindowDetails.Select(a => a.RoomName.Trim()).Distinct();
                foreach (var room in rooms)
                {
                    var dbRooms = _dapperPocDbContext.Rooms.Where(a => a.RoomName.Trim()== room).FirstOrDefault();
                    if (dbRooms != null)
                    {
                        jntfModel.lstWindowDetails.Where(a => a.RoomName.Trim() == room).
                            ToList().ForEach(cc => cc.RoomId = dbRooms.Id);
                        foreach (var windowDtls in jntfModel.lstWindowDetails.Where(a => a.RoomName.Trim() == room))
                        {
                            var dbWindows = _dapperPocDbContext.Windows.Where(a => a.Id == windowDtls.WindowId).FirstOrDefault();
                            if (dbWindows != null)
                            {
                                dbWindows.WindowName = windowDtls.WindowName;
                                dbWindows.Height = windowDtls.Height;
                                dbWindows.Width = windowDtls.Width;
                                dbWindows.Notes = windowDtls.Notes;
                                _dapperPocDbContext.Windows.Update(dbWindows);
                            }
                            else
                            {
                                Window tblWindow = new Window();
                                tblWindow.RoomName = windowDtls.RoomName;
                                tblWindow.RoomId = windowDtls.RoomId;
                                tblWindow.WindowName = windowDtls.WindowName;
                                tblWindow.Height = windowDtls.Height;
                                tblWindow.Width = windowDtls.Width;
                                tblWindow.Notes = windowDtls.Notes;
                                _dapperPocDbContext.Windows.Add(tblWindow);
                            }
                        }
                        _dapperPocDbContext.SaveChanges();
                    }
                    else
                    {
                        Room tblRoom = new Room();
                        tblRoom.RoomName = room;
                        tblRoom.CustomerId = jntfModel.customerId;
                        List<Window> lstWindow = new List<Window>();
                        foreach (var r in jntfModel.lstWindowDetails.Where(a => a.RoomName.Trim() == room).ToList())
                        {
                            Window tblWindow = new Window();
                            tblWindow.RoomName = room;
                            tblWindow.WindowName = r.WindowName;
                            tblWindow.Height = r.Height;
                            tblWindow.Width = r.Width;
                            tblWindow.Notes = r.Notes;
                            lstWindow.Add(tblWindow);
                        }
                        tblRoom.Windows.AddRange(lstWindow);
                        customer.Rooms.Add(tblRoom);
                        _dapperPocDbContext.SaveChanges();
                    }
                
                }
               
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
                        Db.Rooms.Update(result);
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
                            Db.Windows.Update(result);
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

                ViewBag.BlindTypes = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                {
                    Value = y.Name,
                    Text = y.Name
                }).ToList();
                ViewBag.ControlTypes = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                {
                    Value = y.Name,
                    Text = y.Name
                }).ToList();
                ViewBag.ControlPositions = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                {
                    Value = y.Name,
                    Text = y.Name
                }).ToList();
                ViewBag.StackTypes = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                {
                    Value = y.Name,
                    Text = y.Name
                }).ToList();
            }
            return lstRoomDetails;
        }

    }
}

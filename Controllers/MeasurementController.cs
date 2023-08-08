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
using DocumentFormat.OpenXml.Office2010.Excel;

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

        public IActionResult Index(int customerId,string coming=null)
        {
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");
            if (tmpWindow.customerId <= 0 || customerId != tmpWindow.customerId || !string.IsNullOrWhiteSpace(coming))
            {
                tmpWindow.customerId = customerId;

                var result = (from c in _dapperPocDbContext.Customers.Where(a => a.Id == customerId)
                              orderby c.Id
                              select new Measurement
                              {
                                  customerId = c.Id,
                                  IsInchOrMM = c.IsInchOrMm,

                                  lstWindowDetails =
                                  (from w in _dapperPocDbContext.Windows
                                   where w.CustomerId == c.Id
                                   select new WindowDetails
                                   {
                                       Id = w.Id,
                                       Height = w.Height,
                                       Width = w.Width,
                                       Notes = w.Notes,
                                       RoomName = w.RoomName,
                                       WindowName = w.WindowName,


                                   })
                                   .ToList()
                              }).FirstOrDefault();
                if (result == null)
                {
                    result = new Measurement();
                    result.customerId = customerId;
                }
                if (result.lstWindowDetails == null || result.lstWindowDetails.Count() == 0)
                {
                    result.lstWindowDetails.Add(new WindowDetails { });
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
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.JNTFDetails = tmpWindow;
            //    return RedirectToAction("Index", new { customerId = jntfModel.customerId });
            //}
            if (jntfModel != null && jntfModel.lstWindowDetails.Any())
            {
                var customer = _dapperPocDbContext.Customers.First(c => c.Id == jntfModel.customerId);
                customer.IsInchOrMm = jntfModel.IsInchOrMM;
                var excludedBenCodes = jntfModel.lstWindowDetails.Select(x => x.Id).Distinct();
                var deletedRows = _dapperPocDbContext.Windows.Where(b => !excludedBenCodes.Contains(b.Id) && b.CustomerId== jntfModel.customerId);
                _dapperPocDbContext.Windows.RemoveRange(deletedRows);
                //var rooms = jntfModel.lstWindowDetails.Select(a => a.RoomName.Trim()).Distinct();
                foreach (var windowDtls in jntfModel.lstWindowDetails)
                {

                    var dbWindows = _dapperPocDbContext.Windows.Where(a => a.Id == windowDtls.Id).FirstOrDefault();
                    if (dbWindows != null)
                    {
                        dbWindows.RoomName = windowDtls.RoomName;
                        dbWindows.CustomerId = customer.Id;
                        dbWindows.WindowName = windowDtls.WindowName;
                        dbWindows.Height = windowDtls.Height;
                        dbWindows.Width = windowDtls.Width;
                        dbWindows.Notes = windowDtls.Notes;
                        dbWindows.IsItemSelected = windowDtls.IsItemSelection;
                        _dapperPocDbContext.Windows.Update(dbWindows);
                    }
                    else
                    {
                        Window tblWindow = new Window();
                        tblWindow.CustomerId = customer.Id;
                        tblWindow.RoomName = windowDtls.RoomName;
                        tblWindow.WindowName = windowDtls.WindowName;
                        tblWindow.Height = windowDtls.Height;
                        tblWindow.Width = windowDtls.Width;
                        tblWindow.Notes = windowDtls.Notes;
                        tblWindow.IsItemSelected = windowDtls.IsItemSelection;
                        _dapperPocDbContext.Windows.Add(tblWindow);
                    }


                }
                _dapperPocDbContext.SaveChanges();
            }
            return RedirectToAction("RoomDetails", new { customerId = jntfModel.customerId });

        }
      
        public IActionResult RoomDetails(int customerId)
        {
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");
            List<WindowDetails> lstRoomDetails = new List<WindowDetails>();
            lstRoomDetails = GetRoomDetails(customerId);
            lstRoomDetails = (from room in lstRoomDetails.GroupBy(a => a.RoomName.Trim())
                             select new WindowDetails() {
                                 RoomName = room.FirstOrDefault().RoomName,
                                 BlindType=room.FirstOrDefault()?.BlindType,
                                 BasePrice=room.FirstOrDefault()?.BasePrice,
                                 FabricName=room.FirstOrDefault()?.FabricName,
                                 IsItemSelection=room.FirstOrDefault()?.IsItemSelection ?? false

                             }).ToList();
            ViewBag.CutomerId = customerId;
            return View(lstRoomDetails);

        }
        [HttpPost]
        public IActionResult SaveRoomDetails(List<WindowDetails> roomDetails, Int32 customerId)
        {
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("RoomDetails", new { customerId = customerId });
            //}
            using (var Db = _dapperPocDbContext)
            {
                foreach (var roomDtls in roomDetails)
                {
                   
                    var result = Db.Windows.Where(a => a.RoomName.Trim() == roomDtls.RoomName.Trim());
                    (from room in result
                     select room).ToList().ForEach((room) =>
                     {
                         room.FabricName = roomDtls.FabricName;
                         room.BasePrice = roomDtls.BasePrice;
                         room.BlindType = roomDtls.BlindType;    

                     });
                    Db.Windows.UpdateRange(result);
                    
                }
                Db.SaveChanges();
            }

            return RedirectToAction("WindowDetails", new { customerId = customerId });

        }

        public IActionResult WindowDetails(int customerId)
        {
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");
            List<RoomDetails> lstRoomDetails = new List<RoomDetails>();
            lstRoomDetails = GetWindowDetails(customerId);
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
                            result.Notes = windowDtls.Notes;
                            result.IsItemSelected = windowDtls.IsItemSelection;
                            result.Is2In1 = windowDtls.Is2In1;
                            result.IsNoValance = windowDtls.IsNoValance;
                            result.IsNeedExtension = windowDtls.IsNeedExtension;
                            result.StackType = windowDtls.StackType;
                            result.NoOfPanels = windowDtls.NoOfPanels;

                            Db.Windows.Update(result);
                        }
                    }

                }
                Db.SaveChanges();
            }

            return RedirectToAction("Index", "Invoice", new { customerId = customerId });

        }

        private List<WindowDetails> GetRoomDetails(int customerId)
        {
            List<WindowDetails> lstRoomDetails;
            using (var Db = _dapperPocDbContext)
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

                ViewBag.BlindTypes = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
                {
                    Value = y.Name,
                    Text = y.Name
                }).ToList();
            }
            return lstRoomDetails;
        }

        private List<RoomDetails> GetWindowDetails(int cutomerId)
        {
            List<RoomDetails> lstRoomDetails=new List<RoomDetails>();
            using (var Db = _dapperPocDbContext)
            {
                List<WindowDetails>  lstWindowDetails = (from y in Db.Windows.Where(a => a.CustomerId == cutomerId)
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
                                      RoomName = y.RoomName.Trim(),
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
                                      IsNeedExtension = y.IsNeedExtension?? false,
                                      StackType = y.StackType

                                  }).ToList();

                var details = (from room in lstWindowDetails.GroupBy(a => a.RoomName.Trim())
                                  select new 
                                  {
                                      RoomName = room.Key
                                  }).ToList();
                foreach(var room in details)
                {
                    RoomDetails rDetails = new RoomDetails()
                    {
                        WindowDetails = lstWindowDetails.Where(a => a.RoomName == room.RoomName).ToList(),
                        BasePrice = lstWindowDetails.Where(a => a.RoomName == room.RoomName).FirstOrDefault().BasePrice,
                        RoomName = lstWindowDetails.Where(a => a.RoomName == room.RoomName).FirstOrDefault().RoomName,
                        BlindType = lstWindowDetails.Where(a => a.RoomName == room.RoomName).FirstOrDefault().BlindType,
                        Fabric = lstWindowDetails.Where(a => a.RoomName == room.RoomName).FirstOrDefault().FabricName,
                    };
                    lstRoomDetails.Add(rDetails);
                        
                }

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

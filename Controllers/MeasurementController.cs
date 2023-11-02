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
using System.Web.Helpers;
using System.Net;

namespace JTNForms.Controllers
{
    public class MeasurementController : Controller
    {

        //public static List<WindowDetails> details = new List<WindowDetails> { new WindowDetails { } };
        public readonly dapperDbContext Db;

        //public static Measurement tmpWindow = new Measurement
        //{
        //    lstWindowDetails = details
        //};

        public MeasurementController(dapperDbContext dapperDbContext)
        {
            Db = dapperDbContext;
        }

        public IActionResult Measurement(int customerId)
        {
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");
            //if (tmpWindow.customerId <= 0 || customerId != tmpWindow.customerId || !string.IsNullOrWhiteSpace(coming))
            //{

            //tmpWindow.customerId = customerId;

            var result = (from c in Db.Customers.Where(a => a.Id == customerId)
                          orderby c.Id
                          select new Measurement
                          {
                              customerId = c.Id,
                              IsInchOrMM = c.IsInchOrMm,

                              lstWindowDetails =
                              (from w in Db.Windows
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
            //tmpWindow = result;
            ViewBag.JNTFDetails = result;


            //}
            //else
            //{
            //    ViewBag.JNTFDetails = tmpWindow;
            //}


            return View("Index");
        }

        private void SetUserName(int customerId)
        {
            if (HttpContext.Session.GetString("username") == null)
            {

                var customer = Db.Customers.FirstOrDefault(x => x.Id == customerId);
                //TempData["username"] = customer.FirstName +" "+ customer.LastName;
                HttpContext.Session.SetString("username", customer.FirstName + " " + customer.LastName);

            }
        }

        //[HttpPost]
        //public IActionResult Add(Measurement window)
        //{
        //    //var userId = TempData["username"]?.ToString();
        //    //TempData.Keep();
        //    //var customerId = Convert.ToInt32(TempData["CustomerId"]??0);
        //    ViewBag.userName = HttpContext.Session.GetString("username");
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.JNTFDetails = tmpWindow;
        //        return View("Index");
        //    }
        //    window.lstWindowDetails.Add(new WindowDetails { });
        //    tmpWindow = window;
        //    return RedirectToAction("Index", new { customerId = window.customerId });
        //}


        //[HttpPost]
        //public IActionResult Copy(Measurement window, Int32 id)
        //{
        //    //var userId = TempData["username"]?.ToString();
        //    //TempData.Keep();
        //    ViewBag.userName = HttpContext.Session.GetString("username");
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.JNTFDetails = tmpWindow;
        //        return RedirectToAction("Index");
        //    }
        //    WindowDetails wDetails = new WindowDetails { };
        //    if (window.lstWindowDetails.Any())
        //    {
        //        var windowDetails = window.lstWindowDetails.FirstOrDefault(a => a.IndexVal == id);
        //        wDetails = new WindowDetails()
        //        {
        //            BasePrice = windowDetails.BasePrice,
        //            BlindType = windowDetails.BlindType,
        //            RoomName = windowDetails.RoomName,
        //            FabricName = windowDetails.FabricName,
        //            WindowName = windowDetails.WindowName,
        //            Height = windowDetails.Height,
        //            Width = windowDetails.Width,
        //            ControlType = windowDetails.ControlType,
        //            ControlPosition = windowDetails.ControlPosition,
        //            TotalPrice = windowDetails.TotalPrice,
        //            IsItemSelection = windowDetails.IsItemSelection,
        //            NoOfPanels = windowDetails.NoOfPanels,
        //            IsNoValance = windowDetails.IsNoValance,
        //            Notes = windowDetails.Notes,
        //            Is2In1 = windowDetails.Is2In1,
        //            IsNeedExtension = windowDetails.IsNeedExtension,
        //            StackType = windowDetails.StackType
        //        };

        //        window.lstWindowDetails.Add(wDetails);
        //    }
        //    else
        //    {
        //        window.lstWindowDetails.Add(wDetails);
        //    }
        //    tmpWindow = window;
        //    return RedirectToAction("Index", new { customerId = window.customerId });
        //}

        //[HttpPost]
        //public IActionResult Delete(Measurement window, Int32 id)
        //{
        //    ViewBag.userName = HttpContext.Session.GetString("username");

        //    if (window.lstWindowDetails.Any())
        //    {
        //        window.lstWindowDetails.Remove(window.lstWindowDetails.FirstOrDefault(a => a.IndexVal == id));
        //    }
        //    tmpWindow = window;
        //    //return RedirectToAction("Index");
        //    return RedirectToAction("Index", new { customerId = window.customerId });
        //}

        [HttpPost]
        public IActionResult AutoCompleteData(string prefix)
        {

            var names = Db.LookUps.Where(x => x.Type.Trim() == "Room" && x.Name.ToLower().StartsWith(prefix.ToLower())).Select(y => y.Name).ToList();
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
                var customer = Db.Customers.First(c => c.Id == jntfModel.customerId);
                customer.IsInchOrMm = jntfModel.IsInchOrMM;
                var excludedBenCodes = jntfModel.lstWindowDetails.Select(x => x.Id).Distinct();
                var deletedRows = Db.Windows.Where(b => !excludedBenCodes.Contains(b.Id) && b.CustomerId == jntfModel.customerId);
                Db.Windows.RemoveRange(deletedRows);
                //var rooms = jntfModel.lstWindowDetails.Select(a => a.RoomName.Trim()).Distinct();
                foreach (var windowDtls in jntfModel.lstWindowDetails)
                {

                    var dbWindows = Db.Windows.Where(a => a.Id == windowDtls.Id).FirstOrDefault();
                    if (dbWindows != null)
                    {
                        dbWindows.RoomName = windowDtls.RoomName;
                        dbWindows.CustomerId = customer.Id;
                        dbWindows.WindowName = windowDtls.WindowName;
                        dbWindows.Height = windowDtls.Height;
                        dbWindows.Width = windowDtls.Width;
                        dbWindows.Notes = windowDtls.Notes;
                        dbWindows.IsItemSelected = windowDtls.IsItemSelection;
                        Db.Windows.Update(dbWindows);
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
                        Db.Windows.Add(tblWindow);
                    }


                }
                Db.SaveChanges();

            }
            // return RedirectToAction("RoomDetails", new { customerId = jntfModel.customerId });
            return StatusCode((int)HttpStatusCode.OK, "OK");
        }

        public IActionResult RoomDetails(int customerId)
        {
            SetUserName(customerId);
            ViewBag.userName = HttpContext.Session.GetString("username");
            List<WindowDetails> lstRoomDetails = new List<WindowDetails>();
            lstRoomDetails = GetRoomDetails(customerId);
            lstRoomDetails = (from room in lstRoomDetails.GroupBy(a => a.RoomName.Trim())
                              select new 
                              {
                                  RoomName = room.FirstOrDefault().RoomName,
                                  BlindType = room.FirstOrDefault()?.BlindType,
                                  CatalogName = room.FirstOrDefault()?.CatalogType,
                                  FabricName = room.FirstOrDefault()?.FabricName,
                                  IsItemSelection = room.FirstOrDefault()?.IsItemSelection ?? false,


                              }).AsEnumerable().Select( room => new WindowDetails{
                                  RoomName = room.RoomName,
                                  BlindType = room.BlindType,
                                  CatalogType = room.CatalogName,
                                  FabricName = room.FabricName,
                                  IsItemSelection = room.IsItemSelection,
                                  lstFabricNames = GetFabricNamesByCateLogName(room.CatalogName).ToList(),
                                  lstBlindTypes = GetBlindTypeByFabricName(room.CatalogName, room.FabricName).ToList()
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

            var customer = Db.Customers.First(c => c.Id == customerId);

            foreach (var roomDtls in roomDetails)
            {

                var result = Db.Windows.Where(a => a.RoomName.Trim() == roomDtls.RoomName.Trim());
                (from room in result
                 select room).ToList().ForEach((room) =>
                 {
                     room.CatalogName = roomDtls.CatalogType;
                     room.FabricName = roomDtls.FabricName;
                     room.BasePrice = roomDtls.BasePrice;
                     room.BlindType = roomDtls.BlindType;
                     room.OrderedHeight = (decimal)GetOrderedHeight(customer.IsInchOrMm, room.Height);
                     room.OrderedWidth = (decimal)GetOrderedWidth(customer.IsInchOrMm, room.Width);


                 });
                Db.Windows.UpdateRange(result);

            }
            Db.SaveChanges();


            return RedirectToAction("Measurement", new { customerId = customerId });

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
        public IActionResult SaveWindowDetails(List<RoomDetails> roomDetails, Int32 customerId, string viewName)
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

            if (viewName == "Invoice")
            {
                return RedirectToAction("Index", "Invoice", new { customerId = customerId });
            }
            else
            {
                return RedirectToAction("Index", "SalesOrder", new { customerId = customerId });
            }

        }

        private List<WindowDetails> GetRoomDetails(int customerId)
        {
            List<WindowDetails> lstRoomDetails;

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
                                  ControlPosition = y.Option,
                                  TotalPrice = y.TotalPrice,
                                  IsItemSelected = y.IsItemSelected,
                                  NoOfPanels = y.NoOfPanels,
                                  IsNoValance = y.IsNoValance,
                                  Notes = y.Notes,
                                  Is2In1 = y.Is2In1,
                                  IsNeedExtension = y.IsNeedExtension,
                                  StackType = y.StackType,
                                  CatalogName = y.CatalogName

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
                                  ControlPosition = y.ControlPosition,
                                  TotalPrice = y.TotalPrice,
                                  IsItemSelection = y.IsItemSelected ?? false,
                                  NoOfPanels = y.NoOfPanels ?? 0,
                                  IsNoValance = y.IsNoValance ?? false,
                                  Notes = y.Notes,
                                  Is2In1 = y.Is2In1 ?? false,
                                  IsNeedExtension = y.IsNeedExtension ?? false,
                                  StackType = y.StackType,
                                  CatalogType = y.CatalogName
                              }).ToList();

            //ViewBag.BlindTypes = Db.LookUps.Where(x => x.Type.Trim() == "BlindType").Select(y => new SelectListItem()
            //{
            //    Value = y.Name,
            //    Text = y.Name
            //}).ToList();
            ViewBag.CatalogTypes = Db.LookUps.Where(x => x.Type.Trim() == "CatalogName").Select(y => new SelectListItem()
            {
                Value = y.Name,
                Text = y.Name
            }).ToList();

            return lstRoomDetails;
        }

        private List<RoomDetails> GetWindowDetails(int cutomerId)
        {
            List<RoomDetails> lstRoomDetails = new List<RoomDetails>();

            List<WindowDetails> lstWindowDetails = (from y in Db.Windows.Where(a => a.CustomerId == cutomerId)
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
                                                        ControlType = y.ControlType ?? "Stainless Steel Beaded Loop",
                                                        ControlPosition = y.ControlPosition ?? "Right",
                                                        TotalPrice = y.TotalPrice,
                                                        IsItemSelection = y.IsItemSelected ?? false,
                                                        NoOfPanels = y.NoOfPanels ?? 0,
                                                        IsNoValance = y.IsNoValance ?? false,
                                                        Notes = y.Notes,
                                                        Is2In1 = y.Is2In1 ?? false,
                                                        IsNeedExtension = y.IsNeedExtension ?? false,
                                                        StackType = y.StackType ?? ""

                                                    }).ToList();

            var details = (from room in lstWindowDetails.GroupBy(a => a.RoomName.Trim())
                           select new
                           {
                               RoomName = room.Key
                           }).ToList();
            foreach (var room in details)
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

            ViewBag.ControlTypes = Db.LookUps.Where(x => x.Type.Trim() == "ControlType").Select(y => new SelectListItem()
            {
                Value = y.Name,
                Text = y.Name
            }).ToList();
            ViewBag.ControlPositions = Db.LookUps.Where(x => x.Type.Trim() == "ControlPosition").Select(y => new SelectListItem()
            {
                Value = y.Name,
                Text = y.Name
            }).ToList();
            ViewBag.StackTypes = Db.LookUps.Where(x => x.Type.Trim() == "Stack").Select(y => new SelectListItem()
            {
                Value = y.Name,
                Text = y.Name
            }).ToList();

            return lstRoomDetails;

        }
        [HttpGet]
        public IActionResult GetFabricNameByCatalogId(string CatalogId)
        {
            var result = GetFabricNamesByCateLogName(CatalogId);
            return Json(result);


        }

        private IList<SelectListItem> GetFabricNamesByCateLogName(string catalogName)
        {

            var result =
            (from y in Db.Fabrics.Where(a => a.CatalogName == catalogName)
             select new SelectListItem
             { Value = y.FabricName, Text = y.FabricName }).ToList();
            return result;

        }

        [HttpGet]
        public IActionResult GetBlindTypeByFabric(string cateLogName, string fabricName)
        {
            var result = GetBlindTypeByFabricName(cateLogName,fabricName);
            return Json(result);


        }

        private IList<SelectListItem> GetBlindTypeByFabricName(string cateLogName,string fabricName)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var fabric = Db.Fabrics.Where(a => a.CatalogName == cateLogName && a.FabricName == fabricName).FirstOrDefault();
            if (fabric != null)
            {
                result =
                (from y in Db.Skus.Where(a => a.FabricId == fabric.Id)
                 select new SelectListItem
                 { Value = y.BlindType, Text = y.BlindType }).ToList();
            }
            return result;

        }

    }
}

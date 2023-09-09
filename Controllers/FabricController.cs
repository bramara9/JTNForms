using Humanizer.Bytes;
using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Web.Helpers;
using static Azure.Core.HttpHeader;

namespace JTNForms.Controllers
{
    public class FabricController : Controller
    {
        public dapperDbContext _dapperPocDbContext = new dapperDbContext();
        public FabricController(dapperDbContext dapperDbContext)
        {
            _dapperPocDbContext = dapperDbContext;
        }
        // GET: FabricController
        public ActionResult Index()
        {
            FabricModel model = new FabricModel();

            using (var Db = _dapperPocDbContext)
            {
                var lstFabricDetails = Db.Fabrics
                    .Select(a => new FabricModel
                    {
                        Id = a.Id,
                        FabricType = a.FabricType,
                        CatalogName = a.CatalogName,
                        FabricName = a.FabricName,
                        FileName = a.FileName,
                        FabricCode = a.FabricCode,
                        FileBytes = a.Image
                    }).ToList();

                ViewBag.FabricDtls = lstFabricDetails;

                ViewBag.CatelogType = Db.LookUps.Where(x => x.Type.Trim() == "CatalogName").Select(y => new SelectListItem()
                {
                    Value = y.Name,
                    Text = y.Name
                }).ToList();
                ViewBag.FabricTypes = Db.LookUps.Where(x => x.Type.Trim() == "FabricType").Select(y => new SelectListItem()
                {
                    Value = y.Name,
                    Text = y.Name
                }).ToList();
            }
            return View(model);
        }

        [HttpPost]

        public IActionResult UploadFile(FabricModel model)
        {
            if (ModelState.IsValid)
            {
                byte[] fileBytes = null; string fileName = null;
                if (model.File != null)
                {
                    FileInfo fileInfo = new FileInfo(model.File.FileName);
                    string fileExtension = fileInfo.Extension;
                    fileName = fileInfo.Name;
                    //TempData["ImageDataBArrayExt"] = fileExtension;
                    if (model.File.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            model.File.CopyTo(ms);
                            fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);
                            //// act on the Base64 data
                            //TempData["ImageDataBArray"] =  JsonConvert.SerializeObject(fileBytes as byte[]); 
                        }
                    }
                }
                model.lstSkuData = JsonConvert.DeserializeObject<List<SkuData>>(model.FileName);
                using (var Db = _dapperPocDbContext)
                {
                    Fabric fabric = new Fabric();
                    fabric.CatalogName = model.CatalogName;
                    fabric.FabricName = model.FabricName;
                    fabric.Image = fileBytes;
                    fabric.FileName = fileName;
                    fabric.FabricType = model.FabricType;
                    fabric.FabricCode = model.FabricCode;
                    Db.Fabrics.Add(fabric);
                    Db.SaveChanges();
                    int generatedId = fabric.Id;
                    if (model.lstSkuData != null && model.lstSkuData.Count > 0)
                    {
                        foreach (var item in model.lstSkuData)
                        {
                            Sku tblSku = new Sku();
                            tblSku.FabricId = generatedId;
                            tblSku.BlindType = item.BlindType;
                            tblSku.Price = item.Price;
                            Db.Skus.Add(tblSku);
                        }
                        Db.SaveChanges();
                    }
                }
                return StatusCode((int)HttpStatusCode.OK);
            }
            //return RedirectToAction("Index", "Fabric");
            return StatusCode((int)HttpStatusCode.BadRequest);
        }

        public FileResult Download(int Id)
        {
            using (var Db = _dapperPocDbContext)
            {
                var fabric = Db.Fabrics.FirstOrDefault(a => a.Id == Id);
                return File(fabric.Image, System.Net.Mime.MediaTypeNames.Application.Octet, fabric.FileName);
            }
        }
        public IActionResult Delete(int Id)
        {
            using (var Db = _dapperPocDbContext)
            {
                var skus = Db.Skus.Where(a => a.FabricId == Id);
                Db.Skus.RemoveRange(skus);
                var fabric = Db.Fabrics.FirstOrDefault(a => a.Id == Id);
                Db.Fabrics.Remove(fabric);
                Db.SaveChanges();
                return RedirectToAction("Index", "Fabric");
            }
        }

        public IActionResult ViewFabricDtls()
        {
            using (var Db = _dapperPocDbContext)
            {

                var lstFabricDetails = Db.Fabrics
              .Select(a => new FabricModel
              {
                  Id = a.Id,
                  FabricType = a.FabricType,
                  CatalogName = a.CatalogName,
                  FabricName = a.FabricName,
                  FileName = a.FileName,
                  FabricCode = a.FabricCode,
                  FileBytes = a.Image
              }).ToList();

                // return Json(lstFabricDetails);
                return PartialView("_AllFabricDetails", lstFabricDetails);
            }
        }
        public IActionResult ViewFabricSKUDtls(int id)
        {
            using (var Db = _dapperPocDbContext)
            {

                var lstSKUDetails = Db.Skus
              .Select(a => new SkuData
              {

                  BlindType = a.BlindType,
                  Price = a.Price
              }).ToList();

                return Json(lstSKUDetails);
            }
        }
    }
}

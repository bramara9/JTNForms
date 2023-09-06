using Humanizer.Bytes;
using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;

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
                        Id=a.Id,
                        FabricType = a.FabricType,
                        CatalogName = a.CatalogName,
                        FabricName = a.FabricName,
                        FileName=a.FileName
                    }).ToList();

                ViewBag.FabricDtls = lstFabricDetails;

                ViewBag.CatelogType =Db.LookUps.Where(x => x.Type.Trim() == "CatalogName").Select(y => new SelectListItem()
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
                byte[] fileBytes = null;
                FileInfo fileInfo = new FileInfo(model.File.FileName);
                string fileExtension = fileInfo.Extension;
                string fileName = fileInfo.Name;
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
                using (var Db = _dapperPocDbContext)
                {
                    Fabric fabric = new Fabric();
                    fabric.CatalogName = model.CatalogName;
                    fabric.FabricName = model.FabricName;
                    fabric.Image = fileBytes;
                    fabric.FileName = fileName;
                    fabric.FabricType = model.FabricType;
                    Db.Fabrics.Add(fabric);
                    Db.SaveChanges();
                }

            }
            return RedirectToAction("Index", "Fabric");
        }

        public FileResult Download(int Id)
        {
            using (var Db = _dapperPocDbContext)
            {
                var fabric = Db.Fabrics.FirstOrDefault(a=>a.Id==Id);
                return File(fabric.Image, System.Net.Mime.MediaTypeNames.Application.Octet, fabric.FileName);
            }
        }
    }
}

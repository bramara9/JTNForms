using JTNForms.DataModels;
using JTNForms.Models;
using JTNForms.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace JTNForms.Controllers
{
    public class CustomerController : Controller
    {
        public dapperDbContext dapperPocDbContext = new dapperDbContext();

        public CustomerController(dapperDbContext dapperDbContext)
        {
            dapperPocDbContext = dapperDbContext;
        }
        public IActionResult Index()
        {
            var customers = new List<DataModels.Customer>();
            customers = dapperPocDbContext.Customers.ToList();
            return View(customers);
        }
        public IActionResult Customer(int id)
        {            
            var customer = dapperPocDbContext.Customers.FirstOrDefault(x => x.Id == id);
            //TempData["username"] = customer.FirstName +" "+ customer.LastName;
            HttpContext.Session.SetString("username", customer.FirstName + " " + customer.LastName);
            return View(customer);
        }
        [HttpPost]
        public IActionResult Add(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            dapperPocDbContext.Customers.Add(customer);
            dapperPocDbContext.SaveChanges();

            var customers = dapperPocDbContext.Customers.ToList();
            return RedirectToAction("Index", customers);
        }


    }
}

﻿using JTNForms.DataModels;
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

        public static List<WindowDetails> details = new List<WindowDetails> { new WindowDetails { } };
        public static Measurement tmpWindow = new Measurement
        {
            lstWindowDetails = details            
        };

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

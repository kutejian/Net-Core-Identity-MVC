using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_Core_Identity_MVC.Data;
using Net_Core_Identity_MVC.Models;
using System.Diagnostics;

namespace Net_Core_Identity_MVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        //[Authorize(Roles = "Admin")]
        [Authorize(Roles = "User")]
        public IActionResult Employee()
        {
           var employee =  _context.Employees.ToListAsync().Result;
            return View(employee);
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Models;
using ProjectManagement.Utilities;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.DataContextModels;

namespace ProjectManagement.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(UserManager<ApplicationUser> userManager) : base(userManager)
        {

        }
        public IActionResult Index()
        {
            var currentUser = UserIdentity;
            return View();
        }

        [HttpGet]
        public ActionResult GetItems(int? page, int? limit, string sortBy, string direction, string name = null)
        {
            int total;
            var records = GetJsonData(page, limit, sortBy, direction, out total, name);

            var result = Json(new { records, total });

            return result;
        }



        public List<ItemType> GetJsonData(int? page, int? limit, string sortBy, string direction, out int total, string name)
        {
            var results = new List<ItemType>();
            results.Add(new ItemType() { ItemTypeId = 1, ItemTypeName = "Type1", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 2, ItemTypeName = "Type2", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 3, ItemTypeName = "Type3", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 4, ItemTypeName = "Type4", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 5, ItemTypeName = "Type5", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 6, ItemTypeName = "Type6", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 7, ItemTypeName = "Type7", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 8, ItemTypeName = "Type8", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 9, ItemTypeName = "Type9", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 10, ItemTypeName = "Type10", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 11, ItemTypeName = "Type11", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 12, ItemTypeName = "Type12", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 13, ItemTypeName = "Type13", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 14, ItemTypeName = "Type14", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 15, ItemTypeName = "Type15", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 16, ItemTypeName = "Type16", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 17, ItemTypeName = "Type17", ItemTypeDescription = "Type1" });
            results.Add(new ItemType() { ItemTypeId = 18, ItemTypeName = "Type18", ItemTypeDescription = "Type1" });

            var records = results.ToArray().AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                records = records.Where(r => r.ItemTypeName.ToLower().Contains(name.Trim().ToLower()));
            }

            total = records.Count();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sortBy);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sortBy);
                }
            }
            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                records = records.Skip(start).Take(limit.Value);
            }
            return records.ToList();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ItemType
    {
        public int ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemTypeDescription { get; set; }
    }
}

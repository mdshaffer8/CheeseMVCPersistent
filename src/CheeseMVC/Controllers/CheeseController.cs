using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            //List<Cheese> cheeses = context.Cheeses.ToList();
            // modify the call to retrieve all Cheese objects to be:
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();
            // this will ensure that when each Cheese object is retrieved from the database,
            // its Category is retrieved as well.

            return View(cheeses);
        }

        public IActionResult Add()
        {
            var allcategories = context.Categories.ToList(); 
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel(allcategories);
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
                       
            if (ModelState.IsValid)
            {
                // this will fetch a single CheeseCategory object, with ID matching the CategoryID value selected
                CheeseCategory newCheeseCategory = context.Categories.SingleOrDefault(c => c.ID == addCheeseViewModel.CategoryID);

                // Add the new cheese to my existing cheeses
                // creation of the newCheese object
                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };

                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }

            return View(addCheeseViewModel);
        }

        public IActionResult Remove()
        {
            ViewBag.title = "Remove Cheeses";
            ViewBag.cheeses = context.Cheeses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            return Redirect("/");
        }
    }
}

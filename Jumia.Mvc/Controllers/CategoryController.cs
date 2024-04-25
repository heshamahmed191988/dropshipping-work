using Jumia.Application.Services;
using Jumia.Dtos.ViewModel.category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jumia.Mvc.Controllers
{
    [Authorize]

    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        // Constructor injection for the category service
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //  /Category/Index

        public async Task<ActionResult> Index()
        {
            var result = await _categoryService.GetallPigintaion(5, 0);

            var categories = result.Entities; // Assuming Eniti holds the list of categories
            return View(categories);

        }

        // /Category/Create
        public IActionResult Create()
        {

            return View();
        }

        //Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryDto category)
        {
            try
            {

                var res = await _categoryService.Create(category);
                if (res.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = res.Message;
                    return View(category);
                }
            }
            catch
            {
                ViewBag.Error = "An error occurred add category.";
                return View(category);
            }


        }
        //  /Category/Edit/{id}
        public async Task<ActionResult> Edit(int id)
        {
            var category = await _categoryService.GetById(id);
            if (category == null)
            {
                return ViewBag.Error = "An error occurred edit category.";
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDto category)
        {
            if (ModelState.IsValid)
            {
                var res = await _categoryService.Update(category);
                if (res.IsSuccess)
                {
                    return RedirectToAction("Index","category");
                }
                else
                {
                    ViewBag.Error = res.Message;
                    return View(category);
                }
            }
            else
            {
                return View(category);
            }
        }




        public async Task<ActionResult> Delete(int id)
        {
            var deleteResult = await _categoryService.SoftDelete(id);
            if (deleteResult.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = deleteResult.Message;
                return RedirectToAction("Index");
            }
        }



    }
}


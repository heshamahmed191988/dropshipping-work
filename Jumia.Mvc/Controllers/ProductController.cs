using Jumia.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Jumia.Dtos.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Jumia.Context;
using Jumia.Dtos;
using Jumia.Dtos.ViewModel.Product;
using Jumia.Model;
namespace Jumia.Mvc.Controllers
{
    public class ProductController : Controller
    {

        private readonly IProductService _proudectService;
        private readonly IProductImageService _productImageService;
        private readonly IUserService userService;

        public ProductController(IProductService productService, IProductImageService productImageService, IUserService userService)
        {
            _proudectService = productService;
            _productImageService = productImageService;
            this.userService = userService;
        }

        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                // Get products data list for the specified page
                var productsDataList = await _proudectService.GetAllPagination(pageSize, pageNumber);
                var products = productsDataList.Entities;

                // If a search string is provided, filter products based on it
                if (!string.IsNullOrEmpty(searchString))
                {
                    // Assuming p.NameEn and p.NameAr are the properties you want to search in
                    products = products.Where(p => p.NameEn.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                                || p.NameAr.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                      .ToList();
                }

                // Calculate total pages
                var totalPages = (int)Math.Ceiling((double)productsDataList.Count / pageSize);

                ViewBag.PageNumber = pageNumber; // Pass pageNumber to the view
                ViewBag.TotalPages = totalPages; // Pass totalPages to the view

                return View(products);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                TempData["ErrorMessage"] = "An error occurred while retrieving products: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<ActionResult> Create()
        {
            var categories = await _proudectService.GetAllCategories();
            var sellers = await _proudectService.GetAllSellers();

            ViewBag.Categories = new SelectList(categories, "Id", "NameEn");
            ViewBag.Categories = new SelectList(categories, "Id", "NameAr");

            ViewBag.Sellers = new SelectList(sellers, "Id", "UserName");

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(ProuductViewModel product)
        {
            if (ModelState.IsValid)
            {
                var result = await _proudectService.Create(product);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    var categories = await _proudectService.GetAllCategories();
                    var sellers = await _proudectService.GetAllSellers();
                    ViewBag.Categories = new SelectList(categories, "Id", "NameEn");
                    ViewBag.Categories = new SelectList(categories, "Id", "NameAr");
                    ViewBag.Sellers = new SelectList(sellers, "Id", "UserName");
                    return View(product);
                }
            }
            else
            {
                var categories = await _proudectService.GetAllCategories();
                var sellers = await _proudectService.GetAllSellers();
                ViewBag.Categories = new SelectList(categories, "Id", "NameEn");
                ViewBag.Categories = new SelectList(categories, "Id", "NameAr");
                ViewBag.Sellers = new SelectList(sellers, "Id", "UserName");
                return View(product);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var product = await _proudectService.GetOne(id);

            if (product == null)
            {
                return NotFound();
            }

            var categories = await _proudectService.GetAllCategories();
            var sellers = await _proudectService.GetAllSellers();

            ViewBag.Categories = new SelectList(categories, "Id", "NameAr", product.CategoryId);
            ViewBag.Categories = new SelectList(categories, "Id", "NameEn", product.CategoryId);

            ViewBag.Sellers = new SelectList(sellers, "Id", "UserName", product.SellerID);

            return View(product);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, ProuductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _proudectService.Update(productViewModel);
                    if (result.IsSuccess)
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = result.Message;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }

            var categories = await _proudectService.GetAllCategories();
            var sellers = await _proudectService.GetAllSellers();
            ViewBag.Categories = new SelectList(categories, "Id", "NameAr", productViewModel.CategoryId);
            ViewBag.Categories = new SelectList(categories, "Id", "NameEn", productViewModel.CategoryId);

            ViewBag.Sellers = new SelectList(sellers, "Id", "UserName", productViewModel.SellerID);

            return View(productViewModel);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var proudect = await _proudectService.GetOne(id);
            return View(proudect);
        }

        [HttpPost]

        public async Task<ActionResult> Delete(int id, ProuductViewModel deletedproudect)
        {
            var proudect = await _proudectService.GetOne(id);
            var result = await _proudectService.SoftDelete(proudect);
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));

        }
        public async Task<ActionResult> Details(int id)
        {


            var product = await _proudectService.GetOne(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _proudectService.GetAllCategories();
            var categoryNameEn = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.NameEn;
            var categoryNameAr = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.NameAr;


            var sellers = await _proudectService.GetAllSellers();
            var sellerName = sellers.FirstOrDefault(s => s.Id == product.SellerID)?.UserName;


            product.CategoryNameEn = categoryNameEn;
            product.CategoryNameAr = categoryNameAr;

            product.SellerName = sellerName;

            return View(product);

        }

        [HttpGet]
        public ActionResult AssignImage(int productId)
        {
            ViewBag.ProductId = productId;
            return View();
        }

        // Action to save the assigned image
        [HttpPost]
        public async Task<ActionResult> AssignImages(int productId, IEnumerable<IFormFile> imageFiles)
        {
            if (imageFiles != null && imageFiles.Any())
            {
                foreach (var imageFile in imageFiles)
                {
                    if (imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                        // Ensure the directory exists
                        var directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Save the file to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Save each file path and product ID in your database
                        var productImageDto = new ProductImageDto
                        {
                            Path = $"/images/products/{fileName}", // Relative path to be used in your web app
                            ProductID = productId
                        };

                        await _productImageService.CreateAsync(productImageDto);
                    }
                }
            }

            return RedirectToAction(nameof(DisplayImages), new { productId = productId });
        }


        [HttpGet]
        public async Task<ActionResult> DisplayImages(int productId)
        {
            var images = await _productImageService.GetByProductIdAsync(productId);
            return View(images);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            // Retrieve the ProductImage by its ID
            var productImage = await _productImageService.GetByIdAsync(id);
            if (productImage == null)
            {
                TempData["ErrorMessage"] = "Image not found.";
                return RedirectToAction(nameof(DisplayImages)); // or handle the error appropriately
            }

            // Delete the image asynchronously
            bool result = await _productImageService.DeleteAsync(productImage.Id);

            if (result)
            {
                TempData["SuccessMessage"] = "Image marked as deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Image could not be marked as deleted.";
            }

            // Adjust redirection as needed
            return RedirectToAction(nameof(DisplayImages), new { productId = productImage.ProductID });
        }



    }
}
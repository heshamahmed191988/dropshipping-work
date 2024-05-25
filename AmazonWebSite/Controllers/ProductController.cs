using Jumia.Application.Services;
using Jumia.Dtos.ViewModel.Product;
using Jumia.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Jumia.Application.Contract;


namespace AmazonWebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IItemServices _itemServices;
        private readonly IProductImageService _productImageService;
        private readonly IConfiguration configuration;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, IItemServices itemServices, IProductImageService productImageService, IConfiguration configuration, ICategoryReposatory categoryRepository, ICategoryService categoryService)
        {
            _productService = productService;
            _itemServices = itemServices;
            _productImageService = productImageService;
            this.configuration = configuration;
            _categoryService = categoryService;
        }

        //[HttpGet("all/sorted")]
        //public async Task<IActionResult> GetAllSorted([FromQuery] int pageSize, [FromQuery] int pageNumber, [FromQuery] string sortOrder = "asc", [FromQuery] int categoryId = 0)
        //{
        //    // Fetch all products or filtered by categoryId if provided (not equals to 0).
        //    var products = categoryId == 0 ?
        //        await _productService.GetAllPagination(pageSize, pageNumber) :
        //        await _productService.GetByCategory(categoryId, pageSize, pageNumber);

        //    // Sorting the products based on the sortOrder parameter.
        //    var sortedProducts = sortOrder.ToLower() == "desc" ?
        //        products.Entities.OrderByDescending(p => p.Price).ToList() :
        //        products.Entities.OrderBy(p => p.Price).ToList();

        //    var productsDTO = sortedProducts.Select(p => new GetAllPaginationUser
        //    {
        //        Price = p.Price,
        //        DescriptionEn = p.DescriptionEn,
        //        DescriptionAr = p.DescriptionAR,
        //        StockQuantity = p.StockQuantity,
        //        NameEn = p.NameEn,
        //        NameAr = p.NameAr,
        //        id = p.Id,
        //        BrandNameAr = p.BrandNameAr,
        //        BrandNameEn = p.BrandNameEn,
        //        ProductImages = new List<string>(), // Initialize to ensure it's not null.
        //        itemscolor = new List<string>() // Assuming colors will be populated below.
        //    }).ToList();

        //    var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

        //    foreach (var item in productsDTO)
        //    {
        //        var productImagePaths = (await _productImageService.GetByProductIdAsync(item.id)).Select(p => p.Path).ToList();

        //        foreach (var imagePath in productImagePaths)
        //        {
        //            try
        //            {
        //                var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
        //                if (System.IO.File.Exists(fullPath))
        //                {
        //                    var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        //                    var base64String = Convert.ToBase64String(imageBytes);
        //                    item.ProductImages.Add(base64String);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Optionally handle the error.
        //                // Skipping the image in this case.
        //            }
        //        }

        //        // Fetch and assign colors; assuming the logic for fetching colors is similar to the original example.
        //        var items = await _itemServices.GetAllPagination(pageSize, pageNumber);
        //        var productColors = items.Entities.Where(p => p.ProductId == item.id).Select(p => p.Color).ToList();
        //        item.itemscolor = productColors;
        //    }

        //    return Ok(productsDTO);
        //}

        [HttpGet("all/sorted")]
        public async Task<IActionResult> GetAllSorted([FromQuery] int pageSize, [FromQuery] int pageNumber, [FromQuery] string sortOrder = "asc", [FromQuery] int categoryId = 0)
        {
            // Fetch all products or filtered by categoryId if provided (not equals to 0).
            var products = categoryId == 0 ?
                await _productService.GetAll() : // Assuming GetAll() fetches all products without pagination
                await _productService.GetByCategory(categoryId); // Assuming GetByCategory() fetches all products in a category without pagination

            // Sorting the products based on the sortOrder parameter.
            var sortedProducts = sortOrder.ToLower() == "desc" ?
                products.OrderByDescending(p => p.Price).ToList() :
                products.OrderBy(p => p.Price).ToList();

            // Applying pagination after sorting
            var paginatedProducts = sortedProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productsDTO = paginatedProducts.Select(p => new GetAllPaginationUser
            {
                Price = p.Price,
                DescriptionEn = p.DescriptionEn,
                DescriptionAr = p.DescriptionAR,
                StockQuantity = p.StockQuantity,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                id = p.Id,
                BrandNameAr = p.BrandNameAr,
                BrandNameEn = p.BrandNameEn,
                MaxPrice = p.MaxPrice,
                MinPrice = p.MinPrice,
                ProductImages = new List<string>(), // Initialize to ensure it's not null.
                itemscolor = new List<string>() // Assuming colors will be populated below.
            }).ToList();

            var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

            foreach (var item in productsDTO)
            {
                var productImagePaths = (await _productImageService.GetByProductIdAsync(item.id)).Select(p => p.Path).ToList();

                foreach (var imagePath in productImagePaths)
                {
                    try
                    {
                        var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                            var base64String = Convert.ToBase64String(imageBytes);
                            item.ProductImages.Add(base64String);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Optionally handle the error.
                        // Skipping the image in this case.
                    }
                }

                // Fetch and assign colors; assuming the logic for fetching colors is similar to the original example.
                var items = await _itemServices.GetAllPagination(pageSize, pageNumber);
                var productColors = items.Entities.Where(p => p.ProductId == item.id).Select(p => p.Color).ToList();
                item.itemscolor = productColors;
            }

            return Ok(productsDTO);
        }


        [HttpGet("all/filteredprice")]
        public async Task<IActionResult> GetAllFilteredByPrice([FromQuery] int pageSize, [FromQuery] int pageNumber, [FromQuery] decimal minPrice, [FromQuery] decimal maxPrice, [FromQuery] int categoryId = 0)
        {
            // Fetch all products or filtered by categoryId if provided (not equals to 0).
            var products = categoryId == 0 ?
                await _productService.GetAll() : // Assuming GetAll() fetches all products without pagination
                await _productService.GetByCategory(categoryId); // Assuming GetByCategory() fetches all products in a category without pagination

            // Filtering the products based on the minPrice and maxPrice parameters.
            var filteredProducts = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();

            // Applying pagination after filtering
            var paginatedProducts = filteredProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productsDTO = paginatedProducts.Select(p => new GetAllPaginationUser
            {
                Price = p.Price,
                DescriptionEn = p.DescriptionEn,
                DescriptionAr = p.DescriptionAR,
                StockQuantity = p.StockQuantity,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                id = p.Id,
                BrandNameAr = p.BrandNameAr,
                BrandNameEn = p.BrandNameEn,
                MaxPrice = p.MaxPrice,
                MinPrice = p.MinPrice,
                Colors=p.itemscolor,
                ProductImages = new List<string>(), // Initialize to ensure it's not null.
                itemscolor = new List<string>() // Assuming colors will be populated below.
            }).ToList();

            var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

            foreach (var item in productsDTO)
            {
                var productImagePaths = (await _productImageService.GetByProductIdAsync(item.id)).Select(p => p.Path).ToList();

                foreach (var imagePath in productImagePaths)
                {
                    try
                    {
                        var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                            var base64String = Convert.ToBase64String(imageBytes);
                            item.ProductImages.Add(base64String);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Optionally handle the error.
                        // Skipping the image in this case.
                    }
                }

                // Fetch and assign colors; assuming the logic for fetching colors is similar to the original example.
                var items = await _itemServices.GetAllPagination(pageSize, pageNumber);
                var productColors = items.Entities.Where(p => p.ProductId == item.id).Select(p => p.Color).ToList();
                item.itemscolor = productColors;
            }

            return Ok(productsDTO);
        }




        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var products = await _productService.GetAllPagination(pageSize, pageNumber);
            var items = await _itemServices.GetAllPagination(pageSize, pageNumber);
            var productsDTO = products.Entities.Select(p => new GetAllPaginationUser
            {
                Price = p.Price,
                DescriptionEn = p.DescriptionEn,
                DescriptionAr = p.DescriptionAR,
                StockQuantity = p.StockQuantity,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                id = p.Id,
                BrandNameAr = p.BrandNameAr,
                BrandNameEn = p.BrandNameEn,
                MaxPrice = p.MaxPrice,
                MinPrice = p.MinPrice,
                Colors=p.itemscolor,
                

                ProductImages = new List<string>(), // Initialize here to ensure it's not null
                itemscolor = new List<string>() // Assuming you'll populate this similarly
            }).ToList();

            var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

            foreach (var item in productsDTO)
            {
                var productImagePaths = (await _productImageService.GetByProductIdAsync(item.id)).Select(p => p.Path).ToList();

                foreach (var imagePath in productImagePaths)
                {
                    try
                    {
                        var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                            var base64String = Convert.ToBase64String(imageBytes);
                            item.ProductImages.Add(base64String);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        // Consider how to handle errors; skipping image in this example
                    }
                }

                // Fetch and assign colors; assuming this is done correctly above
                var productColors = items.Entities.Where(p => p.ProductId == item.id).Select(p => p.Color).ToList();
                item.itemscolor = productColors;
            }

            return Ok(productsDTO);
        }


        //[HttpGet]
        //public async Task<IActionResult> GetOne(int id)
        //{
        //    var product = await _productService.GetOne(id);
        //    if (product == null)
        //    {
        //        return NotFound("Product not found.");
        //    }

        //    var productDTO = new GetOneUser
        //    {

        //        price = product.Price,
        //        NameEn = product.NameEn,
        //        NameAr = product.NameAr,
        //        StockQuantity = product.StockQuantity,
        //        descriptionAr = product.DescriptionAR,
        //        descriptionEn = product.DescriptionEn,
        //        Id = product.Id,
        //        BrandNameAr = product.BrandNameAr,
        //        BrandNameEn = product.BrandNameEn,
        //        Productimages = new List<string>(), // Initialize here to ensure it's not null
        //        itemimages = new List<string>(), // Assuming similar adjustment needed
        //        colors = new List<string>() // Initialize here; assuming you'll populate this similarly
        //    };

        //    var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");
        //    var productImagePaths = (await _productImageService.GetByProductIdAsync(id)).Select(p => p.Path).ToList();

        //    foreach (var imagePath in productImagePaths)
        //    {
        //        try
        //        {
        //            var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
        //            if (System.IO.File.Exists(fullPath))
        //            {
        //                var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        //                var base64String = Convert.ToBase64String(imageBytes);
        //                productDTO.Productimages.Add(base64String);
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }

        //    // Assuming there's a method to fetch items related to this product
        //    // For item images and colors, apply similar logic as needed based on your model
        //    //var items = await _itemServices.GetOne(id); // This method needs to be defined or adjusted according to your actual service layer
        //    //var itemColors = items.Select(i => i.Color).ToList();
        //    //productDTO.colors = itemColors;




        //    return Ok(productDTO);
        //}



        [HttpGet]
        public async Task<IActionResult> GetOne(int id)
        {
            var product = await _productService.GetOne(id);


            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var productDTO = new GetOneUser
            {

                price = product.Price,
                NameEn = product.NameEn,
                NameAr = product.NameAr,
                StockQuantity = product.StockQuantity,
                descriptionAr = product.DescriptionAR,
                descriptionEn = product.DescriptionEn,
                Id = product.Id,
                BrandNameAr = product.BrandNameAr,
                BrandNameEn = product.BrandNameEn,
                MaxPrice = product.MaxPrice,
                MinPrice = product.MinPrice,
                Productimages = new List<string>(), // Initialize here to ensure it's not null
                itemimages = new List<string>(), // Assuming similar adjustment needed
                colors =product.Colors, // Initialize here; assuming you'll populate this similarly
            };

            var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");
            var productImagePaths = (await _productImageService.GetByProductIdAsync(id)).Select(p => p.Path).ToList();

            foreach (var imagePath in productImagePaths)
            {
                try
                {
                    var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                        var base64String = Convert.ToBase64String(imageBytes);
                        productDTO.Productimages.Add(base64String);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            // Assuming there's a method to fetch items related to this product
            // For item images and colors, apply similar logic as needed based on your model
            //var items = await _itemServices.GetOne(id); // This method needs to be defined or adjusted according to your actual service layer
            //var itemColors = items.Select(i => i.Color).ToList();
            //productDTO.colors = itemColors;



            //--------------------- get category name
            var category = await _categoryService.GetById(product.CategoryId);
            if (category != null)
            {
                productDTO.CategoryNameEn = category.NameEn;
                productDTO.CategoryNameAr = category.NameAr;
            }
            return Ok(productDTO);
        }



        [HttpGet("bycatogry")]
        public async Task<IActionResult> Getbycatogery(int catid, [FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            try
            {
                // Assuming GetAllPagination asynchronously retrieves all products with pagination,
                // and we're interested in the first page with a size of 10 for this example.
                var products = (await _productService.GetAllPaginatedByCategoryId(catid, pageSize, pageNumber)).Entities;

                // Initialize an empty list for your DTOs
                var productsDTO = new List<GetAllPaginationUser>();

                // Process each product to include its color and images
                foreach (var product in products.Where(p => p.CategoryId == catid))
                {
                    // Fetch color for the current product by its ID using the ItemService
                    var color = await _itemServices.GetColorByProductId(product.Id);

                    // Prepare to fetch and process product images
                    var productImagePaths = (await _productImageService.GetByProductIdAsync(product.Id)).Select(p => p.Path).ToList();
                    var imagesBase64 = new List<string>();

                    var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

                    foreach (var imagePath in productImagePaths)
                    {
                        try
                        {
                            var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                                var base64String = Convert.ToBase64String(imageBytes);
                                imagesBase64.Add(base64String);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error. For simplicity, the error handling is omitted in this snippet.
                        }
                    }

                    // Add the fully populated product DTO to the list
                    productsDTO.Add(new GetAllPaginationUser
                    {
                        id = product.Id,
                        NameEn = product.NameEn,
                        NameAr = product.NameAr,
                        Price = product.Price,
                        DescriptionEn = product.DescriptionEn,
                        DescriptionAr = product.DescriptionAR,
                        BrandNameAr = product.BrandNameAr,
                        BrandNameEn = product.BrandNameEn,
                        StockQuantity = product.StockQuantity,
                        MaxPrice = product.MaxPrice,
                        MinPrice = product.MinPrice,
                        itemscolor = new List<string> { color }, // Set the dynamically fetched color
                        ProductImages = imagesBase64
                    });
                }

                return Ok(productsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("searchname")]
        public async Task<IActionResult> SearchByName(string name, int pageSize, int pageNumber)
        {
            var products = await _productService.SearchByName(name, pageSize, pageNumber);

            var productsDTO = products.Entities.Select(p => new GetAllPaginationUser
            {
                id = p.Id,
                NameAr = p.NameAr,
                NameEn = p.NameEn,
                Price = p.Price,
                BrandNameAr = p.BrandNameAr,
                BrandNameEn = p.BrandNameEn,
                StockQuantity = p.StockQuantity,
                DescriptionAr = p.DescriptionAR,
                DescriptionEn = p.DescriptionEn,
                MaxPrice = p.MaxPrice,
                MinPrice = p.MinPrice,
                ProductImages = new List<string>()
            }).ToList();

            var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

            foreach (var product in productsDTO)
            {
                var productImagePaths = (await _productImageService.GetByProductIdAsync(product.id)).Select(p => p.Path).ToList();

                foreach (var imagePath in productImagePaths)
                {
                    try
                    {
                        var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                            var base64String = Convert.ToBase64String(imageBytes);
                            product.ProductImages.Add(base64String);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        // Consider how to handle errors; skipping image in this example
                    }
                }
            }

            return Ok(productsDTO);
        }
        [HttpGet("searchbycategory")]
        public async Task<IActionResult> SearchByCategory(int catid)
        {
            var products = await _productService.SearchByCategoriey(catid, 10, 1);
            return Ok(products);
        }
        [HttpGet("searhbyprice")]
        public async Task<IActionResult> SearchByPrice(int minprice, int maxprice)
        {
            var peoducts = await _productService.SearchByPrice(minprice, maxprice, 10, 1);
            return Ok(peoducts);
        }
        [HttpGet("searchbrand")]
        public async Task<IActionResult> SearchByBrand(string name, int pageSize, int pageNumber)
        {
            var products = await _productService.SearchByBrand(name, pageSize, pageNumber);

            var productsDTO = products.Entities.Select(p => new GetAllPaginationUser
            {
                id = p.Id,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                DescriptionEn = p.DescriptionEn,
                DescriptionAr = p.DescriptionAR,
                BrandNameAr = p.BrandNameAr,
                BrandNameEn = p.BrandNameEn,
                MaxPrice = p.MaxPrice,
                MinPrice = p.MinPrice,
                ProductImages = new List<string>()
            }).ToList();

            var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

            foreach (var product in productsDTO)
            {
                var productImagePaths = (await _productImageService.GetByProductIdAsync(product.id)).Select(p => p.Path).ToList();

                foreach (var imagePath in productImagePaths)
                {
                    try
                    {
                        var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                            var base64String = Convert.ToBase64String(imageBytes);
                            product.ProductImages.Add(base64String);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        // Consider how to handle errors; skipping image in this example
                    }
                }
            }

            return Ok(productsDTO);
        }
        [HttpGet("brands")]
        public async Task<IActionResult> GetAllBrands()
        {
            try
            {
                var brands = await _productService.GetAllBrands();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        ///end
        [HttpGet("ByCategoryAndName")]
        public async Task<IActionResult> FilterProductsByCategoryAndName(int categoryId, string name, int pageSize, int pageNumber)
        {
            try
            {
                // Get products filtered by category
                var productsByCategory = await _productService.GetByCategory(categoryId, pageSize, pageNumber);

                // Filter products by name within the selected category
                var filteredProducts = productsByCategory.Entities
                    .Where(p => p.NameEn.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Initialize an empty list for your DTOs
                var productsDTO = new List<GetAllPaginationUser>();
                var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

                // Process each filtered product
                foreach (var product in filteredProducts)
                {
                    // Fetch color for the current product by its ID using the ItemService
                    var color = await _itemServices.GetColorByProductId(product.Id);

                    var productImagePaths = (await _productImageService.GetByProductIdAsync(product.Id)).Select(p => p.Path).ToList();
                    var imagesBase64 = new List<string>();

                    // Process each image path to convert into base64 string
                    foreach (var imagePath in productImagePaths)
                    {
                        try
                        {
                            var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                                var base64String = Convert.ToBase64String(imageBytes);
                                imagesBase64.Add(base64String);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Optionally handle the error, e.g., by logging it. Skipping the image in this case.
                        }
                    }

                    // Add the fully populated product DTO to the list
                    productsDTO.Add(new GetAllPaginationUser
                    {
                        id = product.Id,
                        NameEn = product.NameEn,
                        NameAr = product.NameAr,
                        Price = product.Price,
                        DescriptionEn = product.DescriptionEn,
                        DescriptionAr = product.DescriptionAR,
                        BrandNameAr = product.BrandNameAr,
                        BrandNameEn = product.BrandNameEn,
                        StockQuantity = product.StockQuantity,
                        MaxPrice = product.MaxPrice,
                        MinPrice = product.MinPrice,
                        itemscolor = new List<string> { color }, // Set the fetched color
                        ProductImages = imagesBase64 // Assign the list of base64 images
                    });
                }

                return Ok(productsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ByCategoryAndNameSortedByPrice")]
        public async Task<IActionResult> FilterProductsByCategoryAndNameSortedByPrice(int categoryId, string name, int pageSize, int pageNumber)
        {
            try
            {
                // Get products filtered by category
                var productsByCategory = await _productService.GetByCategory(categoryId, pageSize, pageNumber);

                // Filter products by name within the selected category and sort by price
                var filteredAndSortedProducts = productsByCategory.Entities
                    .Where(p => p.NameEn.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(p => p.Price) // This line sorts the filtered products by price
                    .ToList();

                // Initialize an empty list for your DTOs
                var productsDTO = new List<GetAllPaginationUser>();
                var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");

                // Process each filtered and sorted product
                foreach (var product in filteredAndSortedProducts)
                {
                    // Fetch color for the current product by its ID using the ItemService
                    var color = await _itemServices.GetColorByProductId(product.Id);

                    var productImagePaths = (await _productImageService.GetByProductIdAsync(product.Id)).Select(p => p.Path).ToList();
                    var imagesBase64 = new List<string>();

                    // Process each image path to convert into base64 string
                    foreach (var imagePath in productImagePaths)
                    {
                        try
                        {
                            var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                                var base64String = Convert.ToBase64String(imageBytes);
                                imagesBase64.Add(base64String);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Optionally handle the error, e.g., by logging it. Skipping the image in this case.
                        }
                    }

                    // Add the fully populated product DTO to the list
                    productsDTO.Add(new GetAllPaginationUser
                    {
                        id = product.Id,
                        NameEn = product.NameEn,
                        NameAr = product.NameAr,
                        Price = product.Price,
                        DescriptionEn = product.DescriptionEn,
                        DescriptionAr = product.DescriptionAR,
                        BrandNameAr = product.BrandNameAr,
                        BrandNameEn = product.BrandNameEn,
                        StockQuantity = product.StockQuantity,
                        MaxPrice = product.MaxPrice,
                        MinPrice = product.MinPrice,
                        itemscolor = new List<string> { color }, // Set the fetched color
                        ProductImages = imagesBase64 // Assign the list of base64 images
                    });
                }

                return Ok(productsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredProducts([FromQuery] ProductFilterCriteria filters, int pageSize, int pageNumber)
        {
            try
            {
                var filteredProducts = await _productService.GetFilteredProducts(filters, pageSize, pageNumber);

                if (filteredProducts == null || !filteredProducts.Entities.Any())
                {
                    // Return an empty list or a not found response as per your API design decision
                    return Ok(new List<GetAllPaginationUser>()); // Assuming you want to return an empty list if no products are found
                }

                var basePath = configuration.GetValue<string>("MvcProject:WwwRootPath");
                var productsDTO = new List<GetAllPaginationUser>();

                foreach (var product in filteredProducts.Entities)
                {
                    var color = await _itemServices.GetColorByProductId(product.Id);
                    var productImagePaths = (await _productImageService.GetByProductIdAsync(product.Id)).Select(p => p.Path).ToList();
                    var imagesBase64 = new List<string>();

                    foreach (var imagePath in productImagePaths)
                    {
                        try
                        {
                            var fullPath = Path.Combine(basePath, imagePath.Replace("/", "\\").TrimStart('\\'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                                var base64String = Convert.ToBase64String(imageBytes);
                                imagesBase64.Add(base64String);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Optionally handle the error, e.g., by logging it. Skipping the image in this case.
                        }
                    }

                    productsDTO.Add(new GetAllPaginationUser
                    {
                        id = product.Id,
                        NameEn = product.NameEn,
                        NameAr = product.NameAr,
                        Price = product.Price,
                        DescriptionEn = product.DescriptionEn,
                        DescriptionAr = product.DescriptionAR,
                        BrandNameAr = product.BrandNameAr,
                        BrandNameEn = product.BrandNameEn,
                        StockQuantity = product.StockQuantity,
                        MaxPrice = product.MaxPrice,
                        MinPrice = product.MinPrice,
                        ProductImages = imagesBase64,
                        itemscolor = new List<string> { color } // Assuming color is a string. Adjust as necessary.
                    });
                }

                return Ok(productsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}



using Jumia.Application.Services;
using Jumia.Dtos.ViewModel.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jumia.Mvc.Controllers
{
    [Authorize]

    public class ItemController : Controller
    {
        private readonly IItemServices _itemServices;

        public ItemController(IItemServices itemServices) 
        { 
            _itemServices = itemServices;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _itemServices.GetAllPagination(10,1);
            return View(items);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(ItemViewModel itemView)
        {
            try
            {
                var ProductId = await _itemServices.GetProductID(itemView.ProductName);
                if (ProductId == 0)
                {
                    ModelState.AddModelError("ProductName", "The Product Name Not Correct");
                    return View(itemView);
                }
                else
                {
                    itemView.ProductId = ProductId;
                    string filename = "";
                    if (itemView.ItemImage != null)
                    {
                        string itemimages = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ItemImages");
                        filename = itemView.ItemImage.FileName;
                        string fullpath = Path.Combine(itemimages, filename);
                        itemView.ItemImage.CopyTo(new FileStream(fullpath, FileMode.Create));
                        itemView.ItemImagestring = filename;
                    }
                    var Result = await _itemServices.Create(itemView);
                    if (Result.Entity == null)
                    {
                        ViewBag.Error = Result.Message;
                        return View(itemView);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch
            {
                return View();
            }
        }
        public async Task<IActionResult> Update(int id)
        {
            var item = await _itemServices.GetOne(id);
            var productname = await _itemServices.GetProductName(item.Entity.ProductId);
            item.Entity.ProductName = productname;
            return View(item.Entity);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ItemViewModel item, int id)
        {
            try
            {
                var olditem = await _itemServices.GetProductID(item.ProductName);
                if (olditem != 0)
                {
                    string filename = "";
                    if (item.ItemImage != null)
                    {
                        //var olditem = await _itemServices.GetOne(id);
                        //var olditemimage = olditem.Entity.ItemImagestring;
                        string itemimages = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ItemImages");
                        filename = item.ItemImage.FileName;
                        string fullpath = Path.Combine(itemimages, filename);

                        //string oldfullpath = Path.Combine(itemimages, olditemimage);
                        //if(fullpath != oldfullpath)
                        //{
                        //System.IO.File.Delete(oldfullpath);
                        item.ItemImage.CopyTo(new FileStream(fullpath, FileMode.Create));
                        item.ItemImagestring = filename;
                        // }
                    }
                    var result = await _itemServices.Update(item);
                    if (result.Entity == null)
                    {
                        ViewBag.Error = result.Message;
                        return View(item);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("ProductName", "The Product Name is not correct");
                }
                return View(item);

            }
            catch
            {
                return View(item);
            }

        }
        public async Task<IActionResult> Delete(int id)
        {
            await _itemServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}

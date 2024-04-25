using AutoMapper;
using Jumia.Application.Contract;
using Jumia.Dtos.ResultView;
using Jumia.Dtos.ViewModel.Item;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public class ItemServices : IItemServices
    {
        private readonly IItemReposatory _IItemRepostoty;
        private readonly IMapper _mapper;

        public ItemServices(IItemReposatory IItemRepostoty, IMapper mapper)
        {
            _IItemRepostoty = IItemRepostoty;
            _mapper = mapper;
        }
        public async Task<ResultView<ItemViewModel>> Create(ItemViewModel newitem)
        {
            var itemExist = (await _IItemRepostoty.GetAllAsync()).Where(p => p.Id == newitem.Id).FirstOrDefault();
            if (itemExist != null)
            {
                return new ResultView<ItemViewModel>{ Entity = null , IsSuccess = false , Message =" The item is exist"};
            }
            else
            {

                var item = _mapper.Map<Item>(newitem);
                var itemcreated = await _IItemRepostoty.CreateAsync(item);
                await _IItemRepostoty.SaveChangesAsync();
                var itemView = _mapper.Map<ItemViewModel>(itemcreated);
                return new ResultView<ItemViewModel> { Entity = itemView, IsSuccess = true, Message = "The Item Created Successfully" };
            }
        }

        public async Task<ResultView<ItemViewModel>> Delete(int id)
        {
            try
            {
                var olditem = await _IItemRepostoty.GetByIdAsync(id);
                olditem.IsDeleted = true;
                await _IItemRepostoty.SaveChangesAsync();
                var olditemviewmodel = _mapper.Map<ItemViewModel>(olditem);
                return new ResultView<ItemViewModel> { Entity = olditemviewmodel, IsSuccess = true, Message = "Deleted Successfully" };
            }
            catch (Exception ex)
            {
                return new ResultView<ItemViewModel> { Entity = null , IsSuccess = false , Message = ex.Message };
            }
        }

        public async Task<ResultDataList<ItemViewModel>> GetAllPagination(int item, int pagenumber)
        {
            var AllItems = await _IItemRepostoty.GetAllAsync();
            var items = AllItems.Skip(item * (pagenumber - 1)).Take(item).Where(p => p.IsDeleted == false)
                                             .Select(p => new ItemViewModel()
                                             {
                                                 Color = p.Color,
                                                 Quantity = p.Quantity,
                                                 ProductName = p.Product.NameEn,
                                                 
                                                 ProductId = p.Product.Id,
                                                 Id = p.Id,
                                                 Size = p.Size,
                                                 IsDeleted = p.IsDeleted,
                                                 ItemImagestring = p.ItemImage
                                             }).ToList();
            var ResultData = new ResultDataList<ItemViewModel>();
            ResultData.Entities = items;
            ResultData.Count = items.Count();
            return ResultData;
        }

        public async Task<ResultView<ItemViewModel>> GetOne(int id)
        {
            var item = await _IItemRepostoty.GetByIdAsync(id);
            if(item == null)
            {
                return new ResultView<ItemViewModel> { Entity = null, IsSuccess = false, Message = " The item Not Found" };
            }
            else
            {
                var itemView = _mapper.Map<ItemViewModel>(item);
                return new ResultView<ItemViewModel> { Entity = itemView, IsSuccess = true, Message = " item Existed" };
            }
        }

        public async Task<ResultView<ItemViewModel>> Update(ItemViewModel item)
        {
            var itemModel = _mapper.Map<Item>(item);
            var newitem = await _IItemRepostoty.UpdateAsync(itemModel);
            await _IItemRepostoty.SaveChangesAsync();
            var itemUpdated = _mapper.Map<ItemViewModel>(newitem);
            return new ResultView<ItemViewModel> { Entity = itemUpdated, IsSuccess = true, Message = "Updated Successfully" };
        }
        public async Task<int> GetProductID (string ProductName)
        {
            int ProductID = await _IItemRepostoty.GetProductID(ProductName);
            return ProductID;
        }
        public async Task<string> GetProductName(int ID)
        {
            string ProductID = await _IItemRepostoty.GetProductName(ID);
            return ProductID;
        }

        public async Task<string> GetColorByProductId(int productId)
        {
            var color = await _IItemRepostoty.GetColorByProductIdAsync(productId);
            return color;
        }
    }
}

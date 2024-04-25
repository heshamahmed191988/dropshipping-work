using Jumia.Dtos.ResultView;
using Jumia.Dtos.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public interface IItemServices
    {
        Task<ResultView<ItemViewModel>> Create(ItemViewModel item);
        Task<ResultView<ItemViewModel>> Update(ItemViewModel item);
        Task<ResultView<ItemViewModel>> Delete(int id);
        Task<ResultView<ItemViewModel>> GetOne(int id);
        Task<ResultDataList<ItemViewModel>> GetAllPagination(int item , int pagenumber);
        Task<int> GetProductID(string ProductName);
        Task<string> GetProductName(int ID);
        Task<string> GetColorByProductId(int productId);
    }
}

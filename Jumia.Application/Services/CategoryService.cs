using AutoMapper;
using Jumia.Application.Contract;
using Jumia.Dtos.ResultView;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumia.Dtos.ViewModel.category;

namespace Jumia.Application.Services
{
    public class CategoryService:ICategoryService
    {
        private readonly ICategoryReposatory categoryRepository; // Ensure the repository interface name is correct
        private readonly IMapper _mapper;

        public CategoryService(ICategoryReposatory categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ResultView<CategoryDto>> Create(CategoryDto categoryDto)
        {
            var existingCategory = (await categoryRepository.GetAllAsync()).FirstOrDefault(c => c.NameEn == categoryDto.NameEn || c.NameAr == categoryDto.NameAr);
            if (existingCategory != null)
            {
                return new ResultView<CategoryDto> { Entity = null, IsSuccess = false, Message = "Already Exist" };
            }

            var category = _mapper.Map<Category>(categoryDto);
            var newCategory = await categoryRepository.CreateAsync(category);
            await categoryRepository.SaveChangesAsync();

            var categoryDtoResult = _mapper.Map<CategoryDto>(newCategory);
            return new ResultView<CategoryDto> { Entity = categoryDtoResult, IsSuccess = true, Message = "Created Successfully" };
        }



        public async Task<ResultView<CategoryDto>> SoftDelete(int categoryId)
        {
            var category = await categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new ResultView<CategoryDto> { Entity = null, IsSuccess = false, Message = "Category not found" };
            }

            category.IsDeleted = true;
            await categoryRepository.SaveChangesAsync();

            var categoryDtoResult = _mapper.Map<CategoryDto>(category);
            return new ResultView<CategoryDto> { Entity = categoryDtoResult, IsSuccess = true, Message = "Soft deleted successfully" };
        }

        public async Task<ResultDataList<CategoryDto>> GetallPigintaion(int iteam, int pageNumbers)
        {
            // Ensure pageNumbers is non-negative
            pageNumbers = Math.Max(pageNumbers, 1);

            // Calculate the skip count
            var skipCount = iteam * (pageNumbers - 1);

            var categories = (await categoryRepository.GetAllAsync())
                // Filter out categories where IsDeleted is either null or false
                .Where(p => p.IsDeleted == false)

                .Skip(skipCount).Take(iteam)
                .Select(p => new CategoryDto
                {
                    Id = p.Id,
                    NameAr = p.NameAr,
                    NameEn = p.NameEn,
                }).ToList();

            var resultDataList = new ResultDataList<CategoryDto>
            {
                Count = categories.Count,
                Entities = categories,


            };

            return resultDataList;
        }

        public async Task<CategoryDto> GetById(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            // Manually map  DTO
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,

            };

            return categoryDto;
        }
        public async Task<ResultView<CategoryDto>> Update(CategoryDto categoryDto)
        {
            try
            {
                var category = await categoryRepository.GetByIdAsync(categoryDto.Id);



                category.NameAr = categoryDto.NameAr;
                category.NameEn = categoryDto.NameEn;



                await categoryRepository.SaveChangesAsync();


                var updatedBookDto = _mapper.Map<CategoryDto>(category);
                return new ResultView<CategoryDto> { Entity = updatedBookDto, IsSuccess = true, Message = " updated successfully." };
            }
            catch (Exception ex)
            {
                return new ResultView<CategoryDto> { Entity = null, IsSuccess = false, Message = ex.Message };
            }
        }


    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.Business.Types;
using BilgeShop.Data.Entities;
using BilgeShop.Data.Repositories;

namespace BilgeShop.Business.Managers
{
    public class CategoryManager : ICategoryService
    {
        private readonly IRepository<CategoryEntity> _categoryRepository;
        public CategoryManager(IRepository<CategoryEntity> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public ServiceMessage AddCategory(CategoryDto categoryDto)
        {
            var hasCategory = _categoryRepository.GetAll(x => x.Name.ToLower() == categoryDto.Name.ToLower()&& x.IsDeleted == false).ToList();
            if(hasCategory.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu isimde bir kategori zaten mevcut"
                };
            }
            var categoryEntity = new CategoryEntity()
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
            };
            _categoryRepository.Add(categoryEntity);
            return new ServiceMessage
            {
                IsSucceed = false,
            };
        }

        public void DeleteCategory(int id)
        {
            _categoryRepository.Delete(id);
        }

        public List<CategoryDto> GetCategories()
        {
            var categoryEntities = _categoryRepository.GetAll().Where(x=>x.IsDeleted==false).OrderBy(x => x.Name);
            var categoryDtoList = categoryEntities.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            }).ToList();
            return categoryDtoList;
        }

        public CategoryDto GetCategoryById(int id)
        {
            var categoryEntity=_categoryRepository.GetById(id);
            var categoryDto = new CategoryDto()
            {
                Id = categoryEntity.Id,
                Name = categoryEntity.Name,
                Description = categoryEntity.Description,
            };
            return categoryDto;
        }

        public void UpdateCategory(CategoryDto categoryDto)
        {
            var categoryEntity = _categoryRepository.GetById(categoryDto.Id);
            categoryEntity.Name = categoryDto.Name;
            categoryEntity.Description = categoryDto.Description;
            _categoryRepository.Update(categoryEntity);
        }
    }
}

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
    public class ProductManager : IProductService
    {
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly ICategoryService _categoryService; 
		public ProductManager(IRepository<ProductEntity> productRepository, ICategoryService categoryService)
		{
			_productRepository = productRepository;
			_categoryService = categoryService;
		}

		public ServiceMessage AddProducut(ProductDto productDto)
        {
         var hasProduct=_productRepository.GetAll(x=>x.Name.ToLower()==productDto.Name.ToLower() && x.IsDeleted==false).ToList();
            if (hasProduct.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu isimde bir ürün zaten mevcut"
                };
            }

            var productEntity = new ProductEntity()
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                UnitInStock = productDto.UnitInStock,
                UnitPrice = productDto.UnitPrice,
                CategoryId = productDto.CategoryId,
                ImagePath = productDto.ImagePath,
            };
            _productRepository.Add(productEntity);
            return new ServiceMessage
            {
                IsSucceed = true,
            };

        }

		public void DeleteProduct(int id)
		{
			_productRepository.Delete(id);

		}

		public void EditProduct(ProductDto productDto)
        {
           var productEntity=_productRepository.GetById(productDto.Id);
            productEntity.Name = productDto.Name;
            productEntity.Description = productDto.Description;
            productEntity.UnitInStock = productDto.UnitInStock;
            productEntity.UnitPrice = productDto.UnitPrice;
            productEntity.CategoryId = productDto.CategoryId;
            productEntity.ImagePath = productDto.ImagePath;

            if (productDto.ImagePath != null)
            {
                productEntity.ImagePath = productDto.ImagePath;
            }
            _productRepository.Update(productEntity);
        }

        public ProductDto GetProductById(int id)
        {
            var productEntity=_productRepository.GetById(id);
            var productDto = new ProductDto()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                UnitInStock = productEntity.UnitInStock,
                UnitPrice = productEntity.UnitPrice,
                CategoryId = productEntity.CategoryId,
                ImagePath= productEntity.ImagePath,
            };
            return productDto;
        }

        public ProductDetailDto GetProductDetailById(int id)
        {
            var productEntity = _productRepository.GetById(id);
            var categoryEntity=_categoryService.GetCategoryById(productEntity.CategoryId);
            var productDetailDto = new ProductDetailDto()
            {
                Name = productEntity.Name,
                Description = productEntity.Description,
                UnitInStock = productEntity.UnitInStock,
                UnitPrice = productEntity.UnitPrice,
                ImagePath = productEntity.ImagePath,
                ModifiedDate=productEntity.ModifiedDate,
                CategoryName=categoryEntity.Name
            };
            return productDetailDto;
        }

        public List<ProductDto> GetProducts()
        {
            var productEntities=_productRepository.GetAll(x=>x.IsDeleted==false).OrderBy(x=>x.Category.Name).ThenBy(x=>x.Name);

            var productDtoList = productEntities.Select(x => new ProductDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                UnitInStock = x.UnitInStock,
                UnitPrice = x.UnitPrice,
                CategoryId = x.CategoryId,
                ImagePath = x.ImagePath,
                CategoryName = x.Category.Name
            }).ToList();

            return productDtoList;
        }

        public List<ProductDto> GetProductsByCategoryId(int? CategoryId = null)
        {
            if (CategoryId.HasValue)
            {
                var productEntities = _productRepository.GetAll(x => x.IsDeleted == false && x.CategoryId == CategoryId).OrderBy(x => x.Name);

                var productDtos = productEntities.Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitInStock = x.UnitInStock,
                    UnitPrice = x.UnitPrice,
                    CategoryId = x.CategoryId,
                    ImagePath = x.ImagePath,
                    CategoryName = x.Category.Name
                }).ToList();
                return productDtos;
            }
            else
            {
                return GetProducts();
            }
               

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BilgeShop.Data.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace BilgeShop.Data.Context
{
    public class BilgeShopContext:DbContext
    {
        private readonly IDataProtector _dataProtector;
        public BilgeShopContext(DbContextOptions<BilgeShopContext>options, IDataProtectionProvider dataProtectionProvider):base(options)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("security");
        }
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<ProductEntity> Products => Set<ProductEntity>();
        public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());

            var pwd = "123";
            pwd = _dataProtector.Protect(pwd);
            modelBuilder.Entity<UserEntity>().HasData(new List<UserEntity>() { 
            
                new UserEntity()
                {
                    Id=1,
                    FirstName="Bilge",
                    LastName="Admin",
                    Email="admin@bilgeshop.com",
                    Password=pwd,
                    UserType=Enums.UserTypeEnum.admin
                }
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}

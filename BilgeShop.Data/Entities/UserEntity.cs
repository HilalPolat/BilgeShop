﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BilgeShop.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BilgeShop.Data.Entities
{
    public class UserEntity:BaseEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserTypeEnum UserType { get; set; }        
    }

    public class UserEntityConfiguration : BaseConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.LastName)
             .IsRequired()
             .HasMaxLength(25);

            builder.Property(x => x.Email)
             .IsRequired()
             .HasMaxLength(50);

            builder.Property(x => x.Password)
             .IsRequired();
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.Business.Types;
using BilgeShop.Data.Entities;
using BilgeShop.Data.Enums;
using BilgeShop.Data.Repositories;
using Microsoft.AspNetCore.DataProtection;

namespace BilgeShop.Business.Managers
{
    public class UserManager : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IDataProtector _dataProtector;
        public UserManager(IRepository<UserEntity> userRepository, IDataProtectionProvider dataProtectionProvider)
        {
            _userRepository = userRepository;
            _dataProtector = dataProtectionProvider.CreateProtector("security");
        }

        public ServiceMessage AddUser(UserDto userDto)
        {
           var hasMail=_userRepository.GetAll(x=>x.Email.ToLower()==userDto.Email.ToLower()).ToList();
            if (hasMail.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu e-posta adresi zaten kayıtlıdır"
                };
            }

            userDto.Password=_dataProtector.Protect(userDto.Password);  

            var userEntity = new UserEntity()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Password = userDto.Password,
                UserType=UserTypeEnum.user
            };
            _userRepository.Add(userEntity);
            return new ServiceMessage
            {
                IsSucceed = true,
            };
        }

        public UserDto Login(LoginDto loginDto)
        {
            var user = _userRepository.Get(x => x.Email.ToLower() == loginDto.Email.ToLower());
            if(user is null)
            {
                return null;
            }
            var rawPassword = _dataProtector.Unprotect(user.Password);
            if (rawPassword != loginDto.Password)
            {
                return null;
            }
            else
            {
                return new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserType = user.UserType,
                };
            }
        }

        public void UpdateUser(UserProfileEditDto userProfileDto)
        {
            var entity=_userRepository.GetById(userProfileDto.Id);
            entity.FirstName = userProfileDto.FirstName;
            entity.LastName = userProfileDto.LastName;
            entity.Email = userProfileDto.Email;
            _userRepository.Update(entity);

        }
        
           
    }
}

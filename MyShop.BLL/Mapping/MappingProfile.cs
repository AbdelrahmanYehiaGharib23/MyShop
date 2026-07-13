using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using AutoMapper;
using MyShop.BLL.Models.Dto.CategoryDto;
using MyShop.BLL.Models.Dto.IdentityDto;
using MyShop.BLL.Models.Dto.ProductDto;
using MyShop.DAL.Entities;
using MyShop.DAL.Entities.IdentityEntity;


namespace MyShop.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Maping Product
            CreateMap<CreateProductDto, Product>();
            CreateMap<Product, ProductDto>();
            CreateMap<Product, ProductDetailsDto>();
            CreateMap<UpdateProductDto, Product>();
            #endregion

            #region Maping Category
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryDetailsDto>();
            CreateMap<UpdateCategoryDto, Category>();
            #endregion
            #region Mapping Identity
            //CreateMap<RegisterDto, ApplicationUser>()
            //  .ForMember(dest => dest.UserName,
            //    opt => opt.MapFrom(src => src.Email));

            //CreateMap<ApplicationUser, UserViewModel>();

            //CreateMap<LoginDto, ApplicationUser>();
            #endregion
        }
    }
}

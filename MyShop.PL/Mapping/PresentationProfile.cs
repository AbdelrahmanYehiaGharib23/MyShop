using AutoMapper;
using MyShop.BLL.Models.Dto.CategoryDto;
using MyShop.BLL.Models.Dto.IdentityDto;
using MyShop.BLL.Models.Dto.ProductDto;
using MyShop.PL.ViewModels;
using MyShop.PL.ViewModels.Identity;

namespace MyShop.PL.Mapping
{
    public class PresentationProfile:Profile
    {
        public PresentationProfile()
        {
            #region Mapping Product
            CreateMap<ProductVM, CreateProductDto>();
            CreateMap<ProductDetailsDto, ProductVM>();
            CreateMap<ProductVM, UpdateProductDto>();
            #endregion

            #region Mapping Category
            CreateMap<CategoryVM, CreateCategoryDto>();
            CreateMap<CategoryDetailsDto, CategoryVM>();
            CreateMap<CategoryVM, UpdateCategoryDto>();
            #endregion

            #region Mapping Identity
            CreateMap<LoginVM, LoginDto>().ReverseMap();

            CreateMap<RegisterVM, RegisterDto>().ReverseMap();
            #endregion
        }
    }
}

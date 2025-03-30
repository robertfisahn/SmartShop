using AutoMapper;
using SmartShopAPI.Entities;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.CartItem;
using SmartShopAPI.Models.Dtos.Category;
using SmartShopAPI.Models.Dtos.Order;
using SmartShopAPI.Models.Dtos.Product;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI
{
    public class SmartShopMappingProfile : Profile
    {
        public SmartShopMappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryUpsertDto, Category>();
            CreateMap<CategoryUpsertDto, Category>();
            CreateMap<UpsertProductDto, Product>()
                .ForMember(dest => dest.ImagePath, opt => opt.Condition(src => src.ImagePath != null));
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<User, UserDto>()
                .ForMember(r=>r.RoleName, d=>d.MapFrom(u=>u.Role.Name))
                .ForMember(r => r.City, d=> d.MapFrom(u=> u.Address.City))
                .ForMember(r => r.Street, d => d.MapFrom(u => u.Address.Street))
                .ForMember(r => r.PostalCode, d => d.MapFrom(u => u.Address.PostalCode));
            CreateMap<RegisterUserDto, User>()
                .ForPath(u => u.Address.City, d => d.MapFrom(r => r.City))
                .ForPath(u => u.Address.Street, d => d.MapFrom(r => r.Street))
                .ForPath(u => u.Address.PostalCode, d => d.MapFrom(r => r.PostalCode));
            CreateMap<CreateCartItemDto, CartItem>();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));
            CreateMap<CartItem, OrderItem>()
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                    src.Address != null ? $"{src.Address.Street}, {src.Address.City}, {src.Address.PostalCode}" : "No Address"))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price));
        }
    }
}

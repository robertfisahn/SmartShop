using AutoMapper;

using SmartShopAPI.Entities;
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
            //category
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryUpsertDto, Category>();

            //product
            CreateMap<UpsertProductDto, Product>()
                .ForMember(dest => dest.ImagePath, opt => opt.Condition(src => src.ImagePath != null));
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            //user
            CreateMap<User, UserDto>()
                .ForMember(r => r.RoleName, d => d.MapFrom(u => u.Role.Name))
                .ForMember(r => r.City, d => d.MapFrom(u => u.Addresses.Where(a => a.IsDefault).Select(a => a.City).FirstOrDefault()))
                .ForMember(r => r.Street, d => d.MapFrom(u => u.Addresses.Where(a => a.IsDefault).Select(a => a.Street).FirstOrDefault()))
                .ForMember(r => r.PostalCode, d => d.MapFrom(u => u.Addresses.Where(a => a.IsDefault).Select(a => a.PostalCode).FirstOrDefault()));
            CreateMap<RegisterUserDto, User>()
                .AfterMap((src, dest) =>
                {
                    dest.Addresses.Add(new Address
                    {
                        City = src.City,
                        Street = src.Street,
                        PostalCode = src.PostalCode,
                        IsDefault = true
                    });
                });
            CreateMap<User, ShippingAddressDto>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
                .ForMember(d => d.Street, o => o.MapFrom(s => s.Addresses.Where(a => a.IsDefault).Select(a => a.Street).FirstOrDefault()))
                .ForMember(d => d.City, o => o.MapFrom(s => s.Addresses.Where(a => a.IsDefault).Select(a => a.City).FirstOrDefault()))
                .ForMember(d => d.PostalCode, o => o.MapFrom(s => s.Addresses.Where(a => a.IsDefault).Select(a => a.PostalCode).FirstOrDefault()));

            //cartitem
            CreateMap<CreateCartItemDto, CartItem>();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));
            CreateMap<CartItemDto, OrderItem>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId))
                .ForMember(d => d.Product, o => o.Ignore())
                .ForMember(d => d.OrderId, o => o.Ignore());

            //order
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.ShippingCity))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.ShippingStreet))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.ShippingPostalCode))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            //orderitem
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price));
        }
    }
}

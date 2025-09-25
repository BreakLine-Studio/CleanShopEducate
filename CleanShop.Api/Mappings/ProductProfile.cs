using System;
using AutoMapper;
using CleanShop.Api.DTOs.Products;
using CleanShop.Domain.Entities;
using CleanShop.Domain.ValueObjects;

namespace CleanShop.Api.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Sku, m => m.MapFrom(s => s.Sku.Value));

        CreateMap<CreateProductDto, Product>()
            .ConstructUsing(src => new Product(
                src.Name,
                Sku.Create(src.Sku),
                src.Price,
                src.Stock
            ));


        CreateMap<UpdateProductDto, Product>()
            .ConstructUsing(src => new Product(
                src.Name,
                Sku.Create(src.Sku),
                src.Price,
                src.Stock
            ));
    }
}

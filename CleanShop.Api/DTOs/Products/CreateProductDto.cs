namespace CleanShop.Api.DTOs.Products;

public record CreateProductDto(string Name,string Sku ,decimal Price ,int Stock );

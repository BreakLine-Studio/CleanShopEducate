namespace CleanShop.Api.DTOs.Products;

public record ProductDto(Guid Id,string Name,string Sku,decimal Price,int Stock,DateTime CreatedAt);


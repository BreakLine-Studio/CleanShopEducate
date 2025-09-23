using MediatR;

namespace CleanShop.Application.Products;

public sealed record CreateProduct(string Name, string Sku, decimal Price, int Stock) : IRequest<Guid>;

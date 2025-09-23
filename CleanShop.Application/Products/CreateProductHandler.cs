using System;
using CleanShop.Application.Abstractions;
using CleanShop.Domain.Entities;
using CleanShop.Domain.ValueObjects;
using MediatR;

namespace CleanShop.Application.Products;

public sealed class CreateProductHandler(IProductRepository repo) : IRequestHandler<CreateProduct, Guid>
{
    public async Task<Guid> Handle(CreateProduct req, CancellationToken ct)
    {
        var sku = Sku.Create(req.Sku);
        if (await repo.ExistsSkuAsync(sku, ct)) throw new InvalidOperationException("SKU duplicado");

        var product = new Product(req.Name, sku, req.Price, req.Stock);
        await repo.AddAsync(product, ct);
        return product.Id;
    }
}

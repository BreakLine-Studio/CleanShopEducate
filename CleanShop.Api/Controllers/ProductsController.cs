using System;
using AutoMapper;
using CleanShop.Api.DTOs.Products;
using CleanShop.Application.Abstractions;
using CleanShop.Domain.Entities;
using CleanShop.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CleanShop.Api.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IProductRepository _repo;
    private readonly IMapper _mapper;
    public ProductsController(IProductRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken ct)
    {
        var products = await _repo.GetAllAsync(ct); // necesitas este método en IProductRepository
        var dto = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(id, ct);
        if (product is null) return NotFound();

        return Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto body, CancellationToken ct)
    {
        // Regla de unicidad por SKU
        var sku = Sku.Create(body.Sku);
        if (await _repo.ExistsSkuAsync(sku, ct))
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "SKU ya existe");

        var product = _mapper.Map<Product>(body);
        await _repo.AddAsync(product, ct);

        var dto = _mapper.Map<ProductDto>(product);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}

using hw_21._11._24.Data;
using hw_21._11._24.DTOs;
using hw_21._11._24.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace hw_21._11._24.Endpoints
{
    public static class ProductsEndpoints
    {
        public static void AddProductsEndpoints(this IEndpointRouteBuilder builder)
        {
            var productsEndpoins = builder.MapGroup("products");

            productsEndpoins.MapGet("/", async (DataContext context, [FromQuery] int skip = 0, [FromQuery] int take = 10, CancellationToken cancellationToken = default) =>
            {
                try
                {
                    return Results.Ok(await context.Products
                        .Skip(skip)
                        .Take(take)
                        .ToArrayAsync(cancellationToken));
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            productsEndpoins.MapGet("/{id:int}", async (DataContext context, [FromRoute] int id, CancellationToken cancellationToken = default) =>
            {
                var product = await context.Products.FindAsync(keyValues: [id], cancellationToken: cancellationToken);

                if (product is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(product);
            });

            productsEndpoins.MapPost("/", async (DataContext context, [FromBody] ProductCreateDTO productCreateDTO, CancellationToken cancellationToken = default) =>
            {
                var createdProduct = await context.AddAsync(new Product()
                {
                    Name = productCreateDTO.Name,
                    Description = productCreateDTO.Description,
                    Price = productCreateDTO.Price,
                    StockQuantity = productCreateDTO.StockQuantity,
                }, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                return Results.Created($"products/{createdProduct.Entity.Id}", createdProduct.Entity);
            });

            productsEndpoins.MapPut("/{id:int}", async (DataContext context, [FromBody] ProductUpdateDTO productUpdateDTO, [FromRoute] int id, CancellationToken cancellationToken = default) =>
            {
                var productFromDb = await context.Products.FindAsync(keyValues: [id], cancellationToken: cancellationToken);

                if (productFromDb is null)
                {
                    return Results.NotFound();
                }

                productFromDb.Name = productUpdateDTO.Name ?? productFromDb.Name;
                productFromDb.Description = productUpdateDTO.Description ?? productFromDb.Description;
                productFromDb.Price = productUpdateDTO.Price ?? productFromDb.Price;
                productFromDb.StockQuantity = productUpdateDTO.StockQuantity ?? productFromDb.StockQuantity;

                await context.SaveChangesAsync(cancellationToken);

                return Results.Ok(productFromDb);
            });

            productsEndpoins.MapDelete("/{id:int}", async (DataContext context, [FromRoute] int id, CancellationToken cancellationToken = default) =>
            {
                var productFromDb = await context.Products.FindAsync(keyValues: [id], cancellationToken: cancellationToken);

                if (productFromDb is not null)
                {
                    context.Products.Remove(productFromDb);

                    await context.SaveChangesAsync(cancellationToken);
                }
                
                return Results.NoContent();
            });
        }
    }
}

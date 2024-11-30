using hw_21._11._24.Data;
using hw_21._11._24.DTOs;
using hw_21._11._24.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hw_21._11._24.Endpoints
{
    public static class ClientsEndpoints
    {
        public static void AddClientsEndpoints(this IEndpointRouteBuilder builder)
        {
            var clientsEndpoints = builder.MapGroup("clients");

            clientsEndpoints.MapGet("/", async (DataContext context, [FromQuery] int skip = 0, [FromQuery] int take = 10, CancellationToken cancellationToken = default) =>
            {
                try
                {
                    return Results.Ok(await context.Clients
                        .Include(c => c.Products)
                        .Skip(skip)
                        .Take(take)
                        .ToArrayAsync(cancellationToken));
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            clientsEndpoints.MapGet("/{id:int}", async (DataContext context, [FromRoute] int id, CancellationToken cancellationToken = default) =>
            {
                var client = await context.Clients
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (client is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(client);
            });

            clientsEndpoints.MapPost("/", async (DataContext context, [FromBody] ClientCreateDTO clientCreateDTO, CancellationToken cancellationToken = default) =>
            {
                var createdClient = await context.AddAsync(new Client()
                {
                    FirstName = clientCreateDTO.FirstName,
                    LastName = clientCreateDTO.LastName,
                    Email = clientCreateDTO.Email,
                    DateOfBirth = clientCreateDTO.DateOfBirth,
                }, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                return Results.Created($"clients/{createdClient.Entity.Id}", createdClient.Entity);
            });

            clientsEndpoints.MapPost("/addProduct", async (DataContext context, [FromBody] AddProductDTO addProductDTO, CancellationToken cancellationToken = default) =>
            {
                var client = await context.Clients
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == addProductDTO.ClientId, cancellationToken);

                var product = await context.Products.FindAsync(keyValues: [addProductDTO.ProductId], cancellationToken: cancellationToken);

                if (client is null || product is null)
                {
                    return Results.NotFound();
                }

                client.Products.Add(product);

                await context.SaveChangesAsync(cancellationToken);

                return Results.NoContent();
            });

            clientsEndpoints.MapPut("/{id:int}", async (DataContext context, [FromBody] ClientUpdateDTO clientUpdateDTO, [FromRoute] int id, CancellationToken cancellationToken = default) =>
            {
                var clientFromDb = await context.Clients.FindAsync(keyValues: [id], cancellationToken: cancellationToken);

                if (clientFromDb is null)
                {
                    return Results.NotFound();
                }

                clientFromDb.FirstName = clientUpdateDTO.FirstName ?? clientFromDb.FirstName;
                clientFromDb.LastName = clientUpdateDTO.LastName ?? clientFromDb.LastName;
                clientFromDb.Email = clientUpdateDTO.Email ?? clientFromDb.Email;
                clientFromDb.DateOfBirth = clientUpdateDTO.DateOfBirth ?? clientFromDb.DateOfBirth;

                await context.SaveChangesAsync(cancellationToken);

                return Results.Ok(clientFromDb);
            });

            clientsEndpoints.MapDelete("/{id:int}", async (DataContext context, [FromRoute] int id, CancellationToken cancellationToken = default) =>
            {
                var clientFromDb = await context.Clients.FindAsync(keyValues: [id], cancellationToken: cancellationToken);

                if (clientFromDb is not null)
                {
                    context.Clients.Remove(clientFromDb);

                    await context.SaveChangesAsync(cancellationToken);
                }

                return Results.NoContent();
            });
        }
    }
}

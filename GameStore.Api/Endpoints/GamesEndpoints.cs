using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetNameEndpointName = "GetName";

    private static readonly List<GameSummaryDto> games = [
        new GameSummaryDto(1, "The Witcher 3", "RPG", 49.99m, new DateOnly(2015, 5, 19)),
        new GameSummaryDto(2, "Stardew Valley", "Simulation", 14.99m, new DateOnly(2016, 2, 26)),
        new GameSummaryDto(3, "Celeste", "Platformer", 19.99m, new DateOnly(2018, 1, 25))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        // Use this to avoid repeating games in the routes
        var group = app.MapGroup("games").
                        WithParameterValidation();

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Games
                     .Include(game => game.Genre)
                     .Select(game => game.ToGameSummaryDto())
                     .AsNoTracking() // As no tracking is a performance optimization
                     .ToListAsync()); // Improve performance by doing it async 

        // GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ?
                Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetNameEndpointName);

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();            

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(
                GetNameEndpointName,
                new { id = game.Id },
                game.ToGameDetailsDto());
        });

        // PUT /games/1
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                     .CurrentValues
                     .SetValues(updatedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                  .Where(game => game.Id == id)
                  .ExecuteDeleteAsync(); // batch delete
            
            return Results.NoContent();
        });

        return group;
    }
}

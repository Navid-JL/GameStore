using System.Text.RegularExpressions;
using GameStore.Server.Data;
using GameStore.Server.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(
    options =>
        options.AddDefaultPolicy(builder =>
        {
            builder.WithOrigins("https://localhost:7224").AllowAnyHeader().AllowAnyMethod();
        })
);

// var connString = builder.Configuration.GetConnectionString("GameStoreContext");
var connString = builder.Configuration["ConnectionStrings:GameStoreContext"];

builder.Services.AddSqlServer<GameStoreContext>(connString);

var app = builder.Build();

app.UseCors();

var group = app.MapGroup("/games").WithParameterValidation();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// GET /games
group.MapGet(
    "/",
    async (string? filter, GameStoreContext context) =>
    {
        var games = context.Games.AsNoTracking();

        if (filter is not null)
        {
            games = games.Where(game => game.Name.Contains(filter) || game.Genre.Contains(filter));
        }

        return await games.ToListAsync();
    }
);

// GET/games/{id}
group
    .MapGet(
        "/{id}",
        async (int id, GameStoreContext context) =>
        {
            Game? game = await context.Games.FindAsync(id);
            if (game is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(game);
        }
    )
    .WithName("GetGame");

// POST /games
group.MapPost(
    "/",
    async (Game game, GameStoreContext context) =>
    {
        context.Games.Add(game);
        await context.SaveChangesAsync();

        return Results.CreatedAtRoute("GetGame", new { id = game.Id }, game);
    }
);

// PUT /games/{id}
group.MapPut(
    "/{id}",
    async (int id, Game updatedGame, GameStoreContext context) =>
    {
        var rowsAffected = await context.Games
            .Where(game => game.Id == id)
            .ExecuteUpdateAsync(
                updates =>
                    updates
                        .SetProperty(game => game.Name, updatedGame.Name)
                        .SetProperty(game => game.Genre, updatedGame.Genre)
                        .SetProperty(game => game.Price, updatedGame.Price)
                        .SetProperty(game => game.ReleaseDate, updatedGame.ReleaseDate)
            );

        return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
    }
);

group.MapDelete(
    "/{id}",
    async (int id, GameStoreContext context) =>
    {
        var rowsAffected = await context.Games.Where(game => game.Id == id).ExecuteDeleteAsync();

        return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
    }
);

app.Run();

using System.Text.RegularExpressions;
using GameStore.Server.Models;

List<Game> games = new List<Game>
{
    new Game()
    {
        Id = 1,
        Name = "Battlefield 1",
        Genre = "Fighting",
        Price = 59.99M,
        ReleaseDate = new DateTime(2015, 9, 14)
    },
    new Game()
    {
        Id = 2,
        Name = "FIFA 16",
        Genre = "Sports",
        Price = 30.99M,
        ReleaseDate = new DateTime(2015, 10, 31)
    },
    new Game()
    {
        Id = 3,
        Name = "Red Dead Redemption",
        Genre = "Roleplaying",
        Price = 59.99M,
        ReleaseDate = new DateTime(2019, 3, 10)
    }
};

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
group.MapGet("/", () => games);

// GET/games/{id}
group
    .MapGet(
        "/{id}",
        (int id) =>
        {
            Game? game = games.Find((game) => game.Id == id);
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
    (Game game) =>
    {
        game.Id = games.Max(game => game.Id) + 1;
        games.Add(game);

        return Results.CreatedAtRoute("GetGame", new { id = game.Id }, game);
    }
);

// PUT /games/{id}
group.MapPut(
    "/{id}",
    (int id, Game updatedGame) =>
    {
        Game? existingGame = games.Find((game) => game.Id == id);

        if (existingGame is null)
        {
            updatedGame.Id = id;
            games.Add(updatedGame);

            return Results.CreatedAtRoute("GetGame", new { id = updatedGame.Id }, updatedGame);
        }
        existingGame.Name = updatedGame.Name;
        existingGame.Genre = updatedGame.Genre;
        existingGame.Price = updatedGame.Price;
        existingGame.ReleaseDate = updatedGame.ReleaseDate;

        return Results.NoContent();
    }
);

group.MapDelete(
    "/{id}",
    (int id) =>
    {
        Game? existingGame = games.Find((game) => game.Id == id);
        if (existingGame is null)
        {
            return Results.NotFound();
        }
        games.Remove(existingGame);
        return Results.NoContent();
    }
);

app.Run();

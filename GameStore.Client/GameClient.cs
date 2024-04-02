using GameStore.Client.Models;

namespace GameStore.Client;

public static class GameClient
{
    private static readonly List<Game> games = new List<Game> {
        new Game() {
        Id= 1,
        Name = "Battlefield 1",
        Genre = "Shooter",
        Price = 59.99M,
        ReleaseDate = new DateTime(2015, 9, 14)
        },
        new Game() {
        Id= 2,
        Name = "FIFA 16",
        Genre = "Sports",
        Price = 30.99M,
        ReleaseDate = new DateTime(2015, 10, 31)
        },
        new Game() {
        Id= 3,
        Name = "Red Dead Redemption",
        Genre = "Story",
        Price = 59.99M,
        ReleaseDate = new DateTime(2019, 3, 10)
        }
    };

    public static Game[] GetGames()
    {
        return games.ToArray();
    }
}

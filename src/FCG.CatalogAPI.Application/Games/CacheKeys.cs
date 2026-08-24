namespace FCG.CatalogAPI.Application.Games;

public static class CacheKeys
{
    public const string AllGames = "catalog:games:all";

    public static string Game(Guid id) => $"catalog:games:{id}";
}

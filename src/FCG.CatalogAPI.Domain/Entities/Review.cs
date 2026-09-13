namespace FCG.CatalogAPI.Domain.Entities;

public class Review
{
    private const int MaxTags = 5;
    private const int MaxCommentLength = 1000;

    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public Guid UserId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public IReadOnlyList<string> Tags { get; private set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; private set; }

    private Review() { }

    public static Review Create(Guid gameId, Guid userId, int rating, string? comment, IEnumerable<string>? tags = null)
    {
        if (gameId == Guid.Empty)
            throw new ArgumentException("Jogo é obrigatório.");
        if (userId == Guid.Empty)
            throw new ArgumentException("Usuário é obrigatório.");
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Nota deve ser entre 1 e 5.");

        var text = (comment ?? string.Empty).Trim();
        if (text.Length > MaxCommentLength)
            throw new ArgumentException($"Comentário deve ter no máximo {MaxCommentLength} caracteres.");

        var normalizedTags = (tags ?? Array.Empty<string>())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .Distinct()
            .ToArray();

        if (normalizedTags.Length > MaxTags)
            throw new ArgumentException($"Informe no máximo {MaxTags} tags.");

        return new Review
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            UserId = userId,
            Rating = rating,
            Comment = text,
            Tags = normalizedTags,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Reconstrói uma avaliação já persistida. Uso exclusivo da camada de infraestrutura.
    /// </summary>
    public static Review Restore(
        Guid id, Guid gameId, Guid userId, int rating,
        string comment, IReadOnlyList<string> tags, DateTime createdAt)
        => new()
        {
            Id = id,
            GameId = gameId,
            UserId = userId,
            Rating = rating,
            Comment = comment,
            Tags = tags,
            CreatedAt = createdAt
        };
}

namespace CivicAction.Domain.Participation;

/// <summary>
/// The identity of a participation opportunity at its source: a source key plus the source's own ID.
/// It is the basis of idempotent re-imports. Retrieval metadata such as hashes and fetch times is not part of it.
/// </summary>
public sealed record ExternalIdentity
{
    public const int SourceKeyMaxLength = 64;
    public const int ExternalIdMaxLength = 128;

    private ExternalIdentity(string sourceKey, string externalId)
    {
        SourceKey = sourceKey;
        ExternalId = externalId;
    }

    /// <summary>Lowercase letters, digits, and hyphens only.</summary>
    public string SourceKey { get; }

    /// <summary>The source's ID, trimmed. It is compared ordinally.</summary>
    public string ExternalId { get; }

    public static ExternalIdentity Create(string? sourceKey, string? externalId)
    {
        ArgumentNullException.ThrowIfNull(sourceKey);

        if (sourceKey.Length == 0 || sourceKey.Length > SourceKeyMaxLength || !sourceKey.All(IsSourceKeyCharacter))
        {
            throw new ArgumentException(
                $"A source key must have 1 to {SourceKeyMaxLength} characters from a-z, 0-9, and '-'.",
                nameof(sourceKey));
        }

        return new ExternalIdentity(sourceKey, Text.Required(externalId, nameof(externalId), ExternalIdMaxLength));
    }

    private static bool IsSourceKeyCharacter(char c) => c is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '-';
}

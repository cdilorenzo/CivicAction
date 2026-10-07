namespace CivicAction.Domain.Participation;

/// <summary>
/// The external address at which a person takes part. It must be an absolute <c>https</c> URI without user information.
/// Which hosts a source may point to is decided by the source connector, not here.
/// </summary>
public sealed record OfficialParticipationRoute
{
    public const int MaxLength = 2048;

    private OfficialParticipationRoute(Uri value) => Value = value;

    public Uri Value { get; }

    public static OfficialParticipationRoute Create(string? value)
    {
        var trimmed = Text.Required(value, nameof(value), MaxLength);
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("A participation route must be an absolute URI.", nameof(value));
        }

        return Create(uri);
    }

    public static OfficialParticipationRoute Create(Uri? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!value.IsAbsoluteUri)
        {
            throw new ArgumentException("A participation route must be an absolute URI.", nameof(value));
        }

        if (value.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException("A participation route must use https.", nameof(value));
        }

        if (value.UserInfo.Length > 0)
        {
            throw new ArgumentException("A participation route must not contain user information.", nameof(value));
        }

        if (value.Host.Length == 0)
        {
            throw new ArgumentException("A participation route must have a host.", nameof(value));
        }

        if (value.OriginalString.Length > MaxLength)
        {
            throw new ArgumentException($"A participation route must not exceed {MaxLength} characters.", nameof(value));
        }

        return new OfficialParticipationRoute(value);
    }
}

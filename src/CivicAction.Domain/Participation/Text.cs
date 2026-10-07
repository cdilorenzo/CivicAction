namespace CivicAction.Domain.Participation;

internal static class Text
{
    /// <summary>Trims the value and rejects it when it is null, empty, or longer than the limit.</summary>
    public static string Required(string? value, string paramName, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(value, paramName);

        var trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            throw new ArgumentException($"{paramName} must not be empty or whitespace.", paramName);
        }

        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"{paramName} must not exceed {maxLength} characters.", paramName);
        }

        return trimmed;
    }

    /// <summary>Returns null for an absent value; a present value follows the rules of <see cref="Required"/>.</summary>
    public static string? Optional(string? value, string paramName, int maxLength) =>
        value is null ? null : Required(value, paramName, maxLength);
}

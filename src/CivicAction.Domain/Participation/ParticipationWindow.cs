namespace CivicAction.Domain.Participation;

/// <summary>
/// The period in which participation is possible, as the half-open interval <c>[Start, End)</c>.
/// Either bound may be missing, but not both. Bounds keep the offset the source delivered.
/// </summary>
public sealed record ParticipationWindow
{
    private ParticipationWindow(DateTimeOffset? start, DateTimeOffset? end)
    {
        Start = start;
        End = end;
    }

    public DateTimeOffset? Start { get; }

    /// <summary>The first instant at which participation is no longer possible.</summary>
    public DateTimeOffset? End { get; }

    public static ParticipationWindow Create(DateTimeOffset? start, DateTimeOffset? end)
    {
        if (start is null && end is null)
        {
            throw new ArgumentException("A participation window needs a start, an end, or both.");
        }

        if (start > end)
        {
            throw new ArgumentException("The end of a participation window must not be before its start.", nameof(end));
        }

        return new ParticipationWindow(start, end);
    }

    /// <summary>Whether participation is possible at the given instant. The caller supplies the time.</summary>
    public bool IsOpenAt(DateTimeOffset instant) =>
        (Start is null || instant >= Start) && (End is null || instant < End);
}

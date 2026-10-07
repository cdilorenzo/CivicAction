namespace CivicAction.Domain.Participation;

/// <summary>Internal identity of a <see cref="CivicAction"/>. It is assigned by the caller and never reused.</summary>
public sealed record CivicActionId
{
    private CivicActionId(Guid value) => Value = value;

    public Guid Value { get; }

    public static CivicActionId Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A CivicActionId must not be an empty GUID.", nameof(value));
        }

        return new CivicActionId(value);
    }
}

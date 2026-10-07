namespace CivicAction.Domain.Participation;

/// <summary>
/// A globally discoverable civic opportunity derived from an external source (ADR-0001).
/// An instance cannot be created in an invalid state and has no mutating members: a re-import builds a new instance
/// with the same <see cref="Id"/> and <see cref="ExternalIdentity"/>. Ranking, explanations, status, and
/// person-related data are not part of it.
/// </summary>
public sealed class CivicAction
{
    public const int TitleMaxLength = 200;
    public const int SummaryMaxLength = 500;
    public const int ResponsibleOrganizationMaxLength = 200;

    private CivicAction(
        CivicActionId id,
        ExternalIdentity externalIdentity,
        string title,
        OfficialParticipationRoute officialParticipationRoute,
        ParticipationWindow participationWindow,
        ParticipationType participationType,
        string? summary,
        Locality? locality,
        string? responsibleOrganization)
    {
        Id = id;
        ExternalIdentity = externalIdentity;
        Title = title;
        OfficialParticipationRoute = officialParticipationRoute;
        ParticipationWindow = participationWindow;
        ParticipationType = participationType;
        Summary = summary;
        Locality = locality;
        ResponsibleOrganization = responsibleOrganization;
    }

    public CivicActionId Id { get; }

    public ExternalIdentity ExternalIdentity { get; }

    /// <summary>Plain text, trimmed, 1 to <see cref="TitleMaxLength"/> characters.</summary>
    public string Title { get; }

    public OfficialParticipationRoute OfficialParticipationRoute { get; }

    public ParticipationWindow ParticipationWindow { get; }

    public ParticipationType ParticipationType { get; }

    /// <summary>Plain text, trimmed, at most <see cref="SummaryMaxLength"/> characters, or null.</summary>
    public string? Summary { get; }

    public Locality? Locality { get; }

    /// <summary>The organization's name, trimmed, at most <see cref="ResponsibleOrganizationMaxLength"/> characters, or null.</summary>
    public string? ResponsibleOrganization { get; }

    public static CivicAction Create(
        CivicActionId id,
        ExternalIdentity externalIdentity,
        string? title,
        OfficialParticipationRoute officialParticipationRoute,
        ParticipationWindow participationWindow,
        ParticipationType participationType,
        string? summary = null,
        Locality? locality = null,
        string? responsibleOrganization = null)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(externalIdentity);
        ArgumentNullException.ThrowIfNull(officialParticipationRoute);
        ArgumentNullException.ThrowIfNull(participationWindow);

        if (!Enum.IsDefined(participationType))
        {
            throw new ArgumentException("Unknown participation type.", nameof(participationType));
        }

        if (locality is { } place && !Enum.IsDefined(place))
        {
            throw new ArgumentException("Unknown locality.", nameof(locality));
        }

        return new CivicAction(
            id,
            externalIdentity,
            Text.Required(title, nameof(title), TitleMaxLength),
            officialParticipationRoute,
            participationWindow,
            participationType,
            Text.Optional(summary, nameof(summary), SummaryMaxLength),
            locality,
            Text.Optional(responsibleOrganization, nameof(responsibleOrganization), ResponsibleOrganizationMaxLength));
    }
}

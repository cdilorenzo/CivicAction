namespace CivicAction.Domain.Participation;

/// <summary>A source-neutral classification of how a person can take part.</summary>
public enum ParticipationType
{
    /// <summary>The source does not tell. This is the value for every meinBerlin record until that is clarified.</summary>
    Unknown = 0,
    IdeaCollection = 1,
    Survey = 2,
    TextDiscussion = 3,
    Event = 4,
    DevelopmentPlanConsultation = 5,
}

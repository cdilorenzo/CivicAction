using CivicAction.Domain.Participation;
using CivicActionAggregate = CivicAction.Domain.Participation.CivicAction;

namespace CivicAction.Domain.Tests;

public class CivicActionTests
{
    private static readonly CivicActionId Id = CivicActionId.Create(Guid.Parse("8c7f2d5e-1b34-4d6a-9f0e-3a2b1c4d5e6f"));
    private static readonly ExternalIdentity Identity = ExternalIdentity.Create("test-source", "1234");
    private static readonly OfficialParticipationRoute Route = OfficialParticipationRoute.Create("https://example.org/projects/1234/");

    private static readonly ParticipationWindow Window = ParticipationWindow.Create(
        null,
        new DateTimeOffset(2026, 12, 31, 23, 59, 0, TimeSpan.FromHours(1)));

    private static CivicActionAggregate Create(
        CivicActionId? id = null,
        ExternalIdentity? identity = null,
        string? title = "Neugestaltung eines Spielplatzes",
        OfficialParticipationRoute? route = null,
        ParticipationWindow? window = null,
        ParticipationType type = ParticipationType.Unknown,
        string? summary = null,
        Locality? locality = null,
        string? organization = null) =>
        CivicActionAggregate.Create(
            id ?? Id,
            identity ?? Identity,
            title,
            route ?? Route,
            window ?? Window,
            type,
            summary,
            locality,
            organization);

    [Fact]
    public void CreateWithRequiredValuesOnlyLeavesOptionalValuesMissing()
    {
        var action = Create();

        Assert.Equal(Id, action.Id);
        Assert.Equal(Identity, action.ExternalIdentity);
        Assert.Equal("Neugestaltung eines Spielplatzes", action.Title);
        Assert.Equal(Route, action.OfficialParticipationRoute);
        Assert.Equal(Window, action.ParticipationWindow);
        Assert.Equal(ParticipationType.Unknown, action.ParticipationType);
        Assert.Null(action.Summary);
        Assert.Null(action.Locality);
        Assert.Null(action.ResponsibleOrganization);
    }

    [Fact]
    public void CreateWithAllValuesKeepsThem()
    {
        var action = Create(
            type: ParticipationType.Survey,
            summary: "Ideen für die Umgestaltung sammeln.",
            locality: Locality.Mitte,
            organization: "Bezirksamt Mitte");

        Assert.Equal(ParticipationType.Survey, action.ParticipationType);
        Assert.Equal("Ideen für die Umgestaltung sammeln.", action.Summary);
        Assert.Equal(Locality.Mitte, action.Locality);
        Assert.Equal("Bezirksamt Mitte", action.ResponsibleOrganization);
    }

    [Fact]
    public void CreateAcceptsUnknownParticipationType() =>
        Assert.Equal(ParticipationType.Unknown, Create(type: ParticipationType.Unknown).ParticipationType);

    [Fact]
    public void CreateAcceptsCityWideLocality() =>
        Assert.Equal(Locality.CityWide, Create(locality: Locality.CityWide).Locality);

    [Fact]
    public void CreateAcceptsAWindowWithoutStartAndAWindowWithoutEnd()
    {
        Assert.NotNull(Create(window: ParticipationWindow.Create(null, DateTimeOffset.UnixEpoch)));
        Assert.NotNull(Create(window: ParticipationWindow.Create(DateTimeOffset.UnixEpoch, null)));
    }

    [Fact]
    public void CreateRejectsMissingId() =>
        Assert.Throws<ArgumentNullException>(() => CivicActionAggregate.Create(
            null!,
            Identity,
            "Title",
            Route,
            Window,
            ParticipationType.Unknown));

    [Fact]
    public void CreateRejectsMissingExternalIdentity() =>
        Assert.Throws<ArgumentNullException>(() => CivicActionAggregate.Create(
            Id,
            null!,
            "Title",
            Route,
            Window,
            ParticipationType.Unknown));

    [Fact]
    public void CreateRejectsMissingRoute() =>
        Assert.Throws<ArgumentNullException>(() => CivicActionAggregate.Create(
            Id,
            Identity,
            "Title",
            null!,
            Window,
            ParticipationType.Unknown));

    [Fact]
    public void CreateRejectsMissingWindow() =>
        Assert.Throws<ArgumentNullException>(() => CivicActionAggregate.Create(
            Id,
            Identity,
            "Title",
            Route,
            null!,
            ParticipationType.Unknown));

    [Fact]
    public void CreateRejectsUndefinedParticipationType() =>
        Assert.Throws<ArgumentException>(() => Create(type: (ParticipationType)999));

    [Fact]
    public void CreateRejectsUndefinedLocality() =>
        Assert.Throws<ArgumentException>(() => Create(locality: (Locality)0));

    [Fact]
    public void CreateRejectsMissingTitle() =>
        Assert.Throws<ArgumentNullException>(() => Create(title: null));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\r\n")]
    public void CreateRejectsEmptyTitle(string title) =>
        Assert.Throws<ArgumentException>(() => Create(title: title));

    [Fact]
    public void CreateTrimsTitle() =>
        Assert.Equal("Titel", Create(title: "  Titel\t").Title);

    [Fact]
    public void CreateAcceptsTitleAtLimit() =>
        Assert.Equal(CivicActionAggregate.TitleMaxLength, Create(title: new string('t', CivicActionAggregate.TitleMaxLength)).Title.Length);

    [Fact]
    public void CreateAcceptsTitleAtLimitWhenPaddedWithWhitespace() =>
        Assert.Equal(
            CivicActionAggregate.TitleMaxLength,
            Create(title: " " + new string('t', CivicActionAggregate.TitleMaxLength) + " ").Title.Length);

    [Fact]
    public void CreateRejectsTitleOverLimit() =>
        Assert.Throws<ArgumentException>(() => Create(title: new string('t', CivicActionAggregate.TitleMaxLength + 1)));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRejectsPresentButEmptySummary(string summary) =>
        Assert.Throws<ArgumentException>(() => Create(summary: summary));

    [Fact]
    public void CreateTrimsSummary() =>
        Assert.Equal("Kurztext", Create(summary: " Kurztext ").Summary);

    [Fact]
    public void CreateAcceptsSummaryAtLimit() =>
        Assert.Equal(CivicActionAggregate.SummaryMaxLength, Create(summary: new string('s', CivicActionAggregate.SummaryMaxLength)).Summary!.Length);

    [Fact]
    public void CreateRejectsSummaryOverLimit() =>
        Assert.Throws<ArgumentException>(() => Create(summary: new string('s', CivicActionAggregate.SummaryMaxLength + 1)));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRejectsPresentButEmptyOrganization(string organization) =>
        Assert.Throws<ArgumentException>(() => Create(organization: organization));

    [Fact]
    public void CreateTrimsOrganization() =>
        Assert.Equal("Bezirksamt Mitte", Create(organization: " Bezirksamt Mitte ").ResponsibleOrganization);

    [Fact]
    public void CreateAcceptsOrganizationAtLimit() =>
        Assert.Equal(
            CivicActionAggregate.ResponsibleOrganizationMaxLength,
            Create(organization: new string('o', CivicActionAggregate.ResponsibleOrganizationMaxLength)).ResponsibleOrganization!.Length);

    [Fact]
    public void CreateRejectsOrganizationOverLimit() =>
        Assert.Throws<ArgumentException>(
            () => Create(organization: new string('o', CivicActionAggregate.ResponsibleOrganizationMaxLength + 1)));

    [Fact]
    public void AggregateExposesOnlyTheApprovedMembers()
    {
        var properties = typeof(CivicActionAggregate)
            .GetProperties()
            .Select(property => property.Name)
            .Order(StringComparer.Ordinal);

        string[] expected =
        [
            nameof(CivicActionAggregate.ExternalIdentity),
            nameof(CivicActionAggregate.Id),
            nameof(CivicActionAggregate.Locality),
            nameof(CivicActionAggregate.OfficialParticipationRoute),
            nameof(CivicActionAggregate.ParticipationType),
            nameof(CivicActionAggregate.ParticipationWindow),
            nameof(CivicActionAggregate.ResponsibleOrganization),
            nameof(CivicActionAggregate.Summary),
            nameof(CivicActionAggregate.Title),
        ];

        Assert.Equal(expected, properties);
    }

    [Fact]
    public void AggregateHasNoPublicSetters() =>
        Assert.All(
            typeof(CivicActionAggregate).GetProperties(),
            property => Assert.Null(property.SetMethod));

    [Fact]
    public void DomainReferencesOnlyTheFrameworkLibraries()
    {
        var outsideFramework = typeof(CivicActionAggregate).Assembly
            .GetReferencedAssemblies()
            .Select(assembly => assembly.Name!)
            .Where(name => name != "System" && !name.StartsWith("System.", StringComparison.Ordinal));

        Assert.Empty(outsideFramework);
    }
}

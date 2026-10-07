using CivicAction.Domain.Participation;

namespace CivicAction.Domain.Tests;

public class OfficialParticipationRouteTests
{
    [Fact]
    public void CreateAcceptsAbsoluteHttpsUri()
    {
        var route = OfficialParticipationRoute.Create("https://example.org/participate/1?a=b#c");

        Assert.Equal(new Uri("https://example.org/participate/1?a=b#c"), route.Value);
    }

    [Fact]
    public void CreateTrimsSurroundingWhitespace() =>
        Assert.Equal(new Uri("https://example.org/"), OfficialParticipationRoute.Create("  https://example.org/ ").Value);

    [Fact]
    public void CreateAcceptsHttpsUriObject() =>
        Assert.Equal(new Uri("https://example.org/"), OfficialParticipationRoute.Create(new Uri("https://example.org/")).Value);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("/projects/1")]
    [InlineData("projects/1")]
    [InlineData("//example.org/projects/1")]
    [InlineData("not a uri")]
    [InlineData("http://example.org/")]
    [InlineData("ftp://example.org/")]
    [InlineData("mailto:someone@example.org")]
    [InlineData("javascript:alert(1)")]
    [InlineData("file:///c:/data")]
    [InlineData("https://user@example.org/")]
    [InlineData("https://user:secret@example.org/")]
    public void CreateRejectsInvalidRoute(string value) =>
        Assert.Throws<ArgumentException>(() => OfficialParticipationRoute.Create(value));

    [Fact]
    public void CreateRejectsMissingString() =>
        Assert.Throws<ArgumentNullException>(() => OfficialParticipationRoute.Create((string?)null));

    [Fact]
    public void CreateRejectsMissingUri() =>
        Assert.Throws<ArgumentNullException>(() => OfficialParticipationRoute.Create((Uri?)null));

    [Fact]
    public void CreateRejectsRelativeUriObject() =>
        Assert.Throws<ArgumentException>(() => OfficialParticipationRoute.Create(new Uri("/projects/1", UriKind.Relative)));

    [Fact]
    public void CreateRejectsHttpUriObject() =>
        Assert.Throws<ArgumentException>(() => OfficialParticipationRoute.Create(new Uri("http://example.org/")));

    [Fact]
    public void CreateAcceptsRouteAtLengthLimit()
    {
        var prefix = "https://example.org/";
        var value = prefix + new string('a', OfficialParticipationRoute.MaxLength - prefix.Length);

        Assert.Equal(OfficialParticipationRoute.MaxLength, value.Length);
        Assert.NotNull(OfficialParticipationRoute.Create(value));
    }

    [Fact]
    public void CreateRejectsRouteOverLengthLimit()
    {
        var prefix = "https://example.org/";
        var value = prefix + new string('a', OfficialParticipationRoute.MaxLength - prefix.Length + 1);

        Assert.Throws<ArgumentException>(() => OfficialParticipationRoute.Create(value));
    }

    [Fact]
    public void RoutesAreEqualByValue() =>
        Assert.Equal(
            OfficialParticipationRoute.Create("https://example.org/a"),
            OfficialParticipationRoute.Create("https://EXAMPLE.org/a"));
}

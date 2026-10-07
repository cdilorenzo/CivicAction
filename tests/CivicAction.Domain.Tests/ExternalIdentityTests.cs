using CivicAction.Domain.Participation;

namespace CivicAction.Domain.Tests;

public class ExternalIdentityTests
{
    [Fact]
    public void CreateAcceptsValidSourceKeyAndExternalId()
    {
        var identity = ExternalIdentity.Create("source-1", "1234");

        Assert.Equal("source-1", identity.SourceKey);
        Assert.Equal("1234", identity.ExternalId);
    }

    [Fact]
    public void CreateTrimsExternalId() =>
        Assert.Equal("1234", ExternalIdentity.Create("source", "  1234 \t").ExternalId);

    [Theory]
    [InlineData("")]
    [InlineData("Source")]
    [InlineData("source key")]
    [InlineData(" source")]
    [InlineData("source_key")]
    [InlineData("quelle-ä")]
    public void CreateRejectsInvalidSourceKey(string sourceKey) =>
        Assert.Throws<ArgumentException>(() => ExternalIdentity.Create(sourceKey, "1"));

    [Fact]
    public void CreateRejectsMissingSourceKey() =>
        Assert.Throws<ArgumentNullException>(() => ExternalIdentity.Create(null, "1"));

    [Fact]
    public void CreateAcceptsSourceKeyAtLimit() =>
        Assert.Equal(
            ExternalIdentity.SourceKeyMaxLength,
            ExternalIdentity.Create(new string('a', ExternalIdentity.SourceKeyMaxLength), "1").SourceKey.Length);

    [Fact]
    public void CreateRejectsSourceKeyOverLimit() =>
        Assert.Throws<ArgumentException>(
            () => ExternalIdentity.Create(new string('a', ExternalIdentity.SourceKeyMaxLength + 1), "1"));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRejectsEmptyExternalId(string externalId) =>
        Assert.Throws<ArgumentException>(() => ExternalIdentity.Create("source", externalId));

    [Fact]
    public void CreateRejectsMissingExternalId() =>
        Assert.Throws<ArgumentNullException>(() => ExternalIdentity.Create("source", null));

    [Fact]
    public void CreateAcceptsExternalIdAtLimit() =>
        Assert.Equal(
            ExternalIdentity.ExternalIdMaxLength,
            ExternalIdentity.Create("source", new string('1', ExternalIdentity.ExternalIdMaxLength)).ExternalId.Length);

    [Fact]
    public void CreateRejectsExternalIdOverLimit() =>
        Assert.Throws<ArgumentException>(
            () => ExternalIdentity.Create("source", new string('1', ExternalIdentity.ExternalIdMaxLength + 1)));

    [Fact]
    public void IdentitiesAreEqualByValue()
    {
        Assert.Equal(ExternalIdentity.Create("source", "1"), ExternalIdentity.Create("source", " 1 "));
        Assert.NotEqual(ExternalIdentity.Create("source", "1"), ExternalIdentity.Create("other", "1"));
        Assert.NotEqual(ExternalIdentity.Create("source", "1"), ExternalIdentity.Create("source", "2"));
    }

    [Fact]
    public void IdentitiesCompareExternalIdOrdinally() =>
        Assert.NotEqual(ExternalIdentity.Create("source", "abc"), ExternalIdentity.Create("source", "ABC"));
}

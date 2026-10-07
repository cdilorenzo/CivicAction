using CivicAction.Domain.Participation;

namespace CivicAction.Domain.Tests;

public class CivicActionIdTests
{
    [Fact]
    public void CreateAcceptsNonEmptyGuid()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(guid, CivicActionId.Create(guid).Value);
    }

    [Fact]
    public void CreateRejectsEmptyGuid() =>
        Assert.Throws<ArgumentException>(() => CivicActionId.Create(Guid.Empty));

    [Fact]
    public void IdsWithSameGuidAreEqual()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(CivicActionId.Create(guid), CivicActionId.Create(guid));
        Assert.NotEqual(CivicActionId.Create(guid), CivicActionId.Create(Guid.NewGuid()));
    }
}

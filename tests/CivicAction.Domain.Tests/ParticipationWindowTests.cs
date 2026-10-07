using CivicAction.Domain.Participation;

namespace CivicAction.Domain.Tests;

public class ParticipationWindowTests
{
    private static readonly DateTimeOffset Start = new(2026, 11, 2, 10, 0, 0, TimeSpan.FromHours(1));
    private static readonly DateTimeOffset End = new(2026, 12, 31, 23, 59, 0, TimeSpan.FromHours(1));

    [Fact]
    public void CreateAcceptsStartAndEnd()
    {
        var window = ParticipationWindow.Create(Start, End);

        Assert.Equal(Start, window.Start);
        Assert.Equal(End, window.End);
    }

    [Fact]
    public void CreateAcceptsMissingStartForRunningProjects()
    {
        var window = ParticipationWindow.Create(null, End);

        Assert.Null(window.Start);
        Assert.Equal(End, window.End);
    }

    [Fact]
    public void CreateAcceptsMissingEndForUpcomingProjects()
    {
        var window = ParticipationWindow.Create(Start, null);

        Assert.Equal(Start, window.Start);
        Assert.Null(window.End);
    }

    [Fact]
    public void CreateRejectsWindowWithoutAnyBound() =>
        Assert.Throws<ArgumentException>(() => ParticipationWindow.Create(null, null));

    [Fact]
    public void CreateRejectsEndBeforeStart() =>
        Assert.Throws<ArgumentException>(() => ParticipationWindow.Create(End, Start));

    [Fact]
    public void CreateRejectsEndOneTickBeforeStart() =>
        Assert.Throws<ArgumentException>(() => ParticipationWindow.Create(Start, Start.AddTicks(-1)));

    [Fact]
    public void CreateAcceptsStartEqualToEnd()
    {
        var window = ParticipationWindow.Create(Start, Start);

        Assert.Equal(window.Start, window.End);
    }

    [Fact]
    public void CreateComparesInstantsAcrossOffsets()
    {
        var berlinSummer = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.FromHours(2));
        var sameInstantUtc = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);

        Assert.NotNull(ParticipationWindow.Create(berlinSummer, sameInstantUtc));
        Assert.Throws<ArgumentException>(() => ParticipationWindow.Create(berlinSummer, sameInstantUtc.AddTicks(-1)));
    }

    [Fact]
    public void CreateKeepsTheOffsetOfTheSource()
    {
        var window = ParticipationWindow.Create(Start, End);

        Assert.Equal(TimeSpan.FromHours(1), window.Start!.Value.Offset);
        Assert.Equal(TimeSpan.FromHours(1), window.End!.Value.Offset);
    }

    [Fact]
    public void IsOpenAtIsClosedBeforeStart() =>
        Assert.False(ParticipationWindow.Create(Start, End).IsOpenAt(Start.AddTicks(-1)));

    [Fact]
    public void IsOpenAtIsOpenAtStart() =>
        Assert.True(ParticipationWindow.Create(Start, End).IsOpenAt(Start));

    [Fact]
    public void IsOpenAtIsOpenJustBeforeEnd() =>
        Assert.True(ParticipationWindow.Create(Start, End).IsOpenAt(End.AddTicks(-1)));

    [Fact]
    public void IsOpenAtIsClosedAtEnd() =>
        Assert.False(ParticipationWindow.Create(Start, End).IsOpenAt(End));

    [Fact]
    public void IsOpenAtIsClosedAfterEnd() =>
        Assert.False(ParticipationWindow.Create(Start, End).IsOpenAt(End.AddDays(1)));

    [Fact]
    public void IsOpenAtWithoutStartIsOpenUntilEnd()
    {
        var window = ParticipationWindow.Create(null, End);

        Assert.True(window.IsOpenAt(DateTimeOffset.MinValue));
        Assert.True(window.IsOpenAt(End.AddTicks(-1)));
        Assert.False(window.IsOpenAt(End));
    }

    [Fact]
    public void IsOpenAtWithoutEndIsOpenFromStart()
    {
        var window = ParticipationWindow.Create(Start, null);

        Assert.False(window.IsOpenAt(Start.AddTicks(-1)));
        Assert.True(window.IsOpenAt(Start));
        Assert.True(window.IsOpenAt(DateTimeOffset.MaxValue));
    }

    [Fact]
    public void IsOpenAtWithStartEqualToEndIsNeverOpen()
    {
        var window = ParticipationWindow.Create(Start, Start);

        Assert.False(window.IsOpenAt(Start.AddTicks(-1)));
        Assert.False(window.IsOpenAt(Start));
        Assert.False(window.IsOpenAt(Start.AddTicks(1)));
    }

    [Fact]
    public void IsOpenAtComparesInstantsAcrossOffsets()
    {
        var window = ParticipationWindow.Create(Start, End);
        var startInUtc = Start.ToUniversalTime();

        Assert.True(window.IsOpenAt(startInUtc));
        Assert.False(window.IsOpenAt(startInUtc.AddTicks(-1)));
    }
}

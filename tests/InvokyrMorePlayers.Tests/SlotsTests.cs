using InvokyrMorePlayers;
using Xunit;

public class SlotsTests
{
    [Fact]
    public void MissingIndices() =>
        Assert.Equal(new[] { 1, 6, 7 }, Slots.MissingIndices(new[] { 0, 2, 3, 4, 5 }, 8));

    [Theory]
    [InlineData(3, 6, 3)]
    [InlineData(7, 6, 1)]
    [InlineData(-1, 6, -1)]
    [InlineData(5, 0, 5)]
    public void Wrap(int index, int count, int expected) => Assert.Equal(expected, Slots.Wrap(index, count));

    [Fact]
    public void LeastUsed()
    {
        Assert.Equal(2, Slots.LeastUsed(new[] { 0, 1, 3, 4, 5, 6, 7 }, 8));
        Assert.Equal(1, Slots.LeastUsed(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 0, -1 }, 8));
    }

    [Theory]
    [InlineData("playerCount", 6, 12)]
    [InlineData("PlayerCount", 5, 5)]
    [InlineData("version", 6, 6)]
    [InlineData(null, 6, 6)]
    public void FilterValue(string key, int value, int expected) => Assert.Equal(expected, Slots.FilterValue(key, value, 12));
}

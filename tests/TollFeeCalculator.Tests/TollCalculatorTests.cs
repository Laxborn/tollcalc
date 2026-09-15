namespace TollFeeCalculator.Tests;

public class TollCalculatorTests
{
    private readonly TollCalculator calculator = new();
    private readonly Car car = new();

    [Theory]
    [InlineData(6, 0, 8)]
    [InlineData(6, 29, 8)]
    [InlineData(6, 30, 13)]
    [InlineData(6, 59, 13)]
    [InlineData(7, 0, 18)]
    [InlineData(7, 59, 18)]
    [InlineData(8, 0, 13)]
    [InlineData(8, 29, 13)]
    [InlineData(8, 30, 8)]
    [InlineData(14, 59, 8)]
    [InlineData(15, 0, 13)]
    [InlineData(15, 29, 13)]
    [InlineData(15, 30, 18)]
    [InlineData(16, 59, 18)]
    [InlineData(17, 0, 13)]
    [InlineData(17, 59, 13)]
    [InlineData(18, 0, 8)]
    [InlineData(18, 29, 8)]
    [InlineData(18, 30, 0)]
    [InlineData(23, 0, 0)]
    public void A_car_is_charged_the_fee_for_the_time_of_day(
        int hour,
        int minute,
        int expectedFee)
    {
        var date = new DateTime(2026, 9, 14, hour, minute, 0);

        var fee = calculator.GetTollFee(date, car);

        Assert.Equal(expectedFee, fee);
    }

    [Fact]
    public void A_weekend_passage_is_free()
    {
        var saturday = new DateTime(2026, 9, 19, 7, 0, 0);

        var fee = calculator.GetTollFee(saturday, car);

        Assert.Equal(0, fee);
    }

    [Fact]
    public void A_passage_in_july_is_free()
    {
        var date = new DateTime(2026, 7, 14, 7, 0, 0);

        var fee = calculator.GetTollFee(date, car);

        Assert.Equal(0, fee);
    }

    [Theory]
    [InlineData(2013, 1, 1)]
    [InlineData(2013, 3, 28)]
    [InlineData(2013, 12, 24)]
    public void A_public_holiday_is_free(int year, int month, int day)
    {
        var date = new DateTime(year, month, day, 7, 0, 0);

        var fee = calculator.GetTollFee(date, car);

        Assert.Equal(0, fee);
    }

    [Fact]
    public void A_motorbike_is_exempt()
    {
        var date = new DateTime(2026, 9, 14, 7, 0, 0);

        var fee = calculator.GetTollFee(date, new Motorbike());

        Assert.Equal(0, fee);
    }

    [Fact]
    public void The_highest_fee_is_used_for_passages_within_one_hour()
    {
        var dates = new[]
        {
            new DateTime(2026, 9, 14, 6, 30, 0),
            new DateTime(2026, 9, 14, 7, 0, 0)
        };

        var fee = calculator.GetTollFee(car, dates);

        Assert.Equal(18, fee);
    }

    [Fact]
    public void Passages_more_than_one_hour_apart_are_charged_separately()
    {
        var dates = new[]
        {
            new DateTime(2026, 9, 14, 6, 0, 0),
            new DateTime(2026, 9, 14, 7, 1, 0)
        };

        var fee = calculator.GetTollFee(car, dates);

        Assert.Equal(26, fee);
    }

    [Fact]
    public void The_daily_fee_is_never_more_than_sixty()
    {
        var dates = new[]
        {
            new DateTime(2026, 9, 14, 6, 0, 0),
            new DateTime(2026, 9, 14, 7, 1, 0),
            new DateTime(2026, 9, 14, 8, 2, 0),
            new DateTime(2026, 9, 14, 9, 3, 0),
            new DateTime(2026, 9, 14, 10, 4, 0),
            new DateTime(2026, 9, 14, 11, 5, 0)
        };

        var fee = calculator.GetTollFee(car, dates);

        Assert.Equal(60, fee);
    }
}

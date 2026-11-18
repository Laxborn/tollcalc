namespace TollFeeCalculator.Tests;

public class TollCalculatorTests
{
    private readonly TollCalculator calculator = new();
    private readonly Car car = new();

    // goes through the whole fee schedule on a normal monday - checks both edges of every
    // interval so an off-by-one minute anywhere gets caught
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(5, 59, 0)]
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

    // saturday at rush hour should still be free
    [Fact]
    public void A_weekend_passage_is_free()
    {
        var saturday = new DateTime(2026, 9, 19, 7, 0, 0);

        var fee = calculator.GetTollFee(saturday, car);

        Assert.Equal(0, fee);
    }

    // same thing but sunday
    [Fact]
    public void A_sunday_passage_is_free()
    {
        var sunday = new DateTime(2026, 9, 20, 7, 0, 0);

        var fee = calculator.GetTollFee(sunday, car);

        Assert.Equal(0, fee);
    }

    // a regular weekday in july at rush hour - whole month is free so this should be 0
    [Fact]
    public void A_passage_in_july_is_free()
    {
        var date = new DateTime(2026, 7, 14, 7, 0, 0);

        var fee = calculator.GetTollFee(date, car);

        Assert.Equal(0, fee);
    }

    // a few dates from the original 2013 holiday list, to make sure we still cover those.
    // 28 march is maundy thursday - free since it's the day before good friday
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

    // fixed-date holidays across different years - the old code only knew about 2013
    [Theory]
    [InlineData(2019, 1, 1)]
    [InlineData(2025, 1, 1)]
    [InlineData(2026, 1, 1)]
    [InlineData(2027, 1, 1)]
    [InlineData(2029, 1, 1)]
    [InlineData(2026, 1, 6)] // Epiphany
    [InlineData(2026, 5, 1)] // May Day
    [InlineData(2026, 6, 6)]
    [InlineData(2026, 12, 24)]
    [InlineData(2026, 12, 25)]
    [InlineData(2026, 12, 26)]
    [InlineData(2026, 12, 31)]
    public void A_fixed_public_holiday_is_free_regardless_of_year(int year, int month, int day)
    {
        var date = new DateTime(year, month, day, 7, 0, 0);

        var fee = calculator.GetTollFee(date, car);

        Assert.Equal(0, fee);
    }

    // holidays that move with easter - checks the easter calculation lands on the right days
    [Theory]
    [InlineData(2025, 4, 18)] // Good Friday (Easter Sunday is April 20, 2025)
    [InlineData(2025, 4, 21)] // Easter Monday
    [InlineData(2026, 4, 3)] // Good Friday (Easter Sunday is April 5, 2026)
    [InlineData(2026, 4, 6)] // Easter Monday
    [InlineData(2026, 5, 14)] // Ascension Day
    public void A_moveable_holiday_based_on_easter_is_free(int year, int month, int day)
    {
        var date = new DateTime(year, month, day, 7, 0, 0);

        var fee = calculator.GetTollFee(date, car);

        Assert.Equal(0, fee);
    }

    // normal weekdays that just happen to be the day before a holiday - also free per the rules
    [Theory]
    [InlineData(2026, 6, 5)] // Friday before National Day (June 6)
    [InlineData(2026, 12, 23)] // Wednesday before Christmas Eve
    [InlineData(2026, 12, 30)] // Wednesday before New Year's Eve
    [InlineData(2026, 5, 13)] // Wednesday before Ascension Day
    [InlineData(2026, 1, 5)] // Monday before Epiphany
    [InlineData(2026, 4, 30)] // Thursday before May Day
    [InlineData(2025, 6, 20)] // Midsummer Eve
    [InlineData(2026, 6, 19)] // Midsummer Eve
    [InlineData(2027, 6, 25)] // Midsummer Eve
    [InlineData(2025, 10, 31)] // Friday before All Saints' Day
    [InlineData(2026, 10, 30)] // Friday before All Saints' Day
    [InlineData(2027, 11, 5)] // Friday before All Saints' Day
    public void The_day_before_a_holiday_is_free(int year, int month, int day)
    {
        var date = new DateTime(year, month, day, 7, 0, 0);

        var fee = calculator.GetTollFee(date, car);

        Assert.Equal(0, fee);
    }

    // normal weekdays that could easily get mixed up with a holiday - should be charged as usual.
    // whit monday hasn't been a swedish holiday since 2005, and the friday after midsummer
    // makes sure we don't pick the wrong saturday
    [Theory]
    [InlineData(2025, 6, 9)] // Whit Monday
    [InlineData(2026, 5, 25)] // Whit Monday
    [InlineData(2026, 6, 26)] // Friday after Midsummer
    public void A_regular_weekday_close_to_a_holiday_is_charged(int year, int month, int day)
    {
        var date = new DateTime(year, month, day, 7, 0, 0);

        var fee = calculator.GetTollFee(date, car);

        Assert.Equal(18, fee);
    }

    // motorbike at rush hour on a normal weekday - should pay nothing
    [Fact]
    public void A_motorbike_is_exempt()
    {
        var date = new DateTime(2026, 9, 14, 7, 0, 0);

        var fee = calculator.GetTollFee(date, new Motorbike());

        Assert.Equal(0, fee);
    }

    // two passages 30 min apart (13 then 18) - only the higher one should count
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

    // three passages in one window with rising fees - the old loop charged 23 here instead of 18
    [Fact]
    public void Only_the_highest_fee_is_charged_for_three_rising_passages_within_one_hour()
    {
        var dates = new[]
        {
            new DateTime(2026, 9, 14, 6, 20, 0), // 8
            new DateTime(2026, 9, 14, 6, 40, 0), // 13
            new DateTime(2026, 9, 14, 7, 10, 0)  // 18
        };

        var fee = calculator.GetTollFee(car, dates);

        Assert.Equal(18, fee);
    }

    // same idea but the highest fee comes before the last passage - old loop gave 28 here
    [Fact]
    public void Only_the_highest_fee_is_charged_when_it_is_not_the_last_passage_within_one_hour()
    {
        var dates = new[]
        {
            new DateTime(2026, 9, 14, 6, 20, 0), // 8
            new DateTime(2026, 9, 14, 7, 10, 0), // 18
            new DateTime(2026, 9, 14, 7, 15, 0)  // 18
        };

        var fee = calculator.GetTollFee(car, dates);

        Assert.Equal(18, fee);
    }

    // 61 min apart so they land in separate windows - both get charged (8 + 18)
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

    // six separate windows adding up to 63 - should get capped at 60
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

    // A fake that always says "not toll-free" - used to isolate the interval/daily-cap
    // logic in GetTollFee from the real holiday rules, so a test can't accidentally
    // pass or fail because a chosen date happens to be a holiday.
    private class AlwaysChargeableHolidayProvider : IHolidayProvider
    {
        public bool IsTollFreeDate(DateTime date) => false;
    }

    // same cap test but with the fake calendar, so it only tests the cap and nothing else
    [Fact]
    public void The_daily_fee_is_never_more_than_sixty_regardless_of_holidays()
    {
        var calculator = new TollCalculator(new AlwaysChargeableHolidayProvider());
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

    // no dates at all (null) should blow up instead of returning 0
    [Fact]
    public void A_null_date_array_throws_an_exception()
    {
        Assert.Throws<ArgumentException>(() => calculator.GetTollFee(car, null!));
    }

    // same for an empty array
    [Fact]
    public void An_empty_date_array_throws_an_exception()
    {
        Assert.Throws<ArgumentException>(() => calculator.GetTollFee(car, Array.Empty<DateTime>()));
    }
}

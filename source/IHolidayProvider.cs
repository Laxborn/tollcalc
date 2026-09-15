namespace TollFeeCalculator
{
    public interface IHolidayProvider
    {
        bool IsTollFreeDate(DateTime date);
    }
}

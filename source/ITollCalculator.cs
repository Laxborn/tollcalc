namespace TollFeeCalculator
{
    public interface ITollCalculator
    {
        int GetTollFee(Vehicle vehicle, DateTime[] dates);
        int GetTollFee(DateTime date, Vehicle vehicle);
    }
}

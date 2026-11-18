namespace TollFeeCalculator
{
    public interface ITollCalculator
    {
        int GetTollFee(IVehicle vehicle, DateTime[] dates);
        int GetTollFee(DateTime date, IVehicle vehicle);
    }
}

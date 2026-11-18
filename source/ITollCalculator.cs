namespace TollFeeCalculator
{
    public interface ITollCalculator
    {
        int GetTollFee(IVehicle vehicle, DateTime[] dates);
        int GetTollFee(IVehicle vehicle, DateTime date);
    }
}

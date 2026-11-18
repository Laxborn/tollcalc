namespace TollFeeCalculator
{
    public interface IVehicle
    {
        String GetVehicleType();
        bool IsTollFree { get; }
    }
}

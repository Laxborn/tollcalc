namespace TollFeeCalculator
{
    public class Motorbike : IVehicle
    {
        public string GetVehicleType()
        {
            return "Motorbike";
        }

        public bool IsTollFree => true;
    }
}

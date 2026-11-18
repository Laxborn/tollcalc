namespace TollFeeCalculator
{
    public class Motorbike : Vehicle
    {
        public string GetVehicleType()
        {
            return "Motorbike";
        }

        public bool IsTollFree => true;
    }
}

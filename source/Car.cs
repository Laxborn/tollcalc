namespace TollFeeCalculator
{
    public class Car : IVehicle
    {
        public String GetVehicleType()
        {
            return "Car";
        }

        public bool IsTollFree => false;
    }
}

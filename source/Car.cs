namespace TollFeeCalculator
{
    public class Car : Vehicle
    {
        public String GetVehicleType()
        {
            return "Car";
        }

        public bool IsTollFree => false;
    }
}

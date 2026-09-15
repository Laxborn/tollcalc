namespace TollFeeCalculator
{
    public class TollCalculator
    {
        /**
         * Calculate the total toll fee for one day
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes on one day
         * @return - the total toll fee for that day
         */

        public int GetTollFee(Vehicle vehicle, DateTime[] dates)
        {
            if (dates == null || dates.Length == 0)
                throw new ArgumentException("Dates cannot be null or empty", nameof(dates));

            DateTime intervalStart = dates[0];
            int totalFee = 0;
            foreach (DateTime date in dates)
            {
                int nextFee = GetTollFee(date, vehicle);
                int tempFee = GetTollFee(intervalStart, vehicle);

                long diffInTicks = date.Ticks - intervalStart.Ticks;
                long minutes = diffInTicks / TimeSpan.TicksPerMinute;

                if (minutes <= TollConfiguration.SameHourWindowMinutes)
                {
                    if (totalFee > 0) totalFee -= tempFee;
                    if (nextFee >= tempFee) tempFee = nextFee;
                    totalFee += tempFee;
                }
                else
                {
                    totalFee += nextFee;
                    intervalStart = date;
                }
            }
            if (totalFee > TollConfiguration.MaxDailyFee) totalFee = TollConfiguration.MaxDailyFee;
            return totalFee;
        }

        private bool IsTollFreeVehicle(Vehicle vehicle)
        {
            if (vehicle == null) return false;
            String vehicleType = vehicle.GetVehicleType();
            return vehicleType.Equals("Motorbike", StringComparison.OrdinalIgnoreCase);
        }

        public int GetTollFee(DateTime date, Vehicle vehicle)
        {
            if (HolidayProvider.IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

            return TollConfiguration.GetFeeByTime(date.Hour, date.Minute);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UrbanVehicleReservationConsole
{
    public class Reservation
    {
        public long ReservationID { get; private set; } = -1;
        private VehicleType vehicleType;
        public VehicleType VehicleType
        {
            get => vehicleType;
            set => vehicleType = value;
        }

        private DateTime acceptanceTime;
        public DateTime AcceptanceTime
        {
            get => acceptanceTime;

            set 
            {
                if (value <= new DateTime(2024, 12, 31, 23, 59, 59))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Invalid date and time format. Please enter a date of 2025");
                }
                acceptanceTime = value;
            }
        }

        private DateTime deliveryTime;
        public DateTime DeliveryTime
        {
            get => deliveryTime;
            set 
            {
                int minMinutes = 20;
                if (value <= this.acceptanceTime)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Acceptance time comes after Delivery time. Please enter a valid date and time.");
                }

                else if ((value - acceptanceTime).TotalMinutes < minMinutes)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"minimum booking time is {minMinutes}. Please enter a valid date and time.");
                }

                deliveryTime = value;
            }
        }

        private string customerName;
        public string CustomerName
        { 
            get => customerName;

            set
            {
                int nameLengthMin = 5;
                int nameLengthMax = 30;
                string nameRegex = @"^[A-Za-z ]+$";

                if (string.IsNullOrEmpty(value)
                || string.IsNullOrWhiteSpace(value)
                || value.Length < nameLengthMin
                || value.Length > nameLengthMax
                || !Regex.IsMatch(value, nameRegex))
                {
                    throw new ArgumentException(
                        $"Invalid customer name. Please enter a non-empty name with a length range between {nameLengthMin} - {nameLengthMax} characters.");
                }
                customerName = value;
            }
        }

        private string customerContact;
        public string CustomerContact
        {
            get => customerContact;

            set 
            {
                int contactLengthMin = 10;
                int contactLengthMax = 30;
                string emailReg = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                string telReg = @"^\+380\d{9}$";

                if (string.IsNullOrEmpty(value)
                || string.IsNullOrWhiteSpace(value)
                || value.Length < contactLengthMin
                || value.Length > contactLengthMax)
                {
                    throw new ArgumentException(
                        $"Invalid customer contact. Please enter a non-empty contact with a a length range between {contactLengthMin} - {contactLengthMax} characters.");
                }

                else if (!Regex.IsMatch(value, emailReg)
                    && !Regex.IsMatch(value, telReg))
                {
                    throw new ArgumentException($"Invalid customer contact. Input should be either tel. number with +380 or valid email address");
                }
                
                customerContact = value;
            }
        }

        public string CustomerInfo
        {
            get { return $"{customerName} ({customerContact})"; }
        }

        public decimal Price { get; private set; }
        public Reservation()
        {
            vehicleType = VehicleType.Car;
            acceptanceTime = DateTime.MinValue;
            deliveryTime = DateTime.MinValue;
            customerName = string.Empty;
            customerContact = string.Empty;
            Price = 0.0m;
        }

        public static bool TryParseVehicleType(string vehicleTypeInput, out VehicleType parsedType, out string errorString)
        {
            parsedType = default;
            if (!int.TryParse(vehicleTypeInput, out int typeNumber)
                || !Enum.IsDefined(typeof(VehicleType), typeNumber))
            {
                errorString = "Invalid vehicle type. Please select a number between 1 and 5.";
                return false;
            }

            parsedType = (VehicleType)typeNumber;
            errorString = string.Empty;
            return true;
        }

        public static bool IsValidDate(string dateTimeInput, out DateTime parsedDate, out string errorString)
        {
            parsedDate = DateTime.MinValue;
            if (!DateTime.TryParseExact(dateTimeInput, "dd.MM.yyyy HH:mm",
                null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                errorString = "Invalid date/time format or Acceptance time comes after Delivery time. Please enter a valid date and time.";
                return false;
            }

            errorString = string.Empty;
            return true;

        }
        public bool IsValidPrice(string priceInput, out string errorString)
        {
            if (!decimal.TryParse(priceInput, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedPrice) || parsedPrice < 0)
            {
                errorString = "Invalid price. Please enter a valid non-negative price.";
                return false;
            }
            errorString = string.Empty;
            this.Price = parsedPrice;
            return true;
        }

        public static bool IsValidIndexOrAcceptanceDateTime(string searchInput, out string errorString, out long reservationID, out DateTime acceptanceDateTime)
        {
            reservationID = long.MinValue;
            acceptanceDateTime = DateTime.MinValue;
            if (long.TryParse(searchInput, out long parsedID) && parsedID >= 0)
            {
                reservationID = parsedID;
                errorString = string.Empty;
                return true;
            }
            else if (DateTime.TryParseExact(searchInput, "dd.MM.yyyy HH:mm",
                null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                acceptanceDateTime = parsedDateTime;
                errorString = string.Empty;
                return true;
            }
            errorString = "Invalid input. Please enter a valid reservation ID or date and time in the format dd:MM:yyyy HH:mm.";
            return false;
        }

        public override string ToString()
        {
            return $"{ReservationID};{VehicleType.ToString()};{AcceptanceTime:dd.MM.yyyy HH:mm};{DeliveryTime:dd.MM.yyyy HH:mm};{Price};{CustomerName};{CustomerContact}";
        }

        public void setIndex (IEnumerable<Reservation> reservations)
        {
            ReservationID = reservations.Count() > 0 ? reservations.Max(res => res.ReservationID) + 1 : 0;
        }

        public void CalculatePrice()
        {
            decimal ratePerHour = vehicleType switch
            {
                VehicleType.Car => 10.0m,
                VehicleType.Motorcycle => 7.0m,
                VehicleType.Bike => 5.0m,
                VehicleType.Scooter => 6.0m,
                VehicleType.ElectricScooter => 8.0m,
                _ => 0.0m
            };
            TimeSpan duration = DeliveryTime - AcceptanceTime;
            decimal totalHours = (decimal)duration.TotalHours;
            int hoursToCharge = (int)Math.Ceiling(totalHours);
            Price = hoursToCharge * ratePerHour;
        }

        public static Reservation FromString(string line)
        {
            var parts = line.Split(';');
            return new Reservation
            {
                ReservationID = long.Parse(parts[0]),
                VehicleType = (VehicleType)Enum.Parse(typeof(VehicleType), parts[1]),
                AcceptanceTime = DateTime.ParseExact(parts[2], "dd.MM.yyyy HH:mm", null),
                DeliveryTime = DateTime.ParseExact(parts[3], "dd.MM.yyyy HH:mm", null),
                Price = decimal.Parse(parts[4]),
                CustomerName = parts[5],
                CustomerContact = parts[6]
            };
        }
    }
}

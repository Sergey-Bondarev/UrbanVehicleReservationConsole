using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanVehicleReservationConsole
{
    public class Reservation
    {
        public Guid reservationID = Guid.NewGuid();
        public VehicleType vehicleType;
        public DateTime acceptanceTime;
        public DateTime deliveryTime;
        public string customerName;
        public string customerContact;
        public decimal price;
        public Reservation()
        {
            vehicleType = VehicleType.Car;
            acceptanceTime = DateTime.MinValue;
            deliveryTime = DateTime.MinValue;
            customerName = string.Empty;
            customerContact = string.Empty;
            price = 0.0m;
        }
        public Reservation(VehicleType vehicleType, DateTime acceptanceTime, DateTime deliveryTime, string customerName, string customerContact, decimal price)
        {
            this.vehicleType = vehicleType;
            this.acceptanceTime = acceptanceTime;
            this.deliveryTime = deliveryTime;
            this.customerName = customerName;
            this.customerContact = customerContact;
            this.price = price;
        }

        public bool IsValidVehicleType(string vehicleTypeInput, out string errorString)
        {
            if (!int.TryParse(vehicleTypeInput, out int typeNumber) || typeNumber < 1 || typeNumber > 5)
            {
                errorString = "Invalid vehicle type. Please select a number between 1 and 5.";
                return false;
            }
            errorString = string.Empty;
            vehicleType = (VehicleType) typeNumber;
            return true;
        }

        public bool IsValidCustomerName(string customerName, out string errorString, int nameLength = 5)
        {
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrWhiteSpace(customerName) || customerName.Length < nameLength)
            {
                errorString = $"Invalid customer name. Please enter a non-empty name with a minimum length of {nameLength} characters.";
                return false;
            }
            errorString = string.Empty;
            this.customerName = customerName;
            return true;
        }
        public bool IsValidCustomerContact(string customerContact, out string errorString, int contactLength = 10)
        {
            if (string.IsNullOrEmpty (customerContact)|| string.IsNullOrWhiteSpace(customerContact) || customerContact.Length < contactLength)
            {
                errorString = $"Invalid customer contact. Please enter a non-empty contact with a minimum length of {contactLength} characters.";
                return false;
            }
            errorString = string.Empty;
            this.customerContact = customerContact;
            return true;
        }

        public bool IsValidAcceptanceDateTime(string dateTimeInput, out string errorString)
        {
            if (!DateTime.TryParseExact(dateTimeInput, "dd-MM-yyyy HH:mm",
                null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                errorString = "Invalid date and time format. Please enter a valid date and time.";
                return false;
            }
            errorString = string.Empty;
            this.acceptanceTime = parsedDateTime;
            return true;
        }

        public bool IsValidDeliveryDateTime(string dateTimeInput, out string errorString, int minMinutes = 20)
        {
            if (!DateTime.TryParseExact(dateTimeInput, "dd-MM-yyyy HH:mm",
                null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                errorString = "Invalid date/time format or Acceptance time comes after Delivery time. Please enter a valid date and time.";
                return false;
            }

            else if (parsedDateTime <= this.acceptanceTime)
            {
                errorString = "Acceptance time comes after Delivery time. Please enter a valid date and time.";
                return false;
            }

            else if ((parsedDateTime - this.acceptanceTime).TotalMinutes < minMinutes)
            {
                errorString = $"minimum booking time is {minMinutes}. Please enter a valid date and time.";
                return false;
            }

            errorString = string.Empty;
            this.deliveryTime = parsedDateTime;
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
            this.price = parsedPrice;
            return true;
        }

        public override string ToString()
        {
            return $"{reservationID};{vehicleType.ToString()};{acceptanceTime:dd.MM.yyyy HH:mm};{deliveryTime:dd.MM.yyyy HH:mm};{price};{customerName};{customerContact}";
        }

        public static Reservation FromString(string line)
        {
            var parts = line.Split(';');
            return new Reservation
            {
                reservationID = Guid.Parse(parts[0]),
                vehicleType = (VehicleType)Enum.Parse(typeof(VehicleType), parts[1]),
                acceptanceTime = DateTime.ParseExact(parts[2], "dd.MM.yyyy HH:mm", null),
                deliveryTime = DateTime.ParseExact(parts[3], "dd.MM.yyyy HH:mm", null),
                price = decimal.Parse(parts[4]),
                customerName = parts[5],
                customerContact = parts[6]
            };
        }

    }
}

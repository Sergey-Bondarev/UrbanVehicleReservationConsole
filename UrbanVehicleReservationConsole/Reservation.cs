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
        public long reservationID = -1;
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

        public bool IsValidCustomerName(string customerName, out string errorString,
            int nameLengthMin = 5, int nameLengthMax = 30, string nameRegex = @"^[A-Za-z ]+$")
        {
            if (string.IsNullOrEmpty(customerName)
                || string.IsNullOrWhiteSpace(customerName)
                || customerName.Length < nameLengthMin
                || customerName.Length > nameLengthMax
                || !Regex.IsMatch(customerName, nameRegex))
            {
                errorString = $"Invalid customer name. Please enter a non-empty name with a length range between {nameLengthMin} - {nameLengthMax} characters.";
                return false;
            }
            errorString = string.Empty;
            this.customerName = customerName;
            return true;
        }
        public bool IsValidCustomerContact(string customerContact, out string errorString, int contactLengthMin = 10, int contactLengthMax = 30,
            string emailReg = @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            string telReg = @"^\+380\d{9}$")
        {
            if (string.IsNullOrEmpty (customerContact)
                || string.IsNullOrWhiteSpace(customerContact)
                || customerContact.Length < contactLengthMin
                || customerContact.Length > contactLengthMax)
            {
                errorString = $"Invalid customer contact. Please enter a non-empty contact with a a length range between {contactLengthMin} - {contactLengthMax} characters.";
                return false;
            }

            else if(!Regex.IsMatch(customerContact, emailReg)
                && !Regex.IsMatch(customerContact, telReg))
            {
                errorString = $"Invalid customer contact. Input should be either tel. number with +380 or valid email address";
                return false;
            }
            errorString = string.Empty;
            this.customerContact = customerContact;
            return true;
        }

        public bool IsValidAcceptanceDateTime(string dateTimeInput, out string errorString)
        {
            if (!DateTime.TryParseExact(dateTimeInput, "dd.MM.yyyy HH:mm",
                null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                errorString = "Invalid date and time format. Please enter a valid date and time.";
                return false;
            }
            else if(parsedDateTime <= new DateTime(2025,01,01,23,59,59))
            {
                errorString = "Invalid date and time format. Please enter a date after 01.01.2025";
                return false;
            }
                errorString = string.Empty;
            this.acceptanceTime = parsedDateTime;
            return true;
        }

        public bool IsValidDeliveryDateTime(string dateTimeInput, out string errorString, int minMinutes = 20)
        {
            if (!DateTime.TryParseExact(dateTimeInput, "dd.MM.yyyy HH:mm",
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
            return $"Reservation ID: {reservationID}\n" +
            $"Vehicle Type: {vehicleType}\n" +
            $"Acceptance Time: {acceptanceTime}\n" +
            $"Delivery Time: {deliveryTime}\n" +
            $"Customer Name: {customerName}\n" +
            $"Customer Contact: {customerContact}\n" +
            $"Price: {price}\n"
            + new string('-', 40);
        }
    }
}

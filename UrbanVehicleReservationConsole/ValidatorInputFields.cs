using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanVehicleReservationConsole
{
    public static class ValidatorInputFields
    {
        public static bool IsValidVehicleType(string vehicleTypeInput, out int vehicleNumber, out string errorString)
        {
            if (!int.TryParse(vehicleTypeInput, out int typeNumber) || typeNumber < 1 || typeNumber > 5)
            {
                errorString = "Invalid vehicle type. Please select a number between 1 and 5.";
                vehicleNumber = -1;
                return false;
            }
            errorString = string.Empty;
            vehicleNumber = typeNumber;
            return true;
        }

        public static bool IsValidCustomerName(string customerName, out string errorString, int nameLength = 5)
        {
            if (string.IsNullOrWhiteSpace(customerName) || customerName.Length < nameLength)
            {
                errorString = $"Invalid customer name. Please enter a non-empty name with a minimum length of {nameLength} characters.";
                return false;
            }
            errorString = string.Empty;
            return true;
        }
        public static bool IsValidCustomerContact(string customerContact, out string errorString, int contactLength = 10)
        {
            if (string.IsNullOrWhiteSpace(customerContact) || customerContact.Length < contactLength)
            {
                errorString = $"Invalid customer contact. Please enter a non-empty contact with a minimum length of {contactLength} characters.";
                return false;
            }
            errorString = string.Empty;
            return true;
        }

        public static bool IsValidAcceptanceDateTime(string dateTimeInput, out DateTime parsedDateTime, out string errorString)
        {
            if (!DateTime.TryParse(dateTimeInput, out parsedDateTime))
            {
                errorString = "Invalid date and time format. Please enter a valid date and time.";
                return false;
            }
            errorString = string.Empty;
            return true;
        }

        public static bool IsValidDeliveryDateTime(string dateTimeInput, DateTime acceptanceTime, out DateTime parsedDateTime, out string errorString)
        {
            if (!DateTime.TryParse(dateTimeInput, out parsedDateTime) || acceptanceTime >= parsedDateTime)
            {
                errorString = "Invalid date/time format or Acceptance time comes after Delivery time. Please enter a valid date and time.";
                return false;
            }
            errorString = string.Empty;
            return true;
        }

    }
}

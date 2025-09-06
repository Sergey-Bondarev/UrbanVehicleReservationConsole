using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanVehicleReservationConsole
{
    public class InterfaceHandler
    {
        public static int N = 0;
        public static List<Reservation> reservations = new List<Reservation>();
        public static void PrintMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Reserve a vehicle");
            Console.WriteLine("2. View reservations");
            Console.WriteLine("3. Find reservations");
            Console.WriteLine("4. Cancel a reservation");
            Console.WriteLine("0. Exit");
        }
        public static void HandleUserMenuInput()
        {
            while (true)
            {
                Console.WriteLine("Please enter the maximum number of reservation you want to work with or press 0 to exit program");
                var inputN = Console.ReadLine();
                if(!int.TryParse(inputN, out int validN) || validN < 0)
                {
                    Console.WriteLine("Invalid input. Please enter a positive integer.");
                    inputN = Console.ReadLine();
                }
                else if (validN == 0)
                {
                    return;
                }
                else
                {
                    N = validN;
                    break;
                }
            }
            
            PrintInterfaceBorder();
            while (true)
            {
                PrintMenu();
                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        if (reservations.Count >= N)
                        {
                            Console.WriteLine($"Cannot add more reservations. Maximum limit of {N} reached.");
                            return;
                        }
                        Console.WriteLine("Reserving a vehicle...");
                        HandleNewReservation();
                        PrintInterfaceBorder();
                        break;
                    case "2":
                        ViewAllReservationsTabular(reservations);
                        PrintInterfaceBorder();
                        break;
                    case "3":
                        var foundReservations = HandleSearchForReservetions("Please enter unique ID or Reservation date and time of reservation");
                        if (foundReservations.Count() == 0)
                        {
                            Console.WriteLine("No reservations found on input data.");
                            break;
                        }
                        ViewAllReservations(foundReservations);
                        PrintInterfaceBorder();
                        break;
                    case "4":
                        int removedCount = 0;
                        HandleDeleteReservation("Please enter unique ID or Reservation date and time of reservation to delete", out removedCount);
                        if (removedCount == 0)
                        {
                            Console.WriteLine("No objects were deleted on your input.");
                            break;
                        }
                        else if(removedCount > 0)
                        {
                            Console.WriteLine($"{removedCount} reservation(s) successfully removed.");
                            break;
                        }
                        PrintInterfaceBorder();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        public static void HandleNewReservation()
        {
            Console.WriteLine("Add Vehicle type number");
            PrintSubMenuForVehicleTypes();
            var vehicleTypeInput = Console.ReadLine();
            Reservation reservation = new Reservation();
            if (!reservation.IsValidVehicleType(vehicleTypeInput, out string errorString))
            {
                Console.WriteLine(errorString);
                return;
            }

            Console.WriteLine("Add Customer Name");
            var customerNameInput = Console.ReadLine();
            if (!reservation.IsValidCustomerName(customerNameInput, out errorString))
            {
                Console.WriteLine(errorString);
                return;
            }

            Console.WriteLine("Add Customer Contact");
            var customerContactInput = Console.ReadLine();
            if (!reservation.IsValidCustomerContact(customerContactInput, out errorString))
            {
                Console.WriteLine(errorString);
                return;
            }

            Console.WriteLine("Add Acceptance Date with dd.MM.yyyy HH:mm format");
            var acceptanceDateInput = Console.ReadLine();
            if(!reservation.IsValidAcceptanceDateTime(acceptanceDateInput, out errorString))
            {
                Console.WriteLine(errorString);
                return;
            }

            Console.WriteLine("Add Delivery Date with dd.MM.yyyy HH:mm format");
            var deliveryDateInput = Console.ReadLine();
            if (!reservation.IsValidDeliveryDateTime(deliveryDateInput, out errorString))
            {
                Console.WriteLine(errorString);
                return;
            }

            Console.WriteLine("Add Price");
            var priceInput = Console.ReadLine();
            if (!reservation.IsValidPrice(priceInput, out errorString))
            {
                Console.WriteLine(errorString);
                return;
            }
            
            reservation.reservationID = reservations.Count > 0? reservations.Max(res => res.reservationID) + 1 : 0;
            reservations.Add(reservation);
            Console.WriteLine($"Reservation successfully created with next index: {reservation.reservationID}");

        }

        public static IEnumerable<Reservation> HandleSearchForReservetions(string searchPurposeMessage)
        {
            Console.WriteLine(searchPurposeMessage);
            var input = Console.ReadLine();
            if (!Reservation.IsValidIndexOrAcceptanceDateTime(input, out string errorString, out long id, out DateTime date))
            {
                Console.WriteLine(errorString);
                return new List<Reservation>();
            }
            return reservations.FindAll(r => r.reservationID == id
                                    || r.acceptanceTime == date).ToList();
        }

        public static void HandleDeleteReservation(string deletePurposeMessage, out int removeCount)
        {
            Console.WriteLine(deletePurposeMessage);
            var input = Console.ReadLine();
            if (!Reservation.IsValidIndexOrAcceptanceDateTime(input, out string errorString, out long id, out DateTime date))
            {
                Console.WriteLine(errorString);
                removeCount = -1;
                return;
            }
            removeCount = reservations.RemoveAll(r => r.reservationID == id 
                                            || r.acceptanceTime == date);

        }
        
        public static void PrintSubMenuForVehicleTypes()
        {
            Console.WriteLine("Vehicle Types:");
            Console.WriteLine("1. Car");
            Console.WriteLine("2. Motorcycle");
            Console.WriteLine("3. Bike");
            Console.WriteLine("4. Scooter");
            Console.WriteLine("5. ElectricScooter");
            Console.WriteLine("0. Back to Main Menu");
        }
        
        public static void ViewAllReservations(IEnumerable<Reservation> reservations)
        {
            if (reservations.Count() == 0)
            {
                Console.WriteLine("No reservations found.");
                return;
            }
            foreach (var reservation in reservations)
            {
                Console.WriteLine($"Reservation ID: {reservation.reservationID}");
                Console.WriteLine($"Vehicle Type: {reservation.vehicleType}");
                Console.WriteLine($"Acceptance Time: {reservation.acceptanceTime}");
                Console.WriteLine($"Delivery Time: {reservation.deliveryTime}");
                Console.WriteLine($"Customer Name: {reservation.customerName}");
                Console.WriteLine($"Customer Contact: {reservation.customerContact}");
                Console.WriteLine($"Price: {reservation.price}");
                Console.WriteLine(new string('-', 40));
            }
        }

        public static void ViewAllReservationsTabular(IEnumerable<Reservation> reservations)
        {
            if (reservations.Count() == 0)
            {
                Console.WriteLine("No reservations found.");
                return;
            }

            int idWidth = Math.Max("ID".Length, reservations.Max(r => r.reservationID.ToString().Length)) + 2;
            int nameWidth = Math.Max("Customer".Length, reservations.Max(r => r.customerName.Length)) + 2;
            int contactWidth = Math.Max("Contact".Length, reservations.Max(r => r.customerContact.Length)) + 2;
            int priceWidth = Math.Max("Price".Length, reservations.Max(r => r.price.ToString().Length)) + 2;

            Console.WriteLine(
                $"{"ID".PadRight(idWidth)}" +
                $"{"Customer".PadRight(nameWidth)}" +
                $"{"Contact".PadRight(contactWidth)}" +
                $"{"Vehicle".PadRight(17)}" +
                $"{"Acceptance".PadRight(20)}" +
                $"{"Delivery".PadRight(20)}" +
                $"{"Price".PadRight(priceWidth)}"
            );

            Console.WriteLine(new string('-', idWidth + nameWidth + contactWidth + 17 + 20 + 20 + priceWidth));
            foreach (var r in reservations)
            {
                string acceptance = r.acceptanceTime.ToString("HH:mm");
                string delivery = r.deliveryTime.ToString("HH:mm");
                Console.WriteLine(
                $"{r.reservationID.ToString().PadRight(idWidth)}" +
                $"{r.customerName.PadRight(nameWidth)}" +
                $"{r.customerContact.PadRight(contactWidth)}" +
                $"{r.vehicleType, -17}" +
                $"{r.acceptanceTime.ToString("dd.MM.yyyy HH:mm"), -20}" +
                $"{r.deliveryTime.ToString("dd.MM.yyyy HH:mm"), -20}" +
                $"{(r.price + "$").PadRight(priceWidth)}");
            }
        }

        public static void PrintInterfaceBorder()
        {
            Console.WriteLine(new string('=', 40));
            Console.WriteLine();
        }
    }
}

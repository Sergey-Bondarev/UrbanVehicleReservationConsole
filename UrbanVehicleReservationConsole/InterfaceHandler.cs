using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanVehicleReservationConsole
{
    public class InterfaceHandler
    {
        public static int N = -1;
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
                PrintMenu();
                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.WriteLine("Reserving a vehicle...");
                        HandleNewReservation();
                        break;
                    case "2":
                        ViewAllReservations(reservations);
                        break;
                    case "3":
                        var foundReservations = HandleSearchForReservetions();
                        if (foundReservations.Count() == 0)
                        {
                            Console.WriteLine("No reservations found on input data.");
                            break;
                        }
                        ViewAllReservations(foundReservations);
                        break;
                    case "4":
                        // Code to cancel a reservation
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

        public static IEnumerable<Reservation> HandleSearchForReservetions()
        {
            Console.WriteLine("Please enter unique ID or Reservation date and time of reservation");
            var input = Console.ReadLine();
            if (!Reservation.IsValidIndexOrAcceptanceDateTime(input, out string errorString, out long id, out DateTime date))
            {
                Console.WriteLine(errorString);
                return new List<Reservation>();
            }
            return reservations.FindAll(r => r.reservationID == id || r.acceptanceTime == date || r.deliveryTime == date).ToList();


        }
        public static void PrintSubMenuForFindingReservations()
        {
            Console.WriteLine("Find Reservations By:");
            Console.WriteLine("1. Reservation ID");
            Console.WriteLine("3. Vehicle Type");
            Console.WriteLine("4. Acceptance Date");
            Console.WriteLine("5. Delivery Date");
            Console.WriteLine("0. Back to Main Menu");
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
        

        public static void PrintSubMenuForDateRanges()
        {
            Console.WriteLine("Date Ranges:");
            Console.WriteLine("1. Acceptance time");
            Console.WriteLine("2. Delivery Week");
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
        public static void ViewInputError(string inputType, string errorText)
        {
            Console.WriteLine($"OOOOPs something went wrong with {inputType}. {errorText}");
        }
    }
}

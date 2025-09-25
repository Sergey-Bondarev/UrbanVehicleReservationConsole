using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanVehicleReservationConsole
{
    public static class InterfaceHandler
    {
        public static List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public static void PrintMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Reserve a vehicle");
            Console.WriteLine("2. View reservations");
            Console.WriteLine("3. Find reservations");
            Console.WriteLine("4. Cancel a reservation");
            Console.WriteLine("5. Add new reservation by string format");
            Console.WriteLine("0. Exit");
        }
        public static void HandleUserMenuInput()
        {
            //LoadData();
            PrintInterfaceBorder();
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
                        PrintInterfaceBorder();
                        break;
                    case "2":
                        ViewAllReservationsTabular(Reservations);
                        PrintInterfaceBorder();
                        break;
                    case "3":
                        var foundReservations = HandleSearchForReservetions("Please enter unique ID or Reservation date and time of reservation");
                        if (foundReservations.Count() == 0)
                        {
                            Console.WriteLine("No reservations found on input data.");
                            break;
                        }
                        ViewAllReservationsTabular(foundReservations);
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
                        else if (removedCount > 0)
                        {
                            Console.WriteLine($"{removedCount} reservation(s) successfully removed.");
                            break;
                        }
                        PrintInterfaceBorder();
                        break;

                    case "5":
                        HandleNewReservationByStringInput();
                        PrintInterfaceBorder();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        PrintInterfaceBorder();
                        break;
                }
            }
        }

        public static void HandleNewReservation()
        {
            Console.WriteLine("Add Vehicle type number");
            string errorString;
            PrintSubMenuForVehicleTypes();
            Reservation reservation = new Reservation();
            while (true)
            {
                var vehicleTypeInput = Console.ReadLine();
                VehicleType parsedType = new VehicleType();
                if (vehicleTypeInput == "0")
                {
                    return;
                }
                else if (!Reservation.TryParseVehicleType(vehicleTypeInput, out parsedType, out errorString))
                {
                    Console.WriteLine(errorString);
                    continue;
                }
                reservation.VehicleType = parsedType;
                break;
            }

            Console.WriteLine("Add Customer Name");
            while (true)
            {
                var customerNameInput = Console.ReadLine();
                if (customerNameInput == "0")
                {
                    return;
                }

                try
                {
                    reservation.CustomerName = customerNameInput;
                    break;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    continue;
                }


                break;
            }

            Console.WriteLine("Add Customer Contact");
            while (true)
            {
                var customerContactInput = Console.ReadLine();
                if (customerContactInput == "0")
                {
                    return;
                }

                try
                {
                    reservation.CustomerContact = customerContactInput;
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    continue;
                }
            }

            Console.WriteLine("Add Acceptance Date with dd.MM.yyyy HH:mm format");
            while (true)
            {
                var acceptanceDateInput = Console.ReadLine();
                DateTime parsedDate = DateTime.Now;
                if (acceptanceDateInput == "0")
                {
                    return;
                }

                else if (!Reservation.IsValidDate(acceptanceDateInput, out parsedDate, out errorString))
                {
                    Console.WriteLine(errorString);
                    continue;
                }

                try
                {
                    reservation.AcceptanceTime = parsedDate;
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    continue;
                }
            }

            Console.WriteLine("Add Delivery Date with dd.MM.yyyy HH:mm format");
            while (true)
            {
                var deliveryDateInput = Console.ReadLine();
                DateTime parsedDate = DateTime.Now;
                if (deliveryDateInput == "0")
                {
                    return;
                }
                else if (!Reservation.IsValidDate(deliveryDateInput, out parsedDate, out errorString))
                {
                    Console.WriteLine(errorString);
                    continue;
                }

                try
                {
                    reservation.DeliveryTime = parsedDate;
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    continue;
                }
            }

            reservation.Deconstructor(out VehicleType vehicleType, out DateTime acceptanceTime, out DateTime deliveryTime, out string customerName, out string customerContact);

            reservation = CreateNewReservationRandomConstructor(vehicleType, acceptanceTime, deliveryTime, customerName, customerContact);

            reservation.CalculatePrice();
            reservation.setIndex(Reservations);
            Reservations.Add(reservation);
            Console.WriteLine($"Reservation successfully created with next index: {reservation.ReservationID}");
        }

        public static Reservation CreateNewReservationRandomConstructor(VehicleType vehicleType, DateTime acceptanceTime,
            DateTime deliveryTime, string customerName, string customerContact)
        {
            Random random = new Random();
            int min = 1;
            int max = 5;
            int randomNumber = random.Next(min, max);

            switch (randomNumber)
            {
                case 1:
                    Console.WriteLine("Default/parameterless constructor was used with initializers. All values secured.");
                    Reservation.ReservationCounter++;
                    return new Reservation()
                    {
                        VehicleType = vehicleType,
                        AcceptanceTime = acceptanceTime,
                        DeliveryTime = deliveryTime,
                        CustomerName = customerName,
                        CustomerContact = customerContact, 
                    };

                case 2:
                    Console.WriteLine("3-parameter constructor was used. Customer information was lost.");
                    return new Reservation(vehicleType, acceptanceTime, deliveryTime);

                case 3:
                    Console.WriteLine("Full-parameter constructor was used. All values secured.");
                    return new Reservation(vehicleType, acceptanceTime, deliveryTime, customerName, customerContact);

                case 4:
                    Console.WriteLine("Copy constructor with full-parameters was used. All values secured.");
                    return new Reservation(new Reservation(vehicleType, acceptanceTime, deliveryTime, customerName, customerContact, false));

                default:
                    Console.WriteLine("Parameterless constructor was used. No values secured.");
                    Reservation.ReservationCounter++;
                    return new Reservation();
            }

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
            return Reservations.FindAll(r => r.ReservationID == id
                                    || r.AcceptanceTime == date).ToList();
        }

        public static void HandleDeleteReservation(string deletePurposeMessage, out int removeCount)
        {
            Console.WriteLine(deletePurposeMessage);
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || input.Equals("0"))
            {
                removeCount = 0;
                return;
            }

            if (!Reservation.IsValidIndexOrAcceptanceDateTime(input, out string errorString, out long id, out DateTime date))
            {
                Console.WriteLine(errorString);
                removeCount = -1;
                return;
            }
            removeCount = Reservations.RemoveAll(r => r.ReservationID == id
                                            || r.AcceptanceTime == date);
            Reservation.ReservationCounter -= removeCount;
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

        public static void ViewAllReservationsTabular(IEnumerable<Reservation> reservations)
        {
            if (reservations.Count() == 0)
            {
                Console.WriteLine("No reservations found.");
                return;
            }

            int vehicleTypeWidth = Math.Max("Vehicle Type".Length, reservations.Max(r => r.VehicleType.ToString().Length)) + 2;
            int dateWidth = Math.Max("Acceptance Date".Length, reservations.Max(r => r.AcceptanceTime.ToString("dd.MM.yyyy HH:mm").Length)) + 2;
            int idWidth = Math.Max("ID".Length, reservations.Max(r => r.ReservationID.ToString().Length)) + 2;
            int nameWidth = Math.Max("Customer".Length, reservations.Max(r => r.CustomerName.Length)) + 2;
            int contactWidth = Math.Max("Contact".Length, reservations.Max(r => r.CustomerContact.Length)) + 2;
            int priceWidth = Math.Max("Price".Length, reservations.Max(r => r.Price.ToString().Length)) + 2;

            Console.WriteLine(
                $"{"ID".PadRight(idWidth)}" +
                $"{"Customer".PadRight(nameWidth)}" +
                $"{"Contact".PadRight(contactWidth)}" +
                $"{"Vehicle".PadRight(vehicleTypeWidth)}" +
                $"{"Acceptance".PadRight(dateWidth)}" +
                $"{"Delivery".PadRight(dateWidth)}" +
                $"{"Price".PadRight(priceWidth)}"
            );

            Console.WriteLine(new string('-', idWidth + nameWidth + contactWidth + vehicleTypeWidth + dateWidth + dateWidth + priceWidth));
            foreach (var r in reservations)
            {
                string acceptance = r.AcceptanceTime.ToString("HH:mm");
                string delivery = r.DeliveryTime.ToString("HH:mm");
                Console.WriteLine(
                $"{r.ReservationID.ToString().PadRight(idWidth)}" +
                $"{r.CustomerName.PadRight(nameWidth)}" +
                $"{r.CustomerContact.PadRight(contactWidth)}" +
                $"{r.VehicleType.ToString().PadRight(vehicleTypeWidth)}" +
                $"{r.AcceptanceTime.ToString("dd.MM.yyyy HH:mm").PadRight(dateWidth)}" +
                $"{r.DeliveryTime.ToString("dd.MM.yyyy HH:mm").PadRight(dateWidth)}" +
                $"{(r.Price + "$").PadRight(priceWidth)}");
            }
            Console.WriteLine($"Current value of reservation counter: {Reservation.ReservationCounter}");

        }

        public static void PrintInterfaceBorder()
        {
            Console.WriteLine(new string('=', 100));
            Console.WriteLine();
        }

        public static void HandleNewReservationByStringInput()
        {
            Console.WriteLine("Add new reservation by string format:");
            Console.WriteLine("{ReservationID};{VehicleType};{AcceptanceTime:dd.MM.yyyy HH:mm};{DeliveryTime:dd.MM.yyyy HH:mm};{Price};{CustomerName};{CustomerContact}");
            string input = Console.ReadLine();
            if (Reservation.TryParse(input, out Reservation reservation))
            {
                Reservation.ReservationCounter++;
                Reservations.Add(reservation);
                Console.WriteLine($"Reservation successfully created with next index: {reservation.ReservationID}");
            }
            return;

        }

        public static void SaveData(string filePath = "../../../data/Reservations.txt")
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var reservation in Reservations)
                {
                    writer.WriteLine(reservation.ToString());
                }
            }
        }

        public static void LoadData(string filePath = "../../../data/Reservations.txt")
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Data file not found. Starting with an empty reservation list.");
                return;
            }
            Reservations.Clear();
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    Reservations.Add(Reservation.Parse(line));
            }
        }

        public static void SaveDataJson(string filePath = "../../../data/Reservations.json")
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
            string jsonString = System.Text.Json.JsonSerializer.Serialize(Reservations, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static void LoadDataJson(string filePath = "../../../data/Reservations.json")
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Data file not found. Starting with an empty reservation list.");
                return;
            }
            string jsonString = File.ReadAllText(filePath);
            var options = new System.Text.Json.JsonSerializerOptions
            {
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
            Reservations = System.Text.Json.JsonSerializer.Deserialize<List<Reservation>>(jsonString, options) ?? new List<Reservation>();
            Reservation.ReservationCounter = Reservations.Count;
        }
    }
}

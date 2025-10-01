using System.Globalization;
using UrbanVehicleReservationConsole;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservationConsoleTest
{
    [TestClass]
    public sealed class ReservationTest
    {
        private Reservation _reservation;

        [TestInitialize]
        public void Setup()
        {
            _reservation = new Reservation();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _reservation = null;
        }

        [TestMethod]
        public void TestAcceptanceTimeProperty_WithExeptionThrow()
        {
            var date = new DateTime(2024, 1, 1, 12, 0, 0);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _reservation.AcceptanceTime = date);

        }

        [TestMethod]
        public void TestAcceptanceTimeProperty()
        {
            var date = new DateTime(2025, 1, 1, 13, 0, 0);

            _reservation.AcceptanceTime = date;

            Assert.AreEqual(date, _reservation.AcceptanceTime);
        }

        [TestMethod]
        public void TestDeliveryTimeProperty()
        {
            var date = new DateTime(2025, 1, 1, 13, 0, 0);

            _reservation.AcceptanceTime = date.AddMinutes(-20);
            _reservation.DeliveryTime = date;

            Assert.AreEqual(date, _reservation.DeliveryTime);

        }

        [TestMethod]
        public void TestDeliveryTimeProperty_WithExeptionThrowOutOfRange()
        {
            var date = new DateTime(2025, 1, 1, 13, 0, 0);

            _reservation.AcceptanceTime = date.AddMinutes(-19);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _reservation.DeliveryTime = date);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("        ")]
        [DataRow("ser")]
        [DataRow("ser10987")]
        [DataRow("serhio&")]
        [DataRow("Serхио")]
        [DataRow("Name testerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCustomerName_WithExeptionThrow(string input)
        {
            _reservation.CustomerName = input;
        }

        [TestMethod]
        [DataRow("Serhio")]
        [DataRow("serhio")]
        [DataRow("ser Hio")]
        public void TestCustomerName(string input)
        {
            _reservation.CustomerName = input;

            Assert.AreEqual(input, _reservation.CustomerName);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("        ")]
        [DataRow("joe.mail.com")]
        [DataRow("joe@mail@com")]
        [DataRow("joe@mail.com")]
        [DataRow("+380")]
        [DataRow("+3809999999999999")]
        [DataRow("+38011234567")]
        [DataRow("380112345678")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCustomerContact_WithExeprionThrow(string input)
        {
            _reservation.CustomerName = input;
        }

        [TestMethod]
        [DataRow("joe@mail.com")]
        [DataRow("joe.Dou@mail.com")]
        [DataRow("Joe_Dou@mail.com")]
        [DataRow("+380112345678")]
        public void TestCustomerContact(string input)
        {
            _reservation.CustomerContact = input;

            Assert.AreEqual(input, _reservation.CustomerContact);
        }

        [TestMethod]
        public void TestPriceCulculation()
        {
            var date = new DateTime(2025, 1, 1, 13, 0, 0);
            _reservation.AcceptanceTime = date.AddHours(-2);
            _reservation.DeliveryTime = date;
            _reservation.VehicleType = VehicleType.Bike;

            Assert.AreEqual(10m, _reservation.Price);
        }

        [TestMethod]
        [DataRow("1;Bike;11.10.2025 12:00;01.12.2025 20:00;6160,0;George;geogre@mail.com", true)]
        [DataRow(null, false)]
        [DataRow("", false)]
        public void TestTryParse_WithFalse(string input, bool result)
        {
            Assert.AreEqual(result, Reservation.TryParse(input, out _reservation));
        }

        [TestMethod]
        [DataRow("1;Bike;11.10.2025 12:00;01.12.202520:00;6160,0;George;geogre@mail.com", typeof(FormatException))]
        [DataRow("1;Bike;11.10.2025 12:00,01.12.2025 20:00;6160,0;George;geogre@mail.com", typeof(FormatException))]
        [DataRow("1;Bike;11.10.2025 12:00;01.12.2025 20:00;6160,0;George;geogre@mail.com", typeof(ArgumentNullException))]
        [DataRow("1;11.10.2025 12:00;01.12.2025 20:00;6160,0;George;geogre@mail.com", typeof(FormatException))]
        [DataRow("  ", typeof(FormatException))]
        public void TestTryParse_WithException(string input, Type expectedExceptionType)
        {
            try
            {
                Reservation.TryParse(input, out _reservation);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(expectedExceptionType, ex.GetType());
            }
            
        }

        [TestMethod]
        [DataRow("1", VehicleType.Car)]
        [DataRow("2", VehicleType.Motorcycle)]
        [DataRow("3", VehicleType.Bike)]
        public void TestTryParseVehicleType_WithFalse(string input, VehicleType expected)
        {
            var result = Reservation.TryParseVehicleType(input, out var parsed, out var error);

            Assert.IsTrue(result);
            Assert.AreEqual(expected, parsed);
            Assert.AreEqual(string.Empty, error);
        }

        [TestMethod]
        [DataRow("0")]        
        [DataRow("6")]
        [DataRow("abc")]
        [DataRow("")]
        [DataRow(null)]
        public void TestTryParseVehicleType_WithFalse(string input)
        {
            var result = Reservation.TryParseVehicleType(input, out var parsed, out var error);

            Assert.IsFalse(result);
            Assert.AreEqual(default(VehicleType), parsed);
            Assert.AreEqual("Invalid vehicle type. Please select a number between 1 and 5.", error);
        }

        [DataTestMethod]
        [DataRow("01.01.2025 13:00", 2025, 1, 1, 13, 0)]
        [DataRow("15.12.2024 08:30", 2024, 12, 15, 8, 30)]
        public void TestIsValidDate_WithTrue(string input, int year, int month, int day, int hour, int minute)
        {
            var result = Reservation.IsValidDate(input, out var parsed, out var error);

            Assert.IsTrue(result);
            Assert.AreEqual(new DateTime(year, month, day, hour, minute, 0), parsed);
            Assert.AreEqual(string.Empty, error);
        }

        [DataTestMethod]
        [DataRow("2025-01-01 13:00")]
        [DataRow("01/01/2025 13:00")]
        [DataRow("01.01.25 13:00")]
        [DataRow("")]
        [DataRow(null)]
        public void TestIsValidDate_WithFalse(string input)
        {
            var result = Reservation.IsValidDate(input, out var parsed, out var error);

            Assert.IsFalse(result);
            Assert.AreEqual(DateTime.MinValue, parsed);
            Assert.AreEqual("Invalid date/time format or Acceptance time comes after Delivery time. Please enter a valid date and time.", error);
        }

        [TestMethod]
        [DataRow("12345", 12345)]
        [DataRow("0", 0)]
        public void TestIsValidIndexOrAcceptanceDateTime_WithTrueIndex(string input, long expectedId)
        {
            var result = Reservation.IsValidIndexOrAcceptanceDateTime(input, out var error, out var reservationId, out var acceptanceDateTime);

            Assert.IsTrue(result);
            Assert.AreEqual(expectedId, reservationId);
            Assert.AreEqual(DateTime.MinValue, acceptanceDateTime);
            Assert.AreEqual(string.Empty, error);
        }

        [TestMethod]
        [DataRow("01.01.2025 13:00", 2025, 1, 1, 13, 0)]
        [DataRow("15.12.2024 08:30", 2024, 12, 15, 8, 30)]
        public void TestIsValidIndexOrAcceptanceDateTime_WithTrueTime(string input, int year, int month, int day, int hour, int minute)
        {
            var result = Reservation.IsValidIndexOrAcceptanceDateTime(input, out var error, out var reservationId, out var acceptanceDateTime);

            Assert.IsTrue(result);
            Assert.AreEqual(new DateTime(year, month, day, hour, minute, 0), acceptanceDateTime);
            Assert.AreEqual(string.Empty, error);
        }

        [TestMethod]
        [DataRow("-1")]
        [DataRow("abc")]
        [DataRow("2025-01-01 13:00")]
        [DataRow("")]
        [DataRow(null)]
        public void TestIsValidIndexOrAcceptanceDateTime_WithFalse(string input)
        {
            var result = Reservation.IsValidIndexOrAcceptanceDateTime(input, out var error, out var reservationId, out var acceptanceDateTime);

            Assert.IsFalse(result);
            Assert.AreEqual(long.MinValue, reservationId);
            Assert.AreEqual(DateTime.MinValue, acceptanceDateTime);
            Assert.AreEqual("Invalid input. Please enter a valid reservation ID or date and time in the format dd:MM:yyyy HH:mm.", error);
        }

        [TestMethod]
        public void TestDeconstructor()
        {
            var vehicleType = VehicleType.Motorcycle;
            var acceptanceTime = DateTime.Now.AddDays(-1);
            var deliveryTime = DateTime.Now;
            var customerName = "Joe Doe";
            var customerContact = "joe.doe@mail.com";
            _reservation = new Reservation(vehicleType, acceptanceTime, deliveryTime, customerName, customerContact, false);

            _reservation.Deconstructor(out vehicleType, out acceptanceTime, out deliveryTime, out customerName, out customerContact);
            Assert.AreEqual(_reservation.VehicleType, vehicleType);
            Assert.AreEqual(_reservation.AcceptanceTime, acceptanceTime);
            Assert.AreEqual(_reservation.DeliveryTime, deliveryTime);
            Assert.AreEqual(_reservation.CustomerName, customerName);
            Assert.AreEqual(_reservation.CustomerContact, customerContact);
        }

        [TestMethod]
        public void TestToString()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            string output = _reservation.ToString();

            Assert.AreEqual(output, "-1;Car;01.01.0001 00:00;01.01.0001 00:00;0.0;;");
        }
    }
}

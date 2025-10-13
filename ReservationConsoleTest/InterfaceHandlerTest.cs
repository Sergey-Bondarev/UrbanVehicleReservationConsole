using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanVehicleReservationConsole;

namespace ReservationConsoleTest
{
    [TestClass]
    [DoNotParallelize]
    public sealed class InterfaceHandlerTest
    {
        [TestInitialize]
        public void Setup()
        {
            InterfaceHandler.LoadData("../../../data/testData.txt");
        }

        [TestCleanup]
        public void CleanUp()
        {
            InterfaceHandler.Reservations.Clear();
        }

        [TestMethod]
        [DataRow("11.03.2025 12:00",2)]
        [DataRow("1",1)]
        [DataRow("3",1)]
        [DataRow("100",0)]
        [DataRow("20.09.2025 12:00", 1)]
        public void TestHandleSearchForReservetions(string input, int counter)
        {
            var resultList = InterfaceHandler.HandleSearchForReservetions(input, out string _);

            Assert.AreEqual(counter, resultList.Count());
        }

        [TestMethod]
        [DataRow("11.03.2025 12:00", 4, 2)]
        [DataRow("1", 5, 1)]
        [DataRow("3", 5, 1)]
        [DataRow("100", 6, 0)]
        [DataRow("20.09.2025 12:00", 5, 1)]
        public void TestHandleDeleteReservation(string input, int listLength, int count)
        {
            InterfaceHandler.HandleDeleteReservation(input, out string _, out int counter);

            Assert.AreEqual(listLength, InterfaceHandler.Reservations.Count());
            Assert.AreEqual(count, counter);
        }

    }
}

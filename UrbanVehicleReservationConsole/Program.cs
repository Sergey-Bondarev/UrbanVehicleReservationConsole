namespace UrbanVehicleReservationConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WELLCOME TO THE URBAN VEHICLE RESERVATION APP");
            InterfaceHandler.LoadDataJson();
            InterfaceHandler.HandleUserMenuInput();
            InterfaceHandler.SaveDataJson();
            InterfaceHandler.SaveData();
        }
    }
}

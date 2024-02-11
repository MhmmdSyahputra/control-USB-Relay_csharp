using DemoUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using UsbRelayNet.RelayLib;

namespace YourNamespace
{
    class Program
    {
        static Relay selectedRelay;
        static RelaysEnumerator relaysEnumerator = new RelaysEnumerator();
        static RelayItem[] devices;
        static List<bool> lightOn = new List<bool>();
        static int relayOn = 1;

        static async Task Main(string[] args)
        {
            await RefreshDevices();
            await SelectRelay();
            ToggleRelayConnection();
            await RelayOpen();
        }

        static async Task RefreshDevices()
        {
            devices = relaysEnumerator.CollectInfo().Select(x => new RelayItem(x)).ToArray();
            Console.WriteLine("Available relay devices:");
            for (int i = 0; i < devices.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {devices[i].RelayInfo.Id} {devices[i].RelayInfo.HidInfo.Version} ({devices[i].RelayInfo.HidInfo.Vendor})");
            }
        }

        static async Task SelectRelay()
        {
            if (devices.Length == 0)
            {
                Console.WriteLine("No relay devices found.");
                return;
            }

            Console.WriteLine("Select a relay device by entering its index:");

            int selectedIndex;
            do
            {
                Console.Write("Enter the index of the device: ");
            } while (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 1 || selectedIndex > devices.Length);

            selectedRelay = new Relay(devices[selectedIndex - 1].RelayInfo);
        }

        static void ToggleRelayConnection()
        {
            if (selectedRelay == null)
            {
                Console.WriteLine("No relay device selected.");
                return;
            }

            if (selectedRelay.IsOpened)
            {
                selectedRelay.Close();
                Console.WriteLine("Relay connection closed.");
            }
            else
            {
                selectedRelay.Open();
                Console.WriteLine("Relay connection opened.");
            }
        }

        static async Task RelayOpen()
        {
            try
            {
                if (selectedRelay == null)
                {
                    Console.WriteLine("No relay device selected.");
                    return;
                }

                if (selectedRelay.ReadChannel(relayOn))
                {
                    selectedRelay.WriteChannel(relayOn, false);
                }
                else
                {
                    selectedRelay.WriteChannel(relayOn, true);
                }

                await Task.Delay(1000);

                if (selectedRelay.ReadChannel(relayOn))
                {
                    selectedRelay.WriteChannel(relayOn, false);
                }
                else
                {
                    selectedRelay.WriteChannel(relayOn, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}

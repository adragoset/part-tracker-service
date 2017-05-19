using System;
using LibUsbDotNet.Main;
using PartTrackerService.Usb;

namespace part_tracker_service
{
    class Program {

        static BarcodeScanner scanner;
        static void Main(string[] args)
        {
            try {
                var usbDevices = new UsbRegDeviceList();
                scanner = new BarcodeScanner("05FE", "1010", 0);
            }
            catch (Exception e) {
                Console.WriteLine($"Could not initialize barcode scanner:{e.Message}");
            }

            if(scanner != null) {
                while(true) {
                    try {
                        var result = scanner.Read();
                        Console.WriteLine($"Bytes read:{result.Bytes.ToString()} Length:{result.Length}");
                    }
                    catch(Exception e) {
                        Console.WriteLine($"Exception:{e.Message}");
                    }
                }
            }
        }
    }
}

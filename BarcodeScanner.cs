using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace PartTrackerService.Usb {
    public class ScannerOutput {

        public byte[] Bytes;

        public int Length;

        public ErrorCode Error;

        public ScannerOutput(byte[] bytes, int length, ErrorCode error) {
            this.Bytes = bytes;
            this.Length = length;
            this.Error = error;
        }
    }

    public class BarcodeScanner : IDisposable {
        private UsbDevice device;
        private UsbDeviceFinder usbFinder;
        private UsbEndpointReader reader;
        private bool disposedValue = false;
        private int interfaceId;
        private string vendorId;
        private string productId;

        public BarcodeScanner(string vendorId, string productId, int interfaceId) {
            this.vendorId = vendorId;
            this.productId = productId;
            usbFinder = new UsbDeviceFinder(
                Int32.Parse(vendorId, System.Globalization.NumberStyles.HexNumber),
                Int32.Parse(productId, System.Globalization.NumberStyles.HexNumber));
            this.interfaceId = interfaceId;
            configureDevice();
        }

        public ScannerOutput Read() {
            byte[] readBuffer = new byte[1024];
            int bytesRead;
            var error = reader.Read(readBuffer, 5000, out bytesRead);

            return new ScannerOutput(readBuffer, bytesRead, error);
        }

        public void Dispose() {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (device != null && device.IsOpen) {
                        IUsbDevice wholeUsbDevice = device as IUsbDevice;
                        if (!ReferenceEquals(wholeUsbDevice, null)) {
                            wholeUsbDevice.ReleaseInterface(interfaceId);
                        }
                        device.Close();
                    }
                }

                disposedValue = true;
            }
        }

        private void configureDevice() {
            device = UsbDevice.OpenUsbDevice(usbFinder);

            if (device == null) {
                throw new InvalidOperationException("Could not find the barcode scanner.");
            }

            IUsbDevice wholeUsbDevice = device as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null)) {

                wholeUsbDevice.SetConfiguration(1);

                wholeUsbDevice.ClaimInterface(interfaceId);
            }


            reader = device.OpenEndpointReader(ReadEndpointID.Ep01);
        }
    }
}
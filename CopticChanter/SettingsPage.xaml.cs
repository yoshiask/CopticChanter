using CoptLib;
using static CopticChanter.Common.RemoteCMDString;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.UI;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            EFontSizeBox.Text = Common.GetEnglishFontSize().ToString();
            CFontSizeBox.Text = Common.GetCopticFontSize().ToString();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Common.SetEnglishFontSize(Convert.ToInt32(EFontSizeBox.Text));
            Common.SetCopticFontSize(Convert.ToInt32(CFontSizeBox.Text));

            Frame.Navigate(typeof(MainPage));
        }

        private void GregorianDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            Common.CopticDate = GregorianDatePicker.Date.Date.ToCoptic();
            CopticDateDisplay.Text = Common.CopticDate.ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Check version for Fluent Design
            try
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.XamlCompositionBrushBase"))
                {
                    AcrylicBrush BackBrush = new AcrylicBrush();
                    BackBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                    BackBrush.TintColor = Color.FromArgb(255, 246, 246, 246);
                    BackBrush.FallbackColor = Color.FromArgb(255, 246, 246, 246);
                    BackBrush.TintOpacity = 0.6;
                    MainGrid.Background = BackBrush;
                }
                else
                {
                    SolidColorBrush BackBrush = new SolidColorBrush(Color.FromArgb(255, 246, 246, 246));
                    MainGrid.Background = BackBrush;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            // Calculate today's date on Coptic calendar
            if (Common.CopticDate != null)
            {
                GregorianDatePicker.Date = Common.CopticDate.ToGregorianDate();
            }
            Common.CopticDate = GregorianDatePicker.Date.Date.ToCoptic();
            CopticDateDisplay.Text = Common.CopticDate.ToString();

            // Set color for RemoteStatusDisplay
            RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);

            // Check if remote is connected
            if (Common.RemoteSocket == null)
            {
                Common.IsConnected = false;
                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);
                RemoteStatusDisplay.Text = "Not Connected";
                ConnectDeviceButton.Content = "Connect to Device";
            }
            else
            {
                Common.IsConnected = true;
                ConnectDeviceButton.Content = "Disconnect";
                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Green);
                RemoteStatusDisplay.Text = "Connected to " + Common.RemoteInfo.Name;
            }
        }

        private void ConnectDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.BluetoothRemoteConnectPage));

            #region Old
            /*if (Common.IsConnected)
            {
                Common.RemoteSocket.Dispose();
                Common.RemoteService.Dispose();
                Common.RemoteDevice.Dispose();
                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);
                RemoteStatusDisplay.Text = "Disconnected";
                ConnectDeviceButton.Content = "Connect to Device";
                Common.IsConnected = false;
            }
            else
            {
                Common.RemoteInfo = await ShowDevicePicker(true);
                if (Common.RemoteInfo != null)
                {
                    RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);
                    RemoteStatusDisplay.Text = "Connecting...";
                    Common.RemoteDevice = await BluetoothDevice.FromIdAsync(Common.RemoteInfo.Id);

                    var accessStatus = await Common.RemoteDevice.RequestAccessAsync();
                    if (accessStatus == DeviceAccessStatus.Allowed)
                    {
                        //var services = await bluetoothDevice.GetRfcommServicesForIdAsync(
                        //RfcommServiceId.FromUuid(Common.RfcommServiceUuid), BluetoothCacheMode.Cached);

                        // Initialize the target Bluetooth BR device
                        //var service = await RfcommDeviceService.FromIdAsync(services.Services[0].ServiceId.);
                        RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);
                        RemoteStatusDisplay.Text = "Pairing...";
                        var pairResult = await Common.RemoteDevice.DeviceInformation.Pairing.PairAsync();

                        if (pairResult.Status == DevicePairingResultStatus.Paired || pairResult.Status == DevicePairingResultStatus.AlreadyPaired)
                        {
                            RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Green);
                            RemoteStatusDisplay.Text = "Paired";

                            //RfcommDeviceService service = await RfcommDeviceService.FromIdAsync(bluetoothDevice.DeviceId);
                            var services = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.ObexObjectPush));
                            var service = await RfcommDeviceService.FromIdAsync(services[0].Id);
                            if (service == null)
                            {
                                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.DarkRed);
                                RemoteStatusDisplay.Text = "Connection Failed: Device does not have Coptic Chanter installed";
                                return;
                            }

                            // Check that the service meets this App's minimum requirement
                            bool isProtected = SupportsProtection(service);
                            bool isCompatible = true; //await IsCompatibleVersion(service);
                            if (isProtected && isCompatible)
                            {
                                Common.RemoteService = service;
                                await Common.RemoteService.RequestAccessAsync();

                                // Create a socket and connect to the target
                                Common.RemoteSocket = new StreamSocket();
                                Common.RemoteSocket.Control.KeepAlive = true;
                                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);
                                RemoteStatusDisplay.Text = "Connecting...";

                                try
                                {
                                    await Common.RemoteSocket.ConnectAsync(
                                    Common.RemoteService.ConnectionHostName,
                                    Common.RemoteService.ConnectionServiceName,
                                    SocketProtectionLevel
                                        .BluetoothEncryptionAllowNullAuthentication);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(SocketError.GetStatus(ex.HResult));
                                }

                                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Green);
                                RemoteStatusDisplay.Text = "Connected";

                                Common.RemoteWriter = new DataWriter(Common.RemoteSocket.OutputStream);
                                DataReader RemoteReader = new DataReader(Common.RemoteSocket.InputStream);
                                
                                Common.IsConnected = true;
                                //ReceiveByteLoop(RemoteReader);
                                RemoteBackgroundSetup();

                                #region Old
                                /*var writeStream = Common.RemoteSocket.OutputStream.AsStreamForWrite();
                                byte[] send = { CMD_SETASDISPLAY, CMD_ENDMSG };
                                writeStream.Write(send, 0, send.Length);

                                var readStream = Common.RemoteSocket.InputStream.AsStreamForRead();
                                byte[] recieve = { };
                                for (int i = 1; i <= 60; i++)
                                {
                                    readStream.Read(recieve, 0, recieve.Length);
                                    if (recieve.Length <= 0)
                                    {
                                        RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.DarkRed);
                                        RemoteStatusDisplay.Text = "Communication Error: Device not responding, attempt " + i.ToString() + "/60. Retrying...";
                                        Debug.WriteLine("Remote could not interpret message.");
                                        continue;
                                    }
                                    else if (recieve[0] == CMD_RECIEVEDERROR)
                                    {
                                        RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.DarkRed);
                                        RemoteStatusDisplay.Text = "Communication Error: Device could not parse instructions";
                                        Debug.WriteLine("Remote could not interpret message.");
                                    }
                                    else if (recieve[0] == CMD_RECIEVEDOK)
                                    {
                                        isConnected = true;
                                        ConnectDeviceButton.Content = "Disconnect";
                                        RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Green);
                                        RemoteStatusDisplay.Text = "Connected to " + bluetoothDevice.Name;
                                        Common.RemoteInfo = bluetoothDevice.DeviceInformation;
                                    }
                                }
                                //#endregion
                            }
                            else if (isProtected && !isCompatible)
                            {
                                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.DarkRed);
                                RemoteStatusDisplay.Text = "Connection Failed: Device is not compatible";
                                Common.Disconnect("Error connecting");
                                return;
                            }
                            else if (!isProtected && isCompatible)
                            {
                                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.DarkRed);
                                RemoteStatusDisplay.Text = "Connection Failed: Device does not have a secure connection";
                                Common.Disconnect("Error connecting");
                                return;
                            }
                            else if (!isProtected && !isCompatible)
                            {
                                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.DarkRed);
                                RemoteStatusDisplay.Text = "Connection Failed: Device is incompatible and does not have a secure connection";
                                Common.Disconnect("Error connecting");
                                return;
                            }
                        }
                        else
                        {
                            RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.DarkRed);
                            RemoteStatusDisplay.Text = "Connection Failed: Unable to pair";
                            Common.Disconnect("Error connecting");
                        }
                    }
                    else
                    {
                        RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.DarkRed);
                        RemoteStatusDisplay.Text = "Connection Failed: Device refused access";
                        Common.Disconnect("Error connecting");
                    }
                }
                else
                {
                    RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);
                    RemoteStatusDisplay.Text = "Not Connected";
                    Common.Disconnect("Error connecting");
                }
            }*/
            #endregion
        }
    }
}

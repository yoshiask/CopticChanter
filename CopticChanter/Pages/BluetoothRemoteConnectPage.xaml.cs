//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace CopticChanter.Pages
{
    public sealed partial class BluetoothRemoteConnectPage : Page
    {
        // Used to display list of available devices to chat with
        public ObservableCollection<RfcommChatDeviceDisplay> ResultCollection {
            get;
            private set;
        }

        private DeviceWatcher _deviceWatcher = null;
        private StreamSocket _chatSocket = null;
        private DataWriter _chatWriter = null;
        private RfcommDeviceService _chatService = null;
        private BluetoothDevice _bluetoothDevice;

        public BluetoothRemoteConnectPage()
        {
            this.InitializeComponent();
            App.Current.Suspending += App_Suspending;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ResultCollection = new ObservableCollection<RfcommChatDeviceDisplay>();
            DataContext = this;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StopWatcher();
        }

        private void StopWatcher()
        {
            if (null != _deviceWatcher)
            {
                if ((DeviceWatcherStatus.Started == _deviceWatcher.Status ||
                     DeviceWatcherStatus.EnumerationCompleted == _deviceWatcher.Status))
                {
                    _deviceWatcher.Stop();
                }
                _deviceWatcher = null;
            }
        }

        void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // Make sure we clean up resources on suspend.
            Disconnect("App Suspension disconnects");
        }

        /// <summary>
        /// When the user presses the run button, query for all nearby unpaired devices
        /// Note that in this case, the other device must be running the Rfcomm Chat Server before being paired.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (_deviceWatcher == null)
            {
                SetDeviceWatcherUi();
                StartUnpairedDeviceWatcher();
            }
            else
            {
                ResetMainUi();
            }
        }

        private void SetDeviceWatcherUi()
        {
            // Disable the button while we do async operations so the user can't Run twice.
            RunButton.Content = "Stop";
            Console.WriteLine("STATUS: Device watcher started");
            resultsListView.Visibility = Visibility.Visible;
            resultsListView.IsEnabled = true;
        }

        private void ResetMainUi()
        {
            RunButton.Content = "Start";
            RunButton.IsEnabled = true;
            ConnectButton.Visibility = Visibility.Visible;
            resultsListView.Visibility = Visibility.Visible;
            resultsListView.IsEnabled = true;

            // Re-set device specific UX
            ChatBox.Visibility = Visibility.Collapsed;
            RequestAccessButton.Visibility = Visibility.Collapsed;
            if (ConversationList.Items != null) ConversationList.Items.Clear();
            StopWatcher();
        }

        private void StartUnpairedDeviceWatcher()
        {
            // Request additional properties
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            _deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);

            // Hook up handlers for the watcher events before starting the watcher
            _deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Make sure device name isn't blank
                    if (deviceInfo.Name != "")
                    {
                        ResultCollection.Add(new RfcommChatDeviceDisplay(deviceInfo));
                        Console.WriteLine("STATUS: " + 
                            String.Format("{0} devices found.", ResultCollection.Count));
                    }

                });
            });

            _deviceWatcher.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    foreach (RfcommChatDeviceDisplay rfcommInfoDisp in ResultCollection)
                    {
                        if (rfcommInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            rfcommInfoDisp.Update(deviceInfoUpdate);
                            break;
                        }
                    }
                });
            });

            _deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    Console.WriteLine("STATUS: " + 
                        String.Format("{0} devices found. Enumeration completed. Watching for updates...", ResultCollection.Count));
                });
            });

            _deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    // Find the corresponding DeviceInformation in the collection and remove it
                    foreach (RfcommChatDeviceDisplay rfcommInfoDisp in ResultCollection)
                    {
                        if (rfcommInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            ResultCollection.Remove(rfcommInfoDisp);
                            break;
                        }
                    }

                    Console.WriteLine("STATUS: " + 
                        String.Format("{0} devices found.", ResultCollection.Count));
                });
            });

            _deviceWatcher.Stopped += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    ResultCollection.Clear();
                });
            });

            _deviceWatcher.Start();
        }

        /// <summary>
        /// Invoked once the user has selected the device to connect to.
        /// Once the user has selected the device,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure user has selected a device first
            if (resultsListView.SelectedItem != null)
            {
                Console.WriteLine("STATUS: Connecting to remote device. Please wait...");
            }
            else
            {
                Console.WriteLine("ERROR: Please select an item to connect to");
                return;
            }

            RfcommChatDeviceDisplay deviceInfoDisp = resultsListView.SelectedItem as RfcommChatDeviceDisplay;

            // Perform device access checks before trying to get the device.
            // First, we check if consent has been explicitly denied by the user.
            DeviceAccessStatus accessStatus = DeviceAccessInformation.CreateFromId(deviceInfoDisp.Id).CurrentStatus;
            if (accessStatus == DeviceAccessStatus.DeniedByUser)
            {
                Console.WriteLine("ERROR: This app does not have access to connect to the remote device (please grant access in Settings > Privacy > Other Devices");
                return;
            }
            // If not, try to get the Bluetooth device
            try
            {
                _bluetoothDevice = await BluetoothDevice.FromIdAsync(deviceInfoDisp.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                ResetMainUi();
                return;
            }
            // If we were unable to get a valid Bluetooth device object,
            // it's most likely because the user has specified that all unpaired devices
            // should not be interacted with.
            if (_bluetoothDevice == null)
            {
                Console.WriteLine("ERROR: Bluetooth Device returned null. Access Status = " + accessStatus.ToString());
            }

            // This should return a list of uncached Bluetooth services (so if the server was not active when paired, it will still be detected by this call
            var rfcommServices = await _bluetoothDevice.GetRfcommServicesForIdAsync(
                RfcommServiceId.FromUuid(Common.RemoteConstants.RfcommChatServiceUuid), BluetoothCacheMode.Uncached);

            if (rfcommServices.Services.Count > 0)
            {
                _chatService = rfcommServices.Services[0];
            }
            else
            {
                Console.WriteLine("ERROR: Could not discover the chat service on the remote device");
                ResetMainUi();
                return;
            }

            // Do various checks of the SDP record to make sure you are talking to a device that actually supports the Bluetooth Rfcomm Chat Service
            var attributes = await _chatService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey(Common.RemoteConstants.SdpServiceNameAttributeId))
            {
                Console.WriteLine("ERROR: The Chat service is not advertising the Service Name attribute (attribute id=0x100). " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUi();
                return;
            }
            var attributeReader = DataReader.FromBuffer(attributes[Common.RemoteConstants.SdpServiceNameAttributeId]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != Common.RemoteConstants.SdpServiceNameAttributeType)
            {
                Console.WriteLine("ERROR: The Chat service is using an unexpected format for the Service Name attribute. " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUi();
                return;
            }
            var serviceNameLength = attributeReader.ReadByte();

            // The Service Name attribute requires UTF-8 encoding.
            attributeReader.UnicodeEncoding = UnicodeEncoding.Utf8;

            StopWatcher();

            lock (this)
            {
                _chatSocket = new StreamSocket();
            }
            try
            {
                await _chatSocket.ConnectAsync(_chatService.ConnectionHostName, _chatService.ConnectionServiceName);

                SetChatUi(attributeReader.ReadString(serviceNameLength), _bluetoothDevice.Name);
                _chatWriter = new DataWriter(_chatSocket.OutputStream);

                DataReader chatReader = new DataReader(_chatSocket.InputStream);
                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80070490) // ERROR_ELEMENT_NOT_FOUND
            {
                Console.WriteLine("ERROR: Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUi();
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072740) // WSAEADDRINUSE
            {
                Console.WriteLine("ERROR: Please verify that there is no other RFCOMM connection to the same device.");
                ResetMainUi();
            }
        }

        /// <summary>
        ///  If you believe the Bluetooth device will eventually be paired with Windows,
        ///  you might want to pre-emptively get consent to access the device.
        ///  An explicit call to RequestAccessAsync() prompts the user for consent.
        ///  If this is not done, a device that's working before being paired,
        ///  will no longer work after being paired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RequestAccessButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure user has given consent to access device
            DeviceAccessStatus accessStatus = await _bluetoothDevice.RequestAccessAsync();

            if (accessStatus != DeviceAccessStatus.Allowed)
            {
                Console.WriteLine("ERROR: Access to the device is denied because the application was not granted access");
            }
            else
            {
                Console.WriteLine("STATUS: Access granted, you are free to pair devices");
            }
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        public void KeyboardKey_Pressed(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SendMessage();
            }
        }

        /// <summary>
        /// Takes the contents of the MessageTextBox and writes it to the outgoing chatWriter
        /// </summary>
        private async void SendMessage()
        {
            try
            {
                if (MessageTextBox.Text.Length != 0)
                {
                    _chatWriter.WriteUInt32((uint)MessageTextBox.Text.Length);
                    _chatWriter.WriteString(MessageTextBox.Text);

                    ConversationList.Items.Add("Sent: " + MessageTextBox.Text);
                    MessageTextBox.Text = "";
                    await _chatWriter.StoreAsync();

                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072745)
            {
                // The remote device has disconnected the connection
                Console.WriteLine("STATUS: Remote side disconnect: " + ex.HResult.ToString() + " - " + ex.Message);
            }
        }

        private async void ReceiveStringLoop(DataReader chatReader)
        {
            try
            {
                uint size = await chatReader.LoadAsync(sizeof(uint));
                if (size < sizeof(uint))
                {
                    Disconnect("Remote device terminated connection - make sure only one instance of server is running on remote device");
                    return;
                }

                uint stringLength = chatReader.ReadUInt32();
                uint actualStringLength = await chatReader.LoadAsync(stringLength);
                if (actualStringLength != stringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data
                    return;
                }

                string message = chatReader.ReadString(stringLength);
                string outputText = "";
                switch (message)
                {
                    case Common.RemoteConstants.RemoteCmdString.CmdNext:
                        outputText = "Device requested next slide";
                        //RightButton_Click(sender, new RoutedEventArgs());
                        break;

                    case Common.RemoteConstants.RemoteCmdString.CmdPrev:
                        outputText = "Device requested previous slide";
                        //LeftButton_Click(sender, new RoutedEventArgs());
                        break;

                    case Common.RemoteConstants.RemoteCmdString.CmdSetasremote:
                        outputText = "Device requested remote";
                        break;

                    case Common.RemoteConstants.RemoteCmdString.CmdSetasdisplay:
                        outputText = "Device requested display";
                        break;

                    case Common.RemoteConstants.RemoteCmdString.CmdEndmsg:
                        // Nothing more to read, continue to listen
                        break;

                    case Common.RemoteConstants.RemoteCmdString.CmdDisconnect:
                        Disconnect("Remote requested disconnect via command");
                        break;

                    case Common.RemoteConstants.RemoteCmdString.CmdRecievedok:
                        outputText = "Device recieved and executed command without errors";
                        break;

                    case Common.RemoteConstants.RemoteCmdString.CmdRecievederror:
                        outputText = "Device failed to parse command";
                        break;

                    case Common.RemoteConstants.RemoteCmdString.CmdError:
                        outputText = "Device failed to execute command";
                        break;

                    default:
                        outputText = "Received: " + message;
                        break;
                }
                ConversationList.Items.Add(outputText);

                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (_chatSocket == null)
                    {
                        // Do not print anything here -  the user closed the socket.
                        if ((uint)ex.HResult == 0x80072745)
                            Console.WriteLine("STATUS: Disconnect triggered by remote device");
                        else if ((uint)ex.HResult == 0x800703E3)
                            Console.WriteLine("STATUS: The I/O operation has been aborted because of either a thread exit or an application request.");
                    }
                    else
                    {
                        Disconnect("Read stream failed with error: " + ex.Message);
                    }
                }
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect("Disconnected");
        }


        /// <summary>
        /// Cleans up the socket and DataWriter and reset the UI
        /// </summary>
        /// <param name="disconnectReason"></param>
        private void Disconnect(string disconnectReason)
        {
            if (_chatWriter != null)
            {
                _chatWriter.DetachStream();
                _chatWriter = null;
            }


            if (_chatService != null)
            {
                _chatService.Dispose();
                _chatService = null;
            }
            lock (this)
            {
                if (_chatSocket != null)
                {
                    _chatSocket.Dispose();
                    _chatSocket = null;
                }
            }

            Console.WriteLine(disconnectReason);
            ResetMainUi();
        }

        private void SetChatUi(string serviceName, string deviceName)
        {
            Console.WriteLine("STATUS: Connected");
            ServiceName.Text = "Service Name: " + serviceName;
            DeviceName.Text = "Connected to: " + deviceName;
            RunButton.IsEnabled = false;
            ConnectButton.Visibility = Visibility.Collapsed;
            RequestAccessButton.Visibility = Visibility.Visible;
            resultsListView.IsEnabled = false;
            resultsListView.Visibility = Visibility.Collapsed;
            ChatBox.Visibility = Visibility.Visible;
        }

        private void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePairingButtons();
        }

        private void UpdatePairingButtons()
        {
            RfcommChatDeviceDisplay deviceDisp = (RfcommChatDeviceDisplay)resultsListView.SelectedItem;

            if (null != deviceDisp)
            {
                ConnectButton.IsEnabled = true;
            }
            else
            {
                ConnectButton.IsEnabled = false;
            }
        }
    }

    public class RfcommChatDeviceDisplay : INotifyPropertyChanged
    {
        private DeviceInformation _deviceInfo;

        public RfcommChatDeviceDisplay(DeviceInformation deviceInfoIn)
        {
            _deviceInfo = deviceInfoIn;
            UpdateGlyphBitmapImage();
        }

        public DeviceInformation DeviceInformation {
            get {
                return _deviceInfo;
            }

            private set {
                _deviceInfo = value;
            }
        }

        public string Id {
            get {
                return _deviceInfo.Id;
            }
        }

        public string Name {
            get {
                return _deviceInfo.Name;
            }
        }

        public BitmapImage GlyphBitmapImage {
            get;
            private set;
        }

        public void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            _deviceInfo.Update(deviceInfoUpdate);
            UpdateGlyphBitmapImage();
        }

        private async void UpdateGlyphBitmapImage()
        {
            DeviceThumbnail deviceThumbnail = await _deviceInfo.GetGlyphThumbnailAsync();
            BitmapImage glyphBitmapImage = new BitmapImage();
            await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
            GlyphBitmapImage = glyphBitmapImage;
            OnPropertyChanged("GlyphBitmapImage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
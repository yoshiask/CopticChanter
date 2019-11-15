using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BluetoothRemoteConnectPage : Page
    {
        private bool Client = true;

        #region Shared
        public BluetoothRemoteConnectPage()
        {
            this.InitializeComponent();
            Common.bluetoothRemoteConnectPage = this;
            App.Current.Suspending += App_Suspending;
            trigger = new RfcommConnectionTrigger();

            // Local service Id is the only mandatory field that should be used to filter a known service UUID.  
            trigger.InboundConnection.LocalServiceId = RfcommServiceId.FromUuid(Constants.RfcommChatServiceUuid);

            // The SDP record is nice in order to populate optional name and description fields
            trigger.InboundConnection.SdpRecord = sdpRecordBlob.AsBuffer();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Client)
            {
                ResultCollection = new ObservableCollection<RfcommChatDeviceDisplay>();
                DataContext = this;
            }
            else
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        AttachProgressAndCompletedHandlers(task.Value);
                    }
                }
            }
        }
        #endregion

        #region Server
        // The background task registration for the background advertisement watcher 
        private IBackgroundTaskRegistration taskRegistration;
        // The watcher trigger used to configure the background task registration 
        private RfcommConnectionTrigger trigger;
        // A name is given to the task in order for it to be identifiable across context. 
        private string taskName = "CCUWPRemoteService";
        // Entry point for the background task. 
        private string taskEntryPoint = "UWPRemoteTask.RfcommServerTask";


        // Define the raw bytes that are converted into SDP record
        private byte[] sdpRecordBlob = new byte[]
        {
            0x35, 0x4a,  // DES len = 74 bytes

            // Vol 3 Part B 5.1.15 ServiceName
            // 34 bytes
            0x09, 0x01, 0x00, // UINT16 (0x09) value = 0x0100 [ServiceName]
            0x25, 0x1d,       // TextString (0x25) len = 29 bytes
                0x42, 0x6c, 0x75, 0x65, 0x74, 0x6f, 0x6f, 0x74, 0x68, 0x20,     // Bluetooth <sp>
                0x52, 0x66, 0x63, 0x6f, 0x6d, 0x6d, 0x20,                       // Rfcomm <sp>
                0x43, 0x68, 0x61, 0x74, 0x20,                                   // Chat <sp>
                0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x65,                       // Service <sp>
            // Vol 3 Part B 5.1.15 ServiceDescription
            // 40 bytes
            0x09, 0x01, 0x01, // UINT16 (0x09) value = 0x0101 [ServiceDescription]
            0x25, 0x23,       // TextString (0x25) = 33 bytes,
                0x42, 0x6c, 0x75, 0x65, 0x74, 0x6f, 0x6f, 0x74, 0x68, 0x20,     // Bluetooth <sp>
                0x52, 0x66, 0x63, 0x6f, 0x6d, 0x6d, 0x20,                       // Rfcomm <sp>
                0x43, 0x68, 0x61, 0x74, 0x20,                                   // Chat <sp>
                0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x65, 0x20,                  // Service <sp>
                0x69, 0x6e, 0x20, 0x43, 0x23                                    // in C#

        };

        /// <summary>
        /// Class containing Attributes and UUIDs that will populate the SDP record.
        /// </summary>
        class Constants
        {
            // The Chat Server's custom service Uuid: 34B1CF4D-1069-4AD6-89B6-E161D79BE4D8
            // "636F7074" spells "copt"
            public static readonly Guid RfcommChatServiceUuid = Guid.Parse("636F7074-1069-4AD6-89B6-E161D79BE4D8");

            // The Id of the Service Name SDP attribute
            public const UInt16 SdpServiceNameAttributeId = 0x100;

            // The SDP Type of the Service Name SDP attribute.
            // The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
            //    -  the Attribute Type size in the least significant 3 bits,
            //    -  the SDP Attribute Type value in the most significant 5 bits.
            public const byte SdpServiceNameAttributeType = (4 << 3) | 5;

            // The value of the Service Name SDP attribute
            public const string SdpServiceName = "Bluetooth CCRemote Service";
            //public const string SdpServiceName = "Bluetooth Rfcomm Chat Service";
        }

        private async void ListenButton_Click(object sender, RoutedEventArgs e)
        {
            ListenButton.IsEnabled = false;
            DisconnectButton.IsEnabled = true;

            // Registering a background trigger if it is not already registered. Rfcomm Chat Service will now be advertised in the SDP record
            // First get the existing tasks to see if we already registered for it

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    taskRegistration = task.Value;
                    break;
                }
            }

            if (taskRegistration != null)
            {
                Debug.WriteLine("Background watcher already registered.");
                return;
            }
            else
            {
                // Applications registering for background trigger must request for permission.
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                var builder = new BackgroundTaskBuilder();
                builder.TaskEntryPoint = taskEntryPoint;
                builder.SetTrigger(trigger);
                builder.Name = taskName;

                try
                {
                    taskRegistration = builder.Register();
                    AttachProgressAndCompletedHandlers(taskRegistration);

                    // Even though the trigger is registered successfully, it might be blocked. Notify the user if that is the case.
                    if ((backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed) || (backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy))
                    {
                        Debug.WriteLine("Background watcher registered.");
                    }
                    else
                    {
                        Debug.WriteLine("Background tasks may be disabled for this app");
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Background task not registered");
                }
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(MessageTextBox.Text);
        }

        public void KeyboardKey_Pressed(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Left)
            {
                SendMessage(Common.RemoteCMDString.CMD_PREV);
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                SendMessage(Common.RemoteCMDString.CMD_NEXT);
            }
            else if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SendMessage(MessageTextBox.Text);
            }
        }

        /// <summary>
        /// Sends the current message in MessageTextBox.  Also makes sure the text is not empty and updates the conversation list.  
        /// </summary>
        public void SendMessage(string message)
        {
            var previousMessage = (string)ApplicationData.Current.LocalSettings.Values["SendMessage"];

            // Make sure previous message has been sent
            if (previousMessage == null || previousMessage == "")
            {
                // Save the current message to local settings so the background task can pick it up. 
                ApplicationData.Current.LocalSettings.Values["SendMessage"] = message;

                // Clear the messageTextBox for a new message
                MessageTextBox.Text = "";
                ConversationListBox.Items.Add("Sent: " + message);
            }
            else
            {
                // Do nothing until previous message has been sent.  
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        /// <summary>
        /// Called when background task defferal is completed.  This can happen for a number of reasons (both expected and unexpected).  
        /// IF this is expected, we'll notify the user.  If it's not, we'll show that this is an error.  Finally, clean up the connection by calling Disconnect().
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey("TaskCancelationReason"))
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                Debug.WriteLine("Task cancelled unexpectedly - reason: " + settings.Values["TaskCancelationReason"].ToString());
                });
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Debug.WriteLine("Background task completed");
                });
            }
            try
            {
                args.CheckResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Disconnect();
        }

        /// <summary>
        /// Handles UX changes and task registration changes when socket is disconnected
        /// </summary>
        private async void Disconnect()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListenButton.IsEnabled = true;
                DisconnectButton.IsEnabled = false;
                ConversationListBox.Items.Clear();

                SendMessage(Common.RemoteCMDString.CMD_DISCONNECT);

                // Unregistering the background task will remove the Rfcomm Chat Service from the SDP record and stop listening for incoming connections
                // First get the existing tasks to see if we already registered for it
                if (taskRegistration != null)
                {
                    taskRegistration.Unregister(true);
                    taskRegistration = null;
                    Debug.WriteLine("Background watcher unregistered.");
                }
                else
                {
                    // At this point we assume we haven't found any existing tasks matching the one we want to unregister
                    Debug.WriteLine("No registered background watcher found.");
                }
            });

        }

        /// <summary>
        /// The background task updates the progress counter.  When that happens, this event handler gets invoked
        /// When the handler is invoked, we will display the value stored in local settings to the user.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="args"></param>
        private async void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
        {

            if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("ReceivedMessage"))
            {
                string backgroundMessage = (string)ApplicationData.Current.LocalSettings.Values["ReceivedMessage"];
                string remoteDeviceName = (string)ApplicationData.Current.LocalSettings.Values["RemoteDeviceName"];

                if (!backgroundMessage.Equals(""))
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Debug.WriteLine("Client Connected: " + remoteDeviceName);
                        ConversationListBox.Items.Add("Received: " + backgroundMessage);
                    });
                }
            }
        }

        private void AttachProgressAndCompletedHandlers(IBackgroundTaskRegistration task)
        {
            task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }
        #endregion

        #region Client
        // Used to display list of available devices to chat with
        public ObservableCollection<RfcommChatDeviceDisplay> ResultCollection {
            get;
            private set;
        }

        private DeviceWatcher deviceWatcher = null;
        private StreamSocket chatSocket = null;
        private DataWriter chatWriter = null;
        private RfcommDeviceService chatService = null;
        private BluetoothDevice bluetoothDevice;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StopWatcher();
        }

        private void StopWatcher()
        {
            if (null != deviceWatcher)
            {
                if ((DeviceWatcherStatus.Started == deviceWatcher.Status ||
                     DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status))
                {
                    deviceWatcher.Stop();
                }
                deviceWatcher = null;
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
            if (deviceWatcher == null)
            {
                SetDeviceWatcherUI();
                StartUnpairedDeviceWatcher();
            }
            else
            {
                ResetMainUI();
            }
        }

        private void SetDeviceWatcherUI()
        {
            // Disable the button while we do async operations so the user can't Run twice.
            RunButton.Content = "Stop";
            Debug.WriteLine("Device watcher started");
            resultsListView.Visibility = Visibility.Visible;
            resultsListView.IsEnabled = true;
        }

        private void ResetMainUI()
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

            deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);

            // Hook up handlers for the watcher events before starting the watcher
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await Common.bluetoothRemoteConnectPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Make sure device name isn't blank
                    if (deviceInfo.Name != "")
                    {
                        ResultCollection.Add(new RfcommChatDeviceDisplay(deviceInfo));
                        Debug.WriteLine(
                            String.Format("{0} devices found.", ResultCollection.Count));
                    }

                });
            });

            deviceWatcher.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                await Common.bluetoothRemoteConnectPage.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
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

            deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await Common.bluetoothRemoteConnectPage.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    Debug.WriteLine(
                        String.Format("{0} devices found. Enumeration completed. Watching for updates...", ResultCollection.Count));
                });
            });

            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await Common.bluetoothRemoteConnectPage.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
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

                    Debug.WriteLine(
                        String.Format("{0} devices found.", ResultCollection.Count));
                });
            });

            deviceWatcher.Stopped += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await Common.bluetoothRemoteConnectPage.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    ResultCollection.Clear();
                });
            });

            deviceWatcher.Start();
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
                Debug.WriteLine("Connecting to remote device. Please wait...");
            }
            else
            {
                Debug.WriteLine("Please select an item to connect to");
                return;
            }

            RfcommChatDeviceDisplay deviceInfoDisp = resultsListView.SelectedItem as RfcommChatDeviceDisplay;

            // Perform device access checks before trying to get the device.
            // First, we check if consent has been explicitly denied by the user.
            DeviceAccessStatus accessStatus = DeviceAccessInformation.CreateFromId(deviceInfoDisp.Id).CurrentStatus;
            if (accessStatus == DeviceAccessStatus.DeniedByUser)
            {
                Debug.WriteLine("This app does not have access to connect to the remote device (please grant access in Settings > Privacy > Other Devices");
                return;
            }
            // If not, try to get the Bluetooth device
            try
            {
                bluetoothDevice = await BluetoothDevice.FromIdAsync(deviceInfoDisp.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ResetMainUI();
                return;
            }
            // If we were unable to get a valid Bluetooth device object,
            // it's most likely because the user has specified that all unpaired devices
            // should not be interacted with.
            if (bluetoothDevice == null)
            {
                Debug.WriteLine("Bluetooth Device returned null. Access Status = " + accessStatus.ToString());
            }

            // This should return a list of uncached Bluetooth services (so if the server was not active when paired, it will still be detected by this call
            var rfcommServices = await bluetoothDevice.GetRfcommServicesForIdAsync(
                RfcommServiceId.FromUuid(Constants.RfcommChatServiceUuid), BluetoothCacheMode.Uncached);

            if (rfcommServices.Services.Count > 0)
            {
                chatService = rfcommServices.Services[0];
            }
            else
            {
                Debug.WriteLine(
                   "Could not discover the chat service on the remote device");
                ResetMainUI();
                return;
            }

            // Do various checks of the SDP record to make sure you are talking to a device that actually supports the Bluetooth Rfcomm Chat Service
            var attributes = await chatService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey(Constants.SdpServiceNameAttributeId))
            {
                Debug.WriteLine(
                    "The Chat service is not advertising the Service Name attribute (attribute id=0x100). " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUI();
                return;
            }
            var attributeReader = DataReader.FromBuffer(attributes[Constants.SdpServiceNameAttributeId]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != Constants.SdpServiceNameAttributeType)
            {
                Debug.WriteLine(
                    "The Chat service is using an unexpected format for the Service Name attribute. " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUI();
                return;
            }
            var serviceNameLength = attributeReader.ReadByte();

            // The Service Name attribute requires UTF-8 encoding.
            attributeReader.UnicodeEncoding = UnicodeEncoding.Utf8;

            StopWatcher();

            lock (this)
            {
                chatSocket = new StreamSocket();
            }
            try
            {
                await chatSocket.ConnectAsync(chatService.ConnectionHostName, chatService.ConnectionServiceName);

                SetChatUI(attributeReader.ReadString(serviceNameLength), bluetoothDevice.Name);
                chatWriter = new DataWriter(chatSocket.OutputStream);

                DataReader chatReader = new DataReader(chatSocket.InputStream);
                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80070490) // ERROR_ELEMENT_NOT_FOUND
            {
                Debug.WriteLine("Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUI();
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072740) // WSAEADDRINUSE
            {
                Debug.WriteLine("Please verify that there is no other RFCOMM connection to the same device.");
                ResetMainUI();
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
            DeviceAccessStatus accessStatus = await bluetoothDevice.RequestAccessAsync();

            if (accessStatus != DeviceAccessStatus.Allowed)
            {
                Debug.WriteLine(
                    "Access to the device is denied because the application was not granted access");
            }
            else
            {
                Debug.WriteLine(
                                    "Access granted, you are free to pair devices");
            }
        }

        /// <summary>
        /// Takes the contents of the MessageTextBox and writes it to the outgoing chatWriter
        /// </summary>
        private async void SendMessage()
        {
            SendMessage(MessageTextBox.Text);
            return;

            try
            {
                if (MessageTextBox.Text.Length != 0)
                {
                    chatWriter.WriteUInt32((uint)MessageTextBox.Text.Length);
                    chatWriter.WriteString(MessageTextBox.Text);

                    ConversationList.Items.Add("Sent: " + MessageTextBox.Text);
                    MessageTextBox.Text = "";
                    await chatWriter.StoreAsync();

                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072745)
            {
                // The remote device has disconnected the connection
                Debug.WriteLine("Remote side disconnect: " + ex.HResult.ToString() + " - " + ex.Message);
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

                ConversationList.Items.Add("Received: " + chatReader.ReadString(stringLength));

                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (chatSocket == null)
                    {
                        // Do not print anything here -  the user closed the socket.
                        if ((uint)ex.HResult == 0x80072745)
                            Debug.WriteLine("Disconnect triggered by remote device");
                        else if ((uint)ex.HResult == 0x800703E3)
                            Debug.WriteLine("The I/O operation has been aborted because of either a thread exit or an application request.");
                    }
                    else
                    {
                        Disconnect("Read stream failed with error: " + ex.Message);
                    }
                }
            }
        }


        /// <summary>
        /// Cleans up the socket and DataWriter and reset the UI
        /// </summary>
        /// <param name="disconnectReason"></param>
        private void Disconnect(string disconnectReason)
        {
            if (chatWriter != null)
            {
                chatWriter.DetachStream();
                chatWriter = null;
            }


            if (chatService != null)
            {
                chatService.Dispose();
                chatService = null;
            }
            lock (this)
            {
                if (chatSocket != null)
                {
                    chatSocket.Dispose();
                    chatSocket = null;
                }
            }

            Debug.WriteLine(disconnectReason);
            ResetMainUI();
        }

        private void SetChatUI(string serviceName, string deviceName)
        {
            Debug.WriteLine("Connected");
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
        #endregion
    }

    public class RfcommChatDeviceDisplay : INotifyPropertyChanged
    {
        private DeviceInformation deviceInfo;

        public RfcommChatDeviceDisplay(DeviceInformation deviceInfoIn)
        {
            deviceInfo = deviceInfoIn;
            UpdateGlyphBitmapImage();
        }

        public DeviceInformation DeviceInformation {
            get {
                return deviceInfo;
            }

            private set {
                deviceInfo = value;
            }
        }

        public string Id {
            get {
                return deviceInfo.Id;
            }
        }

        public string Name {
            get {
                return deviceInfo.Name;
            }
        }

        public BitmapImage GlyphBitmapImage {
            get;
            private set;
        }

        public void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            deviceInfo.Update(deviceInfoUpdate);
            UpdateGlyphBitmapImage();
        }

        private async void UpdateGlyphBitmapImage()
        {
            DeviceThumbnail deviceThumbnail = await deviceInfo.GetGlyphThumbnailAsync();
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

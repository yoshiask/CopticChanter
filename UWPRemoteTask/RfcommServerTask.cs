using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth.Background;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.Networking.Sockets;
using Windows.Devices.Bluetooth;

namespace UWPRemoteTask
{
    // A background task always implements the IBackgroundTask interface.
    public sealed class RfcommServerTask : IBackgroundTask
    {
        // Networking
        private StreamSocket _socket = null;
        private DataReader _reader = null;
        private DataWriter _writer = null;
        private BluetoothDevice _remoteDevice = null;

        private BackgroundTaskDeferral _deferral = null;
        private IBackgroundTaskInstance _taskInstance = null;
        private BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        private bool _cancelRequested = false;

        ThreadPoolTimer _periodicTimer = null;
        /// <summary>
        /// The entry point of a background task.
        /// </summary>
        /// <param name="taskInstance">The current background task instance.</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the deferral to prevent the task from closing prematurely
            _deferral = taskInstance.GetDeferral();

            // Setup our onCanceled callback and progress
            this._taskInstance = taskInstance;
            this._taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
            this._taskInstance.Progress = 0;

            // Store a setting so that the app knows that the task is running. 
            ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"] = true;

            _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(1));

            try
            {
                RfcommConnectionTriggerDetails details = (RfcommConnectionTriggerDetails)taskInstance.TriggerDetails;
                if (details != null)
                {
                    _socket = details.Socket;
                    _remoteDevice = details.RemoteDevice;
                    ApplicationData.Current.LocalSettings.Values["RemoteDeviceName"] = _remoteDevice.Name;

                    _writer = new DataWriter(_socket.OutputStream);
                    _reader = new DataReader(_socket.InputStream);
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values["BackgroundTaskStatus"] = "Trigger details returned null";
                    _deferral.Complete();
                }

                var result = await ReceiveDataAsync();
            }
            catch (Exception ex)
            {
                _reader = null;
                _writer = null;
                _socket = null;
                _deferral.Complete();

                Debug.WriteLine("Exception occurred while initializing the connection, hr = " + ex.HResult.ToString("X"));
            }
        }

        private void OnCanceled(IBackgroundTaskInstance taskInstance, BackgroundTaskCancellationReason reason)
        {
            _cancelReason = reason;
            _cancelRequested = true;

            ApplicationData.Current.LocalSettings.Values["TaskCancelationReason"] = _cancelReason.ToString();
            ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"] = false;
            ApplicationData.Current.LocalSettings.Values["ReceivedMessage"] = "";
            // Complete the background task (this raises the OnCompleted event on the corresponding BackgroundTaskRegistration). 
            _deferral.Complete();
        }

        private async Task<int> ReceiveDataAsync()
        {
            while (true)
            {
                uint readLength = await _reader.LoadAsync(sizeof(uint));
                if (readLength < sizeof(uint))
                {
                    ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"] = false;
                    // Complete the background task (this raises the OnCompleted event on the corresponding BackgroundTaskRegistration). 
                    _deferral.Complete();
                }
                uint currentLength = _reader.ReadUInt32();

                readLength = await _reader.LoadAsync(currentLength);
                if (readLength < currentLength)
                {
                    ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"] = false;
                    // Complete the background task (this raises the OnCompleted event on the corresponding BackgroundTaskRegistration). 
                    _deferral.Complete();
                }
                string message = _reader.ReadString(currentLength);

                ApplicationData.Current.LocalSettings.Values["ReceivedMessage"] = message;
                _taskInstance.Progress += 1;
            }
        }

        /// <summary>
        /// Periodically check if there's a new message and if there is, send it over the socket 
        /// </summary>
        /// <param name="timer"></param>
        private async void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if (!_cancelRequested)
            {
                string message = (string)ApplicationData.Current.LocalSettings.Values["SendMessage"];
                if (!string.IsNullOrEmpty(message))
                {
                    try
                    {
                        // Make sure that the connection is still up and there is a message to send
                        if (_socket != null)
                        {
                            _writer.WriteUInt32((uint)message.Length);
                            _writer.WriteString(message);
                            await _writer.StoreAsync();

                            ApplicationData.Current.LocalSettings.Values["SendMessage"] = null;
                        }
                        else
                        {
                            _cancelReason = BackgroundTaskCancellationReason.ConditionLoss;
                            _deferral.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationData.Current.LocalSettings.Values["TaskCancelationReason"] = ex.Message;
                        ApplicationData.Current.LocalSettings.Values["SendMessage"] = null;
                        _deferral.Complete();
                    }
                }
            }
            else
            {
                // Timer clean up
                _periodicTimer.Cancel();
                //
                // Write to LocalSettings to indicate that this background task ran.
                //
                ApplicationData.Current.LocalSettings.Values["TaskCancelationReason"] = _cancelReason.ToString();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage.Streams;
using CopticChanter;

namespace CoptLib
{
    public static class RemoteCommunicator
    {
        /// <summary>
        /// Creates a loop that adds recieved bytes to BytesRecieved
        /// </summary>
        /// <param name="RemoteReader"></param>
        private static async void ReceiveStringLoop(DataReader RemoteReader)
        {
            try
            {
                uint size = await RemoteReader.LoadAsync(sizeof(uint));
                if (size < sizeof(uint))
                {
                    Common.Disconnect("Remote device terminated connection - make sure only one instance of server is running on remote device");
                    return;
                }

                uint stringLength = RemoteReader.ReadUInt32();
                uint actualStringLength = await RemoteReader.LoadAsync(stringLength);
                if (actualStringLength != stringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data
                    return;
                }

                RemoteReader.ReadString(stringLength);
                Common.RecievedCMD.Add(RemoteReader.ReadString(stringLength));

                ReceiveStringLoop(RemoteReader);
            }
            catch (Exception ex)
            {
                //lock (this)
                //{
                if (Common.RemoteSocket == null)
                {
                    // Do not print anything here -  the user closed the socket.
                    if ((uint)ex.HResult == 0x80072745)
                        Debug.WriteLine("Disconnect triggered by remote device");
                    else if ((uint)ex.HResult == 0x800703E3)
                        Debug.WriteLine("The I/O operation has been aborted because of either a thread exit or an application request.");
                }
                else
                {
                    Common.Disconnect("Read stream failed with error: " + ex.Message);
                }
                //}
            }
        }

        public static void Start(DataReader RemoteReader, IBackgroundTaskInstance task)
        {
            ReceiveStringLoop(RemoteReader);
        }
    }
}

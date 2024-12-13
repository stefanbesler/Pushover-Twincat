using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads.Server;
using TwinCAT.Ads;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace Pushover.Twincat
{
    /*
     * Extend the TcAdsServer class to implement ADS Server
     */
    public class Server : AdsServer
    {

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Ansi)]
        private struct AdsHttpRequest
        {
            public ushort TokenLength;
            public ushort UserLength;
            public ushort TitleLength;
            public ushort MessageLength;

            // todo: maybe make the size dynamic
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
            public byte[] Token;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
            public byte[] User;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Title;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Message;
        }

        const string pushoverServerUrl = "https://api.pushover.net/1/messages.json";

        public Server(ushort port, string portName, ILogger logger) : base(port, portName, logger)
        {
            
        }

        ~Server()
        {
        }


        protected override void OnConnected()
        {
        }

        protected override async Task<ResultReadWriteBytes> OnReadWriteAsync(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, int readLength, ReadOnlyMemory<byte> writeData, CancellationToken cancel)
        {
            ResultReadWriteBytes result = ResultReadWriteBytes.CreateError(AdsErrorCode.DeviceServiceNotSupported);

            // use index group (and offset) to distinguish between the servicesof this server
            if (indexGroup + indexOffset == 0)
            {
                IntPtr ptr = Marshal.AllocHGlobal(writeData.Length);
                byte[] arr = new byte[writeData.Length];
                writeData.CopyTo(arr);

                AdsHttpRequest request = new AdsHttpRequest();
                Marshal.Copy(arr, 0, ptr, Marshal.SizeOf(request));
                request = Marshal.PtrToStructure<AdsHttpRequest>(ptr);

                using (HttpClient client = new HttpClient())
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        ["token"] = Encoding.ASCII.GetString(request.Token.AsSpan(0, request.TokenLength).ToArray()),
                        ["user"] = Encoding.ASCII.GetString(request.User.AsSpan(0, request.UserLength).ToArray()),
                        ["title"] = Encoding.ASCII.GetString(request.Title.AsSpan(0, request.TitleLength).ToArray()),
                        ["message"] = Encoding.ASCII.GetString(request.Message.AsSpan(0, request.MessageLength).ToArray())
                    };

                    try
                    {
                        var response = await client.PostAsync(pushoverServerUrl, new FormUrlEncodedContent(parameters));

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            result = ResultReadWriteBytes.CreateSuccess(null);
                        else
                            result = ResultReadWriteBytes.CreateError((AdsErrorCode)(0xFFFF + response.StatusCode));
                    }
                    catch(Exception ex)
                    {
                        result = ResultReadWriteBytes.CreateError((AdsErrorCode)(0xFFFF));
                    }
                }
            }

            return result;
        }
    }
}

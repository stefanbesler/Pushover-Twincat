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
            public ushort DeviceLength;
            public ushort UrlLength;
            public ushort UrlTitleLength;
            public ushort CallbackLength;
            public ushort ProxyHostLength;
            public ushort Retry;
            public ushort Expire;
            public ushort MessageType;
            public short Priority;
            public ushort Sound;
            public ushort Timestamp;
            public ushort TimeToLive;
            public ushort ProxyPort;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
            public byte[] Token;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
            public byte[] User;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
            public byte[] Device;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] Url;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] UrlTitle;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Callback;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
            public byte[] ProxyHost;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 250)]
            public byte[] Title;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] Message;
        }

        const string _pushoverServerUrl = "https://api.pushover.net/1/messages.json";

        Dictionary<int, string> _sounds = new Dictionary<int, string>
        {
            { 0, "pushover" },
            { 1, "bike" },
            { 2, "bugle" },
            { 3, "cashregister" },
            { 4, "classical" },
            { 5, "cosmic" },
            { 6,  "falling" },
            { 7,  "gamelan" },
            { 8, "incoming" },
            { 9, "intermission" },
            { 10, "magic" },
            { 11, "mechanical" },
            { 12, "pianobar" },
            { 13, "siren" },
            { 14, "spacealarm" },
            { 15, "tugboat" },
            { 16, "alien" },
            { 17, "climb" },
            { 18, "persistent" },
            { 19, "echo" },
            { 20, "updown" },
            { 21, "vibrate" },
            { 22, "none" },
        };

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

                    if (request.CallbackLength > 0)
                        parameters.Add("callback", Encoding.ASCII.GetString(request.Callback.AsSpan(0, request.CallbackLength).ToArray()));

                    if (request.DeviceLength > 0)
                        parameters.Add("device", Encoding.ASCII.GetString(request.Device.AsSpan(0, request.DeviceLength).ToArray()));

                    if (request.UrlLength > 0)
                        parameters.Add("url", Encoding.ASCII.GetString(request.Url.AsSpan(0, request.UrlLength).ToArray()));

                    if (request.UrlTitleLength > 0)
                        parameters.Add("url_title", Encoding.ASCII.GetString(request.UrlTitle.AsSpan(0, request.UrlTitleLength).ToArray()));

                    if (request.Expire > 0)
                        parameters.Add("expire", request.Expire.ToString());

                    if (request.MessageType > 0)
                    {
                        parameters.Add("html", request.MessageType == 1 ? "1" : "0");
                        parameters.Add("monospace", request.MessageType == 1 ? "0" : "1");
                    }

                    if (request.Priority != 0)
                        parameters.Add("priority", request.Priority.ToString());

                    if (request.Priority == 2)
                    {
                        parameters.Add("retry", "60");
                        parameters.Add("expire", "3600");
                    }

                    if (request.Retry > 0)
                        parameters.Add("retry", request.Retry.ToString());

                    if (request.Sound > 0)
                        parameters.Add("sound", _sounds.TryGetValue(request.Sound, out string s) ? s : "pushover");

                    if (request.Timestamp > 0)
                        parameters.Add("timestamp", request.Timestamp.ToString());

                    if (request.TimeToLive > 0)
                        parameters.Add("ttl", request.TimeToLive.ToString());

                    try
                    {
                        var response = await client.PostAsync(_pushoverServerUrl, new FormUrlEncodedContent(parameters));

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            result = ResultReadWriteBytes.CreateSuccess(null);
                        else
                            result = ResultReadWriteBytes.CreateError((AdsErrorCode)(0xFFFF + response.StatusCode));
                    }
                    catch (Exception ex)
                    {
                        result = ResultReadWriteBytes.CreateError((AdsErrorCode)(0xFFFF));
                    }
                }
            }

            return result;
        }
    }
}

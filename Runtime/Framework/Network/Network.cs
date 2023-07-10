using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Threading;
using System.Net.Http;
using System.Net.WebSockets;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.Networking;

using GhysX.Framework.Utilities;

namespace GhysX.Framework.Network
{
    public class PropertyChangedEventArgs : EventArgs
    {
        public string name;
        public object value;
    }

    public static class NetworkProxy
    {
        public static byte[] DesKey { get; set; }
        public static byte[] AesKey { get; set; }

        public static void Auth(BestHTTP.HTTPRequest request, string privateKey, string publicKey)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(publicKey);

            string modulus = doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText;
            string exponent = doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText;

            string data = modulus + "+" + exponent;

            string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
            byte[] dataBuffer = Encoding.UTF8.GetBytes(base64Data);

            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(dataBuffer, 10, 10);
            memoryStream.Write(dataBuffer, 0, 10);
            memoryStream.Write(dataBuffer, 20, dataBuffer.Length - 20);

            request.RawData = memoryStream.ToArray();
            request.Send();
        }

        public static string Decrypt(BestHTTP.HTTPRequest originalRequest, BestHTTP.HTTPResponse response, byte[] key)
        {
            return Encoding.UTF8.GetString(ZipHelper.GZipDeCompress(AESHelper.Decrypt(response.Data, AesKey)));
        }

        public static void Send(BestHTTP.HTTPRequest originalRequest, string data, byte[] key)
        {
            originalRequest.RawData = AESHelper.Encrypt(ZipHelper.GZipCompress(Encoding.UTF8.GetBytes(data)), AesKey);
            originalRequest.Send();
        }

        //~~ public static void WSOnBinaryRecv(LuaTable luaTable, string messageFunctionName, string errorFunctionName, BestHTTP.WebSocket.WebSocket webSocket)
        // {
        //     Action<BestHTTP.WebSocket.WebSocket, string> messageFunc = luaTable.Get<Action<BestHTTP.WebSocket.WebSocket, string>>(messageFunctionName);
        //     Action<BestHTTP.WebSocket.WebSocket, Exception> errorFunc = luaTable.Get<Action<BestHTTP.WebSocket.WebSocket, Exception>>(errorFunctionName);
        //     webSocket.OnBinary += (BestHTTP.WebSocket.WebSocket webSocket, byte[] data) =>
        //     {
        //         try
        //         {
        //             string message = Encoding.UTF8.GetString(ZipHelper.GZipDeCompress(AESHelper.Decrypt(data, AesKey)));
        //             messageFunc?.Invoke(webSocket, message);
        //         }
        //         catch (Exception e)
        //         {
        //             errorFunc?.Invoke(webSocket, e);
        //         }
        //     };
        // }
        //
        // public static void WSSend(BestHTTP.WebSocket.WebSocket webSocket, string data, byte[] key)
        // {
        //     webSocket.Send(AESHelper.Encrypt(ZipHelper.GZipCompress(Encoding.UTF8.GetBytes(data)), AesKey));
        // }
    }
}
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace GhysX.Framework.Network
{
    public enum DisType
    {
        Exception,
        Disconnect,
    }

    public class SocketClient
    {
        private TcpClient client = null;
        private NetworkStream outStream = null;
        private MemoryStream memStream;
        private BinaryReader reader;

        private const int MAX_READ = 8192;
        private byte[] byteBuffer = new byte[MAX_READ];
        public static bool loggedIn = false;

        public SocketClient()
        {
        }

        public void OnRegister()
        {
            memStream = new MemoryStream();
            reader = new BinaryReader(memStream);
        }

        public void OnRemove()
        {
            this.Close();
            reader.Close();
            memStream.Close();
        }

        void ConnectServer(string host, int port)
        {
            client = null;
            try
            {
                IPAddress[] address = Dns.GetHostAddresses(host);
                if (address.Length == 0)
                {
                    Debug.LogError("host invalid");
                    return;
                }
                if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
                {
                    client = new TcpClient(AddressFamily.InterNetworkV6);
                }
                else
                {
                    client = new TcpClient(AddressFamily.InterNetwork);
                }
                client.SendTimeout = 1000;
                client.ReceiveTimeout = 1000;
                client.NoDelay = true;
                client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
            }
            catch (Exception e)
            {
                Close(); Debug.LogError(e.Message);
            }
        }

        void OnConnect(IAsyncResult asr)
        {
            outStream = client.GetStream();
            client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            NetworkManager.AddEvent(Protocal.Connect, new ByteBuffer());
        }

        void WriteMessage(byte[] message)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)message.Length;
                writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                if (client != null && client.Connected)
                {
                    byte[] payload = ms.ToArray();
                    outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
                }
                else
                {
                    Debug.LogError("client.connected----->>false");
                }
            }
        }

        void OnRead(IAsyncResult asr)
        {
            int bytesRead = 0;
            try
            {
                lock (client.GetStream())
                {
                    bytesRead = client.GetStream().EndRead(asr);
                }
                if (bytesRead < 1)
                {
                    OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                    return;
                }
                OnReceive(byteBuffer, bytesRead);
                lock (client.GetStream())
                {
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);
                    client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
                }
            }
            catch (Exception ex)
            {
                OnDisconnected(DisType.Exception, ex.Message);
            }
        }

        void OnDisconnected(DisType dis, string msg)
        {
            Close();
            int protocal = dis == DisType.Exception ?
            Protocal.Exception : Protocal.Disconnect;

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteShort((ushort)protocal);
            NetworkManager.AddEvent(protocal, buffer);
            Debug.LogError("Connection was closed by the server:>" + msg + " Distype:>" + dis);
        }

        void PrintBytes()
        {
            string returnStr = string.Empty;
            for (int i = 0; i < byteBuffer.Length; i++)
            {
                returnStr += byteBuffer[i].ToString("X2");
            }
            Debug.LogError(returnStr);
        }

        void OnWrite(IAsyncResult r)
        {
            try
            {
                outStream.EndWrite(r);
            }
            catch (Exception ex)
            {
                Debug.LogError("OnWrite--->>>" + ex.Message);
            }
        }

        void OnReceive(byte[] bytes, int length)
        {
            memStream.Seek(0, SeekOrigin.End);
            memStream.Write(bytes, 0, length);
            memStream.Seek(0, SeekOrigin.Begin);
            while (RemainingBytes() > 2)
            {
                ushort messageLen = reader.ReadUInt16();
                if (RemainingBytes() >= messageLen)
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    writer.Write(reader.ReadBytes(messageLen));
                    ms.Seek(0, SeekOrigin.Begin);
                    OnReceivedMessage(ms);
                }
                else
                {
                    memStream.Position = memStream.Position - 2;
                    break;
                }
            }
            byte[] leftover = reader.ReadBytes((int)RemainingBytes());
            memStream.SetLength(0);
            memStream.Write(leftover, 0, leftover.Length);
        }

        private long RemainingBytes()
        {
            return memStream.Length - memStream.Position;
        }

        void OnReceivedMessage(MemoryStream ms)
        {
            BinaryReader r = new BinaryReader(ms);
            byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));

            ByteBuffer buffer = new ByteBuffer(message);
            int mainId = buffer.ReadShort();
            NetworkManager.AddEvent(mainId, buffer);
        }

        void SessionSend(byte[] bytes)
        {
            WriteMessage(bytes);
        }

        public void Close()
        {
            if (client != null)
            {
                if (client.Connected) client.Close();
                client = null;
            }
            loggedIn = false;
        }

        public void SendConnect()
        {
            //~~ ConnectServer(Constants.SocketAddress, Constants.SocketPort);
        }

        public void SendMessage(ByteBuffer buffer)
        {
            SessionSend(buffer.ToBytes());
            buffer.Close();
        }
    }
}
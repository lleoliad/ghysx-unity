// using UnityEngine;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Net;
// using System.Net.Sockets;
// using XLua;
// using System.Threading;
// using System.Net.Http;
// using System.Net.WebSockets;
//
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
//
// using UnityEngine.Networking;
//
// using GhysX.Framework.Utilities;
//
// namespace GhysX.Framework.Network
// {
//     [CSharpCallLua]
//     public interface LuaWebScoket
//     {
//         event EventHandler<PropertyChangedEventArgs> PropertyChanged;
//
//         public void OnError(object sender, Exception ex);
//
//         public void OnClose(object sender, EventArgs e);
//
//         public void OnMessage(object sender, string data);
//
//         public void OnOpen(object sender, EventArgs e);
//
//         object this[int index] { get; set; }
//     }
//
//     public class WebSocket
//     {
//         private ClientWebSocket ws = null;
//
//         private Uri uri = null;
//
//         private bool isUserClose = false;
//
//         public WebSocketState? State { get => ws?.State; }
//
//         public delegate void MessageEventHandler(object sender, string data);
//
//         public delegate void ErrorEventHandler(object sender, Exception ex);
//
//         public event EventHandler OnOpen;
//
//         public event MessageEventHandler OnMessage;
//
//         public event ErrorEventHandler OnError;
//
//         public event EventHandler OnClose;
//
//         public LuaTable luaTable;
//
//         public WebSocket(string wsUrl, string luaPath, LuaTable luaTable = null)
//         {
//             uri = new Uri(wsUrl);
//             ws = new ClientWebSocket();
//
//             LuaWebScoket luaWebScoket = Loxodon.Framework.LuaEnvironment.LuaEnv.Global.GetInPath<LuaWebScoket>(luaPath);
//             OnOpen += luaWebScoket.OnOpen;
//             OnMessage += luaWebScoket.OnMessage;
//             OnError += luaWebScoket.OnError;
//             OnClose += luaWebScoket.OnClose;
//
//             Debug.Log(wsUrl);
//         }
//
//         public void Open()
//         {
//             Task.Run(async () =>
//             {
//                 if (ws.State == WebSocketState.Connecting || ws.State == WebSocketState.Open)
//                     return;
//
//                 string netErr = string.Empty;
//                 try
//                 {
//                     isUserClose = false;
//                     ws = new ClientWebSocket();
//                     await ws.ConnectAsync(uri, CancellationToken.None);
//
//                     if (OnOpen != null)
//                         OnOpen(ws, new EventArgs());
//
//                     List<byte> bs = new List<byte>();
//                     var buffer = new byte[1024 * 4];
//                     WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//                     while (!result.CloseStatus.HasValue)
//                     {
//                         if (result.MessageType == WebSocketMessageType.Text)
//                         {
//                             bs.AddRange(buffer.Take(result.Count));
//
//                             if (result.EndOfMessage)
//                             {
//                                 string userMsg = Encoding.UTF8.GetString(bs.ToArray(), 0, bs.Count);
//
//                                 if (OnMessage != null)
//                                     OnMessage(ws, userMsg);
//
//                                 bs = new List<byte>();
//                             }
//                         }
//                         result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     netErr = " .Net发生错误" + ex.Message;
//
//                     if (OnError != null)
//                         OnError(ws, ex);
//
//                 }
//                 finally
//                 {
//                     if (!isUserClose)
//                         Close(ws.CloseStatus.Value, ws.CloseStatusDescription + netErr);
//                 }
//             });
//
//         }
//
//         public bool Send(string mess)
//         {
//             if (ws.State != WebSocketState.Open)
//                 return false;
//
//             Task.Run(async () =>
//             {
//                 var replyMess = Encoding.UTF8.GetBytes(mess);
//                 await ws.SendAsync(new ArraySegment<byte>(replyMess), WebSocketMessageType.Text, true, CancellationToken.None);
//             });
//
//             return true;
//         }
//
//         public bool Send(byte[] bytes)
//         {
//             if (ws.State != WebSocketState.Open)
//                 return false;
//
//             Task.Run(async () =>
//             {
//                 await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
//             });
//
//             return true;
//         }
//
//         public void Close()
//         {
//             isUserClose = true;
//             Close(WebSocketCloseStatus.NormalClosure, "用户手动关闭");
//         }
//
//         public void Close(WebSocketCloseStatus closeStatus, string statusDescription)
//         {
//             Task.Run(async () =>
//             {
//                 try
//                 {
//                     await ws.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
//                 }
//                 catch (Exception ex)
//                 {
//                     Debug.LogError(ex);
//                 }
//
//                 ws.Abort();
//                 ws.Dispose();
//
//                 if (OnClose != null)
//                     OnClose(ws, new EventArgs());
//             });
//         }
//
//         private void Test()
//         {
//             string url = "ws://localhost:8080/" + "${id}";
//
//             try
//             {
//                 System.Net.WebSockets.ClientWebSocket cln = new System.Net.WebSockets.ClientWebSocket();
//                 cln.ConnectAsync(new Uri(url), new CancellationToken()).Wait();
//
//                 cln.SendAsync(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes("my message")), System.Net.WebSockets.WebSocketMessageType.Text, true, new CancellationToken()).Wait();
//             }
//             catch (Exception ex)
//             {
//                 string ss = ex.ToString();
//             }
//         }
//
//     }
//
// }
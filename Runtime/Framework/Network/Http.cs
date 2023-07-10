// using UnityEngine;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Net;
// using System.Net.Sockets;
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
// using GhysX.Framework.Extensions;
// using GhysX.Framework.Utilities;
//
// #pragma warning disable 0618
//
// namespace GhysX.Framework.Network
// {
//     public enum MethodType
//     {
//         GET,
//         POST
//     }
//
//     public class ResponseType
//     {
//         public const string kHttpBYTE = "BYTE";
//         public const string kHttpTEXT = "TEXT";
//     }
//
//     public interface LuaWebRequest
//     {
//         event EventHandler<PropertyChangedEventArgs> PropertyChanged;
//
//         public void OnStart();
//
//         public void OnError(object request, Exception ex);
//
//         public void OnSuccess(object response, string text, byte[] data);
//
//         public void OnComplete();
//
//         public Dictionary<string, object> entity { get; set; }
//
//     }
//
//     public class WebRequest : MonoBehaviour
//     {
//         private LuaTable luaTable;
//
//         private LuaTable luaEntityTable;
//
//         private LuaWebRequest luaWebRequest;
//
//         private UnityWebRequest unityWebRequest = null;
//
//         public void Request(LuaTable luaTable, string key)
//         {
//             this.luaTable = luaTable;
//             luaWebRequest = luaTable.Cast<LuaWebRequest>();
//             luaEntityTable = luaTable.Get<LuaTable>(key);
//         }
//
//         private void Start()
//         {
//             luaWebRequest.OnStart();
//             StartCoroutine(Send());
//
//         }
//
//         public void SetParam(Dictionary<string, object> entity, ref string url)
//         {
//             string param = "";
//             bool result = entity.TryGetTypedValue("param", out param);
//             if (result && param.Length > 0)
//             {
//                 if (url.IndexOf("?") == -1)
//                 {
//                     url += "?";
//                 }
//                 else
//                 {
//                     url += "&";
//                 }
//
//                 url = url + param;
//             }
//         }
//
//         public void SetParams(Dictionary<string, object> entity, ref string url)
//         {
//             LuaTable tparams;
//             bool result = entity.TryGetTypedValue("params", out tparams);
//             if (result && null != tparams)
//             {
//                 Dictionary<string, object> _params = tparams.Cast<Dictionary<string, object>>();
//                 if (_params.Count() > 0)
//                 {
//                     string data = "";
//                     if (_params != null && _params.Count > 0)
//                     {
//                         foreach (var item in _params)
//                         {
//                             data += item.Key + "=";
//                             data += item.Value.ToString() + "&";
//                         }
//                     }
//
//                     if (url.IndexOf("?") == -1)
//                     {
//                         url += "?";
//                     }
//                     else
//                     {
//                         url += "&";
//                     }
//
//                     url += data.TrimEnd(new char[] { '&' });
//                 }
//             }
//         }
//
//         public WWWForm GetForm(Dictionary<string, object> entity)
//         {
//             WWWForm form = null;
//             object tform;
//             bool result = entity.TryGetTypedValue("form", out tform);
//             if (result && null != tform)
//             {
//                 Dictionary<string, object> data;
//                 if (typeof(string) == tform.GetType())
//                 {
//                     data = JSON.parse<Dictionary<string, object>>((string)tform);
//                 }
//                 else
//                 {
//                     data = ((LuaTable)tform).Cast<Dictionary<string, object>>();
//                 }
//
//                 if (data != null && data.Count > 0)
//                 {
//                     form = new WWWForm();
//                     foreach (var item in data)
//                     {
//                         if (item.Value is byte[])
//                         {
//                             form.AddBinaryData(item.Key, item.Value as byte[]);
//                         }
//                         else if (item.Value is int)
//                         {
//                             form.AddField(item.Key, (int)item.Value);
//                         }
//                         else
//                         {
//                             form.AddField(item.Key, item.Value.ToString());
//                         }
//                     }
//                 }
//             }
//             return form;
//         }
//
//         public string GetBody(Dictionary<string, object> entity)
//         {
//             object body;
//             bool result = entity.TryGetTypedValue("cbody", out body);
//             if (result && null != body)
//             {
//                 string data = null;
//
//                 if (typeof(string) == body.GetType())
//                 {
//                     data = (string)body;
//                 }
//                 else
//                 {
//                     Dictionary<string, object> tbody = ((LuaTable)body).Cast<Dictionary<string, object>>();
//                     data = JSON.stringify(tbody);
//                 }
//                 return data;
//             }
//             return null;
//         }
//
//         public void SetRequestHeader(Dictionary<string, object> entity, UnityWebRequest unityWebRequest)
//         {
//             LuaTable theader;
//             bool result = entity.TryGetTypedValue("header", out theader);
//             if (result && null != theader)
//             {
//                 Dictionary<string, string> header = theader.Cast<Dictionary<string, string>>();
//                 if (header.Count() > 0)
//                 {
//                     foreach (var item in header)
//                     {
//                         unityWebRequest.SetRequestHeader(item.Key, item.Value);
//                     }
//                 }
//             }
//         }
//
//         public void SetRequestTimeout(Dictionary<string, object> entity, UnityWebRequest unityWebRequest)
//         {
//             int timeout;
//             bool result = entity.TryGetTypedValue("timeout", out timeout, Convert.ToInt32);
//             if (result && timeout > 0)
//             {
//                 unityWebRequest.timeout = timeout;
//             }
//         }
//
//         IEnumerator Send()
//         {
//             try
//             {
//                 Dictionary<string, object> entity = luaWebRequest.entity;
//
//                 string url = "";
//                 bool result = entity.TryGetTypedValue("url", out url);
//                 if (result && url.Length > 0)
//                 {
//                     MethodType method;
//                     result = entity.TryGetTypedValue("method", out method);
//                     if (result && method == MethodType.GET)
//                     {
//                         SetParam(entity, ref url);
//                         SetParams(entity, ref url);
//                         unityWebRequest = UnityWebRequest.Get(url);
//                     }
//
//                     if (result && method == MethodType.POST)
//                     {
//                         string body = GetBody(entity);
//                         if (null != body)
//                         {
//                             unityWebRequest = new UnityWebRequest(url, "POST");
//                             unityWebRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
//                             unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
//                         }
//                         else
//                         {
//                             WWWForm form = GetForm(entity);
//                             if (null != form)
//                             {
//                                 unityWebRequest = UnityWebRequest.Post(url, form);
//                             }
//                         }
//                     }
//
//                     if (null != unityWebRequest)
//                     {
//                         SetRequestTimeout(entity, unityWebRequest);
//                         SetRequestHeader(entity, unityWebRequest);
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 try
//                 {
//                     luaWebRequest.OnError(unityWebRequest, ex);
//                 }
//                 catch (Exception exception)
//                 {
//                     Debug.LogError(exception);
//                 }
//                 Debug.LogError(ex);
//             }
//             finally
//             {
//                 if (null == unityWebRequest)
//                 {
//                     Destroy(this);
//                 }
//             }
//
//             if (null != unityWebRequest)
//             {
//                 yield return unityWebRequest.SendWebRequest();
//                 if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError || unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
//                 {
//                     Debug.LogError(unityWebRequest.result);
//                     Debug.LogError(unityWebRequest.error);
//                     try
//                     {
//                         luaWebRequest.OnError(unityWebRequest, null);
//                     }
//                     catch (Exception exception)
//                     {
//                         Debug.LogError(exception);
//                     }
//                 }
//                 else
//                 {
//                     luaWebRequest.OnSuccess(unityWebRequest, unityWebRequest.downloadHandler.text, unityWebRequest.downloadHandler.data);
//                 }
//             }
//
//             if (this.isActiveAndEnabled)
//             {
//                 Destroy(this);
//             }
//         }
//
//         void OnDestroy()
//         {
//             try
//             {
//                 luaWebRequest.OnComplete();
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError(e);
//             }
//             finally
//             {
//                 if (null != unityWebRequest)
//                 {
//                     try
//                     {
//                         unityWebRequest.Abort();
//                         unityWebRequest.Dispose();
//                     }
//                     catch (Exception e)
//                     {
//                         Debug.LogError(e);
//                     }
//                 }
//             }
//         }
//     }
//
//     public class HttpHelper
//     {
//
//         public static void Request(MonoBehaviour mono, string url, MethodType method, Dictionary<string, object> form, Action<object> callback, string responseType)
//         {
//             if (method == MethodType.GET)
//             {
//                 url = CreateGetData(url, form);
//                 mono.StartCoroutine(Request(url, null, callback, responseType));
//             }
//             else if (method == MethodType.POST)
//             {
//                 WWWForm formData = CreatePostData(form);
//                 mono.StartCoroutine(Request(url, formData, callback, responseType));
//             }
//             else
//             {
//                 Debug.LogError("你不能这样子哦...");
//             }
//         }
//
//         static IEnumerator Request(string url, WWWForm form, Action<object> callback, string dateType)
//         {
//             UnityWebRequest request = null;
//             if (form == null)
//                 request = UnityWebRequest.Get(url);
//             else
//                 request = UnityWebRequest.Post(url, form);
//
//             yield return request.SendWebRequest();
//             if (request.isHttpError || request.isNetworkError)
//             {
//                 Debug.LogErrorFormat("Request Error: {0}", request.error);
//             }
//             if (request.isDone)
//             {
//                 if (dateType == ResponseType.kHttpTEXT)
//                 {
//                     callback?.Invoke(request.downloadHandler.text);
//                 }
//                 else if (dateType == ResponseType.kHttpBYTE)
//                 {
//                     callback?.Invoke(request.downloadHandler.data);
//                 }
//                 else
//                 {
//                     Debug.LogError("你不能这样子哦...");
//                 }
//             }
//         }
//
//         private static string CreateGetData(string url, Dictionary<string, object> form)
//         {
//             string data = "";
//             if (form != null && form.Count > 0)
//             {
//                 foreach (var item in form)
//                 {
//                     data += item.Key + "=";
//                     data += item.Value.ToString() + "&";
//                 }
//             }
//             if (url.IndexOf("?") == -1)
//                 url += "?";
//             else
//                 url += "&";
//
//             url += data.TrimEnd(new char[] { '&' });
//             return url;
//         }
//
//         private static WWWForm CreatePostData(Dictionary<string, object> formData)
//         {
//             WWWForm form = new WWWForm();
//             if (formData != null && formData.Count > 0)
//             {
//                 foreach (var item in formData)
//                 {
//                     if (item.Value is byte[])
//                         form.AddBinaryData(item.Key, item.Value as byte[]);
//                     else
//                         form.AddField(item.Key, item.Value.ToString());
//                 }
//             }
//             return form;
//         }
//     }
//
//     public class Http : MonoBehaviour
//     {
//         public static void Send(BestHTTP.HTTPRequest request, string data)
//         {
//             request.RawData = System.Text.Encoding.UTF8.GetBytes(data);
//             request.Send();
//         }
//
//         void Awake()
//         {
//
//         }
//
//         public void Request()
//         {
//             StartCoroutine(Get());
//         }
//
//         IEnumerator Get()
//         {
//             UnityWebRequest unityWebRequest = UnityWebRequest.Get("http://www.baidu.com");
//
//             yield return unityWebRequest.SendWebRequest();
//             if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError || unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
//             {
//                 Debug.Log(unityWebRequest.error);
//             }
//             else
//             {
//             }
//         }
//
//         IEnumerator Post()
//         {
//             WWWForm form = new WWWForm();
//             form.AddField("key", "value");
//             form.AddField("name", "mafanwei");
//             form.AddField("blog", "qwe25878");
//
//             UnityWebRequest unityWebRequest = UnityWebRequest.Post("http://www.baidu.com", form);
//
//             yield return unityWebRequest.SendWebRequest();
//             if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError || unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
//             {
//                 Debug.Log(unityWebRequest.error);
//             }
//             else
//             {
//                 Debug.Log(unityWebRequest.downloadHandler.text);
//             }
//         }
//     }
// }
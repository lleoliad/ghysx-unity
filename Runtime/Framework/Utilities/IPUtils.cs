using System;
using System.Collections;
using System.Threading.Tasks;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Prefs;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using IPData = GhysX.Framework.Utilities.IPData;

namespace GhysX.Framework.Utilities
{

    [Serializable]
    public struct IPData
    {
        public string ip;
        
        public string network;
        
        public string version;

        public string city;

        public string region;

        [JsonProperty("region_code")]
        public string regionCode;
        
        public string country;
        
        [JsonProperty("country_name")]
        public string countryName;
        
        [JsonProperty("country_code")]
        public string countryCode;
        
        [JsonProperty("country_code_iso3")]
        public string countryCodeIso3;
        
        [JsonProperty("country_capital")]
        public string countryCapital;
        
        [JsonProperty("country_tld")]
        public string countryTld;
        
        [JsonProperty("continent_code")]
        public string continentCode;
        
        [JsonProperty("in_eu")]
        public bool? inEu;
        
        public string postal;
        
        public float? latitude;
        
        public float? longitude;
        
        public string timezone;
        
        [JsonProperty("utc_offset")] 
        public string utcOffset;
        
        [JsonProperty("country_calling_code")]
        public string countryCallingCode;
        
        public string currency;
        
        [JsonProperty("currency_name")]
        public string currencyName;
        
        public string languages;
        
        [JsonProperty("country_area")]
        public float? countryArea;
        
        [JsonProperty("country_population")] 
        public long? countryPopulation;
        
        public string asn;
        
        public string org;
    }
    
    public static class IPUtils
    {
        public static IEnumerator GetIPData(IPromise<IPData> promise)
        {
            Preferences prefs = Preferences.GetGlobalPreferences();
            var content = prefs.GetString("ipData");
            if (content == null)
            {
                AsyncResult<IPData> result = new AsyncResult<IPData>();
                UnityWebRequest request = UnityWebRequest.Get("https://ipapi.co/json/");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    content = request.downloadHandler.text;
                    prefs.SetString("ipData", content);
                }
                else
                {
                    // Debug.Log("Failed to get country information: " + request.error);
                }
            }
            
            Debug.Log($"Current IPData: {content}");

            if (content != null)
            {
                IPData data = JsonConvert.DeserializeObject<IPData>(content);
                promise.SetResult(data);
            }

            // AsyncResult<IPData> result = new AsyncResult<IPData>();
            // UnityWebRequest request = UnityWebRequest.Get("https://ipapi.co/json/");
            // yield return request.SendWebRequest();
            //
            // if (request.result == UnityWebRequest.Result.Success)
            // {
            //     string response = request.downloadHandler.text;
            //     // IPData data = JsonUtility.FromJson<IPData>(response);
            //     IPData data = JsonConvert.DeserializeObject<IPData>(response);
            //     promise.SetResult(data);
            //     // Debug.Log($"Current IPData: {data}");
            //     // string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            //     // Console.WriteLine(json);
            //     // Debug.Log($"Current IPData: {json}");
            // }
            // else
            // {
            //     // Debug.Log("Failed to get country information: " + request.error);
            // }
        }
    }
}
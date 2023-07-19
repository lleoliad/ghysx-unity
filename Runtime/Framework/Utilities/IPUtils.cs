using System;
using System.Collections;
using System.Threading.Tasks;
using Loxodon.Framework.Asynchronous;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("region_code")] [JsonProperty("region_code")] public string regionCode;

        public string country;

        [FormerlySerializedAs("country_name")] [JsonProperty("country_name")] public string countryName;
        
        [FormerlySerializedAs("country_code")] [JsonProperty("country_code")] public string countryCode;

        [FormerlySerializedAs("country_code_iso3")] [JsonProperty("country_code_iso3")] public string countryCodeIso3;

        [FormerlySerializedAs("country_capital")] [JsonProperty("country_capital")] public string countryCapital;

        [FormerlySerializedAs("country_tld")] [JsonProperty("country_tld")] public string countryTld;

        [FormerlySerializedAs("continent_code")] [JsonProperty("continent_code")] public string continentCode;

        [FormerlySerializedAs("in_eu")] [JsonProperty("in_eu")] public bool inEu;

        public string postal;

        public float latitude;

        public float longitude;

        public string timezone;

        [FormerlySerializedAs("utc_offset")] [JsonProperty("utc_offset")] public string utcOffset;

        [FormerlySerializedAs("country_calling_code")] [JsonProperty("country_calling_code")] public string countryCallingCode;

        public string currency;

        [FormerlySerializedAs("currency_name")] [JsonProperty("currency_name")] public string currencyName;

        public string languages;

        [FormerlySerializedAs("country_area")] [JsonProperty("country_area")] public float countryArea;

        [FormerlySerializedAs("country_population")] [JsonProperty("country_population")] public long countryPopulation;

        public string asn;

        public string org;
    }
    
    public static class IPUtils
    {
        public static IEnumerator GetIPData(IPromise<IPData> promise)
        {
            AsyncResult<IPData> result = new AsyncResult<IPData>();
            UnityWebRequest request = UnityWebRequest.Get("https://ipapi.co/json/");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                // IPData data = JsonUtility.FromJson<IPData>(response);
                IPData data = JsonConvert.DeserializeObject<IPData>(response);
                promise.SetResult(data);
                // Debug.Log($"Current IPData: {data}");
                // string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                // Console.WriteLine(json);
                // Debug.Log($"Current IPData: {json}");
            }
            else
            {
                // Debug.Log("Failed to get country information: " + request.error);
            }
        }
    }
}
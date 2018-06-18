using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SewerScavenger.Models;

namespace SewerScavenger.Services
{
    public class HttpClientService
    {

        // Make this a singleton so it only exist one time because holds all the data records in memory
        private static HttpClientService _instance;

        private HttpClient _httpClientInstance;

        private HttpClient _httpClient
        {
            get
            {
                if (_httpClientInstance == null)
                {
                    _httpClientInstance = new HttpClient();
                }
                return _httpClientInstance;
            }
            set
            {
                _httpClientInstance = _httpClient;
            }
        }

        public static HttpClientService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HttpClientService();
                }
                return _instance;
            }
        }

        public HttpClient SetHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            return _httpClient;
        }

        public async Task<string> JsonParseResult(HttpResponseMessage response)
        {
            if (response == null)
            {
                return null;
            }

            if (response.Content == null)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();

            var responseJson = response.Content.ReadAsStringAsync().Result;

            JObject json;

            // make sure the object is properly formated json
            try
            {
                json = JObject.Parse(responseJson);
            }
            catch (Exception)
            {
                // todo Add Sequence
                // Jump to restart app sequence 
                return null;
            }

            // Check for error code            
            if (JsonHelper.GetJsonInteger(json, "error_code") == WebGlobals.ErrorResultCode)
            {
                // todo Add Sequence
                // Jump to Update App Sequence

                var myError = new
                {
                    ServerError = true,
                    MessageList = JsonHelper.GetJsonString(json, "msg")
                };

                var myString = (JObject)JToken.FromObject(myError);
                return myString.ToString();
            }

            // Version string is  1.1.1.1  MajorCode.MinorCode.MajorData.MinorData
            var versionJsonString = JsonHelper.GetJsonString(json, "version");
            if (string.IsNullOrEmpty(versionJsonString))
            {
                // invalid returned ID, so return fail
                return null;
            }

            // Split the Version string
            var myVersionSplit = versionJsonString.Split('.');
            if (string.IsNullOrEmpty(myVersionSplit[0]))
            {
                var MajorCode = myVersionSplit[0];
                var MinorCode = myVersionSplit[1];
                var MajorData = myVersionSplit[2];
                var MinorData = myVersionSplit[3];

                //Todo Validate on the Version String
            }


            string data;

            data = null;
            var tempJsonObject = json["data"].ToString();

            if (!string.IsNullOrEmpty(tempJsonObject))
            {
                data = tempJsonObject;
            }

            return data;
        }

        public async Task<string> GetJsonPostAsync(string RestUrl, JObject jsonString)
        {
            // Take the post paramaters, and add the Version and Device to it

            if (string.IsNullOrEmpty(RestUrl))
            {
                return null;
            }

            //// Add the Account Authorization Information to it
            var dict = new Dictionary<string, string>
            {
                { "Version", VersionGlobals.GetCodeVersion() },
            };

            JObject finalContentJson = (JObject)JToken.FromObject(dict);

            var finalJson = new JObject();
            if (jsonString != null)
            {
                finalJson = jsonString;
            }

            // Merge Two json objects into a unified one...
            finalJson.Merge(finalContentJson, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            var finalPostString = finalJson.ToString();

            // Set the header context to say it will use json
            var HeaderContent = new StringContent(finalPostString, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(RestUrl, HeaderContent);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return null;
            }

            var data = await JsonParseResult(response);
            return data;
        }

        public async Task<string> GetJsonGetAsync(string RestUrl)
        {
            // Take the post paramaters, and add the Version and Device to it

            if (string.IsNullOrEmpty(RestUrl))
            {
                return null;
            }

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(RestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return null;
            }

            var data = await JsonParseResult(response);
            return data;
        }

    }
}


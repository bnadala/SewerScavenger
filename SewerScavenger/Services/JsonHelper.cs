using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SewerScavenger.Models;
using SewerScavenger.Services;

namespace SewerScavenger.Services
{
    public static class JsonHelper
    {
        // Takes a json object, and retrieves a string from it matching the field
        public static string GetJsonString(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return null;
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return null;
                }

                return tempJsonObject;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return null;
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static DateTime GetJsonDateTime(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return new DateTime();
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return new DateTime();
                }

                var myDateTime = DateTime.Parse(tempJsonObject);
                return myDateTime;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return new DateTime();
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static TimeSpan GetJsonTimeSpan(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return new TimeSpan();
            }
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return new TimeSpan();
                }


                /*Could be in two different formats.  
                    * Calls from the server have the full format for timespan
                    * Local created objects, have the newtonsoft object of a single string
                    * Need to determine which type and then parse correclty
                */

                // Look for Ticks in string to determine which path to follow
                if (tempJsonObject.Contains("Ticks"))
                {

                    // Can't directly parse json to TimeSpan, it is a flaw in .net
                    // Need to do it manualy
                    // So get total seconds, and then convert that to the time span.

                    // Time span looks like:

                    /*
                     *
                            "Ticks": 0,
                            "Days": 0,
                            "Hours": 0,
                            "Milliseconds": 0,
                            "Minutes": 0,
                            "Seconds": 0,
                            "TotalDays": 0,
                            "TotalHours": 0,
                            "TotalMilliseconds": 0,
                            "TotalMinutes": 0,
                            "TotalSeconds": 0

                     */

                    // Split on the comma seperator
                    // Then find the sub string with Ticks
                    // Then split on the :
                    // convert back half to string value of total seconds

                    var myTicksString = string.Empty;
                    var myTimeSpanSplit = tempJsonObject.Split(',');
                    foreach (var item in myTimeSpanSplit)
                    {
                        if (item.Contains("Ticks"))
                        {
                            var myTempTotalSecondsSplit = item.Split(':');
                            myTicksString = myTempTotalSecondsSplit[1].ToString();
                            break;
                        }
                    }

                    long myTicksLong = Convert.ToInt64(myTicksString);
                    var myTimeSpan = TimeSpan.FromTicks(myTicksLong);
                    return myTimeSpan;
                }
                else
                {
                    // It is just a string so convert that with a parse
                    var myTimeSpanFromString = TimeSpan.Parse(tempJsonObject);
                    return myTimeSpanFromString;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return new TimeSpan();
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static bool GetJsonBool(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return false;
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return false;
                }

                if (tempJsonObject == "True")
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return false;
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static int GetJsonInteger(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return -1;
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return -1;
                }

                var myReturn = int.Parse(tempJsonObject);
                return myReturn;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return -1;
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static double GetJsonDouble(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return -1;
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return -1;
                }

                var myReturn = double.Parse(tempJsonObject);
                return myReturn;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return -1;
            }
        }

        public static long GetJsonLong(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return -1;
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return -1;
                }

                var myReturn = long.Parse(tempJsonObject);
                return myReturn;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return -1;
            }
        }

        public static ulong GetJsonuLong(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return 0;
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return 0;
                }

                var myReturn = ulong.Parse(tempJsonObject);
                return myReturn;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return 0;
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static List<T> GetJsonList<T>(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return default(List<T>);
            }

            if (json == null)
            {
                return default(List<T>);
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field];
                if (tempJsonObject == null)
                {
                    return default(List<T>);
                }

                return tempJsonObject.ToObject<List<T>>();

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return default(List<T>);
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static T GetObject<T>(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return default(T);
            }

            if (json == null)
            {
                return default(T);
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field];
                if (tempJsonObject == null)
                {
                    return default(T);
                }

                return tempJsonObject.ToObject<T>();

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return default(T);
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static List<string> GetJsonStringList(JObject json, string field)
        {
            var myStringList = new List<string>();

            if (string.IsNullOrEmpty(field))
            {
                return myStringList;
            }

            if (json == null)
            {
                return myStringList;
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field];
                if (tempJsonObject == null)
                {
                    return myStringList;
                }

                var myJsonString = tempJsonObject.ToString();

                myStringList = JsonConvert.DeserializeObject<List<string>>(myJsonString);
                return myStringList;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return myStringList;
            }
        }

        // Takes a json object, and retrieves a string from it matching the field
        public static JObject GetJsonObject(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return null;
            }

            // Get Field
            try
            {

                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return null;
                }

                // Empty List
                if (tempJsonObject.Equals("[]"))
                {
                    return null;
                }

                //Parse the string to make sure it is valid json
                json = JObject.Parse(tempJsonObject);

                //Could not get unit Test to generate invalid json, so commenting out this block of clode.
                //if (json == null)
                //{
                //    return null;
                //}

                return json;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return null;
            }
        }

        public static List<JObject> GetJObjectList(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return new List<JObject>();
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return new List<JObject>();
                }

                // Empty List
                if (tempJsonObject.Equals("[]"))
                {
                    return new List<JObject>();
                }

                //Parse the string to make sure it is valid json
                var tempJarray = JArray.Parse(tempJsonObject);

                List<JObject> tempJObjectList = new List<JObject>();
                foreach (var item in tempJarray.Children())
                {
                    tempJObjectList.Add((JObject)item);
                }

                return tempJObjectList;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return new List<JObject>();
            }
        }

        public static List<T> GetObjectList<T>(JObject json, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return new List<T>();
            }

            // Get Field
            try
            {
                var tempJsonObject = json[field].ToString();
                if (string.IsNullOrEmpty(tempJsonObject))
                {
                    return new List<T>();
                }

                // Empty List
                if (tempJsonObject.Equals("[]"))
                {
                    return new List<T>();
                }

                //Parse the string to make sure it is valid json
                var tempJarray = JArray.Parse(tempJsonObject);

                List<T> tempJObjectList = new List<T>();
                foreach (var item in tempJarray.Children())
                {
                    tempJObjectList.Add((T)Activator.CreateInstance(typeof(T), new object[] { (JObject)item }));
                }

                return tempJObjectList;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                return new List<T>();
            }
        }

    }
}

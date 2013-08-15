using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Security;

namespace MovieClient
{
    static class Json
    {
        public static Dictionary<string, JObject> entries = new Dictionary<string, JObject>();
        public static Dictionary<string, JObject> newEntries = new Dictionary<string, JObject>();
        static StorageFolder folder;

        public async static void Init(StorageFolder f)
        {
            folder = f;
            try
            {

                StorageFile sf = await KnownFolders.DocumentsLibrary.GetFileAsync("metadata.json");
                string json = await FileIO.ReadTextAsync(sf);

                JArray metadata = JArray.Parse(json);
                for (int i = 0; i < metadata.Count; i++)
                {
                    JArray arr = (JArray)metadata[i];
                    JObject obj = (JObject)arr[0];
                    entries.Add((string)obj["filename"], obj);
                }
            }
            catch (Exception e)
            {
                string test = e.Message;
                // no metadata file, no biggie
                // WinRT is awful and doesn't have the ability to check if a file exists
                // so I have to catch an execption
                // cut me a break, WinRT
                // cut me a break.
            }
        }

        public async static Task Save()
        {
            try
            {
                StorageFile metadata = await KnownFolders.DocumentsLibrary.CreateFileAsync(
                    "metadata.json", CreationCollisionOption.ReplaceExisting);
                
                
                using (IRandomAccessStream fileStream = await metadata.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (IOutputStream outputStream = fileStream.GetOutputStreamAt(0))
                    {
                        using (DataWriter dataWriter = new DataWriter(outputStream))
                        {
                            dataWriter.WriteString("[");

                            for (int i = 0; i < newEntries.Count; i++)
                            {
                                entries.Add(newEntries.ElementAt(i).Key, newEntries.ElementAt(i).Value);
                            }

                            for (int i = 0; i < entries.Count; i++)
                            {
                                dataWriter.WriteString("[");
                                dataWriter.WriteString(entries.ElementAt(i).Value.ToString());
                                dataWriter.WriteString("]");

                                if (i < entries.Count - 1)
                                { dataWriter.WriteString(","); }
                            }
                            
                            dataWriter.WriteString("]");

                            await dataWriter.StoreAsync();
                            await dataWriter.FlushAsync();
                            dataWriter.DetachStream();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string primblem = e.Message;
            }
            
        }

        public static JObject ParseJson(string json)
        {
            json = json.Substring(1, json.Length - 2);

            return JObject.Parse(json);
        }

        public static string GetField(string key, JObject json)
        {
            return (string)json[key];
        }

        public static string[] GetFieldArr(string key, JObject json)
        {
            JArray jArr = (JArray)json[key];
            string[] arr = new string[jArr.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (string)jArr[i];
            }
            return arr;
        }

       
    }
}

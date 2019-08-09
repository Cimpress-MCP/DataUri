using Cimpress.DataUri.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cimpress.DataUri
{
    public class DataUri
    {
        public const string CONTENT_ENCODING = "content-coding";

        private static readonly Dictionary<string, IDataUriDeserializer> _dataUriDeserializers = new Dictionary<string, IDataUriDeserializer>
        {
            { JsonDeserializer.MediaType, new JsonDeserializer() },
        };

        public MediaType MediaType { get; }
        public string Data { get; }
        public bool Base64 { get; }

        /// <summary>
        /// Default serialization settings, using application json, deflate and base64 encoding
        /// </summary>
        public static ObjectSerializationSettings DefaultSerializationSettings { get; } = new ObjectSerializationSettings(new ObjectSerializer(), "application/json", true, null);

        public DataUri(MediaType mediaType, string data, bool base64)
        {
            MediaType = mediaType;
            Data = data;
            Base64 = base64;
        }

        /// <summary>
        /// Add am additional mimeType to the dataDeserializer registry
        /// </summary>
        /// <param name="mimeType">mime type to accept, will override existing deserializers</param>
        /// <param name="dataUriDeserializer">The data deserializer to use</param>
        public static void RegisterDataDeserializer(string mimeType, IDataUriDeserializer dataUriDeserializer)
        {
            _dataUriDeserializers[mimeType] = dataUriDeserializer;
        }

        /// <summary>
        /// Extract a specific data deserializer from the registry
        /// </summary>
        /// <param name="mimeType">mime type of the register</param>
        /// <returns></returns>
        public static IDataUriDeserializer GetDataUriDeserializer(string mimeType)
        {
            if(TryGetDataUriDeserializer(mimeType, out IDataUriDeserializer dataUriDeserializer))
            {
                return dataUriDeserializer;
            }
            throw new ArgumentException($"No Data Uri Deserializer registered for mime type {mimeType}");
        }

        public static bool TryGetDataUriDeserializer(string mimeType, out IDataUriDeserializer dataUriDeserializer)
        {
            if(_dataUriDeserializers.ContainsKey(mimeType))
            {
                dataUriDeserializer = _dataUriDeserializers[mimeType];
                return true;
            }
            dataUriDeserializer = default;
            return false;
        }

        /// <summary>
        /// Converts a string in the form of a data Uri to a dataUri object
        /// </summary>
        /// <param name="dataUri">string representation of a data uri</param>
        /// <returns>Object version of the data uri</returns>
        public static DataUri Parse(string dataUri)
        {
            if (TryParse(dataUri, out DataUri data))
            {
                return data;
            }
            throw new ArgumentException($"data uri {dataUri} is not a valid uri");
        }

        /// <summary>
        /// Attempts to convert a string dataUri into object form
        /// </summary>
        /// <param name="dataUri">string representation of a dataUri</param>
        /// <param name="dataObj">Object representation of a dataUri</param>
        /// <returns>If conversion was a success</returns>
        public static bool TryParse(string dataUri, out DataUri dataObj)
        {
            dataObj = null;

            if(DataUriRegex.DataUri.IsMatch(dataUri))
            {
                bool base64 = dataUri.Contains(";base64,");
                string data = dataUri.Split(',')[1];
                string mediaString = DataUriRegex.CaptureMediaType.Match(dataUri)?.Groups["mime"]?.Value;
                if (!string.IsNullOrEmpty(mediaString) && MediaType.TryParse(mediaString, out MediaType mediaType))
                {
                    dataObj = new DataUri(mediaType, data, base64);
                    return true;
                }
            }

            return false;
        }

        public static T ToObject<T>(string dataUri)
        {
            DataUri data = Parse(dataUri);
            return ToObject<T>(data);
        }

        /// <summary>
        /// Converts a data uri to an object based on known dataUri deserializers
        /// </summary>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <param name="dataUri">DataUri to deserialize</param>
        /// <returns>Object deserialized from the dataUri</returns>
        public static T ToObject<T>(DataUri dataUri)
        {
            IDataUriDeserializer deserializer = GetDataUriDeserializer(dataUri.MediaType.MimeType);
            return (T)deserializer.DeserializeDataUri(dataUri, typeof(T));
        }

        /// <summary>
        /// Converts an object to a dataUri using the default dataUri serialization settings
        /// </summary>
        /// <param name="obj">Object to transform into a dataUri</param>
        /// <returns></returns>
        public static DataUri FromObject(object obj)
        {
            return FromObject(obj, DefaultSerializationSettings);
        }

        /// <summary>
        /// Converts an object to a dataUri using specified ObjectSerialization settings
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static DataUri FromObject(object obj, ObjectSerializationSettings settings)
        {
            byte[] data = settings.Serializer.Serialize(obj);
            return FromByteArray(data, settings.MediaType, settings.Base64, settings.GetAllMediaTypeParameters());
        }

        /// <summary>
        /// Creates a dataUri from all data supplied. If not base64 encoding assumes that bytes are UTF-8
        /// </summary>
        /// <param name="bytes">data of the dataUri</param>
        /// <param name="mediaType">MediaType of the data Uri</param>
        /// <param name="base64">If data should be base64 encoded</param>
        /// <param name="mediaTypeParameters">Additional parameters to add to the data uri</param>
        /// <returns></returns>
        public static DataUri FromByteArray(byte[] bytes, string mediaType, bool base64, Dictionary<string, string> mediaTypeParameters = null)
        {
            string data = base64 ? Convert.ToBase64String(bytes) : Encoding.UTF8.GetString(bytes);
            return new DataUri(new MediaType(mediaType, mediaTypeParameters), data, base64);
        }

        public override string ToString()
        {
            string baseString = $"data:{MediaType.ToString()}";
            return Base64 ? $"{baseString};base64,{Data}" : $"{baseString},{Data}";
        }
    }
}

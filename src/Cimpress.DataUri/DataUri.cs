using Cimpress.DataUri.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cimpress.DataUri
{
    public class DataUri
    {
        public const string CONTENT_ENCODING = "content-coding";

        private static Dictionary<string, IDataUriDeserializer> _dataUriDeserializers = new Dictionary<string, IDataUriDeserializer>
        {
            { JsonDeserializer.MediaType, new JsonDeserializer() },
        };

        public MediaType MediaType { get; }
        public string Data { get; }
        public bool Base64 { get; }
        public static ObjectSerializationSettings DefaultSerializationSettings { get; } = new ObjectSerializationSettings(new ObjectSerializer(), "application/json", true, null);

        public DataUri(MediaType mediaType, string data, bool base64)
        {
            MediaType = mediaType;
            Data = data;
            Base64 = base64;
        }

        public static void RegisterDataDeserializer(string mimeType, IDataUriDeserializer dataUriDeserializer)
        {
            _dataUriDeserializers[mimeType] = dataUriDeserializer;
        }

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

        public static DataUri Parse(string dataUri)
        {
            if (TryParse(dataUri, out DataUri data))
            {
                return data;
            }
            throw new ArgumentException($"data uri {dataUri} is not a valid uri");
        }

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

        public static T ToObject<T>(DataUri dataUri)
        {
            IDataUriDeserializer deserializer = GetDataUriDeserializer(dataUri.MediaType.MimeType);
            return (T)deserializer.DeserializeDataUri(dataUri, typeof(T));
        }

        public static DataUri FromObject(object obj)
        {
            return FromObject(obj, DefaultSerializationSettings);
        }

        public static DataUri FromObject(object obj, ObjectSerializationSettings settings)
        {
            byte[] data = settings.Serializer.Serialize(obj);
            return FromByteArray(data, settings.MediaType, settings.Base64, settings.GetAllMediaTypeParameters());
        }

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

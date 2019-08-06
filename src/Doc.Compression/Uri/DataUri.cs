using Doc.Compression.Compression;
using Doc.Compression.Deserialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doc.Compression.Uri
{
    public class DataUri
    {
        public const string CONTENT_ENCODING = "content-coding";

        private static Dictionary<string, IDataUriDeserializer> _dataUriDeserializers = new Dictionary<string, IDataUriDeserializer>
        {
            { JsonDeserializer.APPLICATION_JSON, new JsonDeserializer() },
        };

        public MediaType MediaType { get; }
        public string Data { get; }
        public bool Base64 { get; }

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
            return deserializer.DeserializeDataUri<T>(dataUri);
        }

        public static DataUri FromObject(object obj, DataUriEncoderSettings settings)
        {
            byte[] data = settings.Serializer().serialize(obj);
            byte[] dataCompressed = settings.Compressor().Compress(data);
            return FromByteArray(dataCompressed, settings.Serializer().MediaType, settings.Base64, settings.Compressor().MediaTypeParams);
        }

        public static DataUri FromObject(object obj, string mediaType, bool base64, string encoding, Dictionary<string, string> mediaTypeParams = null)
        {
            string data = JsonConvert.SerializeObject(obj);
            Dictionary<string, string> mediaParams = mediaTypeParams ?? new Dictionary<string, string>();
            mediaParams.Add(CONTENT_ENCODING, encoding);
            if (Deflate.DEFLATE.Equals(encoding))
            {
                return FromByteArray(Deflate.Encode(data), mediaType, base64, mediaParams);
            }
            else if (GZip.GZIP.Equals(encoding))
            {
                return FromByteArray(GZip.Encode(data), mediaType, base64, mediaParams);
            }
            throw new ArgumentException($"Unknown encoding type for parameter encoding of: {encoding}");
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

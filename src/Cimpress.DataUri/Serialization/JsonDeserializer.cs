using Cimpress.DataUri.Compression;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Cimpress.DataUri.Serialization
{
    /// <summary>
    /// Default deserializer to handle application/json dataUris.
    /// By default handles content-codings for deflate and gzip
    /// </summary>
    public class JsonDeserializer : IDataUriDeserializer
    {
        public const string MediaType = "application/json";
        /// <summary>
        /// Deserializer to handle deserializing dataUris
        /// Will use data and JsonConvert to compose media types of application/json into objects
        /// </summary>
        /// <param name="dataUri">DataUri to deserialize</param>
        /// <param name="targetType">Type of object to turn data into</param>
        /// <returns></returns>
        public object DeserializeDataUri(DataUri dataUri, Type targetType)
        {
            var rawBytes = dataUri.Base64 ? Convert.FromBase64String(dataUri.Data) : null;

            var encoding = dataUri.MediaType.Parameters?.Count > 0 && dataUri.MediaType.Parameters.ContainsKey(DataUri.CONTENT_ENCODING) ? dataUri.MediaType.Parameters[DataUri.CONTENT_ENCODING] : null;

            // Check if there were content codings
            if (Deflate.DEFLATE.Equals(encoding))
            {
                return FromDeflated(rawBytes, targetType);
            }
            else if (GZip.GZIP.Equals(encoding))
            {
                return FromGZip(rawBytes, targetType);
            }
            var data = dataUri.Base64 ? Encoding.UTF8.GetString(rawBytes) : dataUri.Data;
            return JsonConvert.DeserializeObject(data, targetType);
        }

        private object FromGZip(byte[] bytes, Type targetType)
        {
            byte[] decoded = GZip.Decode(bytes);
            string serialized = Encoding.UTF8.GetString(decoded);
            return JsonConvert.DeserializeObject(serialized, targetType);
        }

        private object FromDeflated(byte[] bytes, Type targetType)
        {
            byte[] decoded = Deflate.Decode(bytes);
            string serialized = Encoding.UTF8.GetString(decoded);
            return JsonConvert.DeserializeObject(serialized, targetType);
        }
    }
}

using Doc.Compression.Compression;
using Doc.Compression.Uri;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Doc.Compression.Serialization
{
    public class JsonDeserializer : IDataUriDeserializer
    {
        public const string MediaType = "application/json";
        private readonly Compression
        public T DeserializeDataUri<T>(DataUri dataUri)
        {
            var rawBytes = dataUri.Base64 ? Convert.FromBase64String(dataUri.Data) : null;

            var encoding = dataUri.MediaType.Parameters?.Count > 0 && dataUri.MediaType.Parameters.ContainsKey(DataUri.CONTENT_ENCODING) ? dataUri.MediaType.Parameters[DataUri.CONTENT_ENCODING] : null;
            // Check for deflate
            if (APPLICATION_ZLIB.Equals(dataUri.MediaType.MimeType) || Deflate.DEFLATE.Equals(encoding))
            {
                return FromDeflated<T>(rawBytes);
            }
            else if (APPLICATION_GZIP.Equals(dataUri.MediaType.MimeType) || GZip.GZIP.Equals(encoding))
            {
                return FromGZip<T>(rawBytes);
            }
            var data = dataUri.Base64 ? Encoding.UTF8.GetString(rawBytes) : dataUri.Data;
            return JsonConvert.DeserializeObject<T>(data);
        }

        private T FromGZip<T>(byte[] bytes)
        {
            byte[] decoded = GZip.Decode(bytes);
            string serialized = Encoding.UTF8.GetString(decoded);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        private T FromDeflated<T>(byte[] bytes)
        {
            byte[] decoded = Deflate.Decode(bytes);
            string serialized = Encoding.UTF8.GetString(decoded);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}

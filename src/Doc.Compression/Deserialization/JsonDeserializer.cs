﻿using Doc.Compression.Compression;
using Doc.Compression.Uri;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Doc.Compression.Deserialization
{
    public class JsonDeserializer : IDataUriDeserializer
    {

        public const string APPLICATION_GZIP = "application/gzip";
        public const string APPLICATION_ZLIB = "application/zlib";
        public const string APPLICATION_JSON = "application/json";

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

            return JsonConvert.DeserializeObject<T>(dataUri.Data);
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

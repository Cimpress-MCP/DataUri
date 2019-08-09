using Cimpress.DataUri.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cimpress.DataUri.Serialization
{
    public class ObjectSerializer : IObjectSerializer
    {
        public Dictionary<string, string> GetMediaTypeParameters()
        {
            return new Dictionary<string, string> { { DataUri.CONTENT_ENCODING, Deflate.DEFLATE } };
        }

        public byte[] Serialize(object obj)
        {
            string stringObj = JsonConvert.SerializeObject(obj);
            return Deflate.Encode(stringObj);
        }
    }
}

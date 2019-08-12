using Cimpress.DataUri.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cimpress.DataUri.Tests.Helpers
{
    public class GZipSerializer : IObjectSerializer
    {
        public Dictionary<string, string> GetMediaTypeParameters()
        {
            return new Dictionary<string, string> { { DataUri.CONTENT_ENCODING, GZip.GZIP } };
        }

        public byte[] Serialize(object obj)
        {
            string stringObj = JsonConvert.SerializeObject(obj);
            return GZip.Encode(stringObj);
        }
    }
}

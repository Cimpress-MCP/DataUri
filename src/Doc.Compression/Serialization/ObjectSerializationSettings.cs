using System;
using System.Collections.Generic;

namespace Doc.Compression.Serialization
{
    public class ObjectSerializationSettings
    {

        public ObjectSerializationSettings(Func<object, byte[]> serialize, string mediaType, bool base64, Dictionary<string, string> mediaTypeParams)
        {
            Serialize = serialize;
            MediaType = mediaType;
            Base64 = base64;
            MediaTypeParameters = mediaTypeParams;
        }

        public Func<object, byte[]> Serialize { get; }
        public string MediaType { get; }
        public bool Base64 { get; }
        public Dictionary<string, string> MediaTypeParameters { get; }
    }
}

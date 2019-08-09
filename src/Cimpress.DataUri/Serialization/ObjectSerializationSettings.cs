using System.Collections.Generic;

namespace Cimpress.DataUri.Serialization
{
    public class ObjectSerializationSettings
    {

        public ObjectSerializationSettings(IObjectSerializer serializer, string mediaType, bool base64, Dictionary<string, string> mediaTypeParams)
        {
            Serializer = serializer;
            MediaType = mediaType;
            Base64 = base64;

            MediaTypeParameters = mediaTypeParams;
        }

        public IObjectSerializer Serializer { get; }
        public string MediaType { get; }
        public bool Base64 { get; }
        public Dictionary<string, string> MediaTypeParameters { get; }

        public Dictionary<string, string> GetAllMediaTypeParameters()
        {
            var baseParams = MediaTypeParameters ?? new Dictionary<string, string>();
            if (Serializer.GetMediaTypeParameters()?.Count > 0)
            {
                foreach (var entry in Serializer.GetMediaTypeParameters())
                {
                    baseParams[entry.Key] = entry.Value;
                }
            }
            return baseParams;
        }
    }
}

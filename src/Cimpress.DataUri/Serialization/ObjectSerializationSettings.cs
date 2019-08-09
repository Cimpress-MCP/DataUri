using System.Collections.Generic;

namespace Cimpress.DataUri.Serialization
{
    public class ObjectSerializationSettings
    {
        /// <summary>
        /// Setting for how to transform an object into a dataUri
        /// </summary>
        /// <param name="serializer">Transformer of object into bytes</param>
        /// <param name="mediaType">The mediaType of the end result (application/json, image/png)</param>
        /// <param name="base64">Whether to base 64 encode bytes from the serializer</param>
        /// <param name="mediaTypeParams">Additional parameters to add to data Uri (charset=UTF-8)</param>
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

        /// <summary>
        /// Function to get all media type parameters from the settings and the serializer
        /// </summary>
        /// <returns></returns>
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

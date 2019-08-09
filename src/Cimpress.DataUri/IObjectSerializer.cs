using System.Collections.Generic;

namespace Cimpress.DataUri
{
    public interface IObjectSerializer
    {
        /// <summary>
        /// Function to turn an object into a byte array
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize(object obj);

        /// <summary>
        /// Special parameters to add to the datauri, for example if the serializer uses an encoding an entry could be content-coding
        /// </summary>
        /// <returns>Pairs of media type parameters related to the </returns>
        Dictionary<string, string> GetMediaTypeParameters();
    }
}

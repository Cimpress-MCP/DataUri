using System;

namespace Cimpress.DataUri
{
    public interface IDataUriDeserializer
    {
        /// <summary>
        /// Function to turn a dataUri into an object of a certain type
        /// </summary>
        /// <param name="dataUri">DataUri to convert into a data type</param>
        /// <param name="targetType">The type to transform the data in the data uri into</param>
        /// <returns></returns>
        object DeserializeDataUri(DataUri dataUri, Type targetType);
    }
}

using System;

namespace Cimpress.DataUri
{
    public interface IDataUriDeserializer
    {
        object DeserializeDataUri(DataUri dataUri, Type targetType);
    }
}

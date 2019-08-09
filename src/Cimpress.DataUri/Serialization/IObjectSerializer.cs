using System.Collections.Generic;

namespace Cimpress.DataUri.Serialization
{
    public interface IObjectSerializer
    {
        byte[] Serialize(object obj);
        Dictionary<string, string> GetMediaTypeParameters();
    }
}

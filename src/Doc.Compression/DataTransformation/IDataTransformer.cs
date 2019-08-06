namespace Doc.Compression.DataTransformation
{
    public interface IDataTransformer
    {
        T ToObject<T>(string data, bool base64);
    }
}

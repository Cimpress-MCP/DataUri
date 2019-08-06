namespace Doc.Compression.Uri
{
    public interface IDataUriDeserializer
    {
        T DeserializeDataUri<T>(DataUri dataUri);
    }
}

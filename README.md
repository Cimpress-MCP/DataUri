# Cimpress-MCP.DataUri
A framework to handle converting objects to and from DataUris. With built in support for handling application/json DataUris. The framework is written in compliance with [RFC-2397](https://tools.ietf.org/html/rfc2397).

# Using this library

## Parsing DataUris

There is built-in support for handling dataUris with the media type of `application/json`.

```cs
class Person
{
    public string Name { get; set; }
}

string dataUri = "data:application/json,{\"name\":\"andrew\"}";
Person andrew = DataUri.ToObject<Person>(dataUri);
Assert.Equal("andrew", andrew.Name);
```

Another example with a base64 encoded payload

```cs
string dataUri = "data:application/json;base64,eyJuYW1lIjoiYW5kcmV3In0=";
Person andrew = DataUri.ToObject<Person>(dataUri);
Assert.Equal("andrew", andrew.Name);
```

Additionally when dealing with large json payloadds the data of the uri can be compressed with the deflate or gzip algorithm.

```cs
string dataUri = "data:application/json;content-coding=deflate;base64,q1byS8xNVbJSSsxLKUotV6oFAA==";
Person andrew = DataUri.ToObject<Person>(dataUri);
Assert.Equal("andrew", andrew.Name);
```

### Handling other media types

The rules deserializing data uris is done by the media type of the dataUri. There is a registry of known media types the and interface to deserialize them. For example to add a deserializer for image/png using Bitmap and System.Drawing.

```cs
class ImageDeserializer : IDataUriDeserializer
{
    public const string MEDIA_TYPE = "image/png";

    public object DeserializeDataUri(DataUri dataUri, Type targetType)
    {
        byte[] imageBytes = dataUri.Base64 ? Convert.FromBase64String(dataUri.Data) : Encoding.UTF8.GetBytes(dataUri.Data);

        using (MemoryStream memStream = new MemoryStream(imageBytes))
        {
            return new Bitmap(memStream);
        }
    }
}

DataUri.RegisterDataDeserializer("image/png", new ImageDeserializer());
string dataUri = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAWSURBVChTY6AdqP9/5T+UOQpIBQwMAB4hA1I5813pAAAAAElFTkSuQmCC";
Bitmap image = DataUri.ToObject<Bitmap>(dataUri);
```

## Creating a DataUri from an object

By default when dataUris are created from an object the object is serialized using Newtonsoft.Json, base64 encoded, and labeled as application/json.
```cs
Person andrew = new Person()
{
    Name = "andrew"
};
var dataUri = DataUri.FromObject(andrew);
dataUri.ToString() => "data:application/json;content-coding=deflate;base64,q1byS8xNVbJSSsxLKUotV6oFAA=="
```

### Updating FromObject

Different ObjectSerializationSettings can be provided to the FromObject method to change the behavoir. The most important part of the change is the `IObjectSerializer` that contains the rules for how to change an object into a byte array.
For example to use gzip as an encoding algorithm:

```cs
class GZipSerializer : IObjectSerializer
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
var serializationSettings = new ObjectSerializationSettings(new GZipSerializer(), "application/json", true, null);
Person andrew = new Person()
{
    Name = "andrew"
};
DataUri dataUriGZip = DataUri.FromObject(andrew, serializationSettings);
```
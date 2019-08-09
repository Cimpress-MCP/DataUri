using SkiaSharp;
using System;
using System.Text;

namespace Cimpress.DataUri.Tests.Helpers
{
    internal class ImageDeserializer : IDataUriDeserializer
    {
        public const string MEDIA_TYPE = "image/png";

        public object DeserializeDataUri(DataUri dataUri, Type targetType)
        {
            byte[] imageBytes;
            if(dataUri.Base64)
            {
                imageBytes = Convert.FromBase64String(dataUri.Data);
            }
            else
            {
                imageBytes = Encoding.UTF8.GetBytes(dataUri.Data);
            }
            return SKImage.FromEncodedData(imageBytes);
        }
    }
}

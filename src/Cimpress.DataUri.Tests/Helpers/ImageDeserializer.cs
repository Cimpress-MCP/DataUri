using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Cimpress.DataUri.Tests.Helpers
{
    internal class ImageDeserializer : IDataUriDeserializer
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
}

using Xunit;
using Doc.Compression.Uri;
using Doc.Compression.Tests.Models;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Doc.Compression.Tests
{
    public class DataUriTests
    {
        [Fact]
        public void CreateDataUriObject()
        {
            Person andrew = new Person()
            {
                Name = "andrew"
            };
            var dataUri = DataUri.FromObject(andrew, "application/json", true);
            Assert.True(dataUri.Base64);
            Assert.Equal("application/json", dataUri.MediaType.MimeType);
        }

        [Fact]
        public void CreateDataUriFromByteArray()
        {
            byte[] bytes;
            using (Bitmap bitmap = new Bitmap(5, 5))
            using (MemoryStream memStream = new MemoryStream())
            {
                bitmap.SetPixel(1, 1, Color.Azure);
                bitmap.Save(memStream, ImageFormat.Jpeg);
                bytes = memStream.ToArray();
            }

            var dataUri = DataUri.FromByteArray(bytes, "image/jpeg", true);
            Assert.True(dataUri.Base64);
            Assert.Equal("image/jpeg", dataUri.MediaType.MimeType);
        }
    }
}

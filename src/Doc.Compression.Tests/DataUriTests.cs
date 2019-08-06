using Xunit;
using Doc.Compression.Uri;
using Doc.Compression.Tests.Models;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System;

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

        [Fact]
        public void ParseDataUriUnregistered()
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

            try
            {
                DataUri.ToObject<Bitmap>(dataUri);
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                Assert.True(true);
            }
            catch // If there isn't an argument exception there is an issue
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void CreateDataUriRoundTrip()
        {
            Person andrew = new Person()
            {
                Name = "andrew"
            };
            var dataUri = DataUri.FromObject(andrew, "application/json", true);
            Assert.True(dataUri.Base64);
            Assert.Equal("application/json", dataUri.MediaType.MimeType);

            Person andrewCopy = DataUri.ToObject<Person>(dataUri);

            Assert.Equal(andrew.Name, andrewCopy.Name);
        }
    }
}

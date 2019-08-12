using Xunit;
using System;
using System.Drawing;
using Cimpress.DataUri.Tests.Models;
using System.IO;
using Cimpress.DataUri.Tests.Helpers;
using Cimpress.DataUri.Serialization;
using System.Drawing.Imaging;

namespace Cimpress.DataUri.Tests
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
            var dataUri = DataUri.FromObject(andrew);
            Assert.True(dataUri.Base64);
            Assert.Equal("application/json", dataUri.MediaType.MimeType);
        }

        [Fact]
        public void TransformDataUriBackToObject()
        {
            Person andrew = new Person()
            {
                Name = "andrew"
            };
            var dataUri = DataUri.FromObject(andrew);

            Assert.True(dataUri.Base64);
            Assert.Equal("application/json", dataUri.MediaType.MimeType);

            Assert.True(Uri.IsWellFormedUriString(dataUri.ToString(), System.UriKind.Absolute));

            Person andy = DataUri.ToObject<Person>(dataUri);

            Assert.Equal(andrew.Name, andy.Name);
        }

        [Fact]
        public void DataUriToObjectUnknownMediaType()
        {
            string imageUri = "data:image/jpeg;base64,fooBar";
            try
            {
                var dataUri = DataUri.Parse(imageUri);
                DataUri.ToObject<Bitmap>(dataUri);
                throw new Exception("DataUri Should be invalid");
            }
            catch(ArgumentException)
            {

            }
        }

        [Fact]
        public void AddNewObjectSerializerImage()
        {
            byte[] bytes;
            using (Bitmap bitmap = new Bitmap(10, 10))
            {
                bitmap.SetPixel(2, 2, Color.Aquamarine);
                using(MemoryStream memStream = new MemoryStream())
                {
                    bitmap.Save(memStream, ImageFormat.Png);
                    bytes = memStream.ToArray();
                }
                DataUri dataUri = DataUri.FromByteArray(bytes, "image/png", true);

                DataUri.RegisterDataDeserializer("image/png", new ImageDeserializer());
                Bitmap image = DataUri.ToObject<Bitmap>(dataUri);
                Assert.True(bitmap.Width == image.Width);
                Assert.True(bitmap.Height == image.Height);

                var tmp = image.GetPixel(2, 2);

                Assert.True(image.GetPixel(2, 2).ToArgb() == Color.Aquamarine.ToArgb());
            }
        }

        [Fact]
        public void AddNewObjectSerializerJson()
        {
            var serializationSettings = new ObjectSerializationSettings(new GZipSerializer(), "application/json", true, null);
            Person andrew = new Person()
            {
                Name = "andrew"
            };
            DataUri dataUriGZip = DataUri.FromObject(andrew, serializationSettings);

            DataUri dataUriDeflate = DataUri.FromObject(andrew);

            Assert.Contains("gzip", dataUriGZip.ToString());
            Assert.NotEqual(dataUriGZip.Data, dataUriDeflate.Data);
        }
    }
}

using Xunit;
using System;
using System.Drawing;
using Cimpress.DataUri.Tests.Models;
using SkiaSharp;
using System.IO;
using Cimpress.DataUri.Tests.Helpers;
using Cimpress.DataUri.Serialization;

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
            using (SKBitmap bitmap = new SKBitmap(10, 10))
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                bitmap.SetPixel(2, 2, SKColor.Parse("#FFAA22"));
                SKImage img = SKImage.FromBitmap(bitmap);
                SKData data = img.Encode(SKEncodedImageFormat.Png, 100);
                using(MemoryStream memStream = new MemoryStream())
                {
                    data.SaveTo(memStream);
                    bytes = memStream.ToArray();
                }
                DataUri dataUri = DataUri.FromByteArray(bytes, "image/png", true);

                DataUri.RegisterDataDeserializer("image/png", new ImageDeserializer());
                SKImage image = DataUri.ToObject<SKImage>(dataUri);
                Assert.True(img.Width == image.Width);
                Assert.True(img.Height == image.Height);
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

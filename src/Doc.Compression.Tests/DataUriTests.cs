using Xunit;

namespace Doc.Compression.Tests
{
    public class DataUriTests
    {
        [Fact]
        public void ValidDataUri()
        {
            string dataUriString = "data:application/json;base64,FooBar";
            var dataUri = DataUri.Parse(dataUriString);

            Assert.True(dataUri.Base64);
            Assert.Equal("FooBar", dataUri.Data);
            Assert.Equal("application", dataUri.MediaType.Type);
            Assert.Equal("json", dataUri.MediaType.SubType);
        }

        [Fact]
        public void InvalidDataUri()
        {
            string dataUriString = "data:/application/json;base64,FooBar";
            bool valid = DataUri.TryParse(dataUriString, out DataUri data);

            Assert.False(valid);
        }

        [Fact]
        public void InvalidDataUriData()
        {
            string dataUriString = "data:/application/json;base64";
            bool valid = DataUri.TryParse(dataUriString, out DataUri data);

            Assert.False(valid);
        }

        [Fact]
        public void InvalidDataUriIncomplete()
        {
            string dataUriString = "data:application/patch-ops-error+xml;conte";
            bool valid = DataUri.TryParse(dataUriString, out DataUri data);

            Assert.False(valid);
        }

        [Fact]
        public void InvalidDataUriMissingHead()
        {
            string dataUriString = "application/json;base64";
            bool valid = DataUri.TryParse(dataUriString, out DataUri data);

            Assert.False(valid);
        }

        [Fact]
        public void ValidDataUriNoBase64()
        {
            string dataUriString = "data:application/json,FooBar";
            var dataUri = DataUri.Parse(dataUriString);

            Assert.False(dataUri.Base64);
            Assert.Equal("FooBar", dataUri.Data);
            Assert.Equal("application", dataUri.MediaType.Type);
            Assert.Equal("json", dataUri.MediaType.SubType);
        }

        [Fact]
        public void ValidDataUriNoBase64Parameters()
        {
            string dataUriString = "data:application/json;content-coding=deflate,FooBar";
            var dataUri = DataUri.Parse(dataUriString);

            Assert.False(dataUri.Base64);
            Assert.Equal("FooBar", dataUri.Data);
            Assert.Equal("application", dataUri.MediaType.Type);
            Assert.Equal("json", dataUri.MediaType.SubType);

            Assert.True(dataUri.MediaType.Parameters.ContainsKey("content-coding"));
            Assert.Equal("deflate", dataUri.MediaType.Parameters["content-coding"]);
        }

        [Fact]
        public void ValidDataUriParameters()
        {
            string dataUriString = "data:application/json;content-coding=deflate;base64,FooBar";
            var dataUri = DataUri.Parse(dataUriString);

            Assert.True(dataUri.Base64);
            Assert.Equal("FooBar", dataUri.Data);
            Assert.Equal("application", dataUri.MediaType.Type);
            Assert.Equal("json", dataUri.MediaType.SubType);

            Assert.True(dataUri.MediaType.Parameters.ContainsKey("content-coding"));
            Assert.Equal("deflate", dataUri.MediaType.Parameters["content-coding"]);
        }

        [Fact]
        public void ValidDataUriMediaTypeOutlier()
        {
            string dataUriString = "data:application/patch-ops-error+xml;content-coding=deflate;base64,FooBar";
            var dataUri = DataUri.Parse(dataUriString);

            Assert.True(dataUri.Base64);
            Assert.Equal("FooBar", dataUri.Data);
            Assert.Equal("application", dataUri.MediaType.Type);
            Assert.Equal("patch-ops-error+xml", dataUri.MediaType.SubType);

            Assert.True(dataUri.MediaType.Parameters.ContainsKey("content-coding"));
            Assert.Equal("deflate", dataUri.MediaType.Parameters["content-coding"]);
        }
    }
}

using System;
using System.Collections.Generic;
using Xunit;

namespace Doc.Compression.Tests
{
    public class GZipTests
    {
        [Theory]
        [InlineData("fooBar")]
        [InlineData("helloWorld")]
        [InlineData("asdfasdf64345746jurfstghnAS+DF)AS(R_AW")]
        [InlineData("1234")]
        public void RoundTripString(string start)
        {
            string encoded = GZip.Encode(start);
            // Check if string is base64 encoded
            Assert.Matches(@"^[a-zA-Z0-9\+/]*={0,2}$", encoded);

            byte[] bytes = Convert.FromBase64String(encoded);
            string finish = GZip.Decode<string>(bytes);

            Assert.Equal(start, finish);
        }

        [Fact]
        public void RoundTripList()
        {
            List<int> start = new List<int> { 1, 3, 4 };
            string encoded = GZip.Encode(start);
            // Check if string is base64 encoded
            Assert.Matches(@"^[a-zA-Z0-9\+/]*={0,2}$", encoded);

            byte[] bytes = Convert.FromBase64String(encoded);
            List<int> finish = GZip.Decode<List<int>>(bytes);

            Assert.Equal(start, finish);
        }
    }
}

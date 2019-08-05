using System;
using System.Collections.Generic;
using Xunit;

namespace Doc.Compression.Tests
{
    public class DeflateTests
    {
        [Theory]
        [InlineData("fooBar")]
        [InlineData("helloWorld")]
        [InlineData("asdfasdf64345746jurfstghnAS+DF)AS(R_AW")]
        [InlineData("1234")]
        public void RoundTripString(string start)
        {
            string encoded = Deflate.Encode(start);
            // Check if string is base64 encoded
            Assert.Matches(@"^[a-zA-Z0-9\+/]*={0,2}$", encoded);

            byte[] bytes = Convert.FromBase64String(encoded);
            string finish = Deflate.Decode<string>(bytes);

            Assert.Equal(start, finish);
        }

        [Fact]
        public void RoundTripList()
        {
            List<int> start = new List<int> { 1, 3, 4 };
            string encoded = Deflate.Encode(start);
            // Check if string is base64 encoded
            Assert.Matches(@"^[a-zA-Z0-9\+/]*={0,2}$", encoded);

            byte[] bytes = Convert.FromBase64String(encoded);
            List<int> finish = Deflate.Decode<List<int>>(bytes);

            Assert.Equal(start, finish);
        }
    }
}

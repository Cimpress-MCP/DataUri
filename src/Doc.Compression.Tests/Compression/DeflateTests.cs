using Doc.Compression.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Doc.Compression.Tests.Compression
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
            byte[] encoded = Deflate.Encode(start);

            byte[] finishBytes = Deflate.Decode(encoded);
            string finish = Encoding.UTF8.GetString(finishBytes);
            Assert.Equal(start, finish);
        }

        [Fact]
        public void RoundTripListObject()
        {
            List<int> start = new List<int> { 1, 3, 4 };
            string startString = JsonConvert.SerializeObject(start);
            byte[] encoded = Deflate.Encode(startString);
 
            byte[] finishBytes = Deflate.Decode(encoded);
            string finishString = Encoding.UTF8.GetString(finishBytes);
            List<int> finish = JsonConvert.DeserializeObject<List<int>>(finishString);
            Assert.Equal(start, finish);
        }
    }
}

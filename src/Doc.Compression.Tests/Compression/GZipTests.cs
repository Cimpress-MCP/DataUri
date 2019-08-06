using Doc.Compression.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Doc.Compression.Tests.Compression
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
            byte[] encoded = GZip.Encode(start);

            byte[] finishBytes = GZip.Decode(encoded);
            string finish = Encoding.UTF8.GetString(finishBytes);
            Assert.Equal(start, finish);
        }

        [Fact]
        public void RoundTripListObject()
        {
            List<int> start = new List<int> { 1, 3, 4 };
            string startString = JsonConvert.SerializeObject(start);
            byte[] encoded = GZip.Encode(startString);

            byte[] finishBytes = GZip.Decode(encoded);
            string finishString = Encoding.UTF8.GetString(finishBytes);
            List<int> finish = JsonConvert.DeserializeObject<List<int>>(finishString);
            Assert.Equal(start, finish);
        }
    }
}

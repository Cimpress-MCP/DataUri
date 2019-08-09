using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Cimpress.DataUri.Compression
{
    public static class GZip
    {
        public const string GZIP = "gzip";

        /// <summary>
        /// Decodes the input payload and handles deflating / base64 decoding if nessecary.
        /// </summary>
        public static byte[] Decode(byte[] input)
        {
            if (TryDecoding(input, out byte[] deflatedPayload))
            {
                return deflatedPayload;
            }
            throw new ArgumentException("Input is not a GZipped");
        }

        /// <summary>
        /// Encodes a payload into a deflated string.
        /// </summary>
        public static byte[] Encode(string payload)
        {
            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(payload)))
            using (var outputStream = new MemoryStream())
            using (var zipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(zipStream);
                zipStream.Close();
                byte[] bytes = outputStream.ToArray();
                return bytes;
            }
        }

        // Tries to deflate an input byte array into a given payload
        public static bool TryDecoding(byte[] input, out byte[] output)
        {
            try
            {
                using (var inputStream = new MemoryStream(input))
                using (var zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                using (var outputStream = new MemoryStream())
                {
                    zipStream.CopyTo(outputStream);

                    output = outputStream.ToArray();
                    return true;
                }
            }
            catch
            {
                output = default;
                return false;
            }
        }
    }
}

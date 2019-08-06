using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Doc.Compression.Compression
{
    public static class Deflate
    {
        public const string DEFLATE = "deflate";

        /// <summary>
        /// Defaltes the bytes to an object
        /// </summary>
        public static byte[] Decode(byte[] input)
        {
            if (TryDecoding(input, out byte[] deflatedPayload))
            {
                return deflatedPayload;
            }
            throw new ArgumentException("Input is not a deflated");
        }

        /// <summary>
        /// Encodes a payload into a deflated string. Base64 encoded by default
        /// </summary>
        public static byte[] Encode(string payload)
        {
            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(payload)))
            using (var outputStream = new MemoryStream())
            using (var zipStream = new DeflateStream(outputStream, CompressionMode.Compress))
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
                using (var zipStream = new DeflateStream(inputStream, CompressionMode.Decompress))
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

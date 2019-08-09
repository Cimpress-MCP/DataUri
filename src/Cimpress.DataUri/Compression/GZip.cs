using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Cimpress.DataUri.Compression
{
    /// <summary>
    /// Class to handle encoding and decoding byte array with the gzip algorithm
    /// </summary>
    public static class GZip
    {
        public const string GZIP = "gzip";

        /// <summary>
        /// Decodes a Gziped byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Decoded byte array</returns>
        public static byte[] Decode(byte[] input)
        {
            if (TryDecoding(input, out byte[] deflatedPayload))
            {
                return deflatedPayload;
            }
            throw new ArgumentException("Input is not a GZipped");
        }

        /// <summary>
        /// Encodes a string using the Gzip algorithm
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>Encoded byte array</returns>
        public static byte[] Encode(string payload)
        {
            return Encode(Encoding.UTF8.GetBytes(payload));
        }

        /// <summary>
        /// Encodeds a byte array using the Gzip algorithm
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static byte[] Encode(byte[] payload)
        {
            using (var inputStream = new MemoryStream(payload))
            using (var outputStream = new MemoryStream())
            using (var zipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(zipStream);
                zipStream.Close();
                byte[] bytes = outputStream.ToArray();
                return bytes;
            }
        }

        /// <summary>
        /// Attempt to decode a byte array using the Gzip algorithm
        /// </summary>
        /// <param name="input">byte array to decode</param>
        /// <param name="output">decoded byte array</param>
        /// <returns>If decoding was successful</returns>
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

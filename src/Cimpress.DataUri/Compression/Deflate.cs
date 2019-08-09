using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Cimpress.DataUri.Compression
{
    /// <summary>
    /// Class to handle encoding and decoding byte array with the defalte algorithm
    /// </summary>
    public static class Deflate
    {
        public const string DEFLATE = "deflate";

        /// <summary>
        /// Decode an encoded byte array
        /// </summary>
        /// <param name="input">Byte array encoded with the deflate algorithm</param>
        /// <returns></returns>
        public static byte[] Decode(byte[] input)
        {
            if (TryDecoding(input, out byte[] deflatedPayload))
            {
                return deflatedPayload;
            }
            throw new ArgumentException("Input is not a deflated");
        }

        /// <summary>
        /// Use Deflate to encode a string
        /// </summary>
        /// <param name="payload">string to encode with deflate algorithm</param>
        /// <returns></returns>
        public static byte[] Encode(string payload)
        {
            return Encode(Encoding.UTF8.GetBytes(payload));
        }

        /// <summary>
        /// Use Deflate to encode a byteArray
        /// </summary>
        /// <param name="payload">byte array to encode with deflate algorithm</param>
        /// <returns></returns>
        public static byte[] Encode(byte[] payload)
        {
            using (var inputStream = new MemoryStream(payload))
            using (var outputStream = new MemoryStream())
            using (var zipStream = new DeflateStream(outputStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(zipStream);
                zipStream.Close();
                byte[] bytes = outputStream.ToArray();
                return bytes;
            }
        }

        /// <summary>
        /// Attempt to decode a byte array using the deflate algorithm
        /// </summary>
        /// <param name="input">byte array to decode</param>
        /// <param name="output">decoded byte array</param>
        /// <returns>If decoding was successful</returns>
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

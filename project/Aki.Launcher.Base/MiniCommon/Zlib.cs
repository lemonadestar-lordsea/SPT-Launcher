using System;
using System.IO;
using ComponentAce.Compression.Libs.zlib;

namespace Aki.Launcher.MiniCommon
{
	public enum ZlibCompression
	{
		Store = 0,
		Fastest = 1,
		Fast = 3,
		Normal = 5,
		Ultra = 7,
		Maximum = 9
	}

	public static class Zlib
	{
		// Level |     hex CM/CI FLG    | byte[]
		// 1     | 78 01                | 120 1
		// 2     | 78 5E                | 120 94
		// 3     | 78 5E			    | 120 94
		// 4     | 78 5E			    | 120 94
		// 5     | 78 5E			    | 120 94
		// 6     | 78 9C			    | 120 156
		// 7     | 78 DA			    | 120 218
		// 8     | 78 DA			    | 120 218
		// 9     | 78 DA			    | 120 218

		private const byte CompressionMethodHeader = 120;
		private const byte FastestCompressionHeader = 1;
		private const byte LowCompressHeader = 94;
		private const byte NormalCompressHeader = 156;
		private const byte MaxCompressHeader = 218;

		public static bool CheckHeader(byte[] Data)
		{
			//we need the first two data, if they arn't there, just return false.
			//(first byte) Compression Method / Info (CM/CINFO) Header should always be 120
			if (Data == null || Data.Length < 3 || Data[0] != CompressionMethodHeader)
			{
				return false;
			}

			//(second byte) Flags (FLG) Header, should define our compression level.
			switch (Data[1])
			{
				case FastestCompressionHeader:
				case LowCompressHeader:
				case NormalCompressHeader:
				case MaxCompressHeader:
					return true;
			}

			return false;
		}

        private byte[] Run(byte[] data, bool compress, ZlibCompression level = ZlibCompression.Store)
        {
            using (var msOut = new MemoryStream())
            {
                // ZOutputStream.Close() flushes itself.
                // ZOutputStream.Flush() flushes the target stream.
                // It's fucking stupid, but whatever.
                // -- Waffle.Lord, 2022-12-01

                using (var zsOut = (compress)
                    ? new ZOutputStream(msOut, (int)level)
                    : new ZOutputStream(msOut))
                {
                    await zsOut.WriteAsync(data, 0, data.Length);
                }

                return msOut.ToArray();
            }
        }

        /// <summary>
        /// Deflate data.
        /// </summary>
        public byte[] Compress(byte[] data, ZlibCompression level)
        {
            return Run(data, true, level);
        }

        /// <summary>
		/// Inflate data.
		/// </summary>
        public byte[] Decompress(byte[] data)
        {
            return Run(data, false);
        }
	}
}
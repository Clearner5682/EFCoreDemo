using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Utils
{
    public static class StreamHelper
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            List<byte> bytes = new List<byte>();
            byte[] buffer = new byte[100];
            int length = 0;
            // offset:缓冲区的偏移量
            // count:每次从stream读取的字节长度
            while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                bytes.AddRange(buffer.Take(length));
            }

            return bytes.ToArray();
        }
    }
}

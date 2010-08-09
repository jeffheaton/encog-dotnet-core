using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Encog.Util
{
    public static class FileUtil
    {
        public static int CopyStream(Stream input, Stream output)
        {
            int result = 0;

            byte[] buffer = new byte[32768];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return result;
                output.Write(buffer, 0, read);
                result += read;
            }

            return result;
        }

    }
}

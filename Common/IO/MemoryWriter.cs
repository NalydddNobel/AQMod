using System.IO;

namespace AQMod.Common.IO
{
    internal class MemoryWriter : BinaryWriter
    {
        public MemoryWriter(int capacity = byte.MaxValue + 1) : base(new MemoryStream(capacity))
        {
        }

        /// <summary>
        /// Returns an output of bytes, automatically disposes the object.
        /// </summary>
        /// <returns></returns>
        public byte[] Output()
        {
            byte[] buffer = ((MemoryStream)OutStream).GetBuffer();
            Dispose();
            return buffer;
        }
    }
}
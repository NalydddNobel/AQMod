using System.Runtime.InteropServices;

namespace Aequus.Common.Utilities.Helpers;

internal class BitsConvert {
    [StructLayout(LayoutKind.Explicit)]
    public struct Bit8 {
        [FieldOffset(0)]
        public byte ToByte;
        [FieldOffset(0)]
        public sbyte ToSByte;

        public Bit8(byte value) {
            ToByte = value;
        }

        public Bit8(sbyte value) {
            ToSByte = value;
        }
    }
}

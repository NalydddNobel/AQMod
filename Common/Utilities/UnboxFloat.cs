namespace Aequus.Common.Utilities
{
    public struct UnboxFloat : ITypeUnboxer<float>
    {
        public bool TryUnbox(object obj, out float value)
        {
            value = default(float);
            if (obj is float)
            {
                value = (float)obj;
                return true;
            }
            if (obj is uint)
            {
                value = (uint)obj;
                return true;
            }
            if (obj is byte)
            {
                value = (byte)obj;
                return true;
            }
            if (obj is sbyte)
            {
                value = (sbyte)obj;
                return true;
            }
            if (obj is ushort)
            {
                value = (ushort)obj;
                return true;
            }
            if (obj is short)
            {
                value = (short)obj;
                return true;
            }
            if (obj is int)
            {
                value = (int)obj;
                return true;
            }
            if (obj is double)
            {
                value = (float)(double)obj;
                return true;
            }
            return false;
        }
    }
}

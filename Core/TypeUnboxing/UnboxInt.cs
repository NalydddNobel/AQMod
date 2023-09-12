namespace Aequus.Core.TypeUnboxing;

public class UnboxInt : TypeUnboxer<int> {
    public override bool TryUnbox(object obj, out int value) {
        value = default(int);
        if (obj is int) {
            value = (int)obj;
            return true;
        }
        if (obj is uint) {
            value = (int)(uint)obj;
            return true;
        }
        if (obj is byte) {
            value = (byte)obj;
            return true;
        }
        if (obj is sbyte) {
            value = (sbyte)obj;
            return true;
        }
        if (obj is ushort) {
            value = (ushort)obj;
            return true;
        }
        if (obj is short) {
            value = (short)obj;
            return true;
        }
        if (obj is float) {
            value = (int)(float)obj;
            return true;
        }
        if (obj is double) {
            value = (int)(double)obj;
            return true;
        }
        return false;
    }
}

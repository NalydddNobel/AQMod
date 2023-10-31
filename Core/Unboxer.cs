using Terraria.ModLoader;

namespace Aequus.Core;

public abstract class Unboxer<T> : ILoadable {
    public static Unboxer<T> Instance { get; private set; }

    public void Load(Mod mod) {
        Instance = this;
    }
    public void Unload() {
        Instance = null;
    }

    public abstract bool TryUnbox(object obj, out T value);
}

public class Unboxers {
    public class UnboxBoolean : Unboxer<bool> {
        public override bool TryUnbox(object obj, out bool value) {
            value = default(bool);
            if (obj is bool boolean) {
                value = boolean;
                return true;
            }
            if (Unboxer<int>.Instance.TryUnbox(obj, out int zeroOne) && (zeroOne == 0 || zeroOne == 1)) {
                value = zeroOne == 1;
                return true;
            }
            return false;
        }
    }
    public class UnboxFloat : Unboxer<float> {
        public override bool TryUnbox(object obj, out float value) {
            value = default(float);
            if (obj is float) {
                value = (float)obj;
                return true;
            }
            if (obj is uint) {
                value = (uint)obj;
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
            if (obj is int) {
                value = (int)obj;
                return true;
            }
            if (obj is double) {
                value = (float)(double)obj;
                return true;
            }
            return false;
        }
    }
    public class UnboxInt : Unboxer<int> {
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
}
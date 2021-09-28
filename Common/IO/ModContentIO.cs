namespace AQMod.Common.IO
{
    public abstract class ModContentIO<T>
    {
        public abstract string GetKey(int type);
        public abstract string GetKey(T value);
        public abstract int GetID(string key);
    }
}
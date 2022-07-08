namespace Aequus.Common.Utilities
{
    public struct ArrayInterpreter<T>
    {
        public T[] Arr;

        public ArrayInterpreter(T value)
        {
            Arr = new T[1] { value };
        }

        public ArrayInterpreter(T[] value)
        {
            Arr = value;
        }

        public static implicit operator ArrayInterpreter<T>(T value)
        {
            return new ArrayInterpreter<T>(value);
        }

        public static implicit operator ArrayInterpreter<T>(T[] value)
        {
            return new ArrayInterpreter<T>(value);
        }
    }
}
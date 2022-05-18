namespace Aequus.Common.Utilities
{
    public struct NullInput<T>
    {
        private readonly bool useValue;
        private readonly T value;

        public NullInput(T value)
        {
            useValue = true;
            this.value = value;
        }

        public T Get(T self)
        {
            return useValue ? value : self;
        }

        public static implicit operator NullInput<T>(bool useless)
        {
            return new NullInput<T>();
        }
        public static implicit operator NullInput<T>(T value)
        {
            return new NullInput<T>(value);
        }
    }
}

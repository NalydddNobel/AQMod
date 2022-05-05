namespace Aequus.Common
{
    public struct NullableOption<T>
    {
        private readonly bool useValue;
        private readonly T value;

        public NullableOption(T value)
        {
            useValue = true;
            this.value = value;
        }

        public T Get(T self)
        {
            return useValue ? value : self;
        }

        public static implicit operator NullableOption<T>(bool useless)
        {
            return new NullableOption<T>();
        }
        public static implicit operator NullableOption<T>(T value)
        {
            return new NullableOption<T>(value);
        }
    }
}

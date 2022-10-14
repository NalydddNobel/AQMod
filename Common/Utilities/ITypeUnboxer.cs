namespace Aequus.Common.Utilities
{
    public interface ITypeUnboxer<T>
    {
        bool TryUnbox(object obj, out T value);
        T Unbox(object obj)
        {
            TryUnbox(obj, out T value);
            return value;
        }
    }
}
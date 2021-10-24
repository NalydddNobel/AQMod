namespace AQMod.Common.Utilities
{
    public class OffSet<TValue> // I couldn't resist calling it this
    {
        private TValue[] _values;
        private int offsetID;

        public OffSet(int count, int offset)
        {
            _values = new TValue[count - offset];
            offsetID = offset;
        }

        public TValue this[int id]
        {
            get => _values[id - offsetID];
            set => _values[id - offsetID] = value;
        }
    }
}
namespace Aequus.Common.Utilities.TypeUnboxing
{
    public class UnboxBoolean : TypeUnboxer<bool>
    {
        public override bool TryUnbox(object obj, out bool value)
        {
            value = default(bool);
            if (obj is bool)
            {
                value = (bool)obj;
                return true;
            }
            if (AequusHelpers.UnboxInt.TryUnbox(obj, out int zeroOne) && (zeroOne == 0 || zeroOne == 1))
            {
                value = zeroOne == 1;
                return true;
            }
            return false;
        }
    }
}

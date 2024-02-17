namespace Aequus.Core;

public record class RefManipulator<T>(RefManipulator<T>.GetReferenceDelegate GetReference) {
    public delegate ref T GetReferenceDelegate();

    private T _originalValue;
    private bool _hasOverridenValue;

    /// <summary>Replaces the referenced value with <paramref name="value"/>. The original value will be cached, and can be restored using <see cref="VoidOverriddenValue(ref T)"/>.</summary>
    /// <param name="value"></param>
    public void OverrideValue(T value) {
        if (!_hasOverridenValue) {
            _originalValue = GetReference();
            _hasOverridenValue = true;
        }

        GetReference() = value;
    }

    /// <summary>Restores overriden values back to their original value.</summary>
    public void VoidOverriddenValue() { 
        if (_hasOverridenValue) {
            GetReference() = _originalValue;
            _hasOverridenValue = false;
        }
    }
}

public class CommonRefManipulators {
    public static readonly RefManipulator<bool> BloodMoon = new RefManipulator<bool>(() => ref Main.bloodMoon);
    public static readonly RefManipulator<bool> Eclipse = new RefManipulator<bool>(() => ref Main.eclipse);
    public static readonly RefManipulator<bool> DayTime = new RefManipulator<bool>(() => ref Main.dayTime);
    public static readonly RefManipulator<double> Time = new RefManipulator<double>(() => ref Main.time);
}
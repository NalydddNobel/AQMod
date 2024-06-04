using Aequus.Core.Structures;

namespace Aequus.Core;

public record class ReferenceCache<T>(RefFunc<T> GetReference) {
    private T _originalValue;
    private bool _hasOverridenValue;

    /// <summary>Replaces the referenced value with <paramref name="value"/>. The original value will be cached, and can be restored using <see cref="VoidOverriddenValue(ref T)"/>.</summary>
    /// <param name="value"></param>
    public void OverrideValue(T value) {
        if (!_hasOverridenValue) {
            _originalValue = GetReference();
            _hasOverridenValue = true;
        }
        //else {
        //    Aequus.Instance.Logger.Info(Environment.StackTrace);
        //}

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

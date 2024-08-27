namespace SourceGenerators;

internal readonly struct ParameterInfo {
    public ParameterInfo(string fullDisplay, string name, string fullyQualifiedType, string defaultValue = "") {
        FullDisplay = fullDisplay;
        Name = name;
        FullyQualifiedType = fullyQualifiedType;
        DefaultValue = defaultValue;
    }

    public readonly string FullDisplay;
    public readonly string Name;
    public readonly string FullyQualifiedType;
    public readonly string DefaultValue;

    public bool HasDefaultValue => !string.IsNullOrEmpty(DefaultValue);
}
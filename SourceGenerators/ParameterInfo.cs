namespace SourceGenerators;

internal readonly struct ParameterInfo {
    public ParameterInfo(string fullDisplay, string name, string fullyQualifiedType) {
        FullDisplay = fullDisplay;
        Name = name;
        FullyQualifiedType = fullyQualifiedType;
    }

    public readonly string FullDisplay;
    public readonly string Name;
    public readonly string FullyQualifiedType;
}
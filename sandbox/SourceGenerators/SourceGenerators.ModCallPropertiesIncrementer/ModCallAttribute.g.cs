namespace Aequus;
    
[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ModCallAttribute(string Name = "") : System.Attribute {
    public readonly string Name = Name;
}
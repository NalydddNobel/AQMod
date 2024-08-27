namespace Aequus;
    
[System.AttributeUsage(System.AttributeTargets.Method)]
public class ModCallAttribute(string Name) : System.Attribute {
    public readonly string Name = Name;
}
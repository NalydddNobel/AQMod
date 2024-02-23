using Publicizer.Annotation;

namespace TerrariaProxy;

[Publicize(typeof(TestClass))]
public partial class TestClassProxy {
}

public class TestClass {
    private int _value;
    public int Value => _value;

    public TestClass(int value) { _value = value; }
}

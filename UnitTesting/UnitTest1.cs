using Aequus.Core.CodeGeneration;
using System.Reflection;

namespace UnitTesting;

[TestClass]
public class UnitTest1 {
    const int Value = 0;
    const int Iterations = 100000000;

    [TestMethod]
    public void OptimalTest() {
        for (int i = 0; i < Iterations; i++) {
            var testClass = new OptimalTestClass(Value);
            int value = testClass._value;
            Assert.AreEqual(value, Value);
        }
    }

    [TestMethod]
    public void NewPublicizationTest() {
        for (int i = 0; i < Iterations; i++) {
            var testClass = new TerrariaProxy.TestClassProxy(new TerrariaProxy.TestClass(Value));
            int value = testClass._value;
            Assert.AreEqual(value, Value);
        }
    }

    [TestMethod]
    public void LegacyPublicizationTest() {
        for (int i = 0; i < Iterations; i++) {
            TestClass testClass = new TestClass(Value);
            int value = Publicization<TestClass, int>.Get(testClass, "_value");
            Assert.AreEqual(value, Value);
        }
    }

    [TestMethod]
    public void FieldInfoTest() {
        for (int i = 0; i < Iterations; i++) {
            TestClass testClass = new TestClass(Value);
            int value = (int)typeof(TestClass).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(testClass);
            Assert.AreEqual(value, Value);
        }
    }

    private FieldInfo _fieldInfoCache;
    [TestMethod]
    public void FieldInfoCachedTest() {
        for (int i = 0; i < Iterations; i++) {
            TestClass testClass = new TestClass(Value);
            _fieldInfoCache ??= typeof(TestClass).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
            int value = (int)_fieldInfoCache.GetValue(testClass);
            Assert.AreEqual(value, Value);
        }
    }

    public class TestClass {
        private int _value;
        public int Value => _value;

        public TestClass(int value) { _value = value; }
    }

    public class OptimalTestClass {
        public int _value;

        public OptimalTestClass(int value) { _value = value; }
    }
}
using System.Reflection;

namespace UITest.Models
{
    public class TestMethod<T>: Item
    {
        public T? Value { get; set; }
        public MethodInfo? Info { get; set; }
    }
}

using System.Reflection;
using PassXYZLib;

namespace UITest.Models
{
    public class TestMethod<T>: NewItem
    {
        public T? Value { get; set; }
        public MethodInfo? Info { get; set; }
    }
}

using System.Reflection;

namespace UITest.Services
{
    public class TestRunner<T>
    {
        private static bool ShouldContinue(TestCaseAttribute attribute)
        {
            if (attribute.Explicit)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(attribute.Skip))
            {
                Console.WriteLine(attribute.Skip);
                return true;
            }
            if (!string.IsNullOrEmpty(attribute.SkipUnless))
            {
                return true;
            }
            if (!string.IsNullOrEmpty(attribute.SkipWhen))
            {
                return true;
            }
            if (attribute.SkipType != null)
            {
                return true;
            }
            if (attribute.Timeout != 0)
            {
                return true;
            }
            return false;
        }
        public void InvokeTests(T target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            foreach (var methodInfo in target.GetType().GetMethods())
            {
                var attributes = methodInfo.GetCustomAttributes(typeof(TestCaseAttribute), false);
                if (attributes.Length == 0)
                {
                    continue;
                }
                var attribute = (TestCaseAttribute)attributes[0];
                if (TestRunner<T>.ShouldContinue(attribute))
                {
                    continue;
                }
                methodInfo.Invoke(target, null);
            }
        }

        public IEnumerable<MethodInfo> GetTestMethods(T target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            foreach (var methodInfo in target.GetType().GetMethods())
            {
                var attributes = methodInfo.GetCustomAttributes(typeof(TestCaseAttribute), false);
                if (attributes.Length == 0)
                {
                    continue;
                }
                var attribute = (TestCaseAttribute)attributes[0];
                if (TestRunner<T>.ShouldContinue(attribute))
                {
                    continue;
                }
                yield return methodInfo;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UITest.Services
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestCaseAttribute : Attribute
    {
        /// <inheritdoc/>
        public string? DisplayName { get; set; }

        /// <inheritdoc/>
        public bool Explicit { get; set; }

        /// <inheritdoc/>
        public string? Skip { get; set; }

        /// <inheritdoc/>
        public Type? SkipType { get; set; }

        /// <inheritdoc/>
        public string? SkipUnless { get; set; }

        /// <inheritdoc/>
        public string? SkipWhen { get; set; }

        /// <inheritdoc/>
        public int Timeout { get; set; }

    }
}

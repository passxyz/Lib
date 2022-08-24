using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZLib
{
    public interface IKeyValue
    {
        string Key { get; set; }
        string Value { get; set; }
        bool IsValid { get => !(string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Value)); }
    }
}

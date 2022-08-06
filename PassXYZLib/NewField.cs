using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZLib
{
    public class NewField : Field
    {
        public NewField(string key = "", string value="", bool isProtected=false) : base(key, value, isProtected)
        {
        }
    }
}

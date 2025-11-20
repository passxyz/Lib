using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZLib.Resources
{
    public enum FontType
    {
        FontAwesomeBrands,
        FontAwesomeRegular,
        FontAwesomeSolid
    }

    static public class FontData
    {
        public static readonly Dictionary<string, Type> FontFamily = new Dictionary<string, Type>()
        {
            { nameof(FontType.FontAwesomeBrands), typeof(FontAwesomeBrands)},
            { nameof(FontType.FontAwesomeRegular), typeof(FontAwesomeRegular)},
            { nameof(FontType.FontAwesomeSolid), typeof(FontAwesomeSolid) }
        };

        public static Dictionary<string, string> GetGlyphs(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var dictionary = new Dictionary<string, string>();

            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    dictionary.Add(field.Name, (string)field.GetRawConstantValue());
                }
            }

            return dictionary;
        }
    }
}

using OHC.EAMI.Common.FixedLengthFileGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.Common.FixedLengthFileParser
{
    public abstract class FixedLengthFileParser
    {

        char _paddingChar = (char)0x20;

        public void Parse (string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                foreach (PropertyInfo property in GetType().GetProperties())
                {
                    foreach (Attribute attribute in property.GetCustomAttributes(true))
                    {
                        var stringLayoutAttribute = attribute as FixedLengthFileFieldAttributes;
                        if (null != stringLayoutAttribute)
                        {
                            string tmp = string.Empty;
                            if (stringLayoutAttribute.Position <= input.Length - 1)
                            {
                                tmp = input.Substring(stringLayoutAttribute.Position - 1, stringLayoutAttribute.ByteLength);
                            }

                            property.SetValue(this, tmp, null);
                            break;
                        }
                    }
                }
            }
        }       
    }
}

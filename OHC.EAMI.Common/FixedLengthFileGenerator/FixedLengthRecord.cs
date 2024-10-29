using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OHC.EAMI.Common.FixedLengthFileGenerator
{
    internal class SerializableProperty
    {
        public int Position { get; set; }
        public PropertyInfo Property { get; set; }
        public FixedLengthFileFieldAttributes FixedLengthAttribute { get; set; }

        public SerializableProperty(int pos, PropertyInfo prop, FixedLengthFileFieldAttributes attrib)
        {
            Position = pos;
            Property = prop;
            FixedLengthAttribute = attrib;
        }

        public static int Compare(SerializableProperty obj1, SerializableProperty obj2)
        {
            if (obj1 != null && obj2 != null)
            {
                return obj1.Position.CompareTo(obj2.Position);
            }
            else
            {
                if (obj1 == null && obj2 == null)
                {
                    return 0;
                }
                return (obj1 == null) ? 1 : -1;
            }
        }
    }

    public class FixedLengthRecord
    {
        private static string defaultDateFormat = "yyyyMMdd";

        public static string ConvertFixedLength<T>(T classObject)
        {
            Type classType = typeof(T);
            Type attributeClassType = typeof(FixedLengthFileFieldAttributes);

            List<SerializableProperty> schemaList = GetOrderClassSchema<T>(classObject);
            schemaList.Sort(SerializableProperty.Compare);
            return CreateRow<T>(classObject, schemaList);
        }

        private static string CreateRow<T>(T classObject, List<SerializableProperty> schemaList)
        {
            StringBuilder returnData = new StringBuilder();
            for (int index = 0; index < schemaList.Count(); ++index)
            {
                FixedLengthFileFieldAttributes attrib = schemaList[index].FixedLengthAttribute;

                object obj = schemaList[index].Property.GetValue(classObject, null);

                if (schemaList[index].Property.PropertyType == typeof(String) || schemaList[index].Property.PropertyType == typeof(uint))
                {
                    char filler = schemaList[index].Property.PropertyType == typeof(uint) ? (char)0x30 : attrib.DefaultFiler;
                    string value = obj != null ? obj.ToString() : string.Empty;
                    value = TransformValue(value, attrib.ByteLength, filler);
                    returnData.Append(value);
                }
                else if (schemaList[index].Property.PropertyType == typeof(char))
                {
                    char[] ch = new char[1];
                    ch[0] = (obj != null && (char)obj != '\0') ? Convert.ToChar(obj) : (char)attrib.DefaultFiler;
                    returnData.Append(ch[0]);
                }
                else if (schemaList[index].Property.PropertyType == typeof(DateTime?) || schemaList[index].Property.PropertyType == typeof(DateTime))
                {
                    returnData.Append(obj != null ? (Convert.ToDateTime(obj.ToString())).ToString(string.IsNullOrEmpty(attrib.DateFormat) ? defaultDateFormat : attrib.DateFormat) : null);
                }
                else if (schemaList[index].Property.PropertyType == typeof(byte))
                {
                    short value = Convert.ToInt16(obj);
                    returnData.Append((byte)(value >> 8));
                    returnData.Append((byte)(value & 0xFF));
                }
                else if (schemaList[index].Property.PropertyType == typeof(Decimal) || schemaList[index].Property.PropertyType == typeof(Int32) || schemaList[index].Property.PropertyType == typeof(Int64))
                {
                    decimal value = (obj != null) ? Convert.ToDecimal(obj.ToString()) : 0;
                    decimal beforeDecimal = 0, afterDecimal = 0;
                    SplitDecimal(value, ref beforeDecimal, ref afterDecimal);
                    string integral = RightJustifyDigits(beforeDecimal, attrib.DigitsBeforeDecimal);
                    string mentisssa = LeftJustifyDigits(afterDecimal, attrib.DigitsAfterDecimal);
                    returnData.Append(integral + mentisssa);
                }
                if (attrib.FollowingPlaceholderByteLength > 0)
                {
                    string value = string.Empty.PadRight(attrib.FollowingPlaceholderByteLength, (char)0x20);
                    returnData.Append(value);
                }
            }
            return returnData.ToString();
        }

        private static List<SerializableProperty> GetOrderClassSchema<T>(T classObject)
        {
            List<SerializableProperty> returnType = new List<SerializableProperty>();

            Type attributeClassType = typeof(FixedLengthFileFieldAttributes);
            Type classType = typeof(T);
            Dictionary<int, bool> positionDirectory = new Dictionary<int, bool>();

            PropertyInfo[] properties = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (PropertyInfo property in properties)
            {
                if (Attribute.IsDefined(property, attributeClassType))
                {
                    FixedLengthFileFieldAttributes attributeObject = ((FixedLengthFileFieldAttributes)property.GetCustomAttributes(attributeClassType, false)[0]);

                    if (attributeObject.Position <= 0)
                    {
                        throw new Exception("Position must be greater than 1. Check proprty " + property.Name);
                    }

                    if (attributeObject.ByteLength <= 0)
                    {
                        throw new Exception("Byte length must be greater than 1. Check proprty " + property.Name);
                    }

                    if ((property.PropertyType == typeof(Decimal) || property.PropertyType == typeof(Int32) || property.PropertyType == typeof(Int64)) && ((attributeObject.DigitsAfterDecimal == -1 || attributeObject.DigitsBeforeDecimal == -1)))
                    {
                        throw new Exception("DigitsBeforeDecimal and DigitsAfterDecimal are required for a numeric property. Check proprty " + property.Name);
                    }

                    if (!positionDirectory.ContainsKey(attributeObject.Position))
                    {
                        positionDirectory.Add(attributeObject.Position, true);
                    }
                    else
                    {
                        throw new Exception("Duplicate position value. Check proprty " + property.Name);
                    }
                    returnType.Add(new SerializableProperty(attributeObject.Position, property, attributeObject));
                }
            }
            return returnType;
        }

        private static string TransformValue(string value, int maxLength, char filer)
        {
            string returnValue = string.IsNullOrEmpty(value) ? string.Empty : value;

            return returnValue.Length > maxLength ? returnValue.Substring(0, maxLength) : (filer == (char)0x30 ? returnValue.PadLeft(maxLength, filer) : returnValue.PadRight(maxLength, filer));
        }

        private static void SplitDecimal(decimal value, ref decimal integral, ref decimal fractional)
        {
            integral = Math.Truncate(value);
            fractional = 0;
            string[] str = (value - integral).ToString().Split('.');
            if (str.Length >= 2)
            {
                Decimal.TryParse(str[1], out fractional);
            }
        }

        private static string LeftJustifyDigits(decimal value, int maxDigitsAllowed)
        {
            string newValue = value.ToString();
            bool negative = false;
            int actualDigits = newValue.Length;
            if (actualDigits < maxDigitsAllowed)
            {
                if (value < 0)
                {
                    negative = true;
                    value *= -1m;
                }
                newValue = value.ToString().PadRight(maxDigitsAllowed, (char)0x30);
                if (negative)
                {
                    newValue = "-" + newValue;
                }
            }
            else
            {
                newValue = JustifyDigits(value, maxDigitsAllowed);
            }
            return newValue;
        }

        private static string RightJustifyDigits(decimal value, int maxDigitsAllowed)
        {
            string newValue = value.ToString();
            bool negative = false;
            int actualDigits = newValue.Length;
            if (actualDigits < maxDigitsAllowed)
            {
                if (value < 0)
                {
                    negative = true;
                    value *= -1m;
                }
                newValue = value.ToString().PadLeft(maxDigitsAllowed, (char)0x30);
                if (negative)
                {
                    newValue = "-" + newValue;
                }
            }
            else
            {
                newValue = JustifyDigits(value, maxDigitsAllowed);
            }
            return newValue;
        }

        private static string JustifyDigits(decimal value, int maxDigitsAllowed)
        {
            bool negative = false;
            string newValue = value.ToString();

            int actualDigits = newValue.Length;
            if (actualDigits > maxDigitsAllowed)
            {
                if (value < 0)
                {
                    negative = true;
                    value *= -1m;
                }
                newValue = value.ToString().Substring(0, maxDigitsAllowed);
                if (negative)
                {
                    newValue = "-" + newValue;
                }
            }
            return newValue;
        }
    }
}

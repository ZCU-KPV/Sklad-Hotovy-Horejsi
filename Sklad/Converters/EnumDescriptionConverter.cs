using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection; // Důležité pro čtení atributů
using System.Windows.Data;

namespace Sklad.Converters // Zkontroluj si, že namespace odpovídá tvé struktuře
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            // 1. Převedeme hodnotu na Enum
            Enum myEnum = (Enum)value;

            // 2. Získáme popis
            string description = GetEnumDescription(myEnum);
            return description;
        }

        // ConvertBack nepotřebujeme, protože data jen zobrazujeme
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // Pomocná metoda, která pomocí Reflection přečte atribut [Description]
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            if (fieldInfo == null) return enumObj.ToString(); // Fallback

            object[] attribArray = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString(); // Fallback, pokud není atribut
            }

            DescriptionAttribute attrib = (DescriptionAttribute)attribArray[0];
            return attrib.Description;
        }
    }
}
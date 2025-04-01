using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace API_PCC.Utils
{
    public static class DataRowToObject
    {

        // Convert DataRow to type T
        public static T ToObject<T>(this DataRow dataRow)
   where T : new()
        {
            T item = new T();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                string columnName = column.ColumnName.Replace("_", "");
                PropertyInfo property = GetProperty(typeof(T), columnName);

                if (property != null && dataRow[column] != DBNull.Value && dataRow[column].ToString() != "NULL")
                {
                    property.SetValue(item, ChangeType(dataRow[column], property.PropertyType), null);
                }
            }

            return item;
        }


        // Get Attribute from the specified Entity Type, added BindingFlags Filter to ignore case
        // Parameters:
        //    Type - Entity
        //    AttributeName - columnName
        private static PropertyInfo GetProperty(Type type, string attributeName)
        {
            PropertyInfo property = type.GetProperty(attributeName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                return property;
            }

            return type.GetProperties()
                 .Where(p => p.IsDefined(typeof(DisplayAttribute), false) && p.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name == attributeName)
                 .FirstOrDefault();
        }

        public static object ChangeType(object value, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
            }

            return Convert.ChangeType(value, type);
        }
    }
}

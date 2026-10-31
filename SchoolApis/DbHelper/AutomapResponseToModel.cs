using System.Data;

namespace SchoolApis.DbHelper
{
    public static class AutomapResponseToModel
    {
        public static T Map<T>(this IDataRecord record) where T : new()
        {
            var obj = new T();
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                if (!record.HasColumn(prop.Name)) continue;

                var val = record[prop.Name];
                if (val != DBNull.Value)
                    prop.SetValue(obj, val);
            }

            return obj;
        }

        public static bool HasColumn(this IDataRecord record, string columnName)
        {
            for (int i = 0; i < record.FieldCount; i++)
                if (record.GetName(i).Equals(columnName,
                    StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }

}

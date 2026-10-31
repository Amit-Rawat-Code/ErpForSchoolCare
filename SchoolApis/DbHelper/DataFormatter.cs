using System.Data;

namespace SchoolApis.DbHelper
{
    public static class DataFormatter
    {

        // DataTable -> List<Dictionary<string, object>>
        public static List<Dictionary<string, object?>> ToDictionaryList(this DataTable table)
        {
            var list = new List<Dictionary<string, object?>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object?>();

                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }

                list.Add(dict);
            }

            return list;
        }

        // ------------ TO USE ABOVE METHOD --------------
        // DataTable dt = result;
        // var response = dt.ToDictionaryList();

        // DataTable -> Dictionary (Single Row)
        public static Dictionary<string, object?> ToSingleDictionary(this DataTable table)
        {
            var dict = new Dictionary<string, object?>();

            if (table.Rows.Count == 0)
                return dict;

            var row = table.Rows[0];

            foreach (DataColumn col in table.Columns)
            {
                dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
            }

            return dict;
        }

        // ------------ TO USE ABOVE METHOD --------------
        // var menu = dt.ToSingleDictionary();

        // DataTable -> List<T> (Strongly Typed)
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties();

            foreach (DataRow row in table.Rows)
            {
                var obj = new T();

                foreach (var prop in properties)
                {
                    if (!table.Columns.Contains(prop.Name) || row[prop.Name] == DBNull.Value)
                        continue;

                    prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType));
                }

                list.Add(obj);
            }

            return list;
        }

        // ------------ TO USE ABOVE METHOD --------------
        // List<MenuDto> menus = dt.ToList<MenuDto>();

        // DataSet -> Dictionary (Multiple Result Sets)
        public static Dictionary<string, object> ToDictionary(this DataSet ds)
        {
            var result = new Dictionary<string, object>();

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                var tableName = string.IsNullOrEmpty(ds.Tables[i].TableName)
                    ? $"ResultSet{i + 1}"
                    : ds.Tables[i].TableName;

                result[tableName] = ds.Tables[i].ToDictionaryList();
            }

            return result;
        }

        // ------------ TO USE ABOVE METHOD --------------
        // DataSet ds = await db.ExecuteDataSetAsync("sp_MenuList");
        // var response = ds.ToDictionary();
    }
}

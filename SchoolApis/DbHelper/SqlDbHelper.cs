using Microsoft.Data.SqlClient;
using System.Data;

namespace SchoolApis.DbHelper
{    
    public sealed class SqlDbHelper
    {
        private readonly string _connectionString;

        public SqlDbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("dbcs")!;
        }

        // SELECT SINGLE RESULTSET
        public async Task<DataTable> ExecuteDataTableAsync(string commandText,CommandType commandType,SqlParameter[]? parameters = null)
        {
            var table = new DataTable();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(commandText, con)
            {
                CommandType = commandType,
                CommandTimeout = 300
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            await con.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            table.Load(reader);

            return table;
        }


        // SELECT MANY RESULTSET
        public async Task<DataSet> ExecuteDataSetAsync(string spName,SqlParameter[]? parameters = null)
        {
            var ds = new DataSet();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(spName, con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new SqlDataAdapter(cmd);
            await Task.Run(() => adapter.Fill(ds));

            return ds;
        }

        // INSERT / UPDATE / DELETE
        public async Task<int> ExecuteNonQueryWithOutputAsync(string spName,SqlParameter[] parameters)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(spName, con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            cmd.Parameters.AddRange(parameters);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            var outputParam = parameters
                .First(p => p.Direction == ParameterDirection.Output);

            return Convert.ToInt32(outputParam.Value);
        }

        // SCALAR
        public async Task<T?> ExecuteScalarAsync<T>(string sql,SqlParameter[]? parameters = null,CommandType commandType = CommandType.Text)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, con)
            {
                CommandType = commandType
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            await con.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result == DBNull.Value || result == null
                ? default
                : (T)Convert.ChangeType(result, typeof(T));
        }

    }

}

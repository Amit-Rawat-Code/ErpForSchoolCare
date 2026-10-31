using Microsoft.Data.SqlClient;
using SchoolApis.DbHelper;
using SchoolApis.Model;
using System.Data;

namespace SchoolApis.Services
{
    public class MenuService : IMenuService
    {
        private readonly SqlDbHelper sqlDbHelper;

        public MenuService(SqlDbHelper sqlDbHelper) 
        {
            this.sqlDbHelper = sqlDbHelper;
        }

        public async Task<Dictionary<string, object?>> GetMenusByIdAsync(int? id,int Action)
        {
            var parameters = new[]
            {
                new SqlParameter("@MenuId",id),
                new SqlParameter("@Action",Action)
            };
            var results = await sqlDbHelper.ExecuteDataTableAsync("sp_MenuList", CommandType.StoredProcedure, parameters.ToArray());
            return results.ToSingleDictionary();
        }

        public async Task<List<Dictionary<string,object?>>> GetMenusByRoleAsync(string role,int Action)
        {
            var MenusDictionary = new Dictionary<string, object>();
            var parameters = new[]
            {
                new SqlParameter("@Role",role),
                new SqlParameter("@Action",Action)
            };
            var menus = await sqlDbHelper.ExecuteDataTableAsync("sp_MenuList", CommandType.StoredProcedure, parameters.ToArray());
            return menus.ToDictionaryList();
            //if (role == "Admin")
            //{
            //    menus.Add(new MenuDto { Title = "Dashboard", Url = "/Dashboard", Icon = "bi-speedometer", Order = 1 });
            //    menus.Add(new MenuDto { Title = "Users", Url = "/Users", Icon = "bi-people", Order = 2 });
            //    menus.Add(new MenuDto { Title = "Settings", Url = "/Settings", Icon = "bi-gear", Order = 3 });
            //}
            //else if (role == "Teacher")
            //{
            //    menus.Add(new MenuDto { Title = "Dashboard", Url = "/Dashboard", Icon = "bi-speedometer", Order = 1 });
            //    menus.Add(new MenuDto { Title = "Students", Url = "/Students", Icon = "bi-person", Order = 2 });
            //}
            //else if (role == "Student")
            //{
            //    menus.Add(new MenuDto { Title = "Dashboard", Url = "/Dashboard", Icon = "bi-speedometer", Order = 1 });
            //    menus.Add(new MenuDto { Title = "Profile", Url = "/Profile", Icon = "bi-person-circle", Order = 2 });
            //}

            //return Task.FromResult(menus.OrderBy(m => m.Order).ToList());
        }

        public async Task<int> PostMenu(MenuDto menu)
        {
            var parameters = new[]
            {
                new SqlParameter("@MenuName",menu.Title),
                new SqlParameter("@MenuUrl",menu.Url),
                new SqlParameter("@MenuIcon",menu.Icon),
                new SqlParameter("@MenuOrder",menu.Order),
                new SqlParameter("@CreatedBy",menu.CreatedBY),
                new SqlParameter("@Action",menu.Action),
                new SqlParameter("@Result",SqlDbType.Int){Direction = ParameterDirection.Output}
            };
            await sqlDbHelper.ExecuteNonQueryWithOutputAsync("sp_InsertUpdateDeleteMenuMaster", parameters.ToArray());
            int result = Convert.ToInt32(parameters[parameters.Length - 1].Value);
            return result;
        }
    }

}

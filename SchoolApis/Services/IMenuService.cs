using SchoolApis.Model;

namespace SchoolApis.Services
{
    public interface IMenuService
    {
        Task<List<Dictionary<string, object?>>> GetMenusByRoleAsync(string role,int Action);

        Task<Dictionary<string, object?>> GetMenusByIdAsync(int? id, int Action);
        Task<int> PostMenu(MenuDto menu);
    }
}
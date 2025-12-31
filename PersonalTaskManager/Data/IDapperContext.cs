using System.Data;

namespace PersonalTaskManager.Data
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}

using System.Data.SqlClient;

namespace ConsoleApp;

public class Database
{
    public static SqlConnection GetConnection()
    {
        const string connectionString = "Server=localhost\\SQLEXPRESS;Database=Company;Trusted_Connection=True;";
        var connection = new SqlConnection(connectionString);
        return connection;
    }
}
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApp
{
    public class Cases
    {
        public static void Run(SqlConnection connection, int choice)
        {
            switch(choice) {
                case 1:
                    {
                    Console.WriteLine("Enter department name:");
                    string departmentName = Console.ReadLine();

                    int managerSSN;
                    while (true)
                    {
                        Console.WriteLine("Enter manager SSN:");
                        if (int.TryParse(Console.ReadLine(), out managerSSN))
                        {
                            var checkManagerSSNCommand = new SqlCommand("SELECT COUNT(*) FROM Employee WHERE SSN = @SSN", connection);
                            checkManagerSSNCommand.Parameters.AddWithValue("@SSN", managerSSN);

                            connection.Open();
                            int count = (int)checkManagerSSNCommand.ExecuteScalar();
                            connection.Close();

                            if (count > 0) break;
                        }

                        Console.WriteLine($"Invalid manager SSN. Please enter a valid integer SSN.");
                    }

                    var createDepartmentCommand = new SqlCommand("USP_CreateDepartment", connection);
                    createDepartmentCommand.CommandType = CommandType.StoredProcedure;
                    createDepartmentCommand.Parameters.AddWithValue("@DName", departmentName);
                    createDepartmentCommand.Parameters.AddWithValue("@MgrSSN", managerSSN);

                    connection.Open();
                    createDepartmentCommand.ExecuteNonQuery();
                    connection.Close();

                    Console.WriteLine("Department created successfully.");
                    }
                    break;

                case 2:
                    {
                        Console.WriteLine("Choose a department number:");
                        var selectDepartmentsCommand = new SqlCommand("SELECT DNumber, DName FROM Department", connection);

                        connection.Open();
                        using (var reader = selectDepartmentsCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["DNumber"]}: {reader["DName"]}");
                            }
                        }
                        connection.Close();

                        Console.WriteLine("Enter the department number:");
                        int departmentNumber = int.Parse(Console.ReadLine());

                        Console.WriteLine("Enter new department name:");
                        string newDepartmentName = Console.ReadLine();

                        var updateDepartmentCommand = new SqlCommand("USP_UpdateDepartmentName", connection);
                        updateDepartmentCommand.CommandType = CommandType.StoredProcedure;
                        updateDepartmentCommand.Parameters.AddWithValue("@DNumber", departmentNumber);
                        updateDepartmentCommand.Parameters.AddWithValue("@DName", newDepartmentName);

                        connection.Open();
                        updateDepartmentCommand.ExecuteNonQuery();
                        connection.Close();

                        Console.WriteLine("Department updated successfully.");
                    }
                    break;

                case 3:
                    {
                        Console.WriteLine("Slect a department:");
                        var command = new SqlCommand("SELECT DNumber, DName FROM Department", connection);

                        connection.Open();
                        var reader = command.ExecuteReader();
                        Console.WriteLine("Departments:");
                        Console.WriteLine("------------");

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["DNumber"]}: {reader["DName"]}");
                        }

                        reader.Close();

                        Console.Write("Department number: ");
                        var departmentNumber = int.Parse(Console.ReadLine());

                        var commandA = new SqlCommand("SELECT SSN, Fname, Lname FROM Employee WHERE SSN NOT IN (SELECT MgrSSN FROM Department)", connection);
                        commandA.Parameters.AddWithValue("@DNumber", departmentNumber);

                        reader = commandA.ExecuteReader();
                        Console.WriteLine("\n Unassigned Managers:");
                        Console.WriteLine("---------------------------");

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["SSN"]}: {reader["Fname"]} {reader["Lname"]}");
                        }

                        reader.Close();

                        Console.Write("New manager SSN: ");
                        var newManagerSSN = int.Parse(Console.ReadLine());

                        var commandB = new SqlCommand("USP_UpdateDepartmentManager", connection);
                        commandB.CommandType = System.Data.CommandType.StoredProcedure;
                        commandB.Parameters.AddWithValue("@DNumber", departmentNumber);
                        commandB.Parameters.AddWithValue("@MgrSSN", newManagerSSN);
                        commandB.ExecuteNonQuery();

                        Console.WriteLine("Department manager updated successfully.");

                        connection.Close();
                    }
                    break;

                case 4:
                    {
                        Console.Write("Enter department number: ");
                        int departmentNumber = int.Parse(Console.ReadLine());

                        var deleteDepartmentCommand = new SqlCommand("USP_DeleteDepartment", connection);
                        deleteDepartmentCommand.CommandType = CommandType.StoredProcedure;
                        deleteDepartmentCommand.Parameters.AddWithValue("@DNumber", departmentNumber);

                        connection.Open();
                        deleteDepartmentCommand.ExecuteNonQuery();
                        connection.Close();

                        Console.WriteLine("Department deleted successfully.");
                    }
                    break;

                case 5:
                    {
                        List<(int DNumber, string DName)> departments = new();
                        var deptCommand = new SqlCommand("SELECT DNumber, DName FROM Department", connection);
                        connection.Open();

                        var deptReader = deptCommand.ExecuteReader();

                        while (deptReader.Read())
                        {
                            var dNumber = deptReader.GetInt32(0);
                            var dName = deptReader.GetString(1);

                            departments.Add((dNumber, dName));
                        }

                        deptReader.Close();

                        Console.WriteLine("Departments:");

                        foreach (var dept in departments)
                        {
                            Console.WriteLine($"  {dept.DNumber} - {dept.DName}");
                        }

                        int departmentNumber;

                        while (true)
                        {
                            Console.Write("Enter a department number: ");

                            var input = Console.ReadLine();

                            if (!int.TryParse(input, out departmentNumber))
                            {
                                Console.WriteLine("Invalid department number. Enter a valid integer.");
                            }
                            else if (!departments.Exists(d => d.DNumber == departmentNumber))
                            {
                                Console.WriteLine($"Department {departmentNumber} does not exist. Select a valid department.");
                            }
                            else
                            {
                                break;
                            }
                        }

                        var command = new SqlCommand("USP_GetDepartment", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DNumber", departmentNumber);

                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var dNumber = reader.GetInt32(0);
                                var dName = reader.GetString(1);
                                var mgrSSN = reader.GetDecimal(2);
                                var mgrStartDate = reader.GetDateTime(3);
                                var empCount = reader.GetInt32(4);

                                Console.WriteLine($"Department Number: {dNumber}");
                                Console.WriteLine($"Department Name: {dName}");
                                Console.WriteLine($"Manager SSN: {mgrSSN}");
                                Console.WriteLine($"Manager Start Date: {mgrStartDate}");
                                Console.WriteLine($"Employee Count: {empCount} \n ");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"A department with the id {departmentNumber} does not exist.");
                        }

                        reader.Close();
                        connection.Close();
                    }
                    break;

                case 6:
                    {
                        using var command = new SqlCommand("USP_GetALLDepartments", connection);
                        connection.Open();

                        using var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int dNumber = reader.GetInt32(0);
                                string dName = reader.GetString(1);
                                decimal mgrSSN = reader.GetDecimal(2);
                                DateTime mgrStartDate = reader.GetDateTime(3);
                                int empCount = reader.GetInt32(4);

                                Console.WriteLine($"Department Number: {dNumber}");
                                Console.WriteLine($"Department Name: {dName}");
                                Console.WriteLine($"Manager SSN: {mgrSSN}");
                                Console.WriteLine($"Manager Start Date: {mgrStartDate}");
                                Console.WriteLine($"Employee Count: {empCount} \n ");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No departments found.");
                        }
                    }
                    break;
                }
            }
        }
}


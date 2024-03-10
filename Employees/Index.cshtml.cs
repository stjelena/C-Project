using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;


namespace WebApplication1.Pages.Employees
{
    public class IndexModel : PageModel
    { 
        public List<Employee> employees = new List<Employee>();
        public List<EmployeeDifference> EmployeeDifferences = new List<EmployeeDifference>();

        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=localhost;Initial Catalog=myDatabase;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //This is for testing only
                    String sql = "SELECT * FROM myDB";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Employee employee = new Employee();
                                employee.Id = reader.GetString(0);
                                employee.EmployeeName = reader.GetString(1);
                                employee.StartTimeUtc = reader.GetString(2);
                                employee.EndTimeUtc = reader.GetString(3);
                                employee.EntryNotes = reader.GetString(4);
                                employee.DeletedOn = reader.IsDBNull(5) ? "" : reader.GetString(5);

                                employees.Add(employee);

                            }
                        }



                    }

                    //Employees with total hours worked
                    string sql1 = "SELECT SUM(ABS(DATEDIFF(hour, EndTimeUtc, StartTimeUtc))) AS Difference, EmployeeName " +
                             "FROM myDB " +
                             "GROUP BY EmployeeName " +
                             "ORDER BY Difference DESC";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeDifference employeeDifference = new EmployeeDifference();
                                employeeDifference.Difference = Convert.ToInt32(reader["Difference"]);
                                employeeDifference.EmployeeName = reader["EmployeeName"].ToString();

                                EmployeeDifferences.Add(employeeDifference);
                            }
                        }
                    }
                }


               

            }
            catch (Exception ex) {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }

    public class Employee
    {
        public String Id;
        public String EmployeeName;
        public String StartTimeUtc;
        public String EndTimeUtc;
        public String EntryNotes;
        public String DeletedOn;
    }

    public class EmployeeDifference
    {
        public int Difference;
        public string EmployeeName;
    }
}

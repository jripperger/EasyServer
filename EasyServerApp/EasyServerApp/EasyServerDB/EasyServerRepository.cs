/* 
 * EasyServerRepository
 *  
 * This class provides all the necessary functionality for
 * EasyServer's interaction with a database. Functions in 
 * class are used to both send data to and retrieve data 
 * from the database.
 * 
 */

using Microsoft.EntityFrameworkCore;

namespace EasyServerApp.EasyServerDB
{
    public sealed class EasyServerRepository
    {
        // Employee table list and associated property
        private List<Employee> employees;
        public List<Employee> Employees { get { return employees; } private set { employees = value; } }

        // RestaurantTable table list and associated property
        private List<RestaurantTable> tables;
        public List<RestaurantTable> RestaurantTables { get { return tables; } private set { tables = value; } }

        // List and associated property containing server queues for each of the employees in the Employee table
        private List<ServerQueue> serverQueues;
        public List<ServerQueue> ServerQueues { get { return serverQueues; } set { serverQueues = value; } }

        public EasyServerRepository()
        {
            employees = GetEmployeeList();           // Fetch employees from Employee table
            tables = GetTableList();                 // Fetch tables from RestuarantTable table
            serverQueues = new List<ServerQueue>();  // Initialize list of server queues

            // Create a server queue for each employee
            for (int i = 0; i < employees.Count; i++)
            {
                List<RestaurantTable> queue = new();
                ServerQueue serverQueue = new(employees[i], queue);
                serverQueues.Add(serverQueue);
            }
        }

        // Function to retrieve all employees from the Employee table
        public List<Employee> GetEmployeeList()
        {
            using (var context = new EasyServerContext())
            {
                return employees = context.Employee.FromSqlRaw("SELECT * FROM dbo.Employee").ToList();
            }
        }

        // Function to retrieve all tables from the RestaurantTable table
        public List<RestaurantTable> GetTableList()
        {
            using (var context = new EasyServerContext())
            {
                return tables = context.RestaurantTable.FromSqlRaw("SELECT * FROM dbo.RestaurantTable").ToList();
            }
        }

        // Function to retrieve an employee based on their unique employee ID
        public Employee GetEmployeeById(int id)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.EmployeeId == id).FirstOrDefault();
            }
        }

        // Function to retrieve an employee based on their username and password
        public Employee GetEmployeeByCredentials(string username, string password)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            }              
        }

        // Function to retrieve an employee based on their username
        public Employee GetEmployeeByUsername(string username)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.Username == username).FirstOrDefault();
            }
        }

        // Function to retrieve an employee based on their password
        public Employee GetEmployeeByPassword(string password)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.Password == password).FirstOrDefault();
            }
        }

        // Function to add an employee to the Employee table
        public Employee InsertEmployeeRow(string firstName, string lastName, string username, string password, string role = "Server")
        {
            using (var context = new EasyServerContext())
            {
                Employee newEmployee = new()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Password = password,
                    Role = role
                };

                context.Employee.Add(newEmployee);  // Adds the employee to the Employee table
                context.SaveChanges();              // Updates the table
                Employees = GetEmployeeList();      // Update employees list to include the new employee

                newEmployee = context.Employee.OrderBy(x => x.EmployeeId).LastOrDefault();

                // Create a server queue for the newly added employee
                List<RestaurantTable> queue = new();
                ServerQueue serverQueue = new(newEmployee, queue);
                ServerQueues.Add(serverQueue);

                // Return the inserted employee
                return newEmployee;
            }
        }

        // Function to remove an employee from the Employee table
        public Employee DeleteEmployeeRow(int employeeID)
        {
            using (var context = new EasyServerContext())
            {
                // Retrieve the employee based on the passed in employee ID
                Employee employee = GetEmployeeById(employeeID);

                if (employee != null)  // If an employee was returned, remove them
                {
                    Task task = new(() => { context.Employee.Remove(employee); });  // Removes the employee from the Employee table
                    task.RunSynchronously();

                    // Update all the tables that this employee was serving with a null employee ID
                    for (int i = 0; i < RestaurantTables.Count; i++)
                    {
                        if (RestaurantTables[i].EmployeeId == employeeID)
                        {
                            UpdateTableServer(RestaurantTables[i].TableId, null);
                        }
                    }

                    context.SaveChanges();          // Updates the table
                    Employees = GetEmployeeList();  // Updates employees list to exclude the removed employee

                    // Removes the employee's associated server queue from the server queue list
                    ServerQueue serverQueue = ServerQueues.Where(x => x.Employee.EmployeeId == employeeID).FirstOrDefault();
                    ServerQueues.Remove(serverQueue);
                }

                // Return the deleted employee, possible to return null
                return employee;
            }
        }

        // Function to add a restaurant table to the RestaurantTable table
        public RestaurantTable InsertRestaurantTableRow(string qrCode, int? employeeID)
        {
            using (var context = new EasyServerContext())
            {
                RestaurantTable newTable = new()
                {
                    Qrcode = qrCode,
                    EmployeeId = employeeID
                };

                context.RestaurantTable.Add(newTable);  // Adds the table to the RestaurantTable table
                context.SaveChanges();                  // Updates the table
                RestaurantTables = GetTableList();      // Updates the restaurant tables list to include the new table

                // Return the inserted table
                return newTable;
            }
        }

        // Function to delete a restaurant table from the RestaurantTable table
        public RestaurantTable DeleteRestaurantTableRow(int tableID)
        {
            using (var context = new EasyServerContext())
            {
                // Retrieve the table based on the passed in table ID
                RestaurantTable table = GetTableById(tableID);

                if (table != null)  // If a table was returned, remove it
                {
                    Task task = new(() => { context.RestaurantTable.Remove(table); });  // Removes the table from the RestaurantTable table
                    task.RunSynchronously();
                    context.SaveChanges();              // Updates the table
                    RestaurantTables = GetTableList();  // Updates the restaurant tables list to exclude the removed table
                }

                // Return the deleted table, possible to return null
                return table;
            }
        }

        // Function to retrieve a table based on its unique table ID
        public RestaurantTable GetTableById(int id)
        {
            using (var context = new EasyServerContext())
            {
                return context.RestaurantTable.Where(x => x.TableId == id).FirstOrDefault();
            }
        }

        // Function to retrieve a table based on its unique QR code
        public RestaurantTable GetTableByQRCode(string qrCode)
        {
            using (var context = new EasyServerContext())
            {
                return context.RestaurantTable.Where(x => x.Qrcode == qrCode).FirstOrDefault();
            }
        }

        // Function to update a table's server
        public void UpdateTableServer(int tableID, int? employeeID)
        {
            using (var context = new EasyServerContext())
            {
                // Retrieve the table based on the passed in table ID
                RestaurantTable table = GetTableById(tableID);

                if (table != null)  // If a table was returned, update its server
                {             
                    table.EmployeeId = employeeID;
                    context.RestaurantTable.Update(table);  // Modifies the table's employee ID to reflect the passed in employee ID
                    context.SaveChanges();                  // Updates the table
                    RestaurantTables = GetTableList();      // Updates the restaurant tables list to reflect the change to the table's employee ID
                }             
            }
        }

        // Function to update an employee's role
        public void UpdateEmployeeRole(int employeeID, string role)
        {
            using (var context = new EasyServerContext())
            {
                // Retrieve the employee based on the passed in employee ID
                Employee employee = GetEmployeeById(employeeID);

                if (employee != null)
                {

                    employee.Role = role;
                    context.Employee.Update(employee);  // Modifies the employee's role to reflect the passed in role
                    context.SaveChanges();              // Updates the table
                    Employees = GetEmployeeList();      // Updates the employees list to reflect the change to the employee's role
                }
            }
        }
    }
}

using CloudinaryDotNet.Actions;
using DocumentFormat.OpenXml.Office2010.Excel;
using EasyServerApp.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maui.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyServerApp.EasyServerDB
{
    public sealed class EasyServerRepository
    {
        private List<Employee> employees;
        public List<Employee> Employees { get { return employees; } private set { employees = value; } }

        private List<RestaurantTable> tables;
        public List<RestaurantTable> RestaurantTables { get { return tables; } private set { tables = value; } }

        private List<ServerQueue> serverQueues;
        public List<ServerQueue> ServerQueues { get { return serverQueues; } set { serverQueues = value; } }

        public EasyServerRepository()
        {
            employees = GetEmployeeList();
            tables = GetTableList();
            serverQueues = new List<ServerQueue>();

            for (int i = 0; i < employees.Count; i++)
            {
                List<RestaurantTable> queue = new();
                ServerQueue serverQueue = new(employees[i], queue);
                serverQueues.Add(serverQueue);
            }
        }

        public ServerQueue GetServerQueue(int id)
        {
            ServerQueue serverQueue = serverQueues.Where(x => x.Employee.EmployeeId == id).FirstOrDefault();

            return serverQueue;
        }

        public List<Employee> GetEmployeeList()
        {
            using (var context = new EasyServerContext())
            {
                return employees = context.Employee.FromSqlRaw("SELECT * FROM dbo.Employee").ToList();
            }
        }

        public List<RestaurantTable> GetTableList()
        {
            using (var context = new EasyServerContext())
            {
                return tables = context.RestaurantTable.FromSqlRaw("SELECT * FROM dbo.RestaurantTable").ToList();
            }
        }
        
        public Employee GetEmployeeById(int id)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.EmployeeId == id).FirstOrDefault();
            }
        }

        public Employee GetEmployeeByCredentials(string username, string password)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            }              
        }

        public Employee GetEmployeeByName(string firstName, string lastName)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.FirstName == firstName && x.LastName == lastName).FirstOrDefault();
            }
        }

        public Employee GetEmployeeByUsername(string username)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.Username == username).FirstOrDefault();
            }
        }

        public Employee GetEmployeeByPassword(string password)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => x.Password == password).FirstOrDefault();
            }
        }

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

                context.Employee.Add(newEmployee);
                context.SaveChanges();
                Employees = GetEmployeeList();

                newEmployee = context.Employee.OrderBy(x => x.EmployeeId).LastOrDefault();

                List<RestaurantTable> queue = new();
                ServerQueue serverQueue = new(newEmployee, queue);
                ServerQueues.Add(serverQueue);

                return newEmployee;
            }
        }

        public Employee DeleteEmployeeRow(int employeeID)
        {
            using (var context = new EasyServerContext())
            {
                Employee employee = context.Employee.Where(x => x.EmployeeId == employeeID).FirstOrDefault();

                if (employee != null)
                {
                    Task task = new(() => { context.Employee.Remove(employee); });
                    task.RunSynchronously();

                    for (int i = 0; i < RestaurantTables.Count; i++)
                    {
                        if (RestaurantTables[i].EmployeeId == employeeID)
                        {
                            UpdateTableServer(RestaurantTables[i].TableId, null);
                        }
                    }

                    context.SaveChanges();
                    Employees = GetEmployeeList();

                    ServerQueue serverQueue = ServerQueues.Where(x => x.Employee.EmployeeId == employeeID).FirstOrDefault();
                    ServerQueues.Remove(serverQueue);
                }

                return employee;
            }
        }

        public RestaurantTable InsertRestaurantTableRow(string qrCode, int? employeeID)
        {
            using (var context = new EasyServerContext())
            {
                RestaurantTable newTable = new()
                {
                    Qrcode = qrCode,
                    EmployeeId = employeeID
                };

                context.RestaurantTable.Add(newTable);
                context.SaveChanges();
                RestaurantTables = GetTableList();

                return context.RestaurantTable.OrderBy(x => x.TableId).LastOrDefault();
            }
        }

        public RestaurantTable DeleteRestaurantTableRow(int tableID)
        {
            using (var context = new EasyServerContext())
            {
                RestaurantTable table = context.RestaurantTable.Where(x => x.TableId == tableID).FirstOrDefault();

                if (table != null)
                {
                    Task task = new(() => { context.RestaurantTable.Remove(table); });
                    task.RunSynchronously();
                    context.SaveChanges();
                    RestaurantTables = GetTableList();
                }
              
                return table;
            }
        }


        public RestaurantTable GetTableById(int id)
        {
            using (var context = new EasyServerContext())
            {
                return context.RestaurantTable.Where(x => x.TableId == id).FirstOrDefault();
            }
        }

        public RestaurantTable GetTableByQRCode(string qrCode)
        {
            using (var context = new EasyServerContext())
            {
                return context.RestaurantTable.Where(x => x.Qrcode == qrCode).FirstOrDefault();
            }
        }

        public void UpdateTableServer(int tableID, int? employeeID)
        {
            using (var context = new EasyServerContext())
            {
                RestaurantTable table = GetTableById(tableID);

                if (table != null)
                {             
                    table.EmployeeId = employeeID;
                    context.RestaurantTable.Update(table);
                    context.SaveChanges();
                    RestaurantTables = GetTableList();
                }             
            }
        }

        public void UpdateEmployeeRole(int employeeID, string role)
        {
            using (var context = new EasyServerContext())
            {
                Employee employee = GetEmployeeById(employeeID);

                if (employee != null)
                {

                    employee.Role = role;
                    context.Employee.Update(employee);
                    context.SaveChanges();
                    Employees = GetEmployeeList();
                }
            }
        }
    }
}

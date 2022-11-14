using CloudinaryDotNet.Actions;
using EasyServerApp.Pages;
using Microsoft.EntityFrameworkCore;
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
        public List<Employee> Employees { get { return employees; } }
        
        private List<RestaurantTable> tables;
        public List<RestaurantTable> RestaurantTables { get { return tables; } }

        private Hashtable serverQueues;
        public Hashtable ServerQueues { get { return serverQueues; } set { serverQueues = value; } }

        public EasyServerRepository()
        {
            employees = GetEmployeeList();
            tables = GetTableList();
            serverQueues = new Hashtable();

            for (int i = 0; i < employees.Count; i++)
            {
                List<RestaurantTable> queue = new();
                serverQueues.Add(employees[i].EmployeeId, queue);
            }
        }

        public List<RestaurantTable> GetServerQueue(int id)
        {
            if (serverQueues.ContainsKey(id)) 
            {
                return (List<RestaurantTable>)serverQueues[id];
            }
            else
            {
                return null;
            }
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
                return context.Employee.Where(x => (x.EmployeeId == id)).FirstOrDefault();
            }
        }

        public Employee GetEmployeeByCredentials(string username, string password)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => (x.Username == username && x.Password == password)).FirstOrDefault();
            }              
        }

        public Employee GetEmployeeByName(string firstName, string lastName)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => (x.FirstName == firstName && x.LastName == lastName)).FirstOrDefault();
            }
        }

        public Employee GetEmployeeByUsername(string username)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => (x.Username == username)).FirstOrDefault();
            }
        }

        public Employee GetEmployeeByPassword(string password)
        {
            using (var context = new EasyServerContext())
            {
                return context.Employee.Where(x => (x.Password == password)).FirstOrDefault();
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
                return context.Employee.OrderBy(x => x.EmployeeId).LastOrDefault();
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
                return context.RestaurantTable.OrderBy(x => x.TableId).LastOrDefault();
            }
        }


        public RestaurantTable GetTableById(int id)
        {
            using (var context = new EasyServerContext())
            {
                return context.RestaurantTable.Where(x => (x.TableId == id)).FirstOrDefault();
            }
        }

        public void UpdateTableServer(int tableID, int employeeID)
        {
            using (var context = new EasyServerContext())
            {
                RestaurantTable table = GetTableById(tableID);

                if (table != null)
                {             
                    table.EmployeeId = employeeID;
                    context.RestaurantTable.Update(table);
                    context.SaveChanges();
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
                }

            }
        }
    }
}

using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyServerApp.EasyServerDB
{
    public sealed class EasyServerRepository
    {
        private List<Employee> employees;
        private List<RestaurantTable> tables;

        public EasyServerRepository()
        {
            using (var context = new EasyServerContext())
            {
                employees = GetEmployeeList();
                tables = GetTableList();
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
        
        public void InsertEmployeeRow(string firstName, string lastName, string username, string password, string role = "Server")
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
            }
        }

        public RestaurantTable GetTableById(int id)
        {
            using (var context = new EasyServerContext())
            {
                return context.RestaurantTable.Where(x => (x.TableId == id)).FirstOrDefault();
            }
        }
    }
}

using EasyServerApp.EasyServerDB;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyServerApp
{
    public class ServerQueue
    {
        private List<RestaurantTable> serverQueue;
        public List<RestaurantTable> Queue { get { return serverQueue; } private set { serverQueue = value; } }

        private Employee employee;
        public Employee Employee { get { return employee; } private set { employee = value; } }

        public ServerQueue(Employee employee, List<RestaurantTable> serverQueue) {
            this.serverQueue = serverQueue;
            this.employee = employee;
        }
    }
}

/*
 * ServerQueue
 * 
 * This class represents a server queue. Each server's queue has an associated
 * employee and a queue, which is implemented as a list of RestaurantTable 
 * objects.
 * 
 */

using EasyServerApp.EasyServerDB;

namespace EasyServerApp
{
    public class ServerQueue
    {
        private List<RestaurantTable> serverQueue;  // List of tables awaiting service from the associated server
        private Employee employee;                  // Server associated with the queue

        public List<RestaurantTable> Queue { get { return serverQueue; } set { serverQueue = value; } }       
        public Employee Employee { get { return employee; } private set { employee = value; } }

        public ServerQueue(Employee employee, List<RestaurantTable> serverQueue) {

            // Assign values to global variables
            this.serverQueue = serverQueue;
            this.employee = employee;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args) {

            Server server = new Server("192.168.1.17", 4466);
            server.Start();

        }
    }
}

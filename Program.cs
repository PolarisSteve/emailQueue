using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emailQueue
{
    class Program
    {
        static void Main(string[] args)
        {

            EmailRoutine.SendEmail(args[0], args[1]);



        }
    }
}

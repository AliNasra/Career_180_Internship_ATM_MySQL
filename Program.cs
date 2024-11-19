using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Helpers;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ATM atm = new ATM();
            await atm.handleUser();

        }

    }
}

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AppleTestFlight.Core.Models;
using AppleTestFlight.Core;
namespace AppleTestFlight.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskManager task = new TaskManager();
            task.RestAllUserInviteUrls();
            Console.WriteLine(1);
        }
    }
}

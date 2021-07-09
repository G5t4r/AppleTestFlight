using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AppleTestFlight.Core.Models;
using AppleTestFlight.Core;
using System.Linq;

namespace AppleTestFlight.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestFlightFactory testFlightFactory = new TestFlightFactory("1575052758", "0f8002ef-b934-4ac3-862f-45db06e8d938");
            //    var appInfo = testFlightFactory.GetAllApps();
            // var allInviteUserInfo = testFlightFactory.GetAllInvitedUserInfo();
            Console.WriteLine(1);
        }
    }
}

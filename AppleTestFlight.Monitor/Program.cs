using System;
using AppleTestFlight.Core;
using System.Threading;
using AppleTestFlight.Core.Config;
namespace AppleTestFlight.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var appidAndBetaGroups = AppleTestFlightConfig.GetAppidAndBetaGroups();
                TestFlightTaskManager taskManager = new TestFlightTaskManager(appidAndBetaGroups.Key, appidAndBetaGroups.Value);
                try
                {
                    Thread.Sleep(1000);
                    if (taskManager.GetAllInviteUrlsInRedis().Count <= 0)
                    {
                        Console.WriteLine("无库存了，重头再来");
                        //延迟10秒初始化
                        Thread.Sleep(10000);
                        Console.WriteLine("开始初始化中...");
                        taskManager.Initialize();
                        Console.WriteLine("初始化完成！");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

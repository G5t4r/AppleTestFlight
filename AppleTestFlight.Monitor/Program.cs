using AppleTestFlight.Core;
using AppleTestFlight.Core.Config;
using System;
using System.Threading;
namespace AppleTestFlight.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    var appidAndBetaGroups = AppleTestFlightConfig.GetAppidAndBetaGroups();
                    Thread.Sleep(1000);
                    if (RedisUtils.GetQueueAllToList(appidAndBetaGroups.Key).Count <= 0)
                    {
                        Console.WriteLine("库存已经用完，正在重新初始化...");
                        //延迟10秒初始化
                        Thread.Sleep(10000);
                        TestFlightTaskManager taskManager = new TestFlightTaskManager(appidAndBetaGroups.Key, appidAndBetaGroups.Value);
                        taskManager.Initialize();
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

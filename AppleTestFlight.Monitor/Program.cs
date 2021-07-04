using System;
using AppleTestFlight.Core;
using System.Threading;
namespace AppleTestFlight.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskManager taskManager = new TaskManager();

            while (true)
            {
                try
                {
                    Thread.Sleep(1000);
                    if (taskManager.GetInviteUrlsCount() <= 0)
                    {
                        Console.WriteLine("无库存了，重头再来");
                        taskManager.Initial();
                        Console.WriteLine("初始化完成");
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

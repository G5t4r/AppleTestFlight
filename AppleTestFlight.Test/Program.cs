using StackExchange.Redis;
namespace AppleTestFlight.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var _multiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=true");
            //for (int i = 0; i < 100; i++)
            //{
            //    _multiplexer.GetDatabase(0).ListRightPush("Test", i);
            //}

        }
    }
}

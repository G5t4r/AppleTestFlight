using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppleTestFlight.Core
{
    public class RedisUtils
    {
        private static ConnectionMultiplexer _multiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=true");

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void EnQueue(string key, string value)
        {
            _multiplexer.GetDatabase(0).ListRightPush(key, value);
        }


        /// <summary>
        /// 出队
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DeQueue(string key)
        {
            return _multiplexer.GetDatabase(0).ListLeftPop(key);
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        /// <param name="key"></param>
        public static void FlushQueue(string key)
        {
            _multiplexer.GetDatabase(0).KeyDelete(key);
        }

        /// <summary>
        /// 获取该队列的所有
        /// </summary>
        /// <returns></returns>
        public static List<RedisValue> GetQueueAllToList(string key)
        {
            return _multiplexer.GetDatabase(0).ListRange(key).ToList();
        }

        /// <summary>
        /// 插入一个String类型的键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        public static void InsertString(string key, string value, double expiry)
        {
            _multiplexer.GetDatabase(1).StringSet(key, value, TimeSpan.FromSeconds(expiry));
        }

        /// <summary>
        /// 判断键是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyExits(string key)
        {
            return _multiplexer.GetDatabase(1).KeyExists(key);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Threading;
using AppleTestFlight.Core.Config;
namespace AppleTestFlight.Core
{
    /// <summary>
    /// TestFlight调用任务Manager
    /// </summary>
    public class TestFlightTaskManager
    {

        private readonly ConnectionMultiplexer _multiplexer;
        private readonly TestFlightFactory _testFlightFactory;


        public TestFlightTaskManager(string appid, string betagroups)
        {
            _testFlightFactory = new TestFlightFactory(appid, betagroups);
            _multiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=true");
        }


        /// <summary>
        /// 初始化该任务（清空并重新发送邀请链接&获取邮件邀请链接&保存至Redis）
        /// </summary>
        public void Initialize()
        {
            //清空Redis
            FlushAllRedisDB();
            //获取可操作的Cookie
            _testFlightFactory.GetGlobalOperationCookie();
            //如果已经该APP已经有邀请的用户，则全部移除掉
            var allInvitedUserInfos = _testFlightFactory.GetAllInvitedUserInfo();
            if (allInvitedUserInfos.Count > 0)
            {
                DeleteAllInviteUser();
                Console.WriteLine("删除所有邀请用户完成...");
            }
            //添加所有本地配置的邮箱用户至测试组
            AddAllInviteUser();
            Console.WriteLine("添加所有本地配置的邮箱用户至测试组完成...");
            Thread.Sleep(10000);
            //延迟10秒以后，开始读取接收到的邮件
            var emailAccountInfos = AppleTestFlightConfig.EmailAccountInfos();
            foreach (var email in emailAccountInfos)
            {
                //获取邮箱内邀请链接
                var allEmailInviteUrls = _testFlightFactory.GetEmailInviteUrls(email.Key, email.Value, DateTime.Now.AddDays(-1));
                //过滤掉邮箱内无效的邀请链接
                // allEmailInviteUrls = _testFlightFactory.FilterUselessUrl(allEmailInviteUrls).Distinct().ToList();
                allEmailInviteUrls = allEmailInviteUrls.Distinct().ToList();
                //将获取到的实时可用邀请链接添加至Redis
                foreach (var url in allEmailInviteUrls)
                {
                    InsertSingleInviteUrlToRedis(url);
                }
                Console.WriteLine("读取邮箱内容完成");
            }
        }

        /// <summary>
        /// 删除所有邀请用户
        /// </summary>
        public void DeleteAllInviteUser()
        {
            var allInviteUserInfo = _testFlightFactory.GetAllInvitedUserInfo();
            _testFlightFactory.DeleteInviteUsers(allInviteUserInfo.Values.ToArray());
        }


        /// <summary>
        /// 添加所有邀请用户（本地配置的邮箱）
        /// </summary>
        public void AddAllInviteUser()
        {
            var emailAccountInfos = AppleTestFlightConfig.EmailAccountInfos();
            _testFlightFactory.AddInviteUsers(emailAccountInfos.Keys.ToArray());
        }


        /// <summary>
        /// 清空Redis所有
        /// </summary>
        public void FlushAllRedisDB()
        {
            _multiplexer.GetServer("127.0.0.1:6379").FlushDatabase(0);
        }


        /// <summary>
        /// 插入一个邀请链接至Redis（入队）
        /// </summary>
        public void InsertSingleInviteUrlToRedis(string value)
        {
            _multiplexer.GetDatabase(0).ListRightPush("InviteUrls", value);
        }


        /// <summary>
        /// 获取一个Redis中的邀请链接，并移除(出队)
        /// </summary>
        /// <returns></returns>
        public string GetSingleInviteUrlInRedis()
        {
            return _multiplexer.GetDatabase(0).ListLeftPop("InviteUrls");
        }


        /// <summary>
        /// 获取Redis中的所有邀请链接
        /// </summary>
        /// <returns></returns>
        public List<RedisValue> GetAllInviteUrlsInRedis()
        {
            return _multiplexer.GetDatabase(0).ListRange("InviteUrls").ToList();
        }
    }
}
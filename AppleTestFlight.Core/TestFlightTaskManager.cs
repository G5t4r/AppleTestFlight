using AppleTestFlight.Core.Config;
using System;
using System.Linq;
using System.Threading;
namespace AppleTestFlight.Core
{
    /// <summary>
    /// TestFlight调用任务Manager
    /// </summary>
    public class TestFlightTaskManager
    {
        private readonly TestFlightFactory _testFlightFactory;
        private readonly string APPID;
        public TestFlightTaskManager(string appid, string betagroups)
        {
            APPID = appid;
            _testFlightFactory = new TestFlightFactory(appid, betagroups);
        }


        /// <summary>
        /// 初始化该任务（清空并重新发送邀请链接&获取邮件邀请链接&保存至Redis）
        /// </summary>
        public void Initialize()
        {
            Console.WriteLine("开始初始化中...");
            //清空该APP邀请链接队列
            RedisUtils.FlushQueue(APPID);
            //如果已经该APP已经有邀请的用户，则全部移除掉
            var allInvitedUserInfos = _testFlightFactory.GetAllInvitedUserInfo();
            Console.WriteLine("当前已经邀请用户数量：   " + allInvitedUserInfos.Count);
            if (allInvitedUserInfos.Count > 0)
            {
                DeleteAllInviteUser();
                Console.WriteLine("删除所有邀请用户完成...");
            }
            //添加所有本地配置的邮箱用户至测试组
            AddAllInviteUser();
            Console.WriteLine("添加所有本地配置的邮箱用户至测试组完成...");
            Thread.Sleep(15000);
            //延迟15秒以后，开始读取接收到的邮件
            var emailAccountInfos = AppleTestFlightConfig.EmailAccountInfos();
            foreach (var email in emailAccountInfos)
            {
                //获取邮箱内最新的一个邀请链接
                var inviteUrl = _testFlightFactory.GetEmailInviteUrls(email.Key, email.Value, DateTime.Now.AddSeconds(-30));
                //将获取到的实时可用邀请链接添加至Redis队列
                RedisUtils.EnQueue(APPID, inviteUrl);
                Console.WriteLine(email.Key + "内的邀请链接读取完成...");
            }
            Console.WriteLine("初始化完成!!!");
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
    }
}
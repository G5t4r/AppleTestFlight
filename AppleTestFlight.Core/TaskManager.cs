using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Threading;
namespace AppleTestFlight.Core
{
    /// <summary>
    /// 调用任务Manager
    /// </summary>
    public class TaskManager
    {

        private readonly ConnectionMultiplexer _multiplexer;
        public TaskManager()
        {
            TestFlightUtils.GetGlobalOperationCookie();
            _multiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=true");
        }


        /// <summary>
        /// 初始化（清空并重新发送邀请链接&获取邮件邀请链接&保存至Redis）
        /// </summary>
        public void Initial()
        {
            _multiplexer.GetServer("127.0.0.1:6379").FlushDatabase(0);
            //重新发送所有
            RestAllUserInviteUrls();
            //延迟10秒获取
            Thread.Sleep(10000);
            //再获取邮件里面的链接
            List<string> result = new List<string>();
            //获取本地邮箱配置信息
            var localEmails = EmailUtils.GetLocalEmailConfig();
            foreach (var email in localEmails)
            {
                //获取邮箱里面的邀请链接
                var inviteUrls = TestFlightUtils.GetEmailInviteUrls(email.Key, email.Value, DateTime.Now.AddDays(-1));
                //过滤掉无效的链接
                inviteUrls = TestFlightUtils.FilterUselessUrl(inviteUrls);
                //合并
                result = result.Union(inviteUrls).ToList();
                //过滤重复
                result.Distinct();
                //清空Redis
                _multiplexer.GetServer("127.0.0.1:6379").FlushDatabase(0);
                //重新赋
                foreach (var item in result)
                {
                    _multiplexer.GetDatabase(0).ListRightPush("InviteUrls", item);
                }
            }
        }


        /// <summary>
        /// 获取一个Redis中的邀请链接，并移除
        /// </summary>
        /// <returns></returns>
        public string GetSingleInviteUrl()
        {
            return _multiplexer.GetDatabase(0).ListLeftPop("InviteUrls");
        }


        /// <summary>
        /// 获取数据库中邀请链接的数量
        /// </summary>
        /// <returns></returns>
        public int GetInviteUrlsCount()
        {
            return _multiplexer.GetDatabase(0).ListRange("InviteUrls").Count();
        }



        /// <summary>
        /// 重置所有用户的邀请链接
        /// </summary>
        public void RestAllUserInviteUrls()
        {
            var allInvitedUserInfo = TestFlightUtils.GetAllInvitedUserInfo();//获取所有已经被邀请的用户
            var allLocalEmailInfo = EmailUtils.GetLocalEmailConfig();//获取本地所有可操作用户的邮箱
            //删除已经邀请的用户
            TestFlightUtils.DeleteInviteUsers(allInvitedUserInfo.Values.ToArray());
            //将本地配置的可操作邮箱的用户重新邀请
            TestFlightUtils.AddInviteUsers(allLocalEmailInfo.Keys.ToArray());
        }
    }
}
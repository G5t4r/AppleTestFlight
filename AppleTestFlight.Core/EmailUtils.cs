using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Security;
using MailKit.Net.Imap;
using MailKit.Search;
using System.IO;
using System.Text.RegularExpressions;
namespace AppleTestFlight.Core
{
    /// <summary>
    /// 邮件工具类
    /// </summary>
    public class EmailUtils
    {

        /// <summary>
        /// 获取本地配置的Email账号信息
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetLocalEmailConfig()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Config\\EmailConfig.ini";
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split("----", StringSplitOptions.None);
                    dic.Add(line[0], line[1]);
                }
            }
            return dic;
        }



        /// <summary>
        /// 获取指定时间以后的邮箱里邮件内容
        /// </summary>
        /// <param name="after"></param>
        public static Dictionary<string, string> GetEmailContent(string account, string password, DateTime after)
        {
            using var client = new ImapClient();
            client.Connect("btmail.ym191.com", 143, SecureSocketOptions.None);
            client.Authenticate(account, password);
            client.Inbox.Open(FolderAccess.ReadWrite);
            var query = SearchQuery.DeliveredAfter(after);
            var allEmails = client.Inbox.Search(query);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var newAllEmails = allEmails.Reverse();
            int index = 0;
            foreach (var item in newAllEmails)
            {
                //限流控制，只获取该邮箱的最近2条邮件
                if (index >= 2)
                {
                    break;
                }
                var message = client.Inbox.GetMessage(item);
                dic.Add(message.Subject + message.Date + message.MessageId + Guid.NewGuid().ToString(), message.HtmlBody);
                index++;
            }
            return dic;
        }

        /// <summary>
        /// 删除所有邮件
        /// </summary>
        /// <returns></returns>
        //public static bool DeleteAllEmail()
        //{

        //}
    }
}

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
    /// 邮件系统工厂
    /// </summary>
    public class EmailFactory : IDisposable
    {
        private readonly ImapClient _imapClient;


        public EmailFactory(string account, string password, string host, int port)
        {
            _imapClient = new ImapClient();
            _imapClient.Connect(host, port, SecureSocketOptions.None);
            _imapClient.Authenticate(account, password);
            _imapClient.Inbox.Open(FolderAccess.ReadWrite);
        }


        /// <summary>
        /// 获取指定时间以后的邮箱里邮件内容
        /// </summary>
        /// <param name="after"></param>
        public Dictionary<string, string> GetEmailContentByTime(DateTime after)
        {
            var query = SearchQuery.DeliveredAfter(after);
            var allEmails = _imapClient.Inbox.Search(query);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //拿最新所以需要反转
            var newAllEmails = allEmails.Reverse();
            foreach (var item in newAllEmails)
            {
                var message = _imapClient.Inbox.GetMessage(item);
                dic.Add(message.Subject + item, message.HtmlBody);
            }
            return dic;
        }


        public void Dispose()
        {
            _imapClient.Dispose();
        }
    }
}

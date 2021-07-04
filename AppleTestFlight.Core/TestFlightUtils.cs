using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace AppleTestFlight.Core
{
    /// <summary>
    /// AppleTestFlight工具类
    /// </summary>
    public class TestFlightUtils
    {
        private static string COOKIE = "";//全局操作授权需要的Cookie
        private static string BETAGROUPS = "0f8002ef-b934-4ac3-862f-45db06e8d938";//测试团队组ID
        private static string APPID = "1575052758";//APPID



        /// <summary>
        /// 获取本地配置的管理员总账号的操作Cookie，只需要调用一次即可
        /// </summary>
        public static void SignIn()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Config\\AppleUserConfig.ini";
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                string[] line = reader.ReadLine().Split("----", StringSplitOptions.None);
                string signUrl = "https://idmsa.apple.com/appleauth/auth/signin?isRememberMeEnabled=true";
                string signData = "{\"accountName\":\"" + line[0] + "\",\"rememberMe\":true,\"password\":\"" + line[1] + "\"}";
                var headers = new WebHeaderCollection();
                headers.Add("X-Apple-Widget-Key", "e0b80c3bf78523bfe80974d320935bfa30add02e1bff88ec2166c6bd5a706c42");
                headers.Add("Cookie", line[2]);
                HttpRequest.HttpRequestByPost(signUrl, signData, "application/json", ref headers);
                var myacinfo = headers["Set-Cookie"].Split(';');
                COOKIE = myacinfo.First(o => o.Contains("myacinfo")).Replace("HttpOnly,", "");
            }
        }


        /// <summary>
        /// 获取所有已经邀请的用户（Email&ID）
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllInvitedUserInfo()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string url = "https://appstoreconnect.apple.com/iris/v1/betaTesters?filter[betaGroups]=" + BETAGROUPS;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Cookie", COOKIE);
            JToken result = JToken.Parse(HttpRequest.HttpRequestByGet(url, headers));
            foreach (var item in result["data"])
            {
                dic.Add(item["attributes"]["email"].ToString(), item["id"].ToString());
            }
            return dic;
        }


        /// <summary>
        /// 添加邀请测试用户
        /// </summary>
        /// <param name="guid"></param>
        public static void AddInviteUser(string email)
        {
            string url = "https://appstoreconnect.apple.com/iris/v1/bulkBetaTesterAssignments";
            string data = "{\"data\":{\"type\":\"bulkBetaTesterAssignments\",\"attributes\":{\"betaTesters\":[{\"id\":null,\"email\":\"" + email + "\",\"firstName\":\"沐风\",\"lastName\":\"熊\",\"assignmentResult\":null,\"errors\":[]}]},\"relationships\":{\"betaGroup\":{\"data\":{\"type\":\"betaGroups\",\"id\":\"" + BETAGROUPS + "\"}}}}}";
            var headers = new WebHeaderCollection();
            headers.Add("Cookie", COOKIE);
            HttpRequest.HttpRequestByPost(url, data, "application/json", ref headers);
        }


        /// <summary>
        /// 删除邀请测试用户
        /// </summary>
        /// <param name="guid"></param>
        public static void DeleteInviteUser(string guid)
        {
            string url = "https://appstoreconnect.apple.com/iris/v1/apps/" + APPID + "/relationships/betaTesters";
            string data = "{\"data\":[{\"type\":\"betaTesters\",\"id\":\"" + guid + "\"}]}";
            var headers = new WebHeaderCollection();
            headers.Add("Cookie", COOKIE);
            HttpRequest.HttpRequestByDelete(url, data, "application/json", headers);
        }



        /// <summary>
        /// 获取指定时间以后的邮箱里的邀请链接
        /// </summary>
        /// <returns></returns>
        public static List<string> GetEmailInviteUrls(string account, string password, DateTime after)
        {
            List<string> urls = new List<string>();
            var mails = EmailUtils.GetEmailContent(account, password, after);
            var invites = mails.Where(o => o.Key.Contains("has invited you to test"));
            string leftStr = "href='";
            string rightStr = "\\?ct";
            foreach (var item in invites)
            {
                string url = Regex.Match(item.Value, $"(?<={leftStr})[\\s\\S]*(?={rightStr})").ToString();
                urls.Add(url);
            }
            return urls;
        }


        /// <summary>
        /// 过滤无效的邀请链接
        /// </summary>
        /// <returns></returns>
        public static List<string> FilterUselessUrl(List<string> urls)
        {
            List<string> newUrls = new List<string>();
            foreach (var item in urls)
            {
                if (CheckUrlIsUseful(item))
                {
                    newUrls.Add(item);
                }
            }
            return newUrls;
        }


        /// <summary>
        /// 检验该邀请链接是否有效
        /// </summary>
        /// <returns></returns>
        public static bool CheckUrlIsUseful(string url)
        {
            var result = HttpRequest.HttpRequestByGet(url, new WebHeaderCollection());
            if (result.Length > 3000)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获取已经安装App的邀请用户的信息（已安装状态）
        /// </summary>
        public static Dictionary<string, string> GetAlredyInstallUserInfo()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string url = "https://appstoreconnect.apple.com/iris/v1/betaTesters?filter[betaGroups]=" + BETAGROUPS + "&sort=betaTesterMetrics.betaTesterState&limit=99&include=betaTesterMetrics";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Cookie", COOKIE);
            JToken result = JToken.Parse(HttpRequest.HttpRequestByGet(url, headers));
            foreach (var item in result["included"])
            {
                //如果状态不等于未安装状态
                if (item["attributes"]["betaTesterState"].ToString() != "INVITED")
                {
                    string id = item["id"].ToString().Split(':')[0];
                    //找到这个ID对应的Email
                    foreach (var email in result["data"])
                    {
                        if (email["id"].ToString() == id)
                        {
                            dic.Add(email["attributes"]["email"].ToString(), id);
                        }
                    }
                }
            }
            return dic;
        }
    }
}

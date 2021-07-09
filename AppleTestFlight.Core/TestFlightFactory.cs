using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AppleTestFlight.Core.Models;
using AppleTestFlight.Core.Config;
namespace AppleTestFlight.Core
{
    /// <summary>
    /// AppleTestFlight工厂
    /// </summary>
    public class TestFlightFactory
    {
        private string COOKIE;
        private readonly string BETAGROUPS;
        private readonly string APPID;


        public TestFlightFactory(string appid, string betagroups)
        {
            BETAGROUPS = betagroups;
            APPID = appid;
        }


        /// <summary>
        /// 获取全局操作的Cookie（模拟登录）
        /// </summary>
        public void GetGlobalOperationCookie()
        {
            try
            {
                var trustUserInfo = AppleTestFlightConfig.GloablOperationTrustUserInfo();
                string signUrl = "https://idmsa.apple.com/appleauth/auth/signin?isRememberMeEnabled=true";
                string signData = "{\"accountName\":\"" + trustUserInfo[0] + "\",\"rememberMe\":true,\"password\":\"" + trustUserInfo[1] + "\"}";
                var headers = new WebHeaderCollection();
                headers.Add("X-Apple-Widget-Key", "e0b80c3bf78523bfe80974d320935bfa30add02e1bff88ec2166c6bd5a706c42");
                headers.Add("Cookie", trustUserInfo[2]);
                HttpRequest.HttpRequestByPost(signUrl, signData, "application/json", ref headers);
                var myacinfo = headers["Set-Cookie"].Split(';');
                Console.WriteLine(headers["Set-Cookie"]);
                COOKIE = myacinfo.First(o => o.Contains("myacinfo")).Replace("HttpOnly,", "");
            }
            catch (Exception e)
            {
                throw new Exception("获取全局操作Cookie失败，登录信息有问题" + e.Message);
            }
        }


        /// <summary>
        /// 获取所有已经邀请用户的信息（邮箱&GUID)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllInvitedUserInfo()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string url = "https://appstoreconnect.apple.com/iris/v1/betaTesters?filter[betaGroups]=" + BETAGROUPS + "&limit=100";
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
        /// 获取所有APP(名称&ID)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllApps()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string url = "https://appstoreconnect.apple.com/iris/v1/apps?include=appStoreVersions,appStoreVersionMetrics,betaReviewMetrics&limit=100&filter[removed]=false";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Cookie", COOKIE);
            JToken result = JToken.Parse(HttpRequest.HttpRequestByGet(url, headers));
            foreach (var item in result["data"])
            {
                dic.Add(item["attributes"]["name"].ToString(), item["id"].ToString());
            }
            return dic;
        }


        /// <summary>
        /// 添加邀请测试用户
        /// </summary>
        /// <param name="guid"></param>
        public void AddInviteUsers(string[] emails)
        {
            string url = "https://appstoreconnect.apple.com/iris/v1/bulkBetaTesterAssignments";
            List<Betatester> betatesters = new List<Betatester>();

            #region 批量添加用户
            foreach (var email in emails)
            {
                betatesters.Add(new Betatester()
                {
                    email = email,
                    firstName = "AppleTestFlight",
                    lastName = "AppleTestFlight"
                });
            }
            var bulkModels = new BulkBetaTesterAssignmentsModel()
            {
                data = new Data()
                {
                    type = "bulkBetaTesterAssignments",
                    attributes = new Attributes()
                    {
                        betaTesters = betatesters.ToArray()
                    },
                    relationships = new Relationships()
                    {
                        betaGroup = new Betagroup()
                        {
                            data = new Betagroup.Data()
                            {
                                type = "betaGroups",
                                id = BETAGROUPS
                            }
                        }
                    }
                }
            };
            #endregion

            var headers = new WebHeaderCollection();
            headers.Add("Cookie", COOKIE);
            HttpRequest.HttpRequestByPost(url, JsonConvert.SerializeObject(bulkModels), "application/json", ref headers);
        }


        /// <summary>
        /// 删除邀请测试用户
        /// </summary>
        /// <param name="guid"></param>
        public void DeleteInviteUsers(string[] guids)
        {
            string url = "https://appstoreconnect.apple.com/iris/v1/apps/" + APPID + "/relationships/betaTesters";
            BulkDeleteBetaTesterModel bulk = new BulkDeleteBetaTesterModel();
            List<BulkDeleteBetaTesterModel.Data> testers = new List<BulkDeleteBetaTesterModel.Data>();
            foreach (var guid in guids)
            {
                testers.Add(new BulkDeleteBetaTesterModel.Data()
                {
                    type = "betaTesters",
                    id = guid
                });
            }
            bulk.data = testers.ToArray();
            var headers = new WebHeaderCollection();
            headers.Add("Cookie", COOKIE);
            HttpRequest.HttpRequestByDelete(url, JsonConvert.SerializeObject(bulk), "application/json", headers);
        }

        /// <summary>
        /// 获取指定时间以后的邮箱里的邀请链接
        /// </summary>
        /// <returns></returns>
        public List<string> GetEmailInviteUrls(string account, string password, DateTime after)
        {
            List<string> urls = new List<string>();
            using EmailFactory emailFactory = new EmailFactory(account, password, "btmail.ym191.com", 143);
            var mails = emailFactory.GetEmailContentByTime(after);
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
        public List<string> FilterUselessUrl(List<string> urls)
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
        public bool CheckUrlIsUseful(string url)
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
        public Dictionary<string, string> GetAlredyInstallUserInfo()
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

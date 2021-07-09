using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleTestFlight.Core.Config
{
    /// <summary>
    /// 用于读取本地配置/设置的全局配置文件
    /// </summary>
    public class AppleTestFlightConfig
    {

        /// <summary>
        /// 获取全局操作浏览器信任的账号信息(账号&密码&被信任的Cookie)
        /// </summary>
        /// <returns></returns>
        public static string[] GloablOperationTrustUserInfo()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Config/GloablOperationTrustUserInfo.ini";
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                string[] line = reader.ReadLine().Split("----", StringSplitOptions.None);
                string[] data = new string[3];
                data[0] = line[0];
                data[1] = line[1];
                data[2] = line[2];
                return data;
            }
        }


        /// <summary>
        /// 获取本地所有邮箱的信息（账号&密码）
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> EmailAccountInfos()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Config/EmailInfo.ini";
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
        /// 获取本地Appid和BetaGroups
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetAppidAndBetaGroups()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Config/AppidAndBetaGroups.ini";
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                string[] line = reader.ReadLine().Split("----", StringSplitOptions.None);
                KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(line[0], line[1]);
                return keyValuePair;
            }
        }
    }
}

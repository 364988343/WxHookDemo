using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WxHookDemo
{
    public class WxUserSimple
    {
        public string name { get; set; }
        public string wxid { get; set; }
        public string label => $"{name} | {wxid}";
    }

    public class WxChatMemberSimple
    {
        public string nickname { get; set; }
        public string roomid { get; set; }
        public string wxid { get; set; }
        public string label => $"{nickname} | {wxid}";
    }

    public class WxServerSendData
    {
        public string id { get; set; }
        public string wxid { get; set; }
        public object content { get; set; }
        public int type { get; set; }
        public string roomid { get; set; }
    }


    public class WxServerRecData
    {
        public string id { get; set; }
        public string wxid { get; set; }
        public object content { get; set; }
        public int type { get; set; }
        public string sender { get; set; }
        public string time { get; set; }
        public string status { get; set; }
        public int srvid { get; set; }
        public string receiver { get; set; }
    }

    public class WxUserDetail
    {
        public string big_headimg { get; set; }
        public string city { get; set; }
        public string cover { get; set; }
        public string little_headimg { get; set; }
        public string name1 { get; set; }
        public string name2 { get; set; }
        public string nation { get; set; }
        public string provice { get; set; }
        public string signature { get; set; }
        public string v1 { get; set; }
        public string wxcode { get; set; }
    }
}

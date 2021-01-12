using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WxHookDemo
{
    class EntertainModel
    {
        public class EntertainMember
        {
            public WxChatMemberSimple wxChatModel { get; set; }
            public List<MemberCheckIn> checkinList { get; set; }
            public int gold { get; set; }            
            public int stoneNum { get; set; }
            public bool subscribed { get; set; }
            public DateTime subscribedDate { get; set; }
            public List<GameCharacters> inventory { get; set; }
        }

        public class MemberCheckIn {
            public DateTime time { get; set; }
            public int luckPoint { get; set; }
        }
        public class GameCharacters {
            public string name { get; set; }
            public int starNum { get; set; }
        }

        public class GachaPoll
        {
            public int upProb { get; set; }
            public int upProbSSR { get; set; }
            public int upProbSR { get; set; }
            public List<string> upSSRChar { get; set; }
            public List<string> upSRChar { get; set; }

            public List<string> SSRChar { get; set; }
            public List<string> SRChar { get; set; }
            public List<string> RChar { get; set; }
        }
    }
}

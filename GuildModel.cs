using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WxHookDemo
{
    class GuildModel
    {
        public class Guild
        {
            public string guildName { get; set; }
            public const int guildMemberMax = 30;
            public GuildMember[] members { get; set; }


        }
        public class GuildMember
        {
            public WxChatMemberSimple wxChatModel { get; set; }

        }
    }
}

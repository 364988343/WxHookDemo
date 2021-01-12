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
            public List<GuildMember> members { get; set; }
        }
        public class GuildMember
        {
            public WxChatMemberSimple wxChatModel { get; set; }
            public bool isAdmin { get; set; }
            public int gachaCount { get; set; }
        }

        public class GuildWarModel {
            public string name { get; set; }
            public int currentBoss { get; set; }
            public int currentStage { get; set; }
            public int currentRound { get; set; }
            public bool chainBattleMode { get; set; }

            public bool notifyReservedMembers { get; set; }

            public DateTime currentBattleStartTime { get; set; }
            public DateTime currentBattleEndTime { get; set; }

            public GuildMember currentMember { get; set; }
            public List<OnTreeMemberRecord> onTreeMembers { get; set; }

            public Boss currentBossTemplate { get; set; }

            public List<GuildWarMemberRecord> memberRecords { get; set; }

            public List<Boss> bossList { get; set; }
            
            public List<GuildMember> chainBattleMembers { get; set; }
        }

        public class OnTreeMemberRecord {
            public GuildMember member { get; set; }
            public DateTime time { get; set; }
        }

        public class GuildWarMemberRecord
        {
            public string id { get; set; }
            public GuildMember member { get; set; }
            public DateTime time { get; set; }
            public string target { get; set; }
            public int damage { get; set; }
            public string type { get; set; }
        }

        public class Boss {
            public string name { get; set; }
            public int maxHealth { get; set; }
            public int currentHealth { get; set; }
            public List<ReserveMemberModel> reservedMembers { get; set; }
        }

        public class ReserveMemberModel {
            public GuildMember member { get; set; }
            public string note { get; set; }
        }

        public class CheckAttackModel {
            public string name { get; set; }
            public string wxid { get; set; }
            public int attackCount { get; set; }
            public string type { get; set; }
        }


    }
}

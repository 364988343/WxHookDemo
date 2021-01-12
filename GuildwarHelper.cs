using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static WxHookDemo.EntertainModel;
using static WxHookDemo.GuildModel;

namespace WxHookDemo
{
    internal class GuildwarHelper
    {
        private const string pathToTheFile = "C://BotSettings//";
        GuildWarModel GuildWar = null;

        GachaPoll CurrentPoll = null;
        Random random = null;
        int gachaCount = 0;
        string notification = "";

        private static GuildwarHelper uniqueInstance;
        private static readonly object locker = new object();
        private GuildwarHelper()
        {
            string memberPath = pathToTheFile + "guildmembers.json";
            try
            {
                if (!File.Exists(@memberPath)) {
                    StreamWriter file = File.CreateText(@memberPath);
                    file.Close();
                }
                //InitializeWarBoss();

                //setup timer
                //Timer timer = new Timer();
                //timer.Enabled = true;
                //timer.Interval = 5000;
                //timer.Start();
                //timer.Elapsed += new ElapsedEventHandler(StatusChange);
            }
            catch (Exception e)
            {

            }
        }

        public static GuildwarHelper GetInstance()
        {
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new GuildwarHelper();
                    }
                }
            }
            return uniqueInstance;
        }
        public bool InitializeGuildWar(string warname)
        {

            try
            {
                string warPath = pathToTheFile + warname + ".json";

                GuildWar = new GuildWarModel();
                GuildWar.currentBoss = 1;
                GuildWar.currentRound = 1;
                GuildWar.currentStage = 1;
                GuildWar.currentMember = null;
                GuildWar.chainBattleMode = false;
                GuildWar.notifyReservedMembers = false;

                GuildWar.currentBattleStartTime = DateTime.UtcNow.AddHours(8);
                GuildWar.currentBattleEndTime = DateTime.UtcNow.AddHours(8);
                
                GuildWar.currentBossTemplate = ReadBossData()[GuildWar.currentBoss - 1];
                GuildWar.name = warname;

                GuildWar.memberRecords = new List<GuildWarMemberRecord>();
                GuildWar.bossList = new List<Boss>();
                GuildWar.bossList = ReadBossData();
                foreach (Boss boss in GuildWar.bossList) {
                    boss.reservedMembers = new List<ReserveMemberModel>();
                }                
                GuildWar.onTreeMembers = new List<OnTreeMemberRecord>();

                StreamWriter file = File.CreateText(@warPath);
                file.Close();
                LogWarDetails();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool AddTreeMember(GuildMember member) {
            try {
                OnTreeMemberRecord newRecord = new OnTreeMemberRecord();
                newRecord.member = member;
                newRecord.time = DateTime.UtcNow.AddHours(8);
                GuildWar.onTreeMembers.Add(newRecord);
                if (!GuildWar.chainBattleMode)
                {
                    //normal battle add tree                    
                    //remove current member
                    ResetBattleMember();
                }
                LogWarDetails();
                return true;
            } catch (Exception e) {
                return false;
            }
        }
        public bool RequestBattle(GuildMember member) {
            try
            {
                if (GuildWar.currentMember != null || GuildWar.chainBattleMode == true)
                {
                    return false;
                }
                else
                {
                    GuildWar.currentMember = member;
                    GuildWar.currentBattleStartTime = DateTime.UtcNow.AddHours(8);
                    GuildWar.currentBattleEndTime = DateTime.UtcNow.AddHours(8).AddMinutes(10);
                    LogWarDetails();
                    return true;
                }
            }
            catch (Exception e) {
                return false;
            }
           
        }
        public bool RequestChainBattle(GuildMember member)
        {
            try
            {
                if (GuildWar.currentMember != null && GuildWar.chainBattleMode == false)
                {
                    return false;
                }
                else
                {
                    if (GuildWar.currentMember == null) {
                        GuildWar.currentMember = member;
                        GuildWar.currentBattleStartTime = DateTime.UtcNow.AddHours(8);
                        GuildWar.currentBattleEndTime = DateTime.UtcNow.AddHours(8).AddMinutes(20);
                        GuildWar.chainBattleMembers = new List<GuildMember>();
                    }
                    GuildWar.chainBattleMode = true;
                    GuildWar.chainBattleMembers.Add(member);
                    LogWarDetails();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public bool RemoveLastDamage() {
            try {
                GuildWarMemberRecord lastRecord = GetLatestRecord();
                if (RevertDamageToBoss(lastRecord))
                {
                    GuildWar.memberRecords.Remove(lastRecord);
                    return true;
                }
                else {
                    return false;
                }
            }
            catch(Exception e) {
                return false;
            }
        }
        public string CheckMemberAttack(GuildMember member) {
            try
            {
                string output = "";
                DateTime dateNow = DateTime.Now;
                DateTime startOfToday = DateTime.Now;
                DateTime endOfToday = DateTime.Now;

                DateTime date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 5, 0, 0);
                DateTime today = DateTime.UtcNow.AddHours(8);
                if (today.TimeOfDay < date.TimeOfDay)
                {
                    startOfToday = today.Date.AddHours(-19);
                    endOfToday = today.Date.AddDays(1).AddTicks(-1).AddHours(-19);
                }
                else
                {
                    startOfToday = today.Date.AddHours(5);
                    endOfToday = today.Date.AddDays(1).AddTicks(-1).AddHours(5);
                }
                List<GuildWarMemberRecord> records = GuildWar.memberRecords.Where(s => s.time <= endOfToday && s.time >= startOfToday).ToList();
                
                output += "\n" + member.wxChatModel.nickname + "从" + startOfToday.ToString() + "到" + endOfToday.ToString() + "出刀情况为：";

                foreach (GuildWarMemberRecord record in records)
                {
                    if (record.member.wxChatModel.wxid == member.wxChatModel.wxid) {
                        output += "\n" + record.time.ToShortTimeString() + "对" + record.target + "出" + record.type + "造成" + record.damage.ToString() + "伤害"; 
                    }
                }
                return output;
            }
            catch (Exception e)
            {
                return "错误";
            }
        }
        public string CheckAttacks(string attackType) {
            try
            {
                string output = "";
                DateTime dateNow = DateTime.Now;
                DateTime startOfToday = DateTime.Now;
                DateTime endOfToday = DateTime.Now;

                DateTime date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 5, 0, 0);
                DateTime today = DateTime.UtcNow.AddHours(8);
                if (today.TimeOfDay < date.TimeOfDay)
                {
                    startOfToday = today.Date.AddHours(-19);
                    endOfToday = today.Date.AddDays(1).AddTicks(-1).AddHours(-19);
                }
                else
                {
                    startOfToday = today.Date.AddHours(5);
                    endOfToday = today.Date.AddDays(1).AddTicks(-1).AddHours(5);
                }
                List<GuildWarMemberRecord> records = GuildWar.memberRecords.Where(s => s.time <= endOfToday && s.time >= startOfToday).ToList();

                List<CheckAttackModel> outputData = new List<CheckAttackModel>();                

                foreach (GuildWarMemberRecord record in records) {
                    if (record.type == attackType)
                    {
                        if (outputData.Where(s => s.wxid == record.member.wxChatModel.wxid).FirstOrDefault() != null)
                        {
                            outputData.Where(s => s.wxid == record.member.wxChatModel.wxid).FirstOrDefault().attackCount++;
                        }
                        else
                        {
                            CheckAttackModel tempData = new CheckAttackModel();
                            tempData.wxid = record.member.wxChatModel.wxid;
                            tempData.name = record.member.wxChatModel.nickname;
                            tempData.attackCount = 1;
                            tempData.type = record.type;
                            outputData.Add(tempData);
                        }
                    }
                }

                output += "\n从"+ startOfToday.ToString() + "\n到" + endOfToday.ToString() + "\n出刀情况为：";
                foreach (CheckAttackModel attackRecord in outputData) {
                    output += "\n" + attackRecord.name + " " + attackRecord.attackCount.ToString();
                }
                output += "\n今日仍未出刀：";

                List<GuildMember> members = GetAllMembers();
                foreach (GuildMember member in members) {
                    if (records.Where(s => s.member.wxChatModel.wxid == member.wxChatModel.wxid).Count() <= 0) {
                        output += "\n"+ member.wxChatModel.nickname;
                    }
                }
                return output;
            }
            catch (Exception e) {
                return "";
            }
        }
        public bool RevertDamageToBoss(GuildWarMemberRecord lastRecord) {
            try
            {
                if (GuildWar.currentBossTemplate.name == lastRecord.target)
                {
                    //no boss killed, revert damage only
                    GuildWar.currentBossTemplate.currentHealth += lastRecord.damage;
                }
                else if (GuildWar.currentBossTemplate.name != lastRecord.target && GuildWar.currentBossTemplate.currentHealth == GuildWar.currentBossTemplate.maxHealth)
                {
                    //killed one boss
                    if (GuildWar.currentBoss == 1)
                    {
                        //first boss next round, go back to previous round, last boss
                        GuildWar.currentBoss = 5;
                        GuildWar.currentRound--;
                        if (GuildWar.currentRound >= 2)
                        {
                            GuildWar.currentStage = 2;
                        }
                        else
                        {
                            GuildWar.currentStage = 1;
                        }
                    }
                    else
                    {
                        //normal boss, go back to previous boss
                        GuildWar.currentBoss--;
                    }
                    GuildWar.currentBossTemplate = ReadBossData()[GuildWar.currentBoss - 1];
                    GuildWar.currentBossTemplate.currentHealth = lastRecord.damage;
                }
                else {
                    return false;    
                }
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }
        public bool ReportDamage(GuildMember member, int damage, string replaceTarget) {
            try
            {
                GuildWarMemberRecord record = new GuildWarMemberRecord();
                record.id = Guid.NewGuid().ToString();
                record.damage = damage;
                record.target = GuildWar.currentBossTemplate.name;
                record.time = DateTime.UtcNow.AddHours(8);

                if (replaceTarget != "")
                {
                    record.member = GetMember(replaceTarget);
                }
                else {
                    record.member = member;
                }

                if (damage == -1) {
                    record.damage = 0;
                    record.type = "SL";
                }
                else if (GuildWar.currentBossTemplate.currentHealth > damage)
                {
                    //boss remove health
                    GuildWar.currentBossTemplate.currentHealth -= damage;
                    record.type = "正常刀";
                }
                else if (GuildWar.currentBossTemplate.currentHealth == damage)
                {
                    //boss died, move to next boss
                    GuildWar.currentBossTemplate.currentHealth -= damage;
                    record.type = "尾刀";
                    SetNextBoss();
                    //boss died, remove all tree members
                    ResetTreeMembers();
                }
                else
                {
                    return false;
                }
                GuildWar.memberRecords.Add(record);
                if (!GuildWar.chainBattleMode)
                {
                    ResetBattleMember();
                }
                else {
                    if (replaceTarget == "") {
                        GuildMember tmpMember = GuildWar.chainBattleMembers.Where(s => s.wxChatModel.wxid == member.wxChatModel.wxid).FirstOrDefault();
                        GuildWar.chainBattleMembers.Remove(tmpMember);
                    }
                }
                LogWarDetails();
                return true;
            }
            catch(Exception e) {
                return false;
            }

        }
        public void ResetBattleMember()
        {
            try
            {
                GuildWar.currentMember = null;
                LogWarDetails();
            } catch (Exception e) {
            }

        }
        public bool ResetTreeMembers()
        {
            try
            {
                GuildWar.onTreeMembers = new List<OnTreeMemberRecord>();
                LogWarDetails();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<ReserveMemberModel> GetReservedMembers() {
            try {
                List<ReserveMemberModel> members = new List<ReserveMemberModel>();
                members = GuildWar.bossList[GuildWar.currentBoss-1].reservedMembers;
                return members;
            } catch (Exception e) {
                return null;
            }
        }

        public bool ReserveBoss(GuildMember member, int bossNum, string note) {
            try
            {
                if (GuildWar.bossList[bossNum-1].reservedMembers.Where(s => s.member.wxChatModel.wxid == member.wxChatModel.wxid).FirstOrDefault() == null) {
                    ReserveMemberModel newReserve = new ReserveMemberModel();
                    newReserve.member = member;
                    newReserve.note = note;
                    GuildWar.bossList[bossNum - 1].reservedMembers.Add(newReserve);
                }
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }
        public bool RemoveMemberReserve(GuildMember member) {
            try
            {
                foreach (Boss boss in GuildWar.bossList) {
                    ReserveMemberModel reserve = boss.reservedMembers.Where(s => s.member.wxChatModel.wxid == member.wxChatModel.wxid).FirstOrDefault();
                    if (reserve != null)
                    {
                        boss.reservedMembers.Remove(reserve);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //boss action
        public bool ModifyBoss(string type, string data) {
            try {
                int modifiedData = 0;
                if (int.TryParse(data, out modifiedData))
                {
                    modifiedData = Int32.Parse(data);
                }
                else {
                    return false;
                }
                if (type == "血量")
                {
                    GuildWar.currentBossTemplate.currentHealth = modifiedData;
                }
                else if (type == "周目")
                {
                    GuildWar.currentRound = modifiedData;
                    if (GuildWar.currentRound >= 2)
                    {
                        GuildWar.currentStage = 2;
                    }
                    else {
                        GuildWar.currentStage = 1;
                    }
                }
                else if (type == "boss") {
                    GuildWar.currentBoss = modifiedData;
                    GuildWar.currentBossTemplate = ReadBossData()[GuildWar.currentBoss - 1];
                    GuildWar.currentBossTemplate.currentHealth = GuildWar.currentBossTemplate.maxHealth;
                }
                LogWarDetails();
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        #region guild management
        public string AddMember(GuildMember newMember)
        {
            string path = pathToTheFile + "guildmembers.json";

            var json = File.ReadAllText(path);
            var members = JsonConvert.DeserializeObject<List<GuildMember>>(json);
            var result = new GuildMember();
            if (members != null)
            {
                foreach (var c in members)
                {
                    if (c.wxChatModel.wxid == newMember.wxChatModel.wxid)
                    {
                        return "\n会员无法重复入会";
                    }
                }
                members.Add(newMember);
                File.WriteAllText(path, JsonConvert.SerializeObject(members));
            }
            else
            {
                List<GuildMember> guildMembers = new List<GuildMember>();
                guildMembers.Add(newMember);
                File.WriteAllText(path, JsonConvert.SerializeObject(guildMembers));
            }
            return "\n成功加入公会";
        }
        public GuildMember GetMember(string wxid)
        {
            try
            {
                string path = pathToTheFile + "guildmembers.json";

                var json = File.ReadAllText(path);
                var members = JsonConvert.DeserializeObject<List<GuildMember>>(json);
                GuildMember result = null;
                if (members != null)
                {
                    foreach (var c in members)
                    {
                        if (c.wxChatModel.wxid == wxid)
                        {
                            result = c;
                            break;
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<GuildMember> GetAllMembers()
        {
            try
            {
                string path = pathToTheFile + "guildmembers.json";

                var json = File.ReadAllText(path);
                List<GuildMember> members = JsonConvert.DeserializeObject<List<GuildMember>>(json);
                return members;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion
        #region IO

        public bool ReadGuildWar(string warname)
        {
            try
            {
                string warPath = pathToTheFile + warname + ".json";
                var json = File.ReadAllText(warPath);
                GuildWarModel guildwar = JsonConvert.DeserializeObject<GuildWarModel>(json);
                GuildWar = guildwar;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public List<Boss> ReadBossData()
        {
            try
            {
                string path = pathToTheFile + "bossinfo.json";
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Boss>>(json);
            }
            catch (Exception e)
            {
                return null;
            }

        }
        #endregion
        #region helper functions
        public GuildWarMemberRecord GetLatestRecord()
        {
            try
            {
                DateTime time = DateTime.UtcNow.AddHours(8);
                double difference = 0;
                double minDifference = 9999999;
                string minRecordGUID = "";

                foreach (GuildWarMemberRecord record in GuildWar.memberRecords)
                {
                    difference = (time - record.time).TotalSeconds;
                    if (difference < minDifference)
                    {
                        minDifference = difference;
                        minRecordGUID = record.id;
                    }
                }
                GuildWarMemberRecord latestRecord = GuildWar.memberRecords.Where(s => s.id == minRecordGUID).FirstOrDefault();
                return latestRecord;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public GuildWarModel GetWarModel()
        {
            return GuildWar;
        }
        public GuildMember GetChainBattleMember(string wxid)
        {
            return GuildWar.chainBattleMembers.Where(s => s.wxChatModel.wxid == wxid).FirstOrDefault();
        }
        public List<GuildMember> GetAllChainBattleMembers()
        {
            return GuildWar.chainBattleMembers;
        }
        public string GetTreeMembers()
        {
            try
            {
                string output = "\n当前无人挂树！";
                if (GuildWar.onTreeMembers != null && GuildWar.onTreeMembers.Count() > 0)
                {
                    output = "\n当前挂树成员为：\n";
                    foreach (OnTreeMemberRecord record in GuildWar.onTreeMembers)
                    {
                        output += record.member.wxChatModel.nickname + " 已经挂了" + Math.Ceiling((DateTime.UtcNow.AddHours(8) - record.time).TotalMinutes) + "分钟\n";
                    }
                }
                return output;
            }
            catch (Exception e)
            {
                return "";
            }
        }
        public string GetCurrentBoss()
        {
            string output = "\n错误，boss未初始化";
            if (GuildWar != null)
            {
                output = notification + "\n-----周目：" + GuildWar.currentRound + " 阶段：" + GuildWar.currentStage + "-----\n 当前boss:" + GuildWar.currentBossTemplate.name + " \n" + GuildWar.currentBossTemplate.currentHealth + "/" + GuildWar.currentBossTemplate.maxHealth;
            }
            return output;
        }
        public void SetNextBoss()
        {
            if (GuildWar.currentBoss == 5)
            {
                GuildWar.currentBoss = 1;
                GuildWar.currentRound++;
                if (GuildWar.currentRound >= 2)
                {
                    GuildWar.currentStage = 2;
                }
                GuildWar.currentBossTemplate = ReadBossData()[GuildWar.currentBoss - 1];
            }
            else if (GuildWar.currentBoss < 5)
            {
                GuildWar.currentBoss++;
                GuildWar.currentBossTemplate = ReadBossData()[GuildWar.currentBoss - 1];
            }
            GuildWar.currentBossTemplate.currentHealth = GuildWar.currentBossTemplate.maxHealth;
            GuildWar.notifyReservedMembers = true;
        }
        public bool GetNotifyStatus() {
            try {
                return GuildWar.notifyReservedMembers;
            }catch(Exception e){
                return false;
            }            
        }
        public void SetNotifyStatus(bool status) {
            try
            {
                GuildWar.notifyReservedMembers = status;
            }
            catch (Exception e)
            {
            }
        }
        public GuildMember GetBattleMember()
        {
            return GuildWar.currentMember;
        }
        public bool GetChainBattleStatus()
        {
            return GuildWar.chainBattleMode;
        }
        public void SetChainBattleStatus(bool status)
        {
            GuildWar.chainBattleMode = status;
        }
        public bool SetNotification(string info) {
            try
            {
                if (info == "")
                {
                    notification = "";
                }
                else {
                    notification = "\n" + info;
                }
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }
        public DateTime StartOfDay(DateTime theDate)
        {
            return theDate.Date;
        }
        public DateTime EndOfDay(DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }
        public bool LogWarDetails()
        {
            try
            {
                string warPath = pathToTheFile + GuildWar.name + ".json";
                File.WriteAllText(warPath, JsonConvert.SerializeObject(GuildWar));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public string GetNotification() {
            try {
                return "\n今日公告：" + notification;
            }
            catch (Exception e) {
                return "";
            }
        }

        #endregion

        //archived
        //public bool InitializeWarBoss()
        //{
        //    try {
        //        string path = pathToTheFile + "bossinfo.json";

        //        List<Boss> bosses = new List<Boss>();
        //        Boss boss = new Boss();
        //        boss.name = "一王";
        //        boss.maxHealth = 6000000;
        //        boss.currentHealth = 6000000;
        //        bosses.Add(boss);
        //        boss = new Boss();
        //        boss.name = "二王";
        //        boss.maxHealth = 8000000;
        //        boss.currentHealth = 8000000;
        //        bosses.Add(boss);
        //        boss = new Boss();
        //        boss.name = "三王";
        //        boss.maxHealth = 10000000;
        //        boss.currentHealth = 10000000;
        //        bosses.Add(boss);
        //        boss = new Boss();
        //        boss.name = "四王";
        //        boss.maxHealth = 12000000;
        //        boss.currentHealth = 12000000;
        //        bosses.Add(boss);
        //        boss = new Boss();
        //        boss.name = "五王";
        //        boss.maxHealth = 20000000;
        //        boss.currentHealth = 20000000;
        //        bosses.Add(boss);

        //        //BossList = bosses;
        //        StreamWriter file = File.CreateText(@path);
        //        file.Close();
        //        File.WriteAllText(path, JsonConvert.SerializeObject(bosses));
        //        return true;
        //    }
        //    catch (Exception e) {
        //        return false;
        //    }            
        //}
    }
}

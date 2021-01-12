using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WxHookDemo.EntertainModel;

namespace WxHookDemo
{
    class EntertainHelper
    {
        private const string pathToTheFile = "C://BotSettings//";
        GachaPoll CurrentPoll = null;
        Random random = null;
        private static EntertainHelper uniqueInstance;
        private static readonly object locker = new object();
        private EntertainHelper()
        {
            string memberPath = pathToTheFile + "entertainMember.json";
            try
            {
                if (!File.Exists(@memberPath))
                {
                    StreamWriter file = File.CreateText(@memberPath);
                    file.Close();
                }
            }
            catch (Exception e)
            {

            }
        }
        public static EntertainHelper GetInstance()
        {
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new EntertainHelper();
                    }
                }
            }
            return uniqueInstance;
        }

        public string CreateMember(WxChatMemberSimple newMember) {
            try {
                string path = pathToTheFile + "entertainMember.json";

                var json = File.ReadAllText(path);
                var members = JsonConvert.DeserializeObject<List<EntertainMember>>(json);
                var result = new EntertainMember();
                if (members != null)
                {
                    foreach (var c in members)
                    {
                        if (c.wxChatModel.wxid == newMember.wxid)
                        {
                            return "\n无法重复加入！";
                        }
                    }
                    result.wxChatModel = newMember;
                    result.gold = 30000;
                    result.inventory = new List<GameCharacters>();
                    result.stoneNum = 0;
                    result.checkinList = new List<MemberCheckIn>();
                    result.subscribed = false;
                    result.subscribedDate = DateTime.UtcNow.AddDays(-50);
                    members.Add(result);
                    File.WriteAllText(path, JsonConvert.SerializeObject(members));
                }
                else
                {
                    List<EntertainMember> guildMembers = new List<EntertainMember>();
                    result.wxChatModel = newMember;
                    result.gold = 30000;
                    result.inventory = new List<GameCharacters>();
                    guildMembers.Add(result);
                    File.WriteAllText(path, JsonConvert.SerializeObject(guildMembers));
                }
                return "\n恭喜加入，你已获得 30000 钻石 入会奖励！";
            }
            catch (Exception e) {
                return "错误！"+ e.ToString();
            }
        }

        public EntertainMember GetEntertainMember(string wxid) {
            try {
                string path = pathToTheFile + "entertainMember.json";

                var json = File.ReadAllText(path);
                var members = JsonConvert.DeserializeObject<List<EntertainMember>>(json);
                var result = new EntertainMember();
                if (members != null)
                {
                    foreach (var c in members)
                    {
                        if (c.wxChatModel.wxid == wxid)
                        {
                            result = c;
                            return result;
                        }
                    }
                }
                return null;
            }
            catch (Exception e) {
                return null;
            }
        }
        public string CheckGold(EntertainMember entertainMember) {
            try {
                return "\n你还有 "+ entertainMember.gold.ToString() + " 钻石!";
            } catch (Exception e) {
                return "错误！" + e.ToString();
            }
        }
        public string CheckInventory(EntertainMember entertainMember) {
            try
            {
                string output = "";
                output += "\n你一共拥有" + entertainMember.inventory.Count() + "角色, 三星角色有：";
                foreach (GameCharacters character in entertainMember.inventory) {
                    if (character.starNum == 3) {
                        output += "\n" + character.name;
                    }                    
                }
                output += "\n你有"+ entertainMember.stoneNum+"母猪石";
                if (entertainMember.subscribed)
                {
                    output += "\n你是尊贵的月卡会员！";
                }
                else {
                    output += "\n你的月卡已过期！";
                }                
                return output;
            }
            catch (Exception e)
            {
                return "错误！" + e.ToString();
            }
        }
        public string CheckLuckyPoint(EntertainMember entertainMember)
        {
            try
            {
                int luck = 0;
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
                MemberCheckIn record = entertainMember.checkinList.Where(s => s.time <= endOfToday && s.time >= startOfToday).FirstOrDefault();
                if (record == null) {
                    return "\n你害没签到，就想看运势？";
                }
                luck = record.luckPoint;
                if (luck < 2)
                {
                    output = "\n？！见鬼，是大凶兆！！";
                }
                else if (luck < 6)
                {
                    output = "\n你今天的运气似乎卜太星啊，凶！";
                }
                else if (luck < 50) {
                    output = "\n运气不错，吉！";
                }
                else if (luck < 90)
                {
                    output = "\n运气很棒，中吉！";
                }
                else if (luck <= 100)
                {
                    output = "\n？！大吉！！是大吉啊！！";
                }
                return output;
            }
            catch (Exception e)
            {
                return "错误！" + e.ToString();
            }
        }
        public bool AddGold(string amount)
        {
            try
            {
                int goldAmount = 0;
                if (int.TryParse(amount, out goldAmount))
                {
                    goldAmount = Int32.Parse(amount);
                }
                else
                {
                    return false;
                }
                string path = pathToTheFile + "entertainMember.json";

                var json = File.ReadAllText(path);
                var members = JsonConvert.DeserializeObject<List<EntertainMember>>(json);
                if (members != null)
                {
                    foreach (var c in members)
                    {
                        c.gold += goldAmount;
                        LogMemberDetails(c);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public string SingleGacha(EntertainMember entertainMember)
        {
            try
            {
                random = new Random();
                if (entertainMember.gold < 150)
                {
                    return "？！你钻石不够力！";
                }
                if (CurrentPoll == null)
                {
                    return "\n当前没有扭蛋池子开放！";
                }
                entertainMember.gold -= 150;

                string output = "";
                int rnum = random.Next(1000) + 1;

                GameCharacters newChar = new GameCharacters();

                if (rnum <= CurrentPoll.upProbSSR)
                {
                    if (rnum <= CurrentPoll.upProb)
                    {
                        newChar.name = CurrentPoll.upSSRChar[0];
                        newChar.starNum = 3;
                    }
                    else
                    {
                        int num = random.Next(CurrentPoll.SSRChar.Count());
                        newChar.name = CurrentPoll.SSRChar[num];
                        newChar.starNum = 3;
                    }
                }
                else if (rnum > 1000 - (CurrentPoll.upProbSR))
                {
                    if (rnum >= 970)
                    {
                        newChar.name = CurrentPoll.upSRChar[0];
                        newChar.starNum = 2;
                    }
                    else
                    {
                        int num = random.Next(CurrentPoll.SRChar.Count());
                        newChar.name = CurrentPoll.SRChar[num];
                        newChar.starNum = 2;
                    }
                }
                else
                {
                    int num = random.Next(CurrentPoll.RChar.Count());
                    newChar.name = CurrentPoll.RChar[num];
                    newChar.starNum = 1;
                }

                if (entertainMember.inventory.Where(s => s.name == newChar.name).FirstOrDefault() == null)
                {
                    entertainMember.inventory.Add(newChar);
                    output += "\nNEW!! ";
                    for (int i = 0; i < newChar.starNum; i++)
                    {
                        output += "★";
                    }
                    output += newChar.name;
                }
                else
                {
                    output += "\n";
                    for (int i = 0; i < newChar.starNum; i++)
                    {
                        output += "★";
                    }
                    int stone = 0;
                    switch (newChar.starNum)
                    {
                        case 1:
                            stone = 1;
                            break;
                        case 2:
                            stone = 10;
                            break;
                        case 3:
                            stone = 50;
                            break;
                    }
                    output += newChar.name + " => " + stone + "母！";
                    entertainMember.stoneNum += stone;
                }
                LogMemberDetails(entertainMember);
                return output;
            }
            catch (Exception e) {
                return "错误！" + e.ToString();
            }

        }
        public string TenGacha(EntertainMember entertainMember)
        {
            try
            {
                random = new Random();
                if (entertainMember.gold < 1500)
                {
                    return "\n？！你钻石不够力！";
                }
                if (CurrentPoll == null) {
                    return "\n当前没有扭蛋池子开放！";
                }
                string output = "";
                

                List<GameCharacters> newChars = new List<GameCharacters>();
                entertainMember.gold -= 1500;
                for (int i = 0; i < 10; i++) {
                    GameCharacters newChar = new GameCharacters();
                    int rnum = random.Next(1000) + 1;
                    if (rnum <= CurrentPoll.upProbSSR)
                    {
                        if (rnum <= CurrentPoll.upProb)
                        {
                            newChar.name = CurrentPoll.upSSRChar[0];
                            newChar.starNum = 3;
                        }
                        else
                        {
                            int num = random.Next(CurrentPoll.SSRChar.Count());
                            newChar.name = CurrentPoll.SSRChar[num];
                            newChar.starNum = 3;
                        }
                    }
                    else if (rnum > 1000 - (CurrentPoll.upProbSR))
                    {
                        if (rnum >= 970)
                        {
                            newChar.name = CurrentPoll.upSRChar[0];
                            newChar.starNum = 2;
                        }
                        else
                        {
                            int num = random.Next(CurrentPoll.SRChar.Count());
                            newChar.name = CurrentPoll.SRChar[num];
                            newChar.starNum = 2;
                        }
                    }
                    else
                    {
                        int num = random.Next(CurrentPoll.RChar.Count());
                        newChar.name = CurrentPoll.RChar[num];
                        newChar.starNum = 1;
                    }
                    newChars.Add(newChar);
                }

                if (newChars.Where(s => s.starNum == 2).FirstOrDefault() == null && newChars.Where(s => s.starNum == 3).FirstOrDefault() == null) {
                    GameCharacters newChar = new GameCharacters();
                    int rnum = random.Next(100) + 1;
                    if (rnum <= 16)
                    {
                        newChar.name = CurrentPoll.upSRChar[0];
                    }
                    else {
                        int num = random.Next(CurrentPoll.SRChar.Count());
                        newChar.name = CurrentPoll.SRChar[num];
                    }
                    newChar.starNum = 2;
                    newChars.RemoveAt(9);
                    newChars.Add(newChar);
                }
                foreach (GameCharacters character in newChars) {
                    if (entertainMember.inventory.Where(s => s.name == character.name).FirstOrDefault() == null)
                    {
                        entertainMember.inventory.Add(character);
                        output += "\nNEW!! ";
                        for (int i = 0; i < character.starNum; i++)
                        {
                            output += "★";
                        }
                        output += character.name;
                    }
                    else
                    {
                        output += "\n";
                        for (int i = 0; i < character.starNum; i++)
                        {
                            output += "★";
                        }
                        int stone = 0;
                        switch (character.starNum)
                        {
                            case 1:
                                stone = 1;
                                break;
                            case 2:
                                stone = 10;
                                break;
                            case 3:
                                stone = 50;
                                break;
                        }
                        output += character.name + " => " + stone + "母！";
                        entertainMember.stoneNum += stone;
                    }
                }
                LogMemberDetails(entertainMember);
                return output;
            }
            catch (Exception e)
            {
                return "错误！" + e.ToString();
            }

        }
        public string GetGachaPoll() {
            try {
                string output = "";
                if (CurrentPoll == null)
                {
                    output += "\n当前没有扭蛋池子开放！";
                }
                else {
                    output += "\n当前奖池up角色为：";
                    foreach (string character in CurrentPoll.upSSRChar) {
                        output += "\n" + character + "! ★★★ 概率：" + Math.Round((Convert.ToDouble(CurrentPoll.upProbSSR) / 1000), 3);
                    }
                    foreach (string character in CurrentPoll.upSRChar)
                    {
                        output += "\n" + character + "! ★★概率：" + Math.Round((Convert.ToDouble(CurrentPoll.upProbSR) / 1000), 3);
                    }
                }
                return output;
            }
            catch (Exception e) {
                return "错误 " + e.ToString();
            }
        }
        public string CheckIn(EntertainMember entertainMember) {
            try {
                string output = "";
                random = new Random();
                int rnum = random.Next(100) + 1;

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

                if (entertainMember.checkinList == null)
                    entertainMember.checkinList = new List<MemberCheckIn>();
                MemberCheckIn record = entertainMember.checkinList.Where(s => s.time <= endOfToday && s.time >= startOfToday).FirstOrDefault();

                if (today > entertainMember.subscribedDate) {
                    entertainMember.subscribed = false;
                }
                if (record == null)
                {
                    MemberCheckIn newRecord = new MemberCheckIn();
                    newRecord.time = DateTime.UtcNow.AddHours(8);
                    newRecord.luckPoint = rnum;
                    entertainMember.checkinList.Add(newRecord);
                    entertainMember.gold += 500;
                    output += "\n你已成功签到，获得 500 钻石！";
                    if (entertainMember.subscribed)
                    {
                        entertainMember.gold += 500;
                        output += "\n哦豁，你是尊贵的月卡玩家？额外获得 500 钻石！";
                    }
                }
                else {
                    return "\n？你今天签到过了！";
                }
                output += "\n你的钻石余量为 " + entertainMember.gold.ToString();
                LogMemberDetails(entertainMember);
                return output;
            } catch (Exception e) {
                return "错误！" + e.ToString();
            }

        }

        public string LoadPoll(string pollname) {
            try {
                string path = pathToTheFile + pollname +".json";
                var json = File.ReadAllText(path);
                var poll = JsonConvert.DeserializeObject<GachaPoll>(json);
                CurrentPoll = poll;
                return pollname + "成功读取！";
            } catch (Exception e) {
                return "错误 "+ e.ToString();
            }
        }

        public void LogMemberDetails(EntertainMember entertainMember) {
            try
            {
                string path = pathToTheFile + "entertainMember.json";

                var json = File.ReadAllText(path);
                List<EntertainMember> members = JsonConvert.DeserializeObject<List<EntertainMember>>(json);
                
                if (members != null)
                {
                    members.Remove(members.Where(s => s.wxChatModel.wxid == entertainMember.wxChatModel.wxid).FirstOrDefault());
                    members.Add(entertainMember);
                }
                File.WriteAllText(path, JsonConvert.SerializeObject(members));
            }
            catch (Exception e)
            {
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

        public void makeJSON() {
            string warPath = pathToTheFile + "镜华池.json";
            var json = File.ReadAllText(warPath);
            CurrentPoll = new GachaPoll();
            CurrentPoll.upProb = 7;
            CurrentPoll.upProbSSR = 50;
            CurrentPoll.upProbSR = 180;
            CurrentPoll.upSSRChar = new List<string>();
            CurrentPoll.upSSRChar.Add("初音");

            CurrentPoll.SSRChar = new List<string>();
            CurrentPoll.SRChar = new List<string>();
            CurrentPoll.RChar = new List<string>();

            string[] temp = new string[]{"杏奈","真步","璃乃","伊绪",
                "咲恋","望","妮诺","秋乃","真琴",
                "纯","静流","莫妮卡","吉塔","亚里莎","镜华" };
            foreach (string str in temp)
            {
                CurrentPoll.SSRChar.Add(str);
            }

            temp = new string[]{"茜里","宫子","雪","铃奈","香织","美美","绫音","铃","惠理子",
            "忍","真阳","栞","千歌","空花","珠希","美冬","深月"};
            foreach (string str in temp)
            {
                CurrentPoll.SRChar.Add(str);
            }

            temp = new string[]{"日和","怜","胡桃","依里","铃莓",
            "优花梨","碧","美咲","莉玛"};
            foreach (string str in temp)
            {
                CurrentPoll.RChar.Add(str);
            }

            File.WriteAllText(warPath, JsonConvert.SerializeObject(CurrentPoll));
        }

    }
}

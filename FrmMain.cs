using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using Yus.Apps;
using static WxHookDemo.EntertainModel;
using static WxHookDemo.GuildModel;

namespace WxHookDemo
{
    public partial class FrmMain : Form
    {

        WxChatMemberSimple[] groupMembers;
        GuildwarHelper guildwarHelper = GuildwarHelper.GetInstance();
        EntertainHelper entertainHelper = EntertainHelper.GetInstance();

        const string adminid = "wxid_ogrbmj6u3zwb12";
        const string roomid = "17786517750@chatroom";
        private bool manageMode = true;
        private bool guildwarMode = true;

        private bool notifyReservedMember = false;
        private const string pathToTheFile = "..\\..\\BotSettings\\";

        private DateTime currentBattleEndTime;
        private GuildMember currentBattleMember;

        System.Threading.AutoResetEvent stopWaitHandle = new System.Threading.AutoResetEvent(false);

        #region 消息枚举

        public const int HEART_BEAT = 5005;
        public const int RECV_TXT_MSG = 1;
        public const int RECV_PIC_MSG = 3;
        public const int USER_LIST = 5000;
        public const int GET_USER_LIST_SUCCSESS = 5001;
        public const int GET_USER_LIST_FAIL = 5002;
        public const int TXT_MSG = 555;
        public const int PIC_MSG = 500;
        public const int AT_MSG = 550;
        public const int CHATROOM_MEMBER = 5010;
        public const int CHATROOM_MEMBER_NICK = 5020;
        public const int PERSONAL_INFO = 6500;
        public const int DEBUG_SWITCH = 6000;
        public const int PERSONAL_DETAIL = 6550;

        #endregion

        #region 属性

        #region 组件

        public WebSocket WxServer { get; set; }

        public Timer HeartTimer { get; set; }

        public OpenFileDialog SendPicDialog { get; set; } = new OpenFileDialog()
        {
            RestoreDirectory = true,
            AddExtension = true,
            Filter = "图片文件|*.png;*.jpg;*.jpeg;*.gif",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Title = "请选择发送图片",
            Multiselect = false,
            CheckFileExists = true
        };

        #endregion

        #region 数据

        public List<WxUserSimple> WxUsers { get; set; } = new List<WxUserSimple>();

        public List<WxChatMemberSimple> WxChatMembers { get; set; } = new List<WxChatMemberSimple>();

        #endregion

        #region 状态

        private DateTime lastHeartTime;
        public DateTime LastHeartTime
        {
            get => lastHeartTime;
            set
            {
                lastHeartTime = value;
                RunUi(() =>
                {
                    lblConnectStatus.Text = value.ToString("yyyy-MM-dd hh:mm:ss");
                });
            }
        }

        public bool WxDebug
        {
            get => ckbDebug.Checked;
            set
            {
                RunUi(() =>
                {
                    ckbDebug.Checked = value;
                    if (value)
                    {
                        ckbDebug.ForeColor = Color.DodgerBlue;
                        ckbDebug.Text = "调试已开启";
                    }
                    else
                    {
                        ckbDebug.ForeColor = Color.DimGray;
                        ckbDebug.Text = "调试已关闭";
                    }
                });
            }
        }

        private bool wxServerRunning;
        public bool WxServerRunning
        {
            get => wxServerRunning;
            set
            {
                wxServerRunning = value;
                RunUi(() =>
                {
                    WxUsers.Clear();
                    WxChatMembers.Clear();

                    btnConnect.Enabled = !value;
                    btnDisconnect.Enabled = value;
                    btnGetUserList.Enabled = value;
                    if (!value)
                    {
                        cbUserListGet.Items.Clear();
                        cbUserListSend.Items.Clear();
                        cbChatList.Items.Clear();
                        cbWxChatMemberNickname.Items.Clear();

                        WxChatSelected = false;

                        lblConnectStatus.Text = "未连接";
                        lblConnectStatus.ForeColor = Color.Red;
                        cbUserListGet.Enabled = value;
                        cbUserListSend.Enabled = value;
                        cbWxChatMemberNickname.Enabled = value;
                        cbChatList.Enabled = value;
                        btnGetPersonalInfo.Enabled = value;
                        btnGetPersonalDetail.Enabled = value;
                        btnGetChatMemberNickname.Enabled = value;
                        btnSendTextMsg.Enabled = value;
                        btnSendPicMsg.Enabled = value;
                    }
                    else
                    {
                        lblConnectStatus.Text = "已连接";
                        lblConnectStatus.ForeColor = Color.Green;
                    }
                });
            }
        }

        private bool wxUserFetced;
        public bool WxUserFetced
        {
            get => wxUserFetced;
            set
            {
                wxUserFetced = value;
                RunUi(() =>
                {
                    cbUserListGet.Enabled = value;
                    cbUserListSend.Enabled = value;
                    cbChatList.Enabled = value;
                    btnGetPersonalInfo.Enabled = value;
                    btnGetPersonalDetail.Enabled = value;
                    btnGetChatMemberNickname.Enabled = value;
                    btnSendTextMsg.Enabled = value;
                    btnSendPicMsg.Enabled = value;
                });
            }
        }

        private bool wxChatMemberFetced;
        public bool WxChatMemberFetced
        {
            get => wxChatMemberFetced;
            set
            {
                wxChatMemberFetced = value;
                RunUi(() =>
                {
                    cbWxChatMemberNickname.Enabled = value;
                });
            }
        }

        private bool wxChatSelected;
        public bool WxChatSelected
        {
            get => wxChatSelected;
            set
            {
                wxChatSelected = value;
                RunUi(() =>
                {
                    btnSendChatTextMsg.Enabled = value;
                    btnSendChatPicMsg.Enabled = value;
                });
            }
        }

        #endregion

        #region 属性

        public string WxUserGetWxid => (cbUserListGet.SelectedItem as WxUserSimple)?.wxid;

        public string WxUserSendWxid => (cbUserListSend.SelectedItem as WxUserSimple)?.wxid;

        public string WxChatMemberWxid => (cbWxChatMemberNickname.SelectedItem as WxChatMemberSimple)?.wxid;

        public string WxChatWxid => (cbChatList.SelectedItem as WxUserSimple)?.wxid;

        #endregion

        #endregion

        #region 初始化

        public FrmMain()
        {
            InitializeComponent();
            InitForm();
        }

        public void InitForm()
        {
            cbUserListGet.DisplayMember = "label";
            cbUserListGet.ValueMember = "wxid";
            cbUserListSend.DisplayMember = "label";
            cbUserListSend.ValueMember = "wxid";
            cbWxChatMemberNickname.DisplayMember = "label";
            cbWxChatMemberNickname.ValueMember = "wxid";
            cbChatList.DisplayMember = "label";
            cbChatList.ValueMember = "wxid";

            cbChatList.SelectedIndexChanged += CbChatList_SelectedIndexChanged;
            ckbDebug.CheckedChanged += CkbDebug_CheckedChanged;

            btnConnect.Click += (s, e) => ConnectWx();
            btnDisconnect.Click += (s, e) => DisconnectWx();
            btnClearLog.Click += (s, e) => RunUi(() => txtLog.Clear());
            btnGetUserList.Click += BtnGetUserList_Click;
            btnGetPersonalDetail.Click += BtnGetPersonalDetail_Click;
            btnGetChatMemberNickname.Click += BtnGetChatMemberNickname_Click;
            btnGetPersonalInfo.Click += BtnGetPersonalInfo_Click;
            btnSendTextMsg.Click += BtnSendMsg_Click;
            btnSendChatTextMsg.Click += BtnSendChatMsg_Click;
            btnClearRecMsg.Click += (s, e) => RunUi(() => txtRecMsg.Clear());
            btnSendPicSelect.Click += BtnSendPicSelect_Click;
            btnSendChatPicSelect.Click += BtnSendChatPicSelect_Click;
            btnSendPicMsg.Click += BtnSendPicMsg_Click;
            btnSendChatPicMsg.Click += BtnSendChatPicMsg_Click;
        }

        #endregion

        #region 事件

        private void BtnSendChatPicMsg_Click(object sender, EventArgs e)
        {
            if (txtSendChatPicUrl.Text.YusNullOrWhiteSpace())
            {
                MessageBox.Show("请先选择图片");
                return;
            }

            var sendContent = new
            {
                id = GetId(),
                type = PIC_MSG,
                content = txtSendChatPicUrl.Text,
                wxid = WxChatWxid
            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[发送群组图片消息][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void BtnSendPicMsg_Click(object sender, EventArgs e)
        {
            if (txtSendPicUrl.Text.YusNullOrWhiteSpace())
            {
                MessageBox.Show("请先选择图片");
                return;
            }

            var sendContent = new
            {
                id = GetId(),
                type = PIC_MSG,
                content = txtSendPicUrl.Text,
                wxid = WxUserSendWxid
            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[发送图片消息][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void BtnSendChatPicSelect_Click(object sender, EventArgs e)
        {
            var result = SendPicDialog.ShowDialog();
            if (result != DialogResult.OK) return;
            txtSendChatPicUrl.Text = SendPicDialog.FileName;
        }

        private void BtnSendPicSelect_Click(object sender, EventArgs e)
        {
            var result = SendPicDialog.ShowDialog();
            if (result != DialogResult.OK) return;
            txtSendPicUrl.Text = SendPicDialog.FileName;
        }

        private void CkbDebug_CheckedChanged(object sender, EventArgs e)
        {
            var sendContent = new
            {
                id = GetId(),
                type = DEBUG_SWITCH,
                content = WxDebug ? "on" : "off",
                wxid = "ROOT"
            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[设置调试状态][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void BtnSendChatMsg_Click(object sender, EventArgs e)
        {
            if (WxChatWxid == null)
            {
                MessageBox.Show("请选择群组");
                return;
            }

            string sendContent = null;

            if (ckbAtMember.Checked)
            {
                var member = cbWxChatMemberNickname.SelectedItem as WxChatMemberSimple;
                if (member == null)
                {
                    MessageBox.Show("群员信息不正确");
                    return;
                }
                sendContent = new
                {
                    id = GetId(),
                    type = AT_MSG,
                    content = txtChatMsg.Text,
                    wxid = member.wxid,
                    roomid = member.roomid,
                    nickname = member.nickname
                }.YusToJson();
            }
            else
            {
                sendContent = new
                {
                    id = GetId(),
                    type = TXT_MSG,
                    content = txtChatMsg.Text,
                    wxid = WxChatWxid
                }.YusToJson();
            }

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[发送群消息][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void BtnSendMsg_Click(object sender, EventArgs e)
        {
            var sendContent = new
            {
                id = GetId(),
                type = TXT_MSG,
                content = txtSendMsg.Text,
                wxid = WxUserSendWxid
            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[发送消息][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void BtnGetPersonalInfo_Click(object sender, EventArgs e)
        {
            var sendContent = new
            {
                id = GetId(),
                type = PERSONAL_INFO,
                content = "op:personal info",
                wxid = "ROOT"
            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[获取自身信息][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void BtnGetPersonalDetail_Click(object sender, EventArgs e)
        {
            var sendContent = new
            {
                id = GetId(),
                type = PERSONAL_DETAIL,
                content = "op:personal detail",
                wxid = WxUserGetWxid
            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[获取用户详情][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void BtnGetChatMemberNickname_Click(object sender, EventArgs e)
        {
            var sendContent = new
            {
                id = GetId(),
                type = CHATROOM_MEMBER_NICK,
                content = WxUserGetWxid,
                wxid = "ROOT"
            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[获取群成员昵称][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void BtnGetUserList_Click(object sender, EventArgs e)
        {
            var sendContent = new
            {
                id = GetId(),
                type = USER_LIST,
                content = "user list",
                wxid = "null"
            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLog($"[获取用户列表][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void HeartTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now - LastHeartTime > TimeSpan.FromSeconds(15))
            {
                RunUi(() =>
                {
                    lblConnectStatus.ForeColor = Color.Red;
                });
            }
        }

        private void CbChatList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sendContent = new
            {
                id = GetId(),
                type = CHATROOM_MEMBER_NICK,
                content = WxChatWxid,
                wxid = "ROOT"
            }.YusToJson();

            WxChatSelected = true;

            WxServer.SendAsync(sendContent, (result) =>
            {
                WriteLogFile($"[获取群成员昵称][{YusDate.StringDateTime()}] 发送数据：{sendContent}\n结果：{result}");
            });
        }

        private void WsOnOpen(object sender, EventArgs e)
        {
            WriteLog($"[WS连接事件][{YusDate.StringDateTime()}] 已连接到：{txtUrl.Text}");
        }

        private void WsOnClose(object sender, CloseEventArgs e)
        {
            WriteLog($"[WS关闭事件][{YusDate.StringDateTime()}] 与{txtUrl.Text}的连接已关闭({e.Code})，关闭原因：{e.Reason}");
        }

        private void WsOnMessage(object sender, MessageEventArgs e)
        {
            if (e == null)
            {
                WriteLog($"[WS信息事件][{YusDate.StringDateTime()}] 获取到空信息");
                return;
            }

            if (e.IsText)
            {
                HandleRecMessage(e.Data);
                return;
            }

            WriteLog($"[WS信息事件][{YusDate.StringDateTime()}] 获取到无法处理的事件，消息内容：\n{e.YusToJson()}");
        }

        private void WsOnError(object sender, ErrorEventArgs e)
        {
            WriteLog($"[WS错误事件][{YusDate.StringDateTime()}] 发生错误：{e.Message}{Environment.NewLine}" +
                $"错误类型：{e?.Exception?.GetType()?.Name}{Environment.NewLine}" +
                $"错误堆栈：{e?.Exception?.StackTrace}");
        }

        #endregion

        #region 业务方法

        public string GetId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public void HandleRecMessage(string msg)
        {
            var data = msg.YusToObject<WxServerRecData>();
            // 类型不匹配
            if (data.type == 0)
            {
                WriteLog($"{Environment.NewLine}{msg}{Environment.NewLine}");
            }
            // 可处理
            switch (data.type)
            {
                case RECV_TXT_MSG:
                    HandleRecTxtMsg(data);
                    break;
                case HEART_BEAT:
                    ForceResetBattleMemberAsync();
                    HandleHeartBeat(data);
                    break;
                case DEBUG_SWITCH:
                    HandleSetDebug(data);
                    break;
                case PERSONAL_DETAIL:
                    HandlePersonalDetail(data);
                    break;
                case GET_USER_LIST_SUCCSESS:
                    HandleUserList(data);
                    break;
                case GET_USER_LIST_FAIL:
                    HandleError(data);
                    break;
                case CHATROOM_MEMBER_NICK:
                    HandleChatMemberNickname(data);
                    break;
                default:
                    WriteLog($"{Environment.NewLine}{msg}{Environment.NewLine}");
                    break;
            }
        }

        public void ConnectWx()
        {
            DisconnectWx();
            WriteLog($"[连接服务][{YusDate.StringDateTime()}] 将连接到：{txtUrl.Text}");
            WxServer = new WebSocket(txtUrl.Text);
            WxServer.OnOpen += WsOnOpen;
            WxServer.OnClose += WsOnClose;
            WxServer.OnMessage += WsOnMessage;
            WxServer.OnError += WsOnError;
            try
            {
                WxServer.Connect();
                HeartTimer = new Timer() { Interval = 15000 };
                HeartTimer.Tick += HeartTimer_Tick;
                HeartTimer.Enabled = true;
                HeartTimer.Start();
            }
            catch (Exception ex)
            {
                WxServerRunning = false;
                WriteLog($"[连接服务][{YusDate.StringDateTime()}] 连接出错，{ex.Message}" +
                $"错误类型：{ex.GetType().Name}{Environment.NewLine}" +
                $"错误堆栈：{ex.StackTrace}");
            }
            WriteLog($"[连接服务][{YusDate.StringDateTime()}] 连接操作成功");
            WxServerRunning = true;
        }

        public void DisconnectWx()
        {
            if (WxServer != null)
            {
                if (HeartTimer != null)
                {
                    HeartTimer.Tick -= HeartTimer_Tick;
                    HeartTimer.Enabled = false;
                    HeartTimer.Dispose();
                    HeartTimer = null;
                }
                WriteLog($"[断开服务][{YusDate.StringDateTime()}] 尝试断开服务");
                WxServer.Close(1, "主动关闭");
                WxServer.OnOpen -= WsOnOpen;
                WxServer.OnClose -= WsOnClose;
                WxServer.OnMessage -= WsOnMessage;
                WxServer.OnError -= WsOnError;
                WriteLog($"[断开服务][{YusDate.StringDateTime()}] 服务已断开");
                WxServerRunning = false;
            }
        }

        public void HandleRecTxtMsg(WxServerRecData data)
        {
            string msg = null;
            var br = Environment.NewLine;
            if (data.receiver == "self")
            {
                var nickname = WxUsers.FirstOrDefault(f => f.wxid == data.sender)?.name;
                msg = $"[{data.time}][个人]{(nickname == null ? "" : $"[{nickname}]")}[{data.sender}]{br}{data.content}";
            }
            else
            {
                var chatName = WxUsers.FirstOrDefault(f => f.wxid == data.receiver)?.name.NullReplace(data.receiver);
                msg = $"[{data.time}][群聊][{chatName}][{data.sender}]{br}{data.content}";
                CheckMessageAsync(data);
            }
            WriteRecMsg(msg);
            var contentJson = data.content.YusToJson();
            var log = $"[处理接收文字消息][{YusDate.StringDateTime()}] 已获取信息：{contentJson}";
            WriteLogFile(log);
        }

        public void HandleHeartBeat(WxServerRecData data)
        {
            LastHeartTime = Convert.ToDateTime(data.time);
            var contentJson = data.content.YusToJson();
            var log = $"[处理服务器心跳][{YusDate.StringDateTime()}] 已获取信息：{contentJson}";
            WriteLogFile(log);
        }

        public void HandleSetDebug(WxServerRecData data)
        {
            if (data.status != "SUCCESS")
            {
                return;
            }

            string info;
            if (data.content.ToString() == "debug switch:on!\n")
            {
                info = "调试已开启";
                WxDebug = true;
            }
            else if (data.content.ToString() == "deubg switch:off!\n")
            {
                info = "调试已关闭";
                WxDebug = false;
            }
            else
            {
                info = "未设置成功：" + data.content;
            }
            var log = $"[处理设置调试状态][{YusDate.StringDateTime()}] {info}";
            WriteLogFile(log);
        }

        public void HandlePersonalDetail(WxServerRecData data)
        {
            var contentJson = data.content.YusToJson();
            WriteLog($"[处理用户详情获取][{YusDate.StringDateTime()}] 已获取信息：{contentJson}");
            var detail = contentJson.YusToObject<WxUserDetail>();
            var br = Environment.NewLine;
            var detailStr = $"备注：{detail.name1}{br}" +
                $"昵称：{detail.name2}{br}" +
                $"签名：{detail.signature}{br}" +
                $"国家：{detail.nation}{br}" +
                $"地区：{detail.provice} {detail.city}{br}" +
                $"微信号：{detail.wxcode}{br}" +
                $"头像大图：{detail.big_headimg}{br}" +
                $"头像小图：{detail.little_headimg}{br}" +
                $"朋友圈封面：{detail.cover}{br}" +
                $"v1：{detail.v1}{br}";
            WriteOutput(detailStr);
            pbUserHead.ImageLocation = detail.little_headimg;
            pbUserCover.ImageLocation = detail.cover;
        }

        public void HandleChatMemberNickname(WxServerRecData data)
        {
            var contentJson = data.content.YusToJson();
            if (data.status == "FAIL")
            {
                WriteLog($"[处理群成员昵称获取][{YusDate.StringDateTime()}] 失败：{data.content}");
                return;
            }
            WriteLog($"[处理群成员昵称获取][{YusDate.StringDateTime()}] 已获取信息：{contentJson}");
            var members = contentJson.YusToObject<WxChatMemberSimple[]>();
            WxChatMembers.Clear();
            WxChatMembers.AddRange(members);
            WxChatMemberFetced = true;

            groupMembers = members;
            var memberStr = string.Join(Environment.NewLine, members.Select(f => f.label));
            WriteOutput(memberStr);

            RunUi(() =>
            {
                pbUserHead.ImageLocation = null;
                pbUserCover.ImageLocation = null;
                cbWxChatMemberNickname.Items.Clear();
                cbWxChatMemberNickname.Items.AddRange(members);
                if (members.Length > 0) cbWxChatMemberNickname.SelectedItem = members[0];
            });
        }

        public void HandleUserList(WxServerRecData data)
        {
            var contentJson = data.content.YusToJson();
            var users = contentJson.YusToObject<WxUserSimple[]>();
            WriteLog($"[处理用户列表获取][{YusDate.StringDateTime()}] 已获取信息：{contentJson}");
            WxUsers.Clear();
            WxUsers.AddRange(users);
            WxUserFetced = true;

            RunUi(() =>
            {
                cbUserListGet.Items.Clear();
                cbUserListGet.Items.AddRange(users);
                cbUserListSend.Items.Clear();
                cbUserListSend.Items.AddRange(users);
                cbChatList.Items.Clear();
                cbChatList.Items.AddRange(users.Where(f => f.wxid.EndsWith("@chatroom")).ToArray());
                WxChatSelected = false;
                if (users.Length > 0)
                {
                    cbUserListGet.SelectedItem = users[0];
                    cbUserListSend.SelectedItem = users[0];
                }
            });
        }

        public void HandleError(WxServerRecData data)
        {
            var content = data.content.YusToJson();
            WriteLog($"[处理请求错误][{YusDate.StringDateTime()}] 操作：{data.type}，相关信息：{content}");
        }

        #endregion

        #region 窗体工具方法

        public void WriteRecMsg(string msg)
        {
            RunUi(() =>
            {
                var split = "--------------------";
                var br = Environment.NewLine;
                if (txtRecMsg.Text.Length + msg.Length + br.Length * 2 + split.Length > txtRecMsg.MaxLength) txtRecMsg.Clear();

                txtRecMsg.AppendText(msg);
                txtRecMsg.AppendText(br);
                txtRecMsg.AppendText(split);
                txtRecMsg.AppendText(br);
            });
        }

        public void WriteLog(string log)
        {
            RunUi(() =>
            {
                var br = Environment.NewLine;
                if (txtLog.Text.Length + log.Length + br.Length > txtLog.MaxLength) txtLog.Clear();

                txtLog.AppendText(log + br);
                WriteLogFile(log + br);
            });
        }

        public void WriteOutput(string output)
        {
            RunUi(() =>
            {
                txtGetOutput.Clear();
                txtGetOutput.AppendText(output);
            });
        }

        public void RunUi(Action action)
        {
            BeginInvoke(action);
        }

        public void WriteLogFile(string log)
        {
            var filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.log");
            Console.WriteLine(log);
            System.IO.File.AppendAllText(filename, log);
        }

        #endregion

        #region chatbot logic
        public void CheckMessageAsync(WxServerRecData data) {
            var chatName = WxUsers.FirstOrDefault(f => f.wxid == data.receiver)?.name.NullReplace(data.receiver);
            var sender = data.sender;
            string content = data.content.ToString();
            var time = data.time;
            GuildMember member = null;
            EntertainMember entertainMember = new EntertainMember();
            GuildWarMemberRecord latestRecord = guildwarHelper.GetLatestRecord();

            if (content.ToString().StartsWith("@bot")) {
                member = guildwarHelper.GetMember(sender);
                entertainMember = entertainHelper.GetEntertainMember(sender);
                //send message back
                switch (content.ToString()) {
                    case "@bot 身份":
                    case "@bot 身份":
                        if (sender == adminid) {
                            content = "你是超级管理员";
                        }
                        else{                            
                            if (member != null)
                            {
                                if (member.isAdmin)
                                {
                                    content = "你是管理员";
                                }
                                else
                                {
                                    content = "你是会员";
                                }

                            }
                            else {
                                content = "你的人在水群，但是却不在公会。你到底是谁？";
                            }
                        }
                        AsyncSendMssage(sender, content);

                        break;
                    //super admin commands
                    case "@bot 开启公会申请":
                    case "@bot 开启公会申请":
                        if (sender == adminid)
                        {
                            content = "公会申请已开启，可以申请入会";
                            manageMode = true;
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case "@bot 关闭公会申请":
                    case "@bot 关闭公会申请":
                        if (sender == adminid)
                        {
                            content = "公会申请已关闭，拒绝入会申请";
                            manageMode = false;
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case "@bot 测试发送":
                    case "@bot 测试发送":
                        if (sender == adminid)
                        {
                            SendTestMsg();
                        }
                        break;
                    case "@bot 查公会":
                    case "@bot 查公会":
                        if (sender == adminid)
                        {
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case var someVal when new Regex(@"^@bot 全会发钻 .*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 全会发钻 .*").IsMatch(someVal1):
                        if (sender == adminid)
                        {
                            string amount = content.ToString().Substring(10, content.ToString().Length - 10);
                            if (entertainHelper.AddGold(amount))
                            {
                                content = "充值成功！";
                            }
                            else {
                                content = "错误！";
                            }
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    //case var someVal when new Regex(@"^@bot 开启公会战 [0-9]").IsMatch(someVal):
                    //case var someVal1 when new Regex(@"^@bot 开启公会战 [0-9]").IsMatch(someVal1):
                    //    if (sender == adminid)
                    //    {
                    //        DateTime now = DateTime.UtcNow.AddHours(8);
                    //        content = "公会战已开启于"+ now.ToString()+" 持续时间: "+ content.ToString().Substring(content.ToString().Length - 1) + "天";
                    //        guildwarMode = true;
                    //        BotSendMessage(sender, content);
                    //    }
                    //    break;
                    case "@bot 结束公会战":
                    case "@bot 结束公会战":
                        if (sender == adminid)
                        {
                            content = "公会战已结束";
                            guildwarMode = false;
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case var someVal when new Regex(@"^@bot 公会战初始化 .*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 公会战初始化 .*").IsMatch(someVal1):
                        guildwarMode = true;
                        if (sender == adminid)
                        {
                            string warName = content.ToString().Substring(12, content.ToString().Length - 12);
                            if (guildwarHelper.InitializeGuildWar(warName))
                            {
                                content = warName + " 公会战初始化完成";
                            }
                            else {
                                content = "初始化失败";
                            }

                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case var someVal when new Regex(@"^@bot 读取公会战 .*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 读取公会战 .*").IsMatch(someVal1):
                        guildwarMode = true;
                        if (sender == adminid)
                        {
                            string warName = content.ToString().Substring(11, content.ToString().Length-11);
                            if (guildwarHelper.ReadGuildWar(warName))
                            {
                                content = warName + " 公会战读取成功";
                            }
                            else
                            {
                                content = "读取失败";
                            }

                            AsyncSendMssage(sender, content);
                        }
                        break;
                    //admin commands
                    //case var someVal when new Regex(@"^@bot 改boss .*").IsMatch(someVal):
                    //case var someVal1 when new Regex(@"^@bot 改boss .*").IsMatch(someVal1):
                    //    if (sender == adminid && guildwarMode == true)
                    //    {
                    //        string warName = content.ToString().Substring(12, content.ToString().Length - 12);
                    //        if (guildwarHelper.InitializeGuildWar(warName))
                    //        {
                    //            content = warName + " 公会战初始化完成";
                    //        }
                    //        else
                    //        {
                    //            content = "初始化失败";
                    //        }

                    //        BotSendMessage(sender, content);
                    //    }
                    //    break;
                    case "@bot 查命令":
                    case "@bot 查命令":
                        content = "";
                        if (member != null)
                        {
                            if (member.isAdmin)
                            {
                                content += "\n你是管理员，你可以额外使用以下命令: \n公告\n查全体刀\n强制解锁\n强制删刀\n修正血量 x\n修正周目 y\n修正boss z";
                            }
                            content += "\n会员可使用命令：\n查公告\n查boss\n查会员\n查刀\n查上一刀\n申请出刀\n申请合刀\n结束合刀\n挂树\n查树\n报刀 x(x为伤害值)\n报刀 x y(y为被代刀会员微信id)\n报刀 sl\n删刀\n预约 x y(x为boss序号，y为留言)";
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case "@bot 查全体刀":
                    case "@bot 查全体刀":
                        if (member.isAdmin == true)
                        {
                            content = guildwarHelper.CheckAttacks("正常刀");
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case "@bot 强制解锁":
                    case "@bot 强制解锁":
                        if (member.isAdmin == true)
                        {
                            guildwarHelper.ResetBattleMember();
                            content = "\n解锁成功";
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case "@bot 强制删刀":
                    case "@bot 强制删刀":
                        if (member != null && guildwarMode == true)
                        {
                            if (latestRecord != null && member.isAdmin)
                            {
                                if (guildwarHelper.RemoveLastDamage())
                                {
                                    content = "\n" + latestRecord.member.wxChatModel.nickname + "\n于" + latestRecord.time.ToShortTimeString() + "\n对" + latestRecord.target + "造成的" + latestRecord.damage + "伤害已经删除。\n请重新申请出刀并报刀！" + guildwarHelper.GetCurrentBoss();
                                    guildwarHelper.LogWarDetails();
                                }
                                else
                                {
                                    content = "删刀错误";
                                }

                            }
                            else
                            {
                                content = "没有出刀记录！";
                            }
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;

                    case var someVal when new Regex(@"^@bot 修正血量 .*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 修正血量 .*").IsMatch(someVal1):
                    case var someVal2 when new Regex(@"^@bot 修正周目 .*").IsMatch(someVal2):
                    case var someVal3 when new Regex(@"^@bot 修正周目 .*").IsMatch(someVal3):
                    case var someVal4 when new Regex(@"^@bot 修正boss .*").IsMatch(someVal4):
                    case var someVal5 when new Regex(@"^@bot 修正boss .*").IsMatch(someVal5):
                        if (member.isAdmin == true)
                        {
                            guildwarHelper.ResetBattleMember();
                            if (content.ToString().Contains("血量"))
                            {

                                guildwarHelper.ModifyBoss("血量", content.ToString().Substring(10, content.ToString().Length - 10));
                            }
                            else if (content.ToString().Contains("周目"))
                            {
                                guildwarHelper.ModifyBoss("周目", content.ToString().Substring(10, content.ToString().Length - 10));
                            }
                            else if (content.ToString().Contains("boss")) {
                                guildwarHelper.ModifyBoss("boss", content.ToString().Substring(12, content.ToString().Length - 12));
                            }
                            content = "\nBoss已修正！" + guildwarHelper.GetCurrentBoss();
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    //member commands
                    case "@bot 入会":
                    case "@bot 入会":
                        if (manageMode == true)
                        {                            
                            WxChatMemberSimple wxchatMember = GetSenderNickName(sender);
                            GuildMember newMember = new GuildMember();
                            newMember.wxChatModel = wxchatMember;
                            string status = guildwarHelper.AddMember(newMember);

                            content = status;
                            AsyncSendMssage(sender, content);
                        }                        
                    break;
                    case "@bot 查boss":
                    case "@bot 查boss":
                        content += guildwarHelper.GetCurrentBoss();
                        AsyncSendMssage(sender, content);
                    break;
                    case "@bot 查公告":
                    case "@bot 查公告":
                        content = guildwarHelper.GetNotification();
                        AsyncSendMssage(sender, content);
                        break;
                    case var someVal when new Regex(@"^@bot 公告 .*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 公告 .*").IsMatch(someVal1):
                        if (member.isAdmin == true)
                        {
                            string info = content.ToString().Substring(8, content.ToString().Length - 8);
                            if (guildwarHelper.SetNotification(info))
                            {
                                content = "设置成功！";
                            }
                            else
                            {
                                content = "？出错力！";
                            }
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case "@bot 查会员":
                    case "@bot 查会员":
                        List<GuildMember> tempMembers = new List<GuildMember>();
                        tempMembers = guildwarHelper.GetAllMembers();
                        content = "";
                        foreach (GuildMember tempMember in tempMembers) {
                            content += "\n" + tempMember.wxChatModel.nickname + "|" + tempMember.wxChatModel.wxid;
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case var someVal when new Regex(@"^@bot 查刀.*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 查刀.*").IsMatch(someVal1):
                        if (member != null && guildwarMode == true)
                        {
                            string targetWxid = content.ToString().Substring(7, content.ToString().Length - 7);
                            if (targetWxid == "")
                            {
                                content = guildwarHelper.CheckMemberAttack(member);
                            }
                            else {
                                content = guildwarHelper.CheckMemberAttack(guildwarHelper.GetMember(targetWxid));
                            }
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case var someVal when new Regex(@"^@bot 预约 .*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 预约 .*").IsMatch(someVal1):
                        if (member != null && guildwarMode == true)
                        {
                            int bossNum = 0;
                            string inputStr = content.ToString().Substring(8, content.ToString().Length - 8);
                            string targetBoss = "";
                            string note = "";
                            string[] inputArr = inputStr.Split(' ');
                            if (inputArr.Count() > 1)
                            {
                                targetBoss = inputArr[0];
                                note = inputArr[1];
                            }
                            else {
                                targetBoss = inputArr[0];
                            }

                            if (int.TryParse(targetBoss, out bossNum))
                            {
                                bossNum = Int32.Parse(targetBoss);
                                if (guildwarHelper.ReserveBoss(member, bossNum, note)) {
                                    content = "预约成功！";
                                }
                            }
                            else
                            {
                                content = "命令错误， 请重新预约，命令为：预约 x 留言";
                            }
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 取消预约":
                    case "@bot 取消预约":
                        if (member != null && guildwarMode == true)
                        {
                            if (guildwarHelper.RemoveMemberReserve(member))
                            {
                                content = "\n你的所有预约已解除！";
                            }
                            else
                            {
                                content = "错误！";
                            }
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 查上一刀":
                    case "@bot 查上一刀":
                        if (member != null && guildwarMode == true)
                        {
                            if (latestRecord != null)
                            {
                                content = "\n上一刀为" + latestRecord.member.wxChatModel.nickname + "\n于" + latestRecord.time.ToShortTimeString() + "\n对" + latestRecord.target + "造成" + latestRecord.damage + "伤害";
                            }
                            else
                            {
                                content = "还没有人出刀";
                            }
                        }
                        else {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 申请出刀":
                    case "@bot 申请出刀":
                        if (member != null && guildwarMode == true)
                        {
                            if (!guildwarHelper.RequestBattle(member))
                            {
                                if (guildwarHelper.GetChainBattleStatus())
                                {
                                    content = "\n拒绝申请, 会员合刀中！";
                                }
                                else {
                                    content = "\n拒绝出刀, " + guildwarHelper.GetWarModel().currentMember.wxChatModel.nickname + "正在出刀中, 将自动结束于:\n" + guildwarHelper.GetWarModel().currentBattleEndTime.ToShortTimeString();
                                }
                            }
                            else {
                                content = guildwarHelper.GetNotification() + "\n允许出刀, " + guildwarHelper.GetWarModel().currentMember.wxChatModel.nickname + "于:\n" + guildwarHelper.GetWarModel().currentBattleStartTime.ToShortTimeString() + "\n开始出刀, 将自动结束于:\n" + guildwarHelper.GetWarModel().currentBattleEndTime.ToShortTimeString() + "\n 请出刀后报刀, 祝刀刀暴击！";
                                currentBattleMember = guildwarHelper.GetWarModel().currentMember;
                                currentBattleEndTime = guildwarHelper.GetWarModel().currentBattleEndTime;
                            }
                        }
                        else {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 申请合刀":
                    case "@bot 申请合刀":
                        if (member != null && guildwarMode == true)
                        {
                            if (!guildwarHelper.RequestChainBattle(member))
                            {
                                content = "\n拒绝申请, " + guildwarHelper.GetWarModel().currentMember.wxChatModel.nickname + "正在出刀中, 将自动结束于:\n" + guildwarHelper.GetWarModel().currentBattleEndTime.ToShortTimeString();
                            }                            
                            else
                            {
                                if (guildwarHelper.GetAllChainBattleMembers().Count > 1)
                                {
                                    content = "\n合刀申请成功。";
                                }
                                else {
                                    content = "\n合刀模式开启, " + guildwarHelper.GetWarModel().currentMember.wxChatModel.nickname + "于:\n" + guildwarHelper.GetWarModel().currentBattleStartTime.ToShortTimeString() + "\n开启合刀模式, 将自动结束于:\n" + guildwarHelper.GetWarModel().currentBattleEndTime.ToShortTimeString() + "\n请合刀成员申请合刀。";
                                    currentBattleMember = guildwarHelper.GetWarModel().currentMember;
                                    currentBattleEndTime = guildwarHelper.GetWarModel().currentBattleEndTime;
                                }
                            }
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 结束合刀":
                    case "@bot 结束合刀":
                        if (member != null && guildwarMode == true)
                        {
                            if (!guildwarHelper.GetChainBattleStatus())
                            {
                                content = "\n合刀未开启。";
                            }
                            else
                            {
                                if (guildwarHelper.GetWarModel().currentMember.wxChatModel.wxid == member.wxChatModel.wxid)
                                {
                                    content = "\n合刀已结束。";
                                    guildwarHelper.SetChainBattleStatus(false);
                                    guildwarHelper.ResetBattleMember();
                                }
                                else
                                {
                                    content = "\n你不是合刀发起人，无法结束合刀。";
                                }
                            }
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 挂树":
                    case "@bot 挂树":
                        if (member != null && guildwarMode == true)
                        {
                            if (guildwarHelper.GetBattleMember() != null && guildwarHelper.GetBattleMember().wxChatModel.wxid == member.wxChatModel.wxid && !guildwarHelper.GetChainBattleStatus())
                            {
                                //normal on tree
                                content = "\n" + guildwarHelper.GetWarModel().currentMember.wxChatModel.nickname + "挂树了！\n你的出刀锁定已解除，" + guildwarHelper.GetWarModel().currentBossTemplate.name + "被击杀后将自动下树！";
                                guildwarHelper.AddTreeMember(member);                                
                            }
                            else if (guildwarHelper.GetBattleMember() != null && guildwarHelper.GetChainBattleStatus() && guildwarHelper.GetChainBattleMember(member.wxChatModel.wxid) != null)
                            {
                                //chain battle on tree
                                content = "\n" + guildwarHelper.GetWarModel().currentMember.wxChatModel.nickname + "合刀时挂树了！\n" + guildwarHelper.GetWarModel().currentBossTemplate.name + "被击杀后将自动下树！";
                                guildwarHelper.AddTreeMember(member);                                
                            }
                            else {
                                content = "\n你未申请出刀，无法挂树！";
                            }
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 查树":
                    case "@bot 查树":
                        if (member != null && guildwarMode == true)
                        {
                            content = guildwarHelper.GetTreeMembers();
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case var someVal when new Regex(@"^@bot 报刀 .*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 报刀 .*").IsMatch(someVal1):
                        if (member != null && guildwarMode == true)
                        {
                            string inputStr = content.ToString().Substring(8, content.ToString().Length - 8);
                            string[] inputArr = inputStr.Split(' ');
                            string damageStr = "";
                            string replaceTargetWxid = "";
                            if (inputArr.Count() > 1)
                            {
                                damageStr = inputArr[0];
                                replaceTargetWxid = inputArr[1];
                            }
                            else
                            {
                                damageStr = inputArr[0];
                            }
                            int damage = 0;

                            if (guildwarHelper.GetBattleMember() != null && (guildwarHelper.GetBattleMember().wxChatModel.wxid == member.wxChatModel.wxid || guildwarHelper.GetMember(replaceTargetWxid) != null) && !guildwarHelper.GetChainBattleStatus())
                            {
                                //normal report
                                if (int.TryParse(damageStr, out damage))
                                {
                                    damage = Int32.Parse(damageStr);
                                    if (damage >= 0 && damage <= 9999999 && ReportDamage(member, damage, replaceTargetWxid))
                                    {
                                        content = "\n出刀成功, 已记录 " + damageStr + " 伤害\n出刀锁定已解除！" + guildwarHelper.GetCurrentBoss();
                                        guildwarHelper.LogWarDetails();
                                    }
                                    else
                                    {
                                        content = "\n报刀数据错误，请重新报刀！";
                                    }
                                }
                                else if (damageStr == "SL" || damageStr == "sl") {
                                    damage = -1;
                                    if (ReportDamage(member, damage, replaceTargetWxid))
                                    {
                                        content = "\nSL已记录！";
                                    }
                                    else {
                                        content = "\n报刀数据错误，请重新报刀！\n报刀格式为\n@bot 报刀 12345";
                                    }
                                }
                                else
                                {
                                    content = "\n报刀数据错误，请重新报刀！\n报刀格式为\n@bot 报刀 12345";
                                }
                            }
                            else if (guildwarHelper.GetBattleMember() != null && guildwarHelper.GetChainBattleStatus() && (guildwarHelper.GetChainBattleMember(member.wxChatModel.wxid) != null || guildwarHelper.GetMember(replaceTargetWxid) != null)) {
                                //chain report
                                if (int.TryParse(damageStr, out damage))
                                {
                                    damage = Int32.Parse(damageStr);
                                    if (damage >= 0 && damage <= 9999999 && ReportDamage(member, damage, replaceTargetWxid))
                                    {
                                        content = "\n出刀成功, 已记录 " + damageStr + " 伤害\n请合刀会员继续报刀！" + guildwarHelper.GetCurrentBoss();
                                        notifyReservedMember = guildwarHelper.LogWarDetails();
                                    }
                                    else
                                    {
                                        content = "\n报刀数据错误，请重新报刀！";
                                    }
                                }
                                else
                                {
                                    content = "\n报刀数据错误，请重新报刀！\n报刀格式为\n@bot 报刀 12345";
                                }
                            }
                            else
                            {
                                if (guildwarHelper.GetBattleMember().wxChatModel.wxid != member.wxChatModel.wxid && replaceTargetWxid == "")
                                {
                                    content = "\n你未申请出刀，无法报刀！"; 
                                }
                                else if(replaceTargetWxid != "" && guildwarHelper.GetMember(replaceTargetWxid) == null)
                                {
                                    content = "\n带刀对象不存在，请查询微信id后重新报刀！";
                                }
                                
                            }
                        }
                        else
                        {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        if (guildwarHelper.GetNotifyStatus())
                        {
                            SendReservedNotifiesAsync();
                        }
                        break;

                    case "@bot 删刀":
                    case "@bot 删刀":
                        if (member != null && guildwarMode == true)
                        {
                            if (latestRecord != null && sender == latestRecord.member.wxChatModel.wxid)
                            {
                                if (guildwarHelper.RemoveLastDamage())
                                {
                                    content = "\n" + latestRecord.member.wxChatModel.nickname + "\n于" + latestRecord.time.ToShortTimeString() + "\n对" + latestRecord.target + "造成的" + latestRecord.damage + "伤害已经删除。\n请重新申请出刀并报刀！" + guildwarHelper.GetCurrentBoss();
                                    guildwarHelper.LogWarDetails();
                                }
                                else
                                {
                                    content = "删刀错误";
                                }

                            }
                            else
                            {
                                content = "你不是最后一个出刀玩家";
                            }
                        }
                        else {
                            content = RefuseCommand(member, guildwarMode);
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    //entertain functions
                    case var someVal when new Regex(@"^@bot 读取卡池 .*").IsMatch(someVal):
                    case var someVal1 when new Regex(@"^@bot 读取卡池 .*").IsMatch(someVal1):
                        if (sender == adminid)
                        {
                            string gachaName = content.ToString().Substring(10, content.ToString().Length - 10);
                            content = entertainHelper.LoadPoll(gachaName);
                            AsyncSendMssage(sender, content);
                        }
                        break;
                    case "@bot 查娱乐命令":
                    case "@bot 查娱乐命令":
                        content = "\n创建娱乐档案\n签到\n今日运势\n查钻\n查库存\n查池子\n单抽\n十连";
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 创建娱乐档案":
                    case "@bot 创建娱乐档案":
                        WxChatMemberSimple tmpMember = GetSenderNickName(sender);
                        content = entertainHelper.CreateMember(tmpMember);
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 签到":
                    case "@bot 签到":
                        if (entertainMember != null)
                        {
                            content = entertainHelper.CheckIn(entertainMember);
                        }
                        else
                        {
                            content = "\n未找到你的档案，请先 创建娱乐档案！";
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 今日运势":
                    case "@bot 今日运势":
                        if (entertainMember != null)
                        {
                            content = entertainHelper.CheckLuckyPoint(entertainMember);
                        }
                        else
                        {
                            content = "\n未找到你的档案，请先 创建娱乐档案！";
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 十连":
                    case "@bot 十连":
                        if (entertainMember != null)
                        {
                            content = entertainHelper.TenGacha(entertainMember);
                        }
                        else
                        {
                            content = "\n未找到你的档案，请先 创建娱乐档案！";
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 单抽":
                    case "@bot 单抽":
                        if (entertainMember != null)
                        {
                            content = entertainHelper.SingleGacha(entertainMember);
                        }
                        else {
                            content = "\n未找到你的档案，请先 创建娱乐档案！";
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 查库存":
                    case "@bot 查库存":
                        if (entertainMember != null)
                        {
                            content = entertainHelper.CheckInventory(entertainMember);
                        }
                        else
                        {
                            content = "\n未找到你的档案，请先 创建娱乐档案！";
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 查钻":
                    case "@bot 查钻":
                        if (entertainMember != null)
                        {
                            content = entertainHelper.CheckGold(entertainMember);
                        }
                        else
                        {
                            content = "\n未找到你的档案，请先 创建娱乐档案！";
                        }
                        AsyncSendMssage(sender, content);
                        break;
                    case "@bot 查卡池":
                    case "@bot 查卡池":
                        if (entertainMember != null)
                        {
                            content = entertainHelper.GetGachaPoll();
                        }
                        else
                        {
                            content = "\n未找到你的档案，请先 创建娱乐档案！";
                        }
                        AsyncSendMssage(sender, content);
                        break;
                }                
            }
        }
        public void AsyncSendMssage(string sender, object content) {
            BotSendMessage(sender, content);
            System.Threading.Thread.Sleep(300);
            stopWaitHandle.WaitOne();
            stopWaitHandle.Reset();
        }
        public void BotSendMessage(string senderId, object message)
        {

            //get sender nick
            var sendContent = "";

            if (senderId != "")
            {
                WxChatMemberSimple sender = GetSenderNickName(senderId);

                var backmessage = message.ToString();
                sendContent = new
                {
                    id = GetId(),
                    type = AT_MSG,
                    content = backmessage,
                    wxid = sender.wxid,
                    roomid = sender.roomid,
                    nickname = sender.nickname

                }.YusToJson();
            }
            else
            {
                var backmessage = message.ToString();

                sendContent = new
                {
                    id = GetId(),
                    type = TXT_MSG,
                    content = backmessage,
                    wxid = roomid
                }.YusToJson();
            }


            WxServer.SendAsync(sendContent, (result) =>
            {
                Stop_Callback(result);
            });
        }
        private void Stop_Callback(bool result)
        {
            // signal the wait handle
            stopWaitHandle.Set();
        }


        public WxChatMemberSimple GetSenderNickName(string senderId)
        {

            WxChatMemberSimple sender = groupMembers.Where(f => f.wxid == senderId).FirstOrDefault();

            return sender;
        }
        #endregion

        #region guildwar logic
        private void ForceResetBattleMemberAsync() {
            try
            {
                if (guildwarHelper.GetWarModel() != null && guildwarHelper.GetWarModel().currentMember != null && DateTime.UtcNow.AddHours(8) > guildwarHelper.GetWarModel().currentBattleEndTime)
                {
                    string message = guildwarHelper.GetWarModel().currentMember.wxChatModel.nickname + "出刀已超时，强制解除锁定！";
                    guildwarHelper.ResetBattleMember();
                    AsyncSendMssage("", message);
                }
            }
            catch {

            }
        }
        private bool ReportDamage(GuildMember member, int damage, string replaceTarget) {
            try
            {   
                if (guildwarHelper.GetWarModel() != null && guildwarHelper.GetWarModel().currentMember != null)
                {
                    if (replaceTarget != "" && guildwarHelper.GetMember(replaceTarget) == null) {
                        return false;
                    }
                    //war mode is on and there is a current member attacking boss
                    if (guildwarHelper.ReportDamage(member, damage, replaceTarget))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch {
                return false;
            }
            return false;
        }
        private string RefuseCommand(GuildMember member, bool warmode) {
            string output = "";
            if (member == null)
            {
                output = "\n非会员无法使用该命令！ 请先入会！";
            }
            else
            {
                output = "\n公会战未开启！";
            }
            return output;
        }

        private void SendReservedNotifiesAsync() {
            try {
                guildwarHelper.SetNotifyStatus(false);
                List<ReserveMemberModel> members = guildwarHelper.GetReservedMembers();
                foreach (ReserveMemberModel member in members) {
                    string message = "\n你预约的Boss已刷新！\n" + member.note;
                    AsyncSendMssage(member.member.wxChatModel.wxid, message);
                }
            } catch (Exception e) {

            }
        }


        private void SendTestMsg()
        {
            try
            {
                for(int i = 0; i < 10; i++)
                {
                    string message = "\n第" + i + "次发送！";
                    AsyncSendMssage(adminid, message);
                }
            }
            catch (Exception e)
            {

            }
        }
        #endregion
    }
}

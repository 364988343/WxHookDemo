using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using Yus.Apps;

namespace WxHookDemo
{
    public partial class FrmMain : Form
    {
        WxChatMemberSimple[] groupMembers;
        const string adminid = "wxid_ogrbmj6u3zwb12";













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
                CheckMessage(data);
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
        public void CheckMessage(WxServerRecData data) {
            var chatName = WxUsers.FirstOrDefault(f => f.wxid == data.receiver)?.name.NullReplace(data.receiver);
            var sender = data.sender;
            var content = data.content;
            var time = data.time;
            if (content.ToString().StartsWith("@bot")) {
                //send message back
                switch (content.ToString()) {
                    case "@bot 身份":
                        if (sender == adminid) {
                            content = "@bot 你是超级管理员";
                        }
                        else{
                            content = "";
                        }
                    break;
                }
                BotSendMessage(sender, content);
            }
        }

        public void BotSendMessage(string senderId, object message) {

            //get sender nick
            WxChatMemberSimple sender = GetSenderNickName(senderId);

            string msg = message.ToString();
            msg = msg.Remove(0, 5);

            var backmessage = "收到 " + sender.nickname + " " + msg;

            var sendContent = new
            {
                id = GetId(),
                type = AT_MSG,
                content = backmessage,
                wxid = sender.wxid,
                roomid = sender.roomid,
                nickname = sender.nickname

            }.YusToJson();

            WxServer.SendAsync(sendContent, (result) =>
            {
                //send message
            });
        }

        public WxChatMemberSimple GetSenderNickName(string senderId)
        {

            WxChatMemberSimple sender = groupMembers.Where(f => f.wxid == senderId).FirstOrDefault();

            return sender;
        }
        #endregion

        #region guildwar logic
            
        #endregion
    }
}

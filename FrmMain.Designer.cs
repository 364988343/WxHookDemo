namespace WxHookDemo
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.gbSend = new System.Windows.Forms.GroupBox();
            this.ckbAtMember = new System.Windows.Forms.CheckBox();
            this.btnSendChatTextMsg = new System.Windows.Forms.Button();
            this.txtChatMsg = new System.Windows.Forms.TextBox();
            this.cbChatList = new System.Windows.Forms.ComboBox();
            this.cbWxChatMemberNickname = new System.Windows.Forms.ComboBox();
            this.cbUserListSend = new System.Windows.Forms.ComboBox();
            this.txtSendMsg = new System.Windows.Forms.TextBox();
            this.btnSendTextMsg = new System.Windows.Forms.Button();
            this.gbGet = new System.Windows.Forms.GroupBox();
            this.btnGetChatMemberNickname = new System.Windows.Forms.Button();
            this.pbUserCover = new System.Windows.Forms.PictureBox();
            this.pbUserHead = new System.Windows.Forms.PictureBox();
            this.txtGetOutput = new System.Windows.Forms.TextBox();
            this.cbUserListGet = new System.Windows.Forms.ComboBox();
            this.btnGetUserList = new System.Windows.Forms.Button();
            this.btnGetPersonalInfo = new System.Windows.Forms.Button();
            this.btnGetPersonalDetail = new System.Windows.Forms.Button();
            this.gbClient = new System.Windows.Forms.GroupBox();
            this.lblConnectStatus = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtRecMsg = new System.Windows.Forms.TextBox();
            this.btnClearRecMsg = new System.Windows.Forms.Button();
            this.ckbDebug = new System.Windows.Forms.CheckBox();
            this.txtSendPicUrl = new System.Windows.Forms.TextBox();
            this.btnSendPicSelect = new System.Windows.Forms.Button();
            this.btnSendPicMsg = new System.Windows.Forms.Button();
            this.btnSendChatPicSelect = new System.Windows.Forms.Button();
            this.txtSendChatPicUrl = new System.Windows.Forms.TextBox();
            this.btnSendChatPicMsg = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.gbSend.SuspendLayout();
            this.gbGet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbUserCover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbUserHead)).BeginInit();
            this.gbClient.SuspendLayout();
            this.SuspendLayout();
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 0);
            this.scMain.Name = "scMain";
            this.scMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.btnClearRecMsg);
            this.scMain.Panel1.Controls.Add(this.txtRecMsg);
            this.scMain.Panel1.Controls.Add(this.gbSend);
            this.scMain.Panel1.Controls.Add(this.gbGet);
            this.scMain.Panel1.Controls.Add(this.gbClient);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.btnClearLog);
            this.scMain.Panel2.Controls.Add(this.txtLog);
            this.scMain.Size = new System.Drawing.Size(1184, 761);
            this.scMain.SplitterDistance = 500;
            this.scMain.TabIndex = 0;
            // 
            // gbSend
            // 
            this.gbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbSend.Controls.Add(this.btnSendChatPicMsg);
            this.gbSend.Controls.Add(this.btnSendChatPicSelect);
            this.gbSend.Controls.Add(this.txtSendChatPicUrl);
            this.gbSend.Controls.Add(this.btnSendPicMsg);
            this.gbSend.Controls.Add(this.btnSendPicSelect);
            this.gbSend.Controls.Add(this.txtSendPicUrl);
            this.gbSend.Controls.Add(this.ckbAtMember);
            this.gbSend.Controls.Add(this.btnSendChatTextMsg);
            this.gbSend.Controls.Add(this.txtChatMsg);
            this.gbSend.Controls.Add(this.cbChatList);
            this.gbSend.Controls.Add(this.cbWxChatMemberNickname);
            this.gbSend.Controls.Add(this.cbUserListSend);
            this.gbSend.Controls.Add(this.txtSendMsg);
            this.gbSend.Controls.Add(this.btnSendTextMsg);
            this.gbSend.Location = new System.Drawing.Point(381, 12);
            this.gbSend.Name = "gbSend";
            this.gbSend.Size = new System.Drawing.Size(363, 485);
            this.gbSend.TabIndex = 11;
            this.gbSend.TabStop = false;
            this.gbSend.Text = "发送";
            // 
            // ckbAtMember
            // 
            this.ckbAtMember.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbAtMember.AutoSize = true;
            this.ckbAtMember.Location = new System.Drawing.Point(315, 257);
            this.ckbAtMember.Name = "ckbAtMember";
            this.ckbAtMember.Size = new System.Drawing.Size(42, 21);
            this.ckbAtMember.TabIndex = 16;
            this.ckbAtMember.Text = "AT";
            this.ckbAtMember.UseVisualStyleBackColor = true;
            // 
            // btnSendChatTextMsg
            // 
            this.btnSendChatTextMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendChatTextMsg.Enabled = false;
            this.btnSendChatTextMsg.Location = new System.Drawing.Point(7, 451);
            this.btnSendChatTextMsg.Name = "btnSendChatTextMsg";
            this.btnSendChatTextMsg.Size = new System.Drawing.Size(175, 28);
            this.btnSendChatTextMsg.TabIndex = 15;
            this.btnSendChatTextMsg.Text = "发送群文字消息";
            this.btnSendChatTextMsg.UseVisualStyleBackColor = true;
            // 
            // txtChatMsg
            // 
            this.txtChatMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChatMsg.Location = new System.Drawing.Point(7, 286);
            this.txtChatMsg.Multiline = true;
            this.txtChatMsg.Name = "txtChatMsg";
            this.txtChatMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChatMsg.Size = new System.Drawing.Size(350, 133);
            this.txtChatMsg.TabIndex = 14;
            this.txtChatMsg.Text = "测试群消息文本";
            // 
            // cbChatList
            // 
            this.cbChatList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbChatList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChatList.Enabled = false;
            this.cbChatList.FormattingEnabled = true;
            this.cbChatList.Location = new System.Drawing.Point(7, 255);
            this.cbChatList.Name = "cbChatList";
            this.cbChatList.Size = new System.Drawing.Size(218, 25);
            this.cbChatList.TabIndex = 13;
            // 
            // cbWxChatMemberNickname
            // 
            this.cbWxChatMemberNickname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbWxChatMemberNickname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWxChatMemberNickname.Enabled = false;
            this.cbWxChatMemberNickname.FormattingEnabled = true;
            this.cbWxChatMemberNickname.Location = new System.Drawing.Point(230, 255);
            this.cbWxChatMemberNickname.Name = "cbWxChatMemberNickname";
            this.cbWxChatMemberNickname.Size = new System.Drawing.Size(79, 25);
            this.cbWxChatMemberNickname.TabIndex = 12;
            // 
            // cbUserListSend
            // 
            this.cbUserListSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUserListSend.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUserListSend.Enabled = false;
            this.cbUserListSend.FormattingEnabled = true;
            this.cbUserListSend.Location = new System.Drawing.Point(7, 22);
            this.cbUserListSend.Name = "cbUserListSend";
            this.cbUserListSend.Size = new System.Drawing.Size(350, 25);
            this.cbUserListSend.TabIndex = 9;
            // 
            // txtSendMsg
            // 
            this.txtSendMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSendMsg.Location = new System.Drawing.Point(7, 53);
            this.txtSendMsg.Multiline = true;
            this.txtSendMsg.Name = "txtSendMsg";
            this.txtSendMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSendMsg.Size = new System.Drawing.Size(350, 112);
            this.txtSendMsg.TabIndex = 7;
            this.txtSendMsg.Text = "测试文本";
            // 
            // btnSendTextMsg
            // 
            this.btnSendTextMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendTextMsg.Enabled = false;
            this.btnSendTextMsg.Location = new System.Drawing.Point(7, 200);
            this.btnSendTextMsg.Name = "btnSendTextMsg";
            this.btnSendTextMsg.Size = new System.Drawing.Size(175, 28);
            this.btnSendTextMsg.TabIndex = 8;
            this.btnSendTextMsg.Text = "发送文本消息";
            this.btnSendTextMsg.UseVisualStyleBackColor = true;
            // 
            // gbGet
            // 
            this.gbGet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbGet.Controls.Add(this.btnGetChatMemberNickname);
            this.gbGet.Controls.Add(this.pbUserCover);
            this.gbGet.Controls.Add(this.pbUserHead);
            this.gbGet.Controls.Add(this.txtGetOutput);
            this.gbGet.Controls.Add(this.cbUserListGet);
            this.gbGet.Controls.Add(this.btnGetUserList);
            this.gbGet.Controls.Add(this.btnGetPersonalInfo);
            this.gbGet.Controls.Add(this.btnGetPersonalDetail);
            this.gbGet.Location = new System.Drawing.Point(12, 101);
            this.gbGet.Name = "gbGet";
            this.gbGet.Size = new System.Drawing.Size(363, 396);
            this.gbGet.TabIndex = 10;
            this.gbGet.TabStop = false;
            this.gbGet.Text = "获取";
            // 
            // btnGetChatMemberNickname
            // 
            this.btnGetChatMemberNickname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetChatMemberNickname.Enabled = false;
            this.btnGetChatMemberNickname.Location = new System.Drawing.Point(146, 82);
            this.btnGetChatMemberNickname.Name = "btnGetChatMemberNickname";
            this.btnGetChatMemberNickname.Size = new System.Drawing.Size(210, 25);
            this.btnGetChatMemberNickname.TabIndex = 11;
            this.btnGetChatMemberNickname.Text = "获取群成员名";
            this.btnGetChatMemberNickname.UseVisualStyleBackColor = true;
            // 
            // pbUserCover
            // 
            this.pbUserCover.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbUserCover.Location = new System.Drawing.Point(76, 82);
            this.pbUserCover.Name = "pbUserCover";
            this.pbUserCover.Size = new System.Drawing.Size(64, 64);
            this.pbUserCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbUserCover.TabIndex = 10;
            this.pbUserCover.TabStop = false;
            // 
            // pbUserHead
            // 
            this.pbUserHead.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbUserHead.Location = new System.Drawing.Point(6, 82);
            this.pbUserHead.Name = "pbUserHead";
            this.pbUserHead.Size = new System.Drawing.Size(64, 64);
            this.pbUserHead.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbUserHead.TabIndex = 9;
            this.pbUserHead.TabStop = false;
            // 
            // txtGetOutput
            // 
            this.txtGetOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGetOutput.Location = new System.Drawing.Point(6, 152);
            this.txtGetOutput.Multiline = true;
            this.txtGetOutput.Name = "txtGetOutput";
            this.txtGetOutput.ReadOnly = true;
            this.txtGetOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGetOutput.Size = new System.Drawing.Size(350, 238);
            this.txtGetOutput.TabIndex = 8;
            // 
            // cbUserListGet
            // 
            this.cbUserListGet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUserListGet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUserListGet.Enabled = false;
            this.cbUserListGet.FormattingEnabled = true;
            this.cbUserListGet.Location = new System.Drawing.Point(6, 51);
            this.cbUserListGet.Name = "cbUserListGet";
            this.cbUserListGet.Size = new System.Drawing.Size(245, 25);
            this.cbUserListGet.TabIndex = 6;
            // 
            // btnGetUserList
            // 
            this.btnGetUserList.Enabled = false;
            this.btnGetUserList.Location = new System.Drawing.Point(6, 22);
            this.btnGetUserList.Name = "btnGetUserList";
            this.btnGetUserList.Size = new System.Drawing.Size(150, 23);
            this.btnGetUserList.TabIndex = 1;
            this.btnGetUserList.Text = "获取用户列表";
            this.btnGetUserList.UseVisualStyleBackColor = true;
            // 
            // btnGetPersonalInfo
            // 
            this.btnGetPersonalInfo.Enabled = false;
            this.btnGetPersonalInfo.Location = new System.Drawing.Point(162, 22);
            this.btnGetPersonalInfo.Name = "btnGetPersonalInfo";
            this.btnGetPersonalInfo.Size = new System.Drawing.Size(151, 23);
            this.btnGetPersonalInfo.TabIndex = 5;
            this.btnGetPersonalInfo.Text = "获取自身信息";
            this.btnGetPersonalInfo.UseVisualStyleBackColor = true;
            // 
            // btnGetPersonalDetail
            // 
            this.btnGetPersonalDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetPersonalDetail.Enabled = false;
            this.btnGetPersonalDetail.Location = new System.Drawing.Point(257, 51);
            this.btnGetPersonalDetail.Name = "btnGetPersonalDetail";
            this.btnGetPersonalDetail.Size = new System.Drawing.Size(100, 25);
            this.btnGetPersonalDetail.TabIndex = 4;
            this.btnGetPersonalDetail.Text = "获取用户详情";
            this.btnGetPersonalDetail.UseVisualStyleBackColor = true;
            // 
            // gbClient
            // 
            this.gbClient.Controls.Add(this.ckbDebug);
            this.gbClient.Controls.Add(this.lblConnectStatus);
            this.gbClient.Controls.Add(this.btnDisconnect);
            this.gbClient.Controls.Add(this.txtUrl);
            this.gbClient.Controls.Add(this.btnConnect);
            this.gbClient.Location = new System.Drawing.Point(12, 12);
            this.gbClient.Name = "gbClient";
            this.gbClient.Size = new System.Drawing.Size(363, 83);
            this.gbClient.TabIndex = 9;
            this.gbClient.TabStop = false;
            this.gbClient.Text = "连接";
            // 
            // lblConnectStatus
            // 
            this.lblConnectStatus.AutoSize = true;
            this.lblConnectStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblConnectStatus.ForeColor = System.Drawing.Color.Red;
            this.lblConnectStatus.Location = new System.Drawing.Point(210, 25);
            this.lblConnectStatus.Name = "lblConnectStatus";
            this.lblConnectStatus.Size = new System.Drawing.Size(44, 17);
            this.lblConnectStatus.TabIndex = 4;
            this.lblConnectStatus.Text = "未连接";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(137, 51);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(118, 23);
            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "断开";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(13, 22);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(194, 23);
            this.txtUrl.TabIndex = 2;
            this.txtUrl.Text = "ws://127.0.0.1:5555";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(13, 51);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(118, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLog.Location = new System.Drawing.Point(1110, 3);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(51, 27);
            this.btnClearLog.TabIndex = 2;
            this.btnClearLog.Text = "清空";
            this.btnClearLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.AcceptsTab = true;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1184, 257);
            this.txtLog.TabIndex = 0;
            // 
            // txtRecMsg
            // 
            this.txtRecMsg.AcceptsReturn = true;
            this.txtRecMsg.AcceptsTab = true;
            this.txtRecMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRecMsg.Location = new System.Drawing.Point(750, 20);
            this.txtRecMsg.Multiline = true;
            this.txtRecMsg.Name = "txtRecMsg";
            this.txtRecMsg.ReadOnly = true;
            this.txtRecMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRecMsg.Size = new System.Drawing.Size(422, 477);
            this.txtRecMsg.TabIndex = 12;
            // 
            // btnClearRecMsg
            // 
            this.btnClearRecMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearRecMsg.Location = new System.Drawing.Point(1098, 27);
            this.btnClearRecMsg.Name = "btnClearRecMsg";
            this.btnClearRecMsg.Size = new System.Drawing.Size(51, 27);
            this.btnClearRecMsg.TabIndex = 13;
            this.btnClearRecMsg.Text = "清空";
            this.btnClearRecMsg.UseVisualStyleBackColor = true;
            // 
            // ckbDebug
            // 
            this.ckbDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbDebug.AutoSize = true;
            this.ckbDebug.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckbDebug.ForeColor = System.Drawing.Color.DimGray;
            this.ckbDebug.Location = new System.Drawing.Point(261, 53);
            this.ckbDebug.Name = "ckbDebug";
            this.ckbDebug.Size = new System.Drawing.Size(87, 21);
            this.ckbDebug.TabIndex = 17;
            this.ckbDebug.Text = "调试已关闭";
            this.ckbDebug.UseVisualStyleBackColor = true;
            // 
            // txtSendPicUrl
            // 
            this.txtSendPicUrl.Location = new System.Drawing.Point(7, 171);
            this.txtSendPicUrl.Name = "txtSendPicUrl";
            this.txtSendPicUrl.ReadOnly = true;
            this.txtSendPicUrl.Size = new System.Drawing.Size(259, 23);
            this.txtSendPicUrl.TabIndex = 17;
            // 
            // btnSendPicSelect
            // 
            this.btnSendPicSelect.Location = new System.Drawing.Point(272, 171);
            this.btnSendPicSelect.Name = "btnSendPicSelect";
            this.btnSendPicSelect.Size = new System.Drawing.Size(85, 23);
            this.btnSendPicSelect.TabIndex = 18;
            this.btnSendPicSelect.Text = "选择图片";
            this.btnSendPicSelect.UseVisualStyleBackColor = true;
            // 
            // btnSendPicMsg
            // 
            this.btnSendPicMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendPicMsg.Enabled = false;
            this.btnSendPicMsg.Location = new System.Drawing.Point(182, 200);
            this.btnSendPicMsg.Name = "btnSendPicMsg";
            this.btnSendPicMsg.Size = new System.Drawing.Size(175, 28);
            this.btnSendPicMsg.TabIndex = 19;
            this.btnSendPicMsg.Text = "发送图片消息";
            this.btnSendPicMsg.UseVisualStyleBackColor = true;
            // 
            // btnSendChatPicSelect
            // 
            this.btnSendChatPicSelect.Location = new System.Drawing.Point(272, 425);
            this.btnSendChatPicSelect.Name = "btnSendChatPicSelect";
            this.btnSendChatPicSelect.Size = new System.Drawing.Size(85, 23);
            this.btnSendChatPicSelect.TabIndex = 21;
            this.btnSendChatPicSelect.Text = "选择图片";
            this.btnSendChatPicSelect.UseVisualStyleBackColor = true;
            // 
            // txtSendChatPicUrl
            // 
            this.txtSendChatPicUrl.Location = new System.Drawing.Point(7, 425);
            this.txtSendChatPicUrl.Name = "txtSendChatPicUrl";
            this.txtSendChatPicUrl.ReadOnly = true;
            this.txtSendChatPicUrl.Size = new System.Drawing.Size(259, 23);
            this.txtSendChatPicUrl.TabIndex = 20;
            // 
            // btnSendChatPicMsg
            // 
            this.btnSendChatPicMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendChatPicMsg.Enabled = false;
            this.btnSendChatPicMsg.Location = new System.Drawing.Point(182, 451);
            this.btnSendChatPicMsg.Name = "btnSendChatPicMsg";
            this.btnSendChatPicMsg.Size = new System.Drawing.Size(175, 28);
            this.btnSendChatPicMsg.TabIndex = 22;
            this.btnSendChatPicMsg.Text = "发送群图片消息";
            this.btnSendChatPicMsg.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.scMain);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FrmMain";
            this.Text = "微信测试";
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel1.PerformLayout();
            this.scMain.Panel2.ResumeLayout(false);
            this.scMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.gbSend.ResumeLayout(false);
            this.gbSend.PerformLayout();
            this.gbGet.ResumeLayout(false);
            this.gbGet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbUserCover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbUserHead)).EndInit();
            this.gbClient.ResumeLayout(false);
            this.gbClient.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnGetUserList;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnGetPersonalDetail;
        private System.Windows.Forms.Button btnGetPersonalInfo;
        private System.Windows.Forms.Button btnSendTextMsg;
        private System.Windows.Forms.TextBox txtSendMsg;
        private System.Windows.Forms.GroupBox gbClient;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.GroupBox gbGet;
        private System.Windows.Forms.ComboBox cbUserListGet;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.ComboBox cbUserListSend;
        private System.Windows.Forms.TextBox txtGetOutput;
        private System.Windows.Forms.PictureBox pbUserHead;
        private System.Windows.Forms.PictureBox pbUserCover;
        private System.Windows.Forms.Button btnGetChatMemberNickname;
        private System.Windows.Forms.ComboBox cbWxChatMemberNickname;
        private System.Windows.Forms.Label lblConnectStatus;
        private System.Windows.Forms.ComboBox cbChatList;
        private System.Windows.Forms.Button btnSendChatTextMsg;
        private System.Windows.Forms.TextBox txtChatMsg;
        private System.Windows.Forms.CheckBox ckbAtMember;
        private System.Windows.Forms.TextBox txtRecMsg;
        private System.Windows.Forms.Button btnClearRecMsg;
        private System.Windows.Forms.CheckBox ckbDebug;
        private System.Windows.Forms.Button btnSendPicSelect;
        private System.Windows.Forms.TextBox txtSendPicUrl;
        private System.Windows.Forms.Button btnSendPicMsg;
        private System.Windows.Forms.Button btnSendChatPicMsg;
        private System.Windows.Forms.Button btnSendChatPicSelect;
        private System.Windows.Forms.TextBox txtSendChatPicUrl;
    }
}


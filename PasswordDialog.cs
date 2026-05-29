using System;
using System.Drawing;
using System.Windows.Forms;

namespace WatermarkApp
{
    public class PasswordDialog : Form
    {
        private TextBox txtPassword;
        private Button btnOK;
        private Button btnCancel;
        private Label lblMessage;

        public string Message { get; set; } = "请输入密码以关闭水印：";

        public PasswordDialog()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 在 OnLoad 中绑定消息文本 —— 此时调用方已设置 Message 属性
            lblMessage.Text = Message;
        }

        private void InitializeComponent()
        {
            this.Text = "密码验证";
            this.Size = new Size(350, 180);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;

            // 提示信息
            lblMessage = new Label();
            lblMessage.Location = new Point(20, 20);
            lblMessage.Size = new Size(300, 20);
            this.Controls.Add(lblMessage);

            // 密码输入框
            txtPassword = new TextBox();
            txtPassword.Location = new Point(20, 50);
            txtPassword.Size = new Size(300, 25);
            txtPassword.PasswordChar = '*';
            this.Controls.Add(txtPassword);

            // 确定按钮
            btnOK = new Button();
            btnOK.Text = "确定";
            btnOK.Location = new Point(130, 90);
            btnOK.Size = new Size(80, 30);
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            // 取消按钮
            btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Location = new Point(220, 90);
            btnCancel.Size = new Size(80, 30);
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            // 设置回车键触发确定按钮
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            // 焦点设置到密码框
            txtPassword.Focus();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (ConfigManager.VerifyPassword(txtPassword.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("密码错误，请重试！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace WatermarkApp
{
    public class SettingsDialog : Form
    {
        private TextBox txtLocation;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnSave;
        private Button btnCancel;
        private GroupBox grpLocation;
        private GroupBox grpPassword;

        public string Location { get; private set; }
        public string NewPassword { get; private set; }
        public bool PasswordChanged { get; private set; }

        public SettingsDialog()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "水印设置";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;

            // 位置信息组
            grpLocation = new GroupBox();
            grpLocation.Text = "设备位置";
            grpLocation.Location = new Point(20, 20);
            grpLocation.Size = new Size(400, 80);
            this.Controls.Add(grpLocation);

            txtLocation = new TextBox();
            txtLocation.Location = new Point(15, 30);
            txtLocation.Size = new Size(370, 25);
            grpLocation.Controls.Add(txtLocation);

            // 修改密码组
            grpPassword = new GroupBox();
            grpPassword.Text = "修改密码（留空表示不修改）";
            grpPassword.Location = new Point(20, 110);
            grpPassword.Size = new Size(400, 130);
            this.Controls.Add(grpPassword);

            Label lblNew = new Label();
            lblNew.Text = "新密码：";
            lblNew.Location = new Point(15, 30);
            lblNew.Size = new Size(60, 20);
            grpPassword.Controls.Add(lblNew);

            txtNewPassword = new TextBox();
            txtNewPassword.Location = new Point(80, 27);
            txtNewPassword.Size = new Size(300, 25);
            txtNewPassword.PasswordChar = '*';
            grpPassword.Controls.Add(txtNewPassword);

            Label lblConfirm = new Label();
            lblConfirm.Text = "确认密码：";
            lblConfirm.Location = new Point(15, 70);
            lblConfirm.Size = new Size(60, 20);
            grpPassword.Controls.Add(lblConfirm);

            txtConfirmPassword = new TextBox();
            txtConfirmPassword.Location = new Point(80, 67);
            txtConfirmPassword.Size = new Size(300, 25);
            txtConfirmPassword.PasswordChar = '*';
            grpPassword.Controls.Add(txtConfirmPassword);

            // 按钮
            btnSave = new Button();
            btnSave.Text = "保存";
            btnSave.Location = new Point(230, 260);
            btnSave.Size = new Size(80, 35);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Location = new Point(330, 260);
            btnCancel.Size = new Size(80, 35);
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadCurrentSettings()
        {
            txtLocation.Text = ConfigManager.GetLocation();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // 验证位置信息
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("请输入设备位置信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus();
                return;
            }

            Location = txtLocation.Text.Trim();

            // 验证密码
            if (!string.IsNullOrEmpty(txtNewPassword.Text))
            {
                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("两次输入的密码不一致！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtConfirmPassword.Focus();
                    return;
                }

                if (txtNewPassword.Text.Length < 4)
                {
                    MessageBox.Show("密码长度不能少于4位！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNewPassword.Focus();
                    return;
                }

                NewPassword = txtNewPassword.Text;
                PasswordChanged = true;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;


namespace QLSVNhomApp
{
    public partial class MainForm : Form
    {
        private string manv;
        private string tendn; // Thêm để lưu tên đăng nhập
        private string password;
        private Button btnViewInfo;
        private Button btnClassManagement;
        private Button btnLogout;
        private Label lblWelcome;

        public MainForm(string manv, string tendn, string password)
        {
            this.manv = manv;
            this.tendn = tendn;
            this.password = password;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Hệ thống Quản lý sinh viên";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormClosed += (s, e) => Application.Exit();

            int buttonWidth = 220;
            int buttonHeight = 50;
            int centerX = (this.ClientSize.Width - buttonWidth) / 2;

            // Welcome Label
            lblWelcome = new Label
            {
                Text = "Chào mừng bạn đến với hệ thống!",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblWelcome.Location = new Point((this.ClientSize.Width - lblWelcome.Width) / 2, 40);
            // Button View Information
            btnViewInfo = new Button
            {
                Text = "Xem thông tin cá nhân",
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(centerX, 120),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnViewInfo.FlatAppearance.BorderSize = 0;
            btnViewInfo.Click += BtnViewInfo_Click;

            // Button Class Management
            btnClassManagement = new Button
            {
                Text = "Quản lý lớp học",
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(centerX, 190),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnClassManagement.FlatAppearance.BorderSize = 0;
            btnClassManagement.Click += BtnClassManagement_Click;

            // Button Logout
            btnLogout = new Button
            {
                Text = "Đăng xuất",
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(centerX, 260),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;

            // Add controls
            this.Controls.AddRange(new Control[] { lblWelcome, btnViewInfo, btnClassManagement, btnLogout });

            // Resize event để canh giữa khi thay đổi kích thước form
            this.Resize += (s, e) =>
            {
                centerX = (this.ClientSize.Width - buttonWidth) / 2;
                lblWelcome.Location = new Point(centerX - 50, 40);
                btnViewInfo.Location = new Point(centerX, 120);
                btnClassManagement.Location = new Point(centerX, 190);
                btnLogout.Location = new Point(centerX, 260);
            };
        }


        private void BtnViewInfo_Click(object sender, EventArgs e)
        {
            var infoForm = new EmployeeInfoForm(manv, tendn, password,this);
            this.Hide();
            infoForm.ShowDialog();
            this.Show();
        }

        private void BtnClassManagement_Click(object sender, EventArgs e)
        {
            var classForm = new ClassManagementForm(manv, password,this);
            this.Hide();
            classForm.ShowDialog();
            this.Show();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.Show();
            loginForm.FormClosed += (s, args) => Application.Exit();
            this.Hide();
        }
    }
}
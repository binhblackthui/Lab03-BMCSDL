using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class LoginForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblTitle;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Đăng nhập hệ thống Quản lý sinh viên";
            this.Size = new Size(500, 400);
            this.MinimumSize = new Size(450, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Title Label
            lblTitle = new Label
            {
                Text = "Đăng nhập hệ thống",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, 30),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Label Username
            lblUsername = new Label
            {
                Text = "Tên đăng nhập:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(140, 90),
                Size = new Size(220, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Textbox Username
            txtUsername = new TextBox
            {
                Location = new Point(140, 115),
                Size = new Size(220, 30),
                Font = new Font("Segoe UI", 10),
                Text = "Nhập tên đăng nhập",
                ForeColor = Color.Gray
            };
            txtUsername.Enter += (s, e) =>
            {
                if (txtUsername.Text == "Nhập tên đăng nhập")
                {
                    txtUsername.Text = "";
                    txtUsername.ForeColor = Color.Black;
                }
            };
            txtUsername.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    txtUsername.Text = "Nhập tên đăng nhập";
                    txtUsername.ForeColor = Color.Gray;
                }
            };

            // Label Password
            lblPassword = new Label
            {
                Text = "Mật khẩu:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(140, 160),
                Size = new Size(220, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Textbox Password
            txtPassword = new TextBox
            {
                Location = new Point(140, 185),
                Size = new Size(220, 30),
                Font = new Font("Segoe UI", 10),
                Text = "Nhập mật khẩu",
                ForeColor = Color.Gray,
                UseSystemPasswordChar = false
            };
            txtPassword.Enter += (s, e) =>
            {
                if (txtPassword.Text == "Nhập mật khẩu")
                {
                    txtPassword.Text = "";
                    txtPassword.ForeColor = Color.Black;
                    txtPassword.UseSystemPasswordChar = true;
                }
            };
            txtPassword.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    txtPassword.Text = "Nhập mật khẩu";
                    txtPassword.ForeColor = Color.Gray;
                    txtPassword.UseSystemPasswordChar = false;
                }
            };

            // Button Login
            btnLogin = new Button
            {
                Text = "Đăng nhập",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Location = new Point(175, 250),
                Size = new Size(150, 40)
            };
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(30, 144, 255);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(0, 122, 204);

            // Add controls
            this.Controls.AddRange(new Control[] { lblTitle, txtUsername, txtPassword, btnLogin, lblUsername, lblPassword });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Nhập tên đăng nhập" || txtPassword.Text == "Nhập mật khẩu")
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_NHANVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TENDN", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@MK", txtPassword.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string manv = reader["MANV"].ToString();
                                MessageBox.Show("Đăng nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                var mainForm = new MainForm(manv, txtUsername.Text, txtPassword.Text);
                                mainForm.FormClosed += (s, args) => Application.Exit();
                                mainForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
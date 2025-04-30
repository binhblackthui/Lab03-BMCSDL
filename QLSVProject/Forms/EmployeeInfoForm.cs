using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class EmployeeInfoForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
        private string manv;
        private string tendn;
        private string mk;
        private MainForm mainForm;

        private Label lblManv;
        private Label lblHoten;
        private Label lblEmail;
        private Label lblLuong;
        private TextBox txtManv;
        private TextBox txtHoten;
        private TextBox txtEmail;
        private TextBox txtLuong;
        private Button btnSave;
        private Button btnCancel;

        public EmployeeInfoForm(string manv, string tendn, string mk, MainForm mainForm)
        {
            this.manv = manv;
            this.tendn = tendn;
            this.mk = mk;
            this.mainForm = mainForm;
            InitializeComponent();
            LoadEmployeeInfo();
            this.FormClosed += (s, e) => mainForm.Show();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Thông tin nhân viên";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Label Mã nhân viên
            lblManv = new Label
            {
                Text = "Mã nhân viên:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 50),
                Size = new Size(120, 25)
            };

            // TextBox Mã nhân viên (chỉ đọc)
            txtManv = new TextBox
            {
                Location = new Point(180, 50),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            // Label Họ tên
            lblHoten = new Label
            {
                Text = "Họ tên:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 100),
                Size = new Size(120, 25)
            };

            // TextBox Họ tên
            txtHoten = new TextBox
            {
                Location = new Point(180, 100),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10)
            };

            // Label Email
            lblEmail = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 150),
                Size = new Size(120, 25)
            };

            // TextBox Email
            txtEmail = new TextBox
            {
                Location = new Point(180, 150),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10)
            };

            // Label Lương
            lblLuong = new Label
            {
                Text = "Lương:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 200),
                Size = new Size(120, 25)
            };

            // TextBox Lương (chỉ đọc)
            txtLuong = new TextBox
            {
                Location = new Point(180, 200),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            // Button Save
            btnSave = new Button
            {
                Text = "Lưu",
                Size = new Size(120, 40),
                Location = new Point(180, 270),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // Button Cancel
            btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(120, 40),
                Location = new Point(310, 270),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            // Add controls
            this.Controls.AddRange(new Control[] { lblManv, txtManv, lblHoten, txtHoten, lblEmail, txtEmail, lblLuong, txtLuong, btnSave, btnCancel });
        }

        private void LoadEmployeeInfo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_NHANVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TENDN", tendn);
                        cmd.Parameters.AddWithValue("@MK", mk);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtManv.Text = reader["MANV"].ToString();
                                txtHoten.Text = reader["HOTEN"].ToString();
                                txtEmail.Text = reader["EMAIL"].ToString();
                                txtLuong.Text = reader["LUONGCB"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin nhân viên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                mainForm.Show();
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mainForm.Show();
                this.Close();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoten.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ họ tên và email.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_UPD_PUBLIC_NHANVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MANV", txtManv.Text);
                        cmd.Parameters.AddWithValue("@HOTEN", txtHoten.Text);
                        cmd.Parameters.AddWithValue("@EMAIL", txtEmail.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadEmployeeInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            mainForm.Show();
            this.Close();
        }
    }
}
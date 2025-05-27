using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QLSVProject.Helpers;
namespace QLSVNhomApp
{
    public partial class ScoreManagementForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
        private string masv;
        private string manv;
        private string password;
        private StudentManagementForm studentManagementForm;
        public ScoreManagementForm(string masv, string manv, string password,StudentManagementForm studentManagementForm)
        {
            this.masv = masv;
            this.manv = manv;
            this.password = password;
            this.studentManagementForm = studentManagementForm;
            InitializeComponent();
            LoadHocPhan(); // Load data immediately since password is provided
            this.FormClosed += (s, e) => studentManagementForm.Show();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Nhập bảng điểm";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248); // Light background

            // Initialize controls
            this.dgvHocPhan = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(740, 280),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            StyleDataGridView();
            this.dgvHocPhan.SelectionChanged += DgvHocPhan_SelectionChanged;

            // Title Label
            var lblTitle = new Label
            {
                Text = "Quản lý điểm sinh viên",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(740, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Load Button
            var btnLoad = new Button
            {
                Text = "Tải học phần",
                Size = new Size(150, 30),
                Location = new Point(20, 60),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Click += (s, e) => LoadHocPhan();
            btnLoad.MouseEnter += (s, e) => btnLoad.BackColor = Color.FromArgb(30, 144, 255);
            btnLoad.MouseLeave += (s, e) => btnLoad.BackColor = Color.FromArgb(0, 122, 204);

            this.lblMAHP = new Label
            {
                Text = "Mã học phần:",
                Font = new Font("Segoe UI", 10),
                Size = new Size(220, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            

            this.txtMAHP = new TextBox
            {
                Size = new Size(220, 30),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true
            };
            txtMAHP.Location = new Point((this.ClientSize.Width - txtMAHP.Width) / 2 - 110, 425);
            lblMAHP.Location = new Point(txtMAHP.Location.X, 400);


            this.lblDIEMTHI = new Label
            {
                Text = "Điểm thi:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(140, 450),
                Size = new Size(220, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            

            this.txtDIEMTHI = new TextBox
            {
                Location = new Point(140, 475),
                Size = new Size(220, 30),
                Font = new Font("Segoe UI", 10),
                Text = "Nhập điểm thi",
                ForeColor = Color.Gray
            };
            txtDIEMTHI.Location = new Point((this.ClientSize.Width - txtDIEMTHI.Width) / 2 +110, 425);
            lblDIEMTHI.Location = new Point(txtDIEMTHI.Location.X, 400);

            txtDIEMTHI.Enter += (s, e) =>
            {
                if (txtDIEMTHI.Text == "Nhập điểm thi")
                {
                    txtDIEMTHI.Text = "";
                    txtDIEMTHI.ForeColor = Color.Black;
                }
            };
            txtDIEMTHI.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtDIEMTHI.Text))
                {
                    txtDIEMTHI.Text = "Nhập điểm thi";
                    txtDIEMTHI.ForeColor = Color.Gray;
                }
            };

            this.btnSave = new Button
            {
                Text = "Lưu điểm",
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.Location = new Point((this.ClientSize.Width - btnSave.Width) / 2, 470);
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.Click += BtnSave_Click;
            this.btnSave.MouseEnter += (s, e) => btnSave.BackColor = Color.FromArgb(30, 144, 255);
            this.btnSave.MouseLeave += (s, e) => btnSave.BackColor = Color.FromArgb(0, 122, 204);

            // Add controls to form
            this.Controls.AddRange(new Control[] { lblTitle, dgvHocPhan, txtMAHP, txtDIEMTHI, btnSave, lblMAHP, lblDIEMTHI, btnLoad });
        }

        private DataGridView dgvHocPhan;
        private TextBox txtMAHP;
        private TextBox txtDIEMTHI;
        private Button btnSave;
        private Label lblMAHP;
        private Label lblDIEMTHI;

        private void StyleDataGridView()
        {
            dgvHocPhan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvHocPhan.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHocPhan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvHocPhan.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvHocPhan.DefaultCellStyle.BackColor = Color.White;
            dgvHocPhan.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
            dgvHocPhan.EnableHeadersVisualStyles = false;
            dgvHocPhan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHocPhan.MultiSelect = false;
        }

        private void LoadHocPhan()
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    
                    string privateKey = SecurityHelper.LoadPrivateKeyFromFile(manv);
                    privateKey = SecurityHelper.DecryptWithPrivateKey(privateKey, password); // Giải mã private key bằng mật khẩu
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_ENCRYPT_BANGDIEM", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MASV", masv);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("Không có dữ liệu bảng điểm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["DIEMTHI"] != DBNull.Value)
                                {
                                    row["DIEMTHI"] = SecurityHelper.DecryptWithPrivateKey(row["DIEMTHI"].ToString(), privateKey);
                                }

                            }
                            dgvHocPhan.DataSource = dt;
                        }
                        /*cmd.Parameters.AddWithValue("@MANV", manv);
                        cmd.Parameters.AddWithValue("@MK", password);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("Mật khẩu không đúng hoặc không có dữ liệu bảng điểm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            // Kiểm tra dữ liệu DIEMTHI để tránh lỗi hiển thị
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["DIEMTHI"] != DBNull.Value)
                                {
                                    string diemThi = row["DIEMTHI"]?.ToString();
                                    if (string.IsNullOrEmpty(diemThi))
                                    {
                                        row["DIEMTHI"] = DBNull.Value; // Đặt lại thành NULL nếu giải mã thất bại
                                    }
                                }
                            }
                            dgvHocPhan.DataSource = dt;
                        }*/
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvHocPhan_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHocPhan.SelectedRows.Count > 0)
            {
                txtMAHP.Text = dgvHocPhan.SelectedRows[0].Cells["MAHP"].Value?.ToString() ?? "";
                var diemThi = dgvHocPhan.SelectedRows[0].Cells["DIEMTHI"].Value;
                txtDIEMTHI.Text = diemThi != DBNull.Value && diemThi != null ? diemThi.ToString() : "";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDIEMTHI.Text) || txtDIEMTHI.Text == "Nhập điểm thi")
            {
                MessageBox.Show("Vui lòng nhập điểm thi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int diemThi;
                if (!int.TryParse(txtDIEMTHI.Text, out diemThi))
                {
                    MessageBox.Show("Điểm thi phải là số nguyên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (diemThi < 0 || diemThi > 10)
                {
                    MessageBox.Show("Điểm thi phải nằm trong khoảng từ 0 đến 10.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string sql = "SELECT PUBKEY FROM NHANVIEN WHERE MANV = @MANV";
                string publicKey = "";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MANV", manv);
                        publicKey = cmd.ExecuteScalar()?.ToString();
                    }
                }
                if(string.IsNullOrEmpty(publicKey))
                {
                    MessageBox.Show("Không tìm thấy khóa công khai.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
               

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_UPD_PUBLIC_ENCRYPT_BANGDIEM", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MASV", masv);
                        cmd.Parameters.AddWithValue("@MAHP", txtMAHP.Text);
                        cmd.Parameters.AddWithValue("@DIEMTHI", SecurityHelper.EncryptWithPublicKey(diemThi.ToString(), publicKey));

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Lưu điểm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadHocPhan();

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
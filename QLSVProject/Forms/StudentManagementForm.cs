using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class StudentManagementForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
        private string malop;
        private string manv;
        private string password;
        private ClassManagementForm classManagementForm;

        public StudentManagementForm(string malop, string manv,string password, ClassManagementForm classManagementForm)
        {
            this.malop = malop;
            this.manv = manv;
            this.password = password;
            this.classManagementForm = classManagementForm;
            InitializeComponent();
            this.FormClosed += (s, e) => classManagementForm.Show();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Quản lý sinh viên";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248); // Light background

            // Initialize controls
            this.dgvStudents = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(740, 300),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            StyleDataGridView();
            this.dgvStudents.SelectionChanged += DgvStudents_SelectionChanged;

            this.lblMASV = new Label
            {
                Text = "Mã SV:",
                Location = new Point(20, 340),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 10)
            };

            this.txtMASV = new TextBox
            {
                Location = new Point(100, 340),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true
            };

            this.lblHOTEN = new Label
            {
                Text = "Họ tên:",
                Location = new Point(20, 380),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 10)
            };

            this.txtHOTEN = new TextBox
            {
                Location = new Point(100, 380),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10)
            };

            this.lblNGAYSINH = new Label
            {
                Text = "Ngày sinh:",
                Location = new Point(320, 340),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 10)
            };

            this.txtNGAYSINH = new TextBox
            {
                Location = new Point(400, 340),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10)
            };

            this.lblDIACHI = new Label
            {
                Text = "Địa chỉ:",
                Location = new Point(320, 380),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 10)
            };

            this.txtDIACHI = new TextBox
            {
                Location = new Point(400, 380),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10)
            };

            this.btnUpdate = new Button
            {
                Text = "Cập nhật",
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 122, 204), // Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnUpdate.Location = new Point((this.ClientSize.Width - btnUpdate.Width) / 2 - 85, 440);
            this.btnUpdate.FlatAppearance.BorderSize = 0;
            this.btnUpdate.Click += BtnUpdate_Click;

            this.btnManageScores = new Button
            {
                Text = "Nhập điểm",
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 122, 204), // Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnManageScores.Location = new Point((this.ClientSize.Width - btnManageScores.Width) / 2 + 85, 440);
            this.btnManageScores.FlatAppearance.BorderSize = 0;
            this.btnManageScores.Click += BtnManageScores_Click;

            // Add controls to form
            this.Controls.AddRange(new Control[] { dgvStudents, txtMASV, txtHOTEN, txtNGAYSINH, txtDIACHI, btnUpdate, btnManageScores, lblMASV, lblHOTEN, lblNGAYSINH, lblDIACHI });

            // Load data
            LoadStudents();
        }

        private DataGridView dgvStudents;
        private TextBox txtMASV;
        private TextBox txtHOTEN;
        private TextBox txtNGAYSINH;
        private TextBox txtDIACHI;
        private Button btnUpdate;
        private Button btnManageScores;
        private Label lblMASV;
        private Label lblHOTEN;
        private Label lblNGAYSINH;
        private Label lblDIACHI;

        private void StyleDataGridView()
        {
            dgvStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvStudents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStudents.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvStudents.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvStudents.DefaultCellStyle.BackColor = Color.White;
            dgvStudents.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
            dgvStudents.EnableHeadersVisualStyles = false;
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.MultiSelect = false;
        }

        private void LoadStudents()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_SINHVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvStudents.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                txtMASV.Text = dgvStudents.SelectedRows[0].Cells["MASV"].Value.ToString();
                txtHOTEN.Text = dgvStudents.SelectedRows[0].Cells["HOTEN"].Value.ToString();
                txtNGAYSINH.Text = dgvStudents.SelectedRows[0].Cells["NGAYSINH"].Value.ToString();
                txtDIACHI.Text = dgvStudents.SelectedRows[0].Cells["DIACHI"].Value.ToString();
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_UPD_PUBLIC_SINHVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MASV", txtMASV.Text);
                        cmd.Parameters.AddWithValue("@HOTEN", txtHOTEN.Text);
                        cmd.Parameters.AddWithValue("@NGAYSINH", DateTime.Parse(txtNGAYSINH.Text));
                        cmd.Parameters.AddWithValue("@DIACHI", txtDIACHI.Text);
                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStudents();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnManageScores_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                string masv = dgvStudents.SelectedRows[0].Cells["MASV"].Value.ToString();
                var scoreForm = new ScoreManagementForm(masv, manv, password,this);
                this.Hide();
                scoreForm.ShowDialog();

            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sinh viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
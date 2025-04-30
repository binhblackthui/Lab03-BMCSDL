using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class ClassManagementForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
        private string manv;
        private string password;
        private MainForm mainForm;

        private DataGridView dgvClasses;
        private Button btnManageStudents;
        private TextBox txtSearch;
        private Label lblSearch;

        public ClassManagementForm(string manv, string password, MainForm mainForm)
        {
            this.manv = manv;
            this.password = password;
            this.mainForm = mainForm;
            InitializeComponent();
            this.FormClosed += (s, e) => mainForm.Show();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Quản lý lớp học";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248); // Light background

            // Initialize controls
            dgvClasses = new DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(740, 400),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            StyleDataGridView();

            btnManageStudents = new Button
            {
                Text = "Quản lý sinh viên",
                Size = new Size(150, 40),
                Location = new Point(20, 500),
                BackColor = Color.FromArgb(0, 122, 204), // Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnManageStudents.FlatAppearance.BorderSize = 0;
            btnManageStudents.Click += BtnManageStudents_Click;

            lblSearch = new Label
            {
                Text = "Tìm kiếm lớp:",
                Location = new Point(20, 30),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 10)
            };

            txtSearch = new TextBox
            {
                Location = new Point(120, 30),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                Text = "Nhập mã lớp hoặc tên lớp", // Placeholder
                ForeColor = Color.Gray
            };
            // Placeholder behavior
            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == "Nhập mã lớp hoặc tên lớp")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Nhập mã lớp hoặc tên lớp";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.Text != "Nhập mã lớp hoặc tên lớp")
                {
                    SearchClasses();
                }
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { dgvClasses, btnManageStudents, lblSearch, txtSearch });

            // Load data
            LoadClasses();
        }

        private void StyleDataGridView()
        {
            dgvClasses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvClasses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvClasses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvClasses.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvClasses.DefaultCellStyle.BackColor = Color.White;
            dgvClasses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
            dgvClasses.EnableHeadersVisualStyles = false;
            dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClasses.MultiSelect = false;
        }

        private void LoadClasses()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_LOP", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MANV", manv);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvClasses.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchClasses()
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            if (searchText == "nhập mã lớp hoặc tên lớp" || string.IsNullOrEmpty(searchText))
            {
                LoadClasses();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM LOP WHERE MALOP LIKE @Search OR TENLOP LIKE @Search", conn))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + searchText + "%");
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvClasses.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnManageStudents_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count > 0)
            {
                string malop = dgvClasses.SelectedRows[0].Cells["MALOP"].Value.ToString();
                var studentForm = new StudentManagementForm(malop, manv,password,this);
                this.Hide();
                studentForm.ShowDialog();
                LoadClasses(); // Refresh after managing students
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một lớp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
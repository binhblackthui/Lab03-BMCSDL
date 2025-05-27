using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using System.Windows.Forms.VisualStyles;

namespace QLSVNhomApp
{
    public partial class EmployeeListForm : Form
    {
        // Fields to hold employee data
        private DataGridView dgvEmployees;
        private string connectionString = ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
        private MainForm mainForm;
        private string manv; // Employee ID

        public EmployeeListForm(string manv,MainForm mainForm)
        {
            this.mainForm = mainForm;
            this.manv = manv; // Store the employee ID passed from the main form
            InitializeComponent();
            this.FormClosed += (s, e) => mainForm.Show(); // Show the main form when this form is closed
        }

        private void InitializeComponent()
        {
            // Initialize components and set up the form
            this.Text = "Employee Management";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248); // Light background

            // Initialize DataGridView
            dgvEmployees = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(740, 300),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
               
            };

            // Style the DataGridView
            StyleDataGridView();

            // Add DataGridView to the form's controls
            this.Controls.Add(dgvEmployees);

            // Wire up the form's load event
            this.Load += new EventHandler(EmployeeListForm_Load);
        }

        private void StyleDataGridView()
        {
            dgvEmployees.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvEmployees.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEmployees.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvEmployees.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvEmployees.DefaultCellStyle.BackColor = Color.White;
            dgvEmployees.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
            dgvEmployees.EnableHeadersVisualStyles = false;
            dgvEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmployees.MultiSelect = false;
        }

        private void EmployeeListForm_Load(object sender, EventArgs e)
        {
            // Load employee data into the form
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            try
            {
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_NHANVIEN", connection))
                    {
                        
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TRUONGP", this.manv); // Pass the employee ID from the main form

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvEmployees.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
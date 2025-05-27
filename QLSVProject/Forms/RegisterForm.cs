using System;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using QLSVNhomApp;
using QLSVProject.Helpers;
using System.Security.Cryptography.X509Certificates;
namespace QLSVProject.Forms
{
    internal class RegisterForm : Form // Inherit from Form to access Form properties like Size
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
        private Label lblMaNV;
        private Label lblHoTen;
        private Label lblEmail;
        private Label lblLuong;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtMaNV;
        private TextBox txtHoTen;
        private TextBox txtEmail;
        private TextBox txtLuong;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnRegister;
        private TextBox txtMaTrgPhong;
        private Label lblMaTrgPhong;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Đăng ký tài khoản";
            this.Size = new Size(400, 500); // Size is a property of Form
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Label Mã nhân viên
            lblMaNV = new Label
            {
                Text = "Mã nhân viên:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 50),
                Size = new Size(120, 25)
            };

            // TextBox Mã nhân viên
            txtMaNV = new TextBox
            {
                Location = new Point(200, 50),
                Size = new Size(150, 25)
            };

            // Label Họ tên
            lblHoTen = new Label
            {
                Text = "Họ tên:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 100),
                Size = new Size(120, 25)
            };

            // TextBox Họ tên
            txtHoTen = new TextBox
            {
                Location = new Point(200, 100),
                Size = new Size(150, 25)
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
                Location = new Point(200, 150),
                Size = new Size(150, 25)
            };

            // Label Lương
            lblLuong = new Label
            {
                Text = "Lương:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 200),
                Size = new Size(120, 25)
            };

            // TextBox Lương
            txtLuong = new TextBox
            {
                Location = new Point(200, 200),
                Size = new Size(150, 25)
            };
            lblMaTrgPhong = new Label
            {
                Text = "Mã Trưởng Phòng:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 250),
                Size = new Size(120, 25)
            };
            // TextBox Mã Trưởng Phòng
            txtMaTrgPhong = new TextBox
            {
                Location = new Point(200, 250),
                Size = new Size(150, 25)
            };

            // Label Tên đăng nhập
            lblUsername = new Label
            {
                Text = "Tên đăng nhập:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 300),
                Size = new Size(120, 25)
            };
            // TextBox Tên đăng nhập
            txtUsername = new TextBox
            {
                Location = new Point(200, 300),
                Size = new Size(150, 25)
            };
            // Label Mật khẩu
            lblPassword = new Label
            {
                Text = "Mật khẩu:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 350),
                Size = new Size(120, 25)
            };
            // TextBox Mật khẩu
            txtPassword = new TextBox
            {
                Location = new Point(200, 350),
                Size = new Size(150, 25),
                PasswordChar = '*'
            };
            // Button Đăng ký
            btnRegister = new Button
            {
                Text = "Đăng ký",
                Location = new Point(150, 400),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 105, 217);
            btnRegister.Click += new EventHandler(btnRegister_Click);
            // Add controls to the form
            this.Controls.Add(lblMaNV);
            this.Controls.Add(txtMaNV);
            this.Controls.Add(lblHoTen);
            this.Controls.Add(txtHoTen);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblLuong);
            this.Controls.Add(txtLuong);
            this.Controls.Add(lblMaTrgPhong);
            this.Controls.Add(txtMaTrgPhong);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnRegister);
            
        }
       
        private (string publicKeyXml, string privateKeyXml) GenerateRSAKeyPair()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                string publicKey = rsa.ToXmlString(false); // public key
                string privateKey = rsa.ToXmlString(true); // private key
                return (publicKey, privateKey);
            }
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string manv = txtMaNV.Text;
            string hoten = txtHoTen.Text;
            string email = txtEmail.Text;
            string luong = txtLuong.Text;
            

            string tendn = txtUsername.Text;
            string matkhau = txtPassword.Text;
            string matTrgPhong = txtMaTrgPhong.Text;
        
                // Fix for CS0019: Operator '==' cannot be applied to operands of type 'string' and 'char'
                if (matTrgPhong.Length == 0) // Compare the length as an integer
                {
                    matTrgPhong = null;
                }
            MessageBox.Show(matTrgPhong);

            // Băm mật khẩu
            string hashedPassword = Helpers.SecurityHelper.HashPasswordSHA1(matkhau);
            // Tạo cặp khóa RSA
            var (publicKeyXml, privateKeyXml) = GenerateRSAKeyPair();
            privateKeyXml = Helpers.SecurityHelper.encryptPrivateWithPassword(privateKeyXml, matkhau); // Mã hóa private key bằng mật khẩu (mã nhân viên)
            // Lưu private key vào file

            Helpers.SecurityHelper.SavePrivateKeyToFile(privateKeyXml, manv);

            string encryptedLuong = Helpers.SecurityHelper.EncryptWithPublicKey(luong, publicKeyXml);
            // Gửi dữ liệu lên SQL Server
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_INS_PUBLIC_ENCRYPT_NHANVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MANV", manv);
                        cmd.Parameters.AddWithValue("@HOTEN", hoten);
                        cmd.Parameters.AddWithValue("@EMAIL", email);
                        cmd.Parameters.AddWithValue("@LUONGCB", encryptedLuong);
                        cmd.Parameters.AddWithValue("@TENDN", tendn);
                        cmd.Parameters.AddWithValue("@MK", hashedPassword);
                        cmd.Parameters.AddWithValue("@PUB", publicKeyXml);
                        if (string.IsNullOrEmpty(matTrgPhong))
                        {
                            cmd.Parameters.AddWithValue("@TRGPHONG", DBNull.Value); // Thêm tham số mã trưởng phòng nếu không có
                        }
                        else
                            cmd.Parameters.AddWithValue("@TRGPHONG", matTrgPhong); // Thêm tham số mã trưởng phòng

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Đăng ký thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        var login = new LoginForm();
                        this.Hide();
                        login.ShowDialog();
                        

                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

       

    }
}

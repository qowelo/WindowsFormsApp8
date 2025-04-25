using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Runtime.CompilerServices.RuntimeHelpers;


namespace WindowsFormsApp8
{

    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {


           // RegisterUser();
        }

        string generatedCode = ""; // временный код
        bool emailConfirmed = false; // подтверждение почты
     
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void buttonGetCode_Click(object sender, EventArgs e)
        {
            string userEmail = textBoxEmail.Text.Trim();

            if (string.IsNullOrEmpty(userEmail))
            {
                MessageBox.Show("Введите почту.");
                return;
            }

            // Генерируем код
            Random rand = new Random();
            generatedCode = rand.Next(1000, 9999).ToString();

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("dariana2622@gmail.com");
                mail.To.Add(userEmail);
                mail.Subject = "Код подтверждения";
                mail.Body = $"Ваш код подтверждения: {generatedCode}";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("dariana2622@gmail.com", "mbmt wjwl pmhh bfdp");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                MessageBox.Show("Код отправлен на вашу почту.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке: " + ex.Message);
            }
        }

        private void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxLastName.Text) ||
                string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(textBoxPass.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!");
                return;
            }

            string connStr = "server=localhost;user=root;database=databasedariana;password=Dasha2843!;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string hashedPassword = HashPassword(textBoxPass.Text);
                    string generatedCode = Guid.NewGuid().ToString(); // если используешь getscode

                    string query = "INSERT INTO users (name, lastname, email, getscode, password) " +
                                   "VALUES (@name, @lastname, @email, @getscode, @password)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@name", textBoxName.Text);
                    cmd.Parameters.AddWithValue("@lastname", textBoxLastName.Text);
                    cmd.Parameters.AddWithValue("@email", textBoxEmail.Text);
                    cmd.Parameters.AddWithValue("@getscode", generatedCode);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Регистрация прошла успешно!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения или записи: " + ex.Message);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2")); // hex строка
                }
                return sb.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string enteredCode = textBoxGetsCode.Text.Trim(); // поле, куда пользователь вводит полученный код
            string email = textBoxEmail.Text.Trim();
            string password = textBoxPass.Text.Trim();
            string name = textBoxName.Text.Trim();
            string lastname = textBoxLastName.Text.Trim();
            string hashedPassword = HashPassword(password);


            // Проверка на пустые поля
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastname))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Проверка кода подтверждения
            if (enteredCode != generatedCode)
            {
                MessageBox.Show("Код подтверждения неверный. Пожалуйста, проверьте почту.");
                return;
            }

            // Если код верен, продолжаем регистрацию
            try
            {
               
                string connStr = "server=localhost;user=root;password=Dasha2843!;database=databasedariana;";
                using (MySqlConnection conn = new MySqlConnection(connStr))

             
                {
                  
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @checkEmail";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@checkEmail", email);
                    object result = checkCmd.ExecuteScalar();
                    int userExists = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;

                    if (userExists > 0)
                    {
                        MessageBox.Show("Пользователь с такой почтой уже зарегистрирован!");
                        return;
                    }

                    string query = "INSERT INTO users (lastname, name, email, getscode, password) " +
                                   "VALUES (@lastname, @name, @email, @getscode, @password)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@lastname", lastname);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@getscode", enteredCode); // можно сохранять
                    cmd.Parameters.AddWithValue("@password", hashedPassword);    // лучше хешировать

                    cmd.ExecuteNonQuery();

                }

                MessageBox.Show("Регистрация прошла успешно!");
                // Очистка форм или переход на следующую форму
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка регистрации: " + ex.Message);
            }
        }
    }
}

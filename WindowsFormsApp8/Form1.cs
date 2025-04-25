using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace WindowsFormsApp8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            Form2 registrationForm = new Form2();
            registrationForm.Show();
            this.Hide(); // скрываем текущую форму, если нужно
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
        
            string email = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Text;
         

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите email и пароль.");
                return;
            }

            string hashedPassword = HashPassword(password);
            string connStr = "server=localhost;user=root;database=databasedariana;password=Dasha2843!;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE email = @email AND password = @password";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Вход выполнен успешно!");
                        Form3 form3 = new Form3(); // открываем форму с данными
                        form3.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Неверный email или пароль.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения: " + ex.Message);
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
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private void buttonAdmin_Click(object sender, EventArgs e)
        {
            string adminUsername = "admin";  // Логин админа
            string adminPassword = "123";  // Пароль админа
            if (adminUsername == "admin" && adminPassword == "123")
            {
                // Если логин и пароль совпадают, переходим на форму админа
                Form4 adminForm = new Form4();
                adminForm.Show();
                this.Hide();  // Прячем текущую форму
            }
        }

        private void buttonForgotPass_Click(object sender, EventArgs e)
        {
            string userEmail = textBoxLogin.Text.Trim();

            if (string.IsNullOrEmpty(userEmail))
            {
                MessageBox.Show("Введите вашу почту.");
                return;
            }

            // Генерируем уникальный код для сброса пароля
            Random rand = new Random();
            string resetCode = rand.Next(1000, 9999).ToString(); // это можно заменить на более сложный код

            // Отправляем код на почту пользователя
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("dariana2622@gmail.com");
                mail.To.Add(userEmail);
                mail.Subject = "Сброс пароля";
                mail.Body = $"Для сброса пароля перейдите по ссылке: http://localhost/reset-pass/index.html?code={resetCode}";


                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("dariana2622@gmail.com", "mbmt wjwl pmhh bfdp");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                // Сохраняем код в БД для дальнейшего подтверждения
                string connStr = "server=localhost;user=root;password=Dasha2843!;database=databasedariana;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "UPDATE users SET getscode = @getscode WHERE email = @userEmail";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@getscode", resetCode);
                    cmd.Parameters.AddWithValue("@userEmail", userEmail);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Ссылка для сброса пароля отправлена на вашу почту.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке: " + ex.Message);
            }
        }
    }
}
    


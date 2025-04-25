using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp8
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadUserList();
        }
        private void LoadUserList()
        {
            string connectionString = "server=localhost;user=root;password=Dasha2843!;database=databasedariana;";
            string query = "SELECT idusers, lastname, name, email, getscode FROM users";  // Получаем необходимые столбцы

            DataTable table = new DataTable();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    adapter.Fill(table);
                }

                // Загружаем данные в DataGridView
                dataGridViewUsers.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пользователей: {ex.Message}\n{ex.StackTrace}", "Ошибка");
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}

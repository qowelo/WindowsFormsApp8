using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;




namespace WindowsFormsApp8
{


    public partial class Form3 : Form
    {

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            /*  LoadUsersData();*/
            LoadWarehouses();

        }
        /*  private void LoadUsersData()
          {
              string connStr = "server=localhost;user=root;database=databasedariana;password=Dasha2843!;";

              try
              {
                  using (MySqlConnection conn = new MySqlConnection(connStr))
                  {
                      conn.Open();
                      string query = "SELECT * FROM my_warehouses";

                      MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                      DataTable dt = new DataTable();
                      da.Fill(dt);

                      dataGridViewUsers.DataSource = dt;

                      // Автоматическое расширение столбцов
                      dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                  }
              }
              catch (Exception ex)
              {
                  MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
              }
          }*/
        private void LoadWarehouses()
        {
            string connStr = "server=localhost;user=root;database=databasedariana;password=Dasha2843!;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT * FROM my_warehouses";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridViewUsers.DataSource = dt;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string connStr = "server=localhost;user=root;database=databasedariana;password=Dasha2843!;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "INSERT INTO my_warehouses (name, quantity, category, place_in_the_waregouse, supplier, date_of_receipt) " +
                               "VALUES (@name, @quantity, @category, @place, @supplier, @date)";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@name", textBoxName.Text);
                cmd.Parameters.AddWithValue("@quantity", Convert.ToInt32(textBoxQuantity.Text));
                cmd.Parameters.AddWithValue("@category", textBoxCategory.Text);
                cmd.Parameters.AddWithValue("@place", textBoxPlace.Text);
                cmd.Parameters.AddWithValue("@supplier", textBoxSupplier.Text);
                cmd.Parameters.AddWithValue("@date", dateTimePicker1.Value);

                cmd.ExecuteNonQuery();
            }

            LoadWarehouses(); // обновить таблицу
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridViewUsers.SelectedRows[0].Cells["id"].Value);

                string connStr = "server=localhost;user=root;database=databasedariana;password=Dasha2843!;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "UPDATE my_warehouses SET name = @name, quantity = @quantity, category = @category, " +
                                   "place_in_the_waregouse = @place, supplier = @supplier, date_of_receipt = @date WHERE id = @id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", textBoxName.Text);
                    cmd.Parameters.AddWithValue("@quantity", Convert.ToInt32(textBoxQuantity.Text));
                    cmd.Parameters.AddWithValue("@category", textBoxCategory.Text);
                    cmd.Parameters.AddWithValue("@place", textBoxPlace.Text);
                    cmd.Parameters.AddWithValue("@supplier", textBoxSupplier.Text);
                    cmd.Parameters.AddWithValue("@date", dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }

                LoadWarehouses();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridViewUsers.SelectedRows[0].Cells["id"].Value);

                string connStr = "server=localhost;user=root;database=databasedariana;password=Dasha2843!;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "DELETE FROM my_warehouses WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                LoadWarehouses();
            }
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // 1. Подключение к базе
            string connectionString = "server=localhost;user=root;password=Dasha2843!;database=databasedariana;";
            string query = "SELECT id, name, quantity, category, place_in_the_waregouse, supplier, date_of_receipt FROM my_warehouses";  //SQL-запрос — взять ВСЕ строки и поля из таблицы my_warehouses

            DataTable table = new DataTable();  // Создание таблицы Ексель

            try  // обрабатывает ошибки, если их нет, то выполняется действие в любом случае иначе ошибка
            {
                using (var connection = new MySqlConnection(connectionString))  //открываем соединение с БД
                using (var adapter = new MySqlDataAdapter(query, connection))   //открываем соединение с БД
                {
                    adapter.Fill(table);
                }

                // 2. Создание Excel-файла
                using (var workbook = new XLWorkbook())   //Создание Excel-файла
                {
                    var worksheet = workbook.Worksheets.Add("Склад");   // Название файла

                    // Заголовки
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        worksheet.Cell(1, j + 1).Value = table.Columns[j].ColumnName;
                    }

                    // Данные
                    for (int i = 0; i < table.Rows.Count; i++)  // i — строки (от 0 до количества записей)
                    {
                        for (int j = 0; j < table.Columns.Count; j++)  // j — колонки (от 0 до количества столбцов)
                        {
                            var value = table.Rows[i][j];
                            worksheet.Cell(i + 2, j + 1).Value = value == DBNull.Value ? "" : value.ToString(); // i + 2 — потому что первая строка (1) уже занята заголовками value == DBNull.Value ? "" : value.ToString() — проверка: если NULL, вставляем пусто, иначе — значение
                        }
                    }

                    // Автоширина. Автоматически расширяет колонки по содержимому, чтобы всё влезло красиво
                    worksheet.Columns().AdjustToContents();

                    // 3. Сохранение файла  Склеиваем путь до рабочего стола + имя файла
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Склад.xlsx");
                    workbook.SaveAs(path);  // Сохраняем книгу с помощью SaveAs(path)

                    MessageBox.Show("Файл успешно создан на рабочем столе!", "Успех");
                }
            }
            catch (Exception ex) // Exception ex Внутри catch мы можем получить объект исключения, который содержит информацию о возникшей ошибке.


            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
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
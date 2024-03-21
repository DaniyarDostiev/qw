using Microsoft.Win32;
using qw.application_frames.view_frames;
using qw.database;
using qw.util;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;
using OfficeOpenXml;

namespace qw.application_pages.edit_pages
{
    /// <summary>
    /// Логика взаимодействия для CustomerEditPage.xaml
    /// </summary>
    public partial class CustomerEditPage : Page
    {
        private Заказчик customer;

        public CustomerEditPage(Заказчик customer)
        {
            InitializeComponent();
            this.customer = customer;
            fieldFill();
        }

        private void fieldFill()
        {
            nameTextBox.Text = customer.имя;
            legalAddressTextBox.Text = customer.юр__адрес;
            actualAddressTextBox.Text = customer.физ__адрес;
            innTextBox.Text = customer.инн;
            kpkTextBox.Text = customer.кпк;
            rsTextBox.Text = customer.р_с;
            spokesmanTextBox.Text = customer.представитель;
            phoneNumberTextBox.Text = customer.номер_телефона;
            emailTextBox.Text = customer.эл__почта;
            websiteLinkTextBox.Text = customer.ссылка_на_сайт;

            addDateTextBox.Text = customer.дата_добавления_записи.ToString();
            editDateTextBox.Text = customer.дата_последнего_изменения_записи.ToString();
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new Customer());
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                customer.имя = nameTextBox.Text;
                customer.юр__адрес = legalAddressTextBox.Text;
                customer.физ__адрес = actualAddressTextBox.Text;
                customer.инн = innTextBox.Text;
                customer.кпк = kpkTextBox.Text;
                customer.р_с = rsTextBox.Text;
                customer.представитель = spokesmanTextBox.Text;
                customer.номер_телефона = phoneNumberTextBox.Text;
                customer.эл__почта = emailTextBox.Text;
                customer.ссылка_на_сайт = websiteLinkTextBox.Text;

                if (customer.дата_добавления_записи != null)
                {
                    customer.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                }
                else
                {
                    customer.дата_добавления_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    customer.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    DbWorker.GetContext().Заказчик.Add(customer);
                }
                DbWorker.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены");
                AppFrame.mainFrame.Navigate(new Customer());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: " + ex.Message);
            }
        }

        private void importInfoButtonClick(object sender, object e)
        {
            new ImportWindow().Show();
        }

        private void importButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|Text Files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ImportData(filePath);
            }
        }

        private void ImportData(string filePath)
        {
            if (Path.GetExtension(filePath).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                excelImport(filePath);
            }
            else if (Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                txtImport(filePath);
            }
            else
            {
                MessageBox.Show("Формат файла не поддерживается.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void excelImport(String filePath)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    for (int col = 1; col <= colCount; col++)
                    {
                        string header = worksheet.Cells[1, col].Value?.ToString();

                        // Ищем соответствующий TextBox по его имени (в нижнем регистре)
                        TextBox textBox = FindTextBoxByHeader(header.ToLower());

                        if (textBox != null)
                        {
                            // Заполняем TextBox данными из Excel файла
                            textBox.Text = worksheet.Cells[2, col].Value?.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте данных из Excel: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtImport(String filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                // Проверяем, что файл содержит необходимое количество строк
                if (lines.Length >= 2)
                {
                    string[] headers = lines[0].Split(';');
                    string[] values = lines[1].Split(';');

                    // Проверяем, что количество заголовков и значений совпадает
                    if (headers.Length == values.Length)
                    {
                        for (int i = 0; i < headers.Length; i++)
                        {
                            // Находим соответствующий TextBox по имени заголовка и заполняем его значением
                            TextBox textBox = FindTextBoxByHeader(headers[i].ToLower());
                            if (textBox != null)
                            {
                                textBox.Text = values[i];
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Количество заголовков не совпадает с количеством значений.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Файл не содержит достаточно строк.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private TextBox FindTextBoxByHeader(string header)
        {
            switch (header)
            {
                case "имя":
                    return nameTextBox;
                case "юр. адрес":
                    return legalAddressTextBox;
                case "физ. адрес":
                    return actualAddressTextBox;
                case "инн":
                    return innTextBox;
                case "кпк":
                    return kpkTextBox;
                case "р/с":
                    return rsTextBox;
                case "представитель":
                    return spokesmanTextBox;
                case "номер телефона":
                    return phoneNumberTextBox;
                case "эл. почта":
                    return emailTextBox;
                case "ссылка на сайт":
                    return websiteLinkTextBox;
                default:
                    return null;
            }
        }
    }
}

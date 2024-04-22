using Microsoft.Win32;
using OfficeOpenXml;
using qw.application_pages.additional_views;
using qw.application_pages.views;
using qw.database;
using qw.util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
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
using Path = System.IO.Path;

namespace qw.application_pages.edits
{
    /// <summary>
    /// Логика взаимодействия для ProjectEditPage.xaml
    /// </summary>
    public partial class ProjectEditPage : Page
    {
        private Заказчик customer;
        private Проект project;
        public ProjectEditPage(Заказчик customer, Проект project)
        {
            InitializeComponent();
            if (project.id != 0)
            {
                importButton.Visibility = Visibility.Hidden;
                importInfoButton.Visibility = Visibility.Hidden;
            }
            this.customer = customer;
            this.project = project;

            contractComboBox.ItemsSource = DbWorker.GetContext().Договор
                .Where(x => x.удален != true).Select(x => x.название).ToList();

            fieldFill();
        }

        private void fieldFill()
        {
            nameTextBox.Text = project.название;

            if (project.Договор == null)
            {
                contractComboBox.SelectedIndex = 0;
            }
            else
            {
                contractComboBox.SelectedItem = project.Договор.название;
            }

            addDateTextBox.Text = project.дата_добавления_записи.ToString();
            editDateTextBox.Text = project.дата_последнего_изменения_записи.ToString() ;
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                project.название = nameTextBox.Text;
                project.id_заказчика = customer.id;
                project.id_договора = DbWorker.GetContext().Договор
                    .FirstOrDefault(x => x.название == contractComboBox.Text)?.id;

                if (project.дата_добавления_записи != null)
                {
                    project.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                }
                else
                {
                    project.дата_добавления_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    project.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    project.удален = false;
                    DbWorker.GetContext().Проект.Add(project);
                }
                DbWorker.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены");
                AppFrame.mainFrame.Navigate(new ProjectPage(customer));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: " + ex.Message);
            }
        }

        private void contractPageButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ContractCrudPage(customer, project));
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProjectPage(customer));
        }

        private void importInfoButtonClick(object sender, RoutedEventArgs e)
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

        private void excelImport(string filePath)
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

                        // Ищем соответствующее TextBox по его имени (в нижнем регистре)
                        if (header.ToLower() == "название")
                        {
                            // Заполняем nameTextBox данными из Excel файла
                            nameTextBox.Text = worksheet.Cells[2, col].Value?.ToString();
                        }
                        else if (header.ToLower() == "договор")
                        {
                            // Заполняем contractComboBox данными из Excel файла
                            var name = worksheet.Cells[2, col].Value?.ToString();
                            var foundedObj = DbWorker.GetContext().Договор.FirstOrDefault(x => x.название.Equals(name));
                            if (foundedObj != null)
                            {
                                contractComboBox.SelectedItem = foundedObj.название;
                            }
                            else
                            {
                                MessageBox.Show("не все данные были корректны");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте данных из Excel: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtImport(string filePath)
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
                            if (headers[i].ToLower() == "название")
                            {
                                // Заполняем nameTextBox данными из текстового файла
                                nameTextBox.Text = values[i];
                            }
                            else if (headers[i].ToLower() == "договор")
                            {
                                // Заполняем contractComboBox данными из текстового файла
                                var name = values[i];
                                var foundedObj = DbWorker.GetContext().Договор.FirstOrDefault(x => x.название.Equals(name));
                                if (foundedObj != null)
                                {
                                    contractComboBox.SelectedItem = foundedObj.название;
                                }
                                else
                                {
                                    MessageBox.Show("не все данные были корректны");
                                }
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
    }
}

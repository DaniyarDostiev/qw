using Microsoft.Win32;
using OfficeOpenXml;
using qw.application_pages.additional_views;
using qw.application_pages.views;
using qw.database;
using qw.util;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AreaEditPage.xaml
    /// </summary>
    public partial class AreaEditPage : Page
    {
        private Проект project;
        private Площадь area;
        public AreaEditPage(Проект prject, Площадь area)
        {
            InitializeComponent();
            if (area.id != 0)
            {
                importButton.Visibility = Visibility.Hidden;
                importInfoButton.Visibility = Visibility.Hidden;
            }
            else
            {
                coordinatesButton.Visibility = Visibility.Hidden;
            }
            this.project = prject;
            this.area = area;

            fieldSupervisorComboBox.ItemsSource = DbWorker.GetContext().Сотрудник
                .Where(x => x.удален != true)
                .Select(x => x.логин).ToList();
            dataProcessingSupervisorComboBox.ItemsSource = DbWorker.GetContext().Сотрудник
                .Where(x => x.удален != true)
                .Select(x => x.логин).ToList();

            fieldFill();
        }

        private void fieldFill()
        {
            nameTextBox.Text = area.название;
            if (area.Сотрудник == null && area.Сотрудник1 == null)
            {
                fieldSupervisorComboBox.SelectedIndex = 0;
                dataProcessingSupervisorComboBox.SelectedIndex = 0;
            }
            else
            {
                fieldSupervisorComboBox.SelectedItem = area.Сотрудник.логин;
                dataProcessingSupervisorComboBox.SelectedItem = area.Сотрудник1.логин;
            }
            perimeterLengthTextBox.Text = area.длина_периметра.ToString();
            perimeterAreaTextBox.Text = area.площадь_периметра.ToString();
            dateOfBeginTextBox.Text = area.дата_начала_работ.ToString();
            dateOfEndTextBox.Text = area.дата_окончания_работ.ToString();
            addDateTextBox.Text = area.дата_добавления_записи.ToString();
            editDateTextBox.Text = area.дата_последнего_изменения_записи.ToString();
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                area.название = nameTextBox.Text;
                area.id_проекта = project.id;
                area.id_супервайзера_полевых_работ = DbWorker.GetContext().Сотрудник
                    .FirstOrDefault(x => x.логин == fieldSupervisorComboBox.Text).id;
                area.id_супервайзера_обработки_данных = DbWorker.GetContext().Сотрудник
                    .FirstOrDefault(x => x.логин == fieldSupervisorComboBox.Text).id;
                area.длина_периметра = Double.Parse(perimeterLengthTextBox.Text);
                area.площадь_периметра = Double.Parse(perimeterAreaTextBox.Text);
                area.дата_начала_работ = DateTime.Parse(dateOfBeginTextBox.Text);
                area.дата_окончания_работ = DateTime.Parse(dateOfEndTextBox.Text);

                if (area.дата_добавления_записи != null)
                {
                    area.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                }
                else
                {
                    area.дата_добавления_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    area.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    area.удален = false;
                    DbWorker.GetContext().Площадь.Add(area);
                }
                DbWorker.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены");
                AppFrame.mainFrame.Navigate(new AreaPage(project));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: " + ex.Message);
            }
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new AreaPage(project));
        }
        
        private void employeePageButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new EmployeeCrudPage(area));
        }

        private void coordinatesPageClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new AreaPerimeterAngles(area));
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

                        // Ищем соответствующий TextBox по его имени (в нижнем регистре)
                        if (header.ToLower() == "название")
                        {
                            // Заполняем nameTextBox данными из Excel файла
                            nameTextBox.Text = worksheet.Cells[2, col].Value?.ToString();
                        }
                        else if (header.ToLower() == "длина периметра")
                        {
                            // Заполняем perimeterLengthTextBox данными из Excel файла
                            perimeterLengthTextBox.Text = worksheet.Cells[2, col].Value?.ToString();
                        }
                        else if (header.ToLower() == "площадь периметра")
                        {
                            // Заполняем perimeterAreaTextBox данными из Excel файла
                            perimeterAreaTextBox.Text = worksheet.Cells[2, col].Value?.ToString();
                        }
                        else if (header.ToLower() == "дата начала")
                        {
                            // Заполняем dateOfBeginTextBox данными из Excel файла
                            dateOfBeginTextBox.Text = worksheet.Cells[2, col].Value?.ToString();
                        }
                        else if (header.ToLower() == "дата окончания")
                        {
                            // Заполняем dateOfEndTextBox данными из Excel файла
                            dateOfEndTextBox.Text = worksheet.Cells[2, col].Value?.ToString();
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
                            else if (headers[i].ToLower() == "длина периметра")
                            {
                                // Заполняем perimeterLengthTextBox данными из текстового файла
                                perimeterLengthTextBox.Text = values[i];
                            }
                            else if (headers[i].ToLower() == "площадь периметра")
                            {
                                // Заполняем perimeterAreaTextBox данными из текстового файла
                                perimeterAreaTextBox.Text = values[i];
                            }
                            else if (headers[i].ToLower() == "дата начала")
                            {
                                // Заполняем dateOfBeginTextBox данными из текстового файла
                                dateOfBeginTextBox.Text = values[i];
                            }
                            else if (headers[i].ToLower() == "дата окончания")
                            {
                                // Заполняем dateOfEndTextBox данными из текстового файла
                                dateOfEndTextBox.Text = values[i];
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

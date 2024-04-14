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
    /// Логика взаимодействия для PicketEditPage.xaml
    /// </summary>
    public partial class PicketEditPage : Page
    {
        private Профиль profile;
        private Пикет picket;
        public PicketEditPage(Профиль profile, Пикет picket)
        {
            InitializeComponent();
            if (picket.id != 0)
            {
                importButton.Visibility = Visibility.Hidden;
                importInfoButton.Visibility = Visibility.Hidden;
            }
            else
            {
                picketCoordinatesButton.Visibility = Visibility.Hidden;
                measurementResultButton.Visibility = Visibility.Hidden;
                intermediateResultsButton.Visibility = Visibility.Hidden;
                commonTransformantsButton.Visibility = Visibility.Hidden;
                intermediateTransformantsButton.Visibility = Visibility.Hidden;
                picketEquipmentPageButton.Visibility = Visibility.Hidden;
                picketEmployeePageButton.Visibility= Visibility.Hidden;
            }
            this.profile = profile;
            this.picket = picket;

            picketTypeComboBox.ItemsSource = DbWorker.GetContext().Виды_пикетов
                .Where(x => x.удален != true)
                .Select(x => x.название).ToList();

            fieldFill();
        }

        private void fieldFill()
        {
            nameTextBox.Text = picket.название_пикета;
            if (picket.Виды_пикетов == null)
            {
                picketTypeComboBox.SelectedIndex = 0;
            }
            else
            {
                picketTypeComboBox.SelectedItem = picket.Виды_пикетов.название;
            }
            resultDateTextBox.Text = picket.дата_и_время_получения_окончательного_результата_измерения.ToString();
            addDateTextBox.Text = picket.дата_добавления_записи.ToString();
            editDateTextBox.Text = picket.дата_последнего_изменения_записи.ToString();
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                picket.название_пикета = nameTextBox.Text;
                picket.id_профиля = profile.id;
                picket.вид_пикета = DbWorker.GetContext().Виды_пикетов
                    .FirstOrDefault(x => x.название == picketTypeComboBox.Text).id;
                picket.дата_и_время_получения_окончательного_результата_измерения = DateTime.Parse(resultDateTextBox.Text);

                if (picket.дата_добавления_записи != null)
                {
                    picket.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                }
                else
                {
                    picket.дата_добавления_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    picket.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    picket.удален = false;
                    DbWorker.GetContext().Пикет.Add(picket);
                }
                DbWorker.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены");
                AppFrame.mainFrame.Navigate(new PicketPage(profile));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: " + ex.Message);
            }
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new PicketPage(profile));
        }

        private void picketTypesButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new PicketTypesCrudPage(picket));
        }

        private void picketCoordinatesButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new PicketCoordinatesCrudPage(picket));
        }

        private void measurementResultPageClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new MeasurementResultCrudPage(picket));
        }

        private void intermediateResultsButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new IntermediateResultsCrudPage(picket));
        }

        private void commonTransformantsButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new CommonTransformantsCrudPage(picket));
        }

        private void intermediateTransformantsButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new IntermediateTransformantsCrudPage(picket));
        }

        private void picketEquipmentPageButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new PicketEquipmentCrudPage(picket));
        }

        private void picketEmployeePageButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new PicketEmployeeCrudPage(picket));
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
                        else if (header.ToLower() == "дата получения результата")
                        {
                            // Заполняем resultDateTextBox данными из Excel файла
                            resultDateTextBox.Text = worksheet.Cells[2, col].Value?.ToString();
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
                            else if (headers[i].ToLower() == "дата получения результата")
                            {
                                // Заполняем resultDateTextBox данными из текстового файла
                                resultDateTextBox.Text = values[i];
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

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
    /// Логика взаимодействия для ProfileEditPage.xaml
    /// </summary>
    public partial class ProfileEditPage : Page
    {
        private Площадь area;
        private Профиль profile;
        public ProfileEditPage(Площадь area, Профиль profile)
        {
            InitializeComponent();
            if (profile.id != 0)
            {
                importButton.Visibility = Visibility.Hidden;
                importInfoButton.Visibility = Visibility.Hidden;
            }
            else
            {
                startCoordinatesButton.Visibility = Visibility.Hidden;
                endCoordinatesButton.Visibility= Visibility.Hidden;
                breakpointsPageButton.Visibility= Visibility.Hidden;
                additionalButtonStackPanel.Visibility = Visibility.Hidden;
            }
            this.area = area;
            this.profile = profile;

            methodologyComboBox.ItemsSource = DbWorker.GetContext().Методика
                .Where(x => x.удален != true)
                .Select(x => x.название_методики).ToList();
            profileHandlerComboBox.ItemsSource = DbWorker.GetContext().Сотрудник
                .Where(x => x.удален != true && x.Должность.название != "Админ")
                .Select(x => x.логин).ToList();
            profileHandlerComboBox.SelectedIndex = 0;

            fieldFill();
        }

        private void fieldFill()
        {
            nameTextBox.Text = profile.название_профиля;
            if (profile.Методика == null)
            {
                methodologyComboBox.SelectedIndex = 0;
            }
            else
            {
                methodologyComboBox.SelectedItem = profile.Методика.название_методики;
            }
            profileLengthTextBox.Text = profile.длина_профиля.ToString();
            dateOfBeginTextBox.Text = profile.дата_начала_работ.ToString();
            dateOfEndTextBox.Text = profile.дата_окончания_работ.ToString();
            addDateTextBox.Text = profile.дата_добавления_записи.ToString();
            editDateTextBox.Text = profile.дата_последнего_изменения_записи.ToString();
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                profile.название_профиля = nameTextBox.Text;
                profile.id_площади = area.id;
                profile.id_методики = DbWorker.GetContext().Методика
                    .FirstOrDefault(x => x.название_методики == methodologyComboBox.Text).id;
                profile.длина_профиля = Double.Parse(profileLengthTextBox.Text);
                profile.дата_начала_работ = DateTime.Parse(dateOfBeginTextBox.Text);
                profile.дата_окончания_работ = DateTime.Parse(dateOfEndTextBox.Text);

                if (profile.дата_добавления_записи != null)
                {
                    profile.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                }
                else
                {
                    profile.дата_добавления_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    profile.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    profile.удален = false;
                    DbWorker.GetContext().Профиль.Add(profile);
                }
                DbWorker.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены");
                AppFrame.mainFrame.Navigate(new ProfilePage(area));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: " + ex.Message);
            }
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProfilePage(area));
        }

        private void startCoordinatesButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProfileStartAndEndCoordinates(profile, 
                ProfileStartAndEndCoordinates.StartOrEndCoordinates.start));
        }

        private void endCoordinatesButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProfileStartAndEndCoordinates(profile,
                ProfileStartAndEndCoordinates.StartOrEndCoordinates.end));
        }

        private void methodologyButtonCLick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new MethodologyCrudPage(profile));
        }

        private void processingPageButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = DbWorker.GetContext().Сотрудник
                .FirstOrDefault(x => x.логин == profileHandlerComboBox.Text);
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                AppFrame.mainFrame.Navigate(new ProfileProcessingPage(profile, selectedElement));
            }
        }

        private void telemetricPageClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new TelemetricPage(profile));
        }

        private void electromagneticMeasurementsPage(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ElectromagneticMeasurementsPage(profile));
        }

        private void breakpointsButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new BreakpointsPage(profile));
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
                        else if (header.ToLower() == "длина профиля")
                        {
                            // Заполняем profileLengthTextBox данными из Excel файла
                            profileLengthTextBox.Text = worksheet.Cells[2, col].Value?.ToString();
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
                            else if (headers[i].ToLower() == "длина профиля")
                            {
                                // Заполняем profileLengthTextBox данными из текстового файла
                                profileLengthTextBox.Text = values[i];
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

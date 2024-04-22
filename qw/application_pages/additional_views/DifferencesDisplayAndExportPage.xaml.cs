using OxyPlot.Series;
using OxyPlot;
using qw.database;
using qw.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using System.IO.Packaging;
using OfficeOpenXml;
using System.IO;

namespace qw.application_pages.additional_views
{
    /// <summary>
    /// Логика взаимодействия для DifferencesDisplayAndExportPage.xaml
    /// </summary>
    public partial class DifferencesDisplayAndExportPage : Page
    {
        private Профиль profile;
        public DifferencesDisplayAndExportPage(Профиль profile)
        {
            InitializeComponent();
            this.profile = profile;
            nameTextBox.Text = profile.название_профиля;
            controlDiffTextBox.Text = differenceWithControl();
            opDiffTextBox.Text = differenceWithOp();
        }

        private string differenceWithControl()
        {
            // список из двух значений: первое - частота, второе - ЭДС. 
            // относительно всего пикета
            List<double> avgFromPrivate = DiscrepancyCalculation.avgOfAllPickets(profile, "Рядовой");
            List<double> avgFromControl = DiscrepancyCalculation.avgOfAllPickets(profile, "Контрольный");

            double privateFreq = avgFromPrivate[0];
            double controlFreq = avgFromControl[0];
            double percentDiffFreq = percentDifference(privateFreq, controlFreq);

            double privateEds = avgFromPrivate[1];
            double controlEds = avgFromControl[1];
            double percentDiffEds = percentDifference(privateEds, controlEds);

            string result = $"ЭДС: {percentDiffEds:F2}%";
            return result;
        }

        private string differenceWithOp()
        {
            // список из двух значений: первое - частота, второе - ЭДС. 
            // относительно всего пикета
            List<double> avgFromPrivate = DiscrepancyCalculation.avgOfAllPickets(profile, "Рядовой");
            List<double> avgFromOp = DiscrepancyCalculation.avgOfAllPickets(profile, "Опытно-методический");

            double privateFreq = avgFromPrivate[0];
            double opFreq = avgFromOp[0];
            double percentDiffFreq = percentDifference(privateFreq, opFreq);

            double privateEds = avgFromPrivate[1];
            double opEds = avgFromOp[1];
            double percentDiffEds = percentDifference(privateEds, opEds);

            string result = $"ЭДС: {percentDiffEds:F2}%";
            return result;
        }

        private double percentDifference(double firstValue, double secondValue)
        {
            return Math.Abs(((secondValue - firstValue) / firstValue) * 100);
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProfileDifferencesGraphPage(profile));
        }

        private void exportButtonClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.FileName = "ExportedData.xlsx";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                ExportToExcel(dialog.FileName);
            }
        }

        private void ExportToExcel(string filePath)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Добавьте ваш код для заполнения Excel файла данными
                    // Добавление заголовка
                    
                    var chartTypes = DbWorker.GetContext().Виды_пикетов
                        .Where(x => x.удален != true)
                        .Select(x => x.название)
                        .ToList();
                    int row = 1;
                    int column = 1;
                    foreach (var chartType in chartTypes)
                    {
                        List<double> allMeasurements = DiscrepancyCalculation.avgOfOnePicketType(profile, chartType);
                        
                        worksheet.Cells[row, column].Value = chartType;
                        row++;
                        worksheet.Cells[row, column].Value = "Частота";
                        column++;
                        worksheet.Cells[row, column].Value = "Значение ЭДС";
                        row++;
                        column--;

                        for (int i = 0; i < allMeasurements.Count; i += 2)
                        {
                            worksheet.Cells[row, column].Value = allMeasurements[i];
                            column++;
                            worksheet.Cells[row, column].Value = allMeasurements[i + 1];
                            row++;
                            column--;
                        }
                        row++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    package.Save();
                }

                MessageBox.Show("Экспорт завершен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при экспорте данных в Excel: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

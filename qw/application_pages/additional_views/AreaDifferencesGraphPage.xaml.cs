using OxyPlot.Axes;
using OxyPlot;
using qw.database;
using qw.util;
using System;
using System.Collections.Generic;
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
using OxyPlot.Legends;
using qw.application_pages.views;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Drawing.Chart;

namespace qw.application_pages.additional_views
{
    /// <summary>
    /// Логика взаимодействия для AreaDifferencesGraphPage.xaml
    /// </summary>
    public partial class AreaDifferencesGraphPage : Page
    {
        private Площадь area;
        public AreaDifferencesGraphPage(Площадь area)
        {
            InitializeComponent();
            this.area = area;
            var chartTypes = DbWorker.GetContext().Виды_пикетов
                .Where(x => x.удален != true)
                .Select(x => x.название)
                .ToList();
            chartTypesCombobox.ItemsSource = chartTypes;
            chartTypesCombobox.SelectedIndex = 0;
        }

        private void graphDisplay()
        {
            var plotModel = GraphModel.divergenceModelArea(area, chartTypesCombobox.SelectedItem.ToString());

            // Создаем ось X и подписываем ее
            var xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Частота" };
            plotModel.Axes.Add(xAxis);

            // Создаем ось Y и подписываем ее
            var yAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Значение ЭДС", AxisTitleDistance = 20 };
            plotModel.Axes.Add(yAxis);

            plotModel.Legends.Add(new Legend()
            {
                LegendTitle = "Легенда",
                LegendPosition = LegendPosition.LeftMiddle,
            });

            plotView.Model = plotModel;
        }

        private void chartTypesCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            graphDisplay();
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            var linkingEntry = DbWorker.GetContext().Проект
                .FirstOrDefault(x => x.id == area.id_проекта);
            AppFrame.mainFrame.Navigate(new AreaPage(linkingEntry));
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

                    List<Профиль> allProfiles = DbWorker.GetContext().Профиль
                        .Where(x => x.удален != true && x.id_площади == area.id)
                        .ToList();

                    int row = 1;
                    int column = 1;
                    foreach (var  profile in allProfiles)
                    {
                        worksheet.Cells[row, column].Value = profile.название_профиля;
                        row++;


                        var chartTypes = DbWorker.GetContext().Виды_пикетов
                        .Where(x => x.удален != true)
                        .Select(x => x.название)
                        .ToList();

                        worksheet.Cells[row, column].Value = profile.название_профиля;
                        row++;
                        worksheet.Cells[row, column].Value = "Частота";
                        column++;
                        worksheet.Cells[row, column].Value = "Значение ЭДС";
                        row++;
                        column--;
                        foreach (var chartType in chartTypes)
                        {

                        }
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

        private string differenceWithControl(Профиль profile)
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

            string result = $"Частота: {percentDiffFreq:F2}%; ЭДС: {percentDiffEds:F2}%";
            return result;
        }

        private string differenceWithOp(Профиль profile)
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

            string result = $"Частота: {percentDiffFreq:F2}%; ЭДС: {percentDiffEds:F2}%";
            return result;
        }

        private double percentDifference(double firstValue, double secondValue)
        {
            return Math.Abs(((secondValue - firstValue) / firstValue) * 100);
        }
    }
}

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using qw.application_pages.views;
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

namespace qw.application_pages.additional_views
{
    /// <summary>
    /// Логика взаимодействия для ProfileDifferencesGraphPage.xaml
    /// </summary>
    public partial class ProfileDifferencesGraphPage : Page
    {
        private Профиль profile;
        public ProfileDifferencesGraphPage(Профиль profile)
        {
            InitializeComponent();
            this.profile = profile;
            var chartTypes = DbWorker.GetContext().Виды_пикетов
                .Where(x => x.удален != true)
                .Select(x => x.название)
                .ToList();
            chartTypes.Add("Все виды");
            chartTypesCombobox.ItemsSource = chartTypes;
            chartTypesCombobox.SelectedIndex = 0;
        }

        private void graphDisplay()
        {
            var plotModel = new PlotModel();

            // Создаем ось X и подписываем ее
            var xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Частота" };
            plotModel.Axes.Add(xAxis);

            // Создаем ось Y и подписываем ее
            var yAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Значение ЭДС", AxisTitleDistance = 20};
            plotModel.Axes.Add(yAxis);

            plotModel.Legends.Add(new Legend()
            {
                LegendTitle = "Легенда",
                LegendPosition = LegendPosition.LeftMiddle,
            });
            if (chartTypesCombobox.SelectedItem.Equals("Все виды"))
            {
                List<string> allTypes = DbWorker.GetContext().Виды_пикетов
                .Where(x => x.удален != true)
                .Select(x => x.название)
                .ToList();
                foreach(var type in allTypes)
                {
                    plotModel.Series.Add(GraphModel.divergenceModelProfile(profile, type));
                }
            }
            else
            {
                plotModel.Series.Add(GraphModel.divergenceModelProfile(profile, chartTypesCombobox.SelectedItem.ToString()));
            }
            plotView.Model = plotModel;
        }

        private void chartTypesCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            graphDisplay();
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            var linkingEntry = DbWorker.GetContext().Площадь
                .FirstOrDefault(x => x.id == profile.id_площади);
            AppFrame.mainFrame.Navigate(new ProfilePage(linkingEntry));
        }

        private void diffDisplayPageButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new DifferencesDisplayAndExportPage(profile));
        }
    }
}

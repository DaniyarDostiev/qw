using OxyPlot;
using OxyPlot.Legends;
using qw.application_pages.edits;
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
    /// Логика взаимодействия для ProfileGraph.xaml
    /// </summary>
    public partial class ProfileGraph : Page
    {
        private Площадь area;
        private Профиль profile;
        public ProfileGraph(Площадь area, Профиль profile)
        {
            InitializeComponent();
            this.area = area;
            this.profile = profile;
            graphDisplay();
        }

        private void graphDisplay()
        {
            var plotModel = new PlotModel();
            plotModel.Legends.Add(new Legend()
            {
                LegendTitle = "Легенда",
                LegendPosition = LegendPosition.LeftMiddle,
            });
            plotModel.Series.Add(GraphModel.areaModel(area));
            plotModel.Series.Add(GraphModel.profileModel(profile));
            plotView.Model = plotModel;

        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProfileEditPage(area, profile));
        }
    }
}

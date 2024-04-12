using qw.application_pages.additional_views;
using qw.application_pages.views;
using qw.database;
using qw.util;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
            }
            this.area = area;
            this.profile = profile;

            methodologyComboBox.ItemsSource = DbWorker.GetContext().Методика.Select(x => x.название_методики).ToList();

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
                //profile.id_методики = DbWorker.GetContext().Методика
                //    .FirstOrDefault(x => x.название_методики == methodologyComboBox.Text).id;
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

        private void importInfoButtonClick(object sender, RoutedEventArgs e)
        {
            new ImportWindow().Show();
        }

        private void importButtonClick(object sender, RoutedEventArgs e)
        {

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
    }
}

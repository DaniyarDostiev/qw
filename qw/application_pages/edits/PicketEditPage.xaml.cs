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
                //startCoordinatesButton.Visibility = Visibility.Hidden;
                // здесь будут кнопки, которые не должны
                // быть видимыми во время добавления новой записи
                
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
                //picket.вид_пикета = DbWorker.GetContext().Виды_пикетов
                //    .FirstOrDefault(x => x.название == picketTypeComboBox.Text).id;
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

        private void importInfoButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void importButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

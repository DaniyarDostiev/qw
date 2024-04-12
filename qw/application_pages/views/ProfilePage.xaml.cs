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

namespace qw.application_pages.views
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private Площадь area;
        public ProfilePage(Площадь area)
        {
            InitializeComponent();

            this.area = area;

            comboBoxSort.Items.Add("Сортировка");
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;
        }

        private void updateElementList()
        {
            List<Профиль> listBoxItems = DbWorker.GetContext().Профиль.
                Where(x => x.название_профиля.Contains(textBoxFinder.Text) && x.удален != true && x.id_площади == area.id).ToList();

            switch (comboBoxSort.SelectedIndex)
            {
                case 0:; break;
                case 1: listBoxItems = listBoxItems.OrderBy(x => x.название_профиля).ToList(); break;
                case 2: listBoxItems = listBoxItems.OrderByDescending(x => x.название_профиля).ToList(); break;
            }

            ListBoxOfEntries.ItemsSource = listBoxItems;
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            Проект project = DbWorker.GetContext().Проект.FirstOrDefault(x => x.id == area.id_проекта);
            AppFrame.mainFrame.Navigate(new AreaPage(project));
        }

        private void textBoxFinderChanged(object sender, TextChangedEventArgs e)
        {
            updateElementList();
        }

        private void comboBoxSortChanged(object sender, SelectionChangedEventArgs e)
        {
            updateElementList();
        }

        private void showDeletedEntries()
        {
            ListBoxOfEntries.ItemsSource = DbWorker.GetContext().Профиль.Where(x => x.удален == true).ToList();
        }

        private void deletedEntriesButtonClick(object sender, RoutedEventArgs e)
        {
            crudButtonStackPanel.Visibility = Visibility.Hidden;
            deletedEntriesButtonStackPanel.Visibility = Visibility.Visible;
            showDeletedEntries();
        }

        private void closeDeletedEntriesButtonClick(object sender, RoutedEventArgs e)
        {
            crudButtonStackPanel.Visibility = Visibility.Visible;
            deletedEntriesButtonStackPanel.Visibility = Visibility.Hidden;
            updateElementList();
        }

        private void recoverEntryButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Профиль;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите восстановить запись?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    selectedElement.удален = false;
                    DbWorker.GetContext().SaveChanges();
                    showDeletedEntries();
                }
            }
        }

        private void editButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Профиль;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                AppFrame.mainFrame.Navigate(new ProfileEditPage(area, selectedElement));
            }
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProfileEditPage(area, new Профиль()));
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Профиль;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    selectedElement.удален = true;
                    DbWorker.GetContext().SaveChanges();
                    updateElementList();
                }
            }
        }

        private void nextButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

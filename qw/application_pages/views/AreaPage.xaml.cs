using qw.application_pages.additional_views;
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
    /// Логика взаимодействия для AreaPage.xaml
    /// </summary>
    public partial class AreaPage : Page
    {
        private Проект project;
        public AreaPage(Проект project)
        {
            InitializeComponent();

            this.project = project;

            comboBoxSort.Items.Add("Сортировка");
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;
        }

        private void updateElementList()
        {
            List<Площадь> listBoxItems = DbWorker.GetContext().Площадь.
                Where(x => x.название.Contains(textBoxFinder.Text) && x.удален != true && x.id_проекта == project.id).ToList();

            switch (comboBoxSort.SelectedIndex)
            {
                case 0:; break;
                case 1: listBoxItems = listBoxItems.OrderBy(x => x.название).ToList(); break;
                case 2: listBoxItems = listBoxItems.OrderByDescending(x => x.название).ToList(); break;
            }

            ListBoxOfEntries.ItemsSource = listBoxItems;
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            Заказчик customer = DbWorker.GetContext().Заказчик.FirstOrDefault(x => x.id == project.id_заказчика);
            AppFrame.mainFrame.Navigate(new ProjectPage(customer));
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
            ListBoxOfEntries.ItemsSource = DbWorker.GetContext().Площадь.
                Where(x => x.удален == true && x.id_проекта == project.id).ToList();
        }

        private void deletedEntriesButtonClick(object sender, RoutedEventArgs e)
        {
            crudButtonStackPanel.Visibility = Visibility.Hidden;
            deletedEntriesButtonStackPanel.Visibility = Visibility.Visible;
            navigationStackPanel.Visibility = Visibility.Hidden;
            nextButton.Visibility = Visibility.Hidden;
            graphButton.Visibility = Visibility.Hidden;
            showDeletedEntries();
        }

        private void closeDeletedEntriesButtonClick(object sender, RoutedEventArgs e)
        {
            crudButtonStackPanel.Visibility = Visibility.Visible;
            deletedEntriesButtonStackPanel.Visibility = Visibility.Hidden;
            navigationStackPanel.Visibility = Visibility.Visible;
            nextButton.Visibility = Visibility.Visible;
            graphButton.Visibility = Visibility.Visible;
            updateElementList();
        }

        private void recoverEntryButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Площадь;
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
            var selectedElement = ListBoxOfEntries.SelectedItem as Площадь;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                AppFrame.mainFrame.Navigate(new AreaEditPage(project, selectedElement));
            }
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new AreaEditPage(project, new Площадь()));
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Площадь;
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
            var selectedElement = ListBoxOfEntries.SelectedItem as Площадь;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                AppFrame.mainFrame.Navigate(new ProfilePage(selectedElement));
            }
        }

        private void graphButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Площадь;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                AppFrame.mainFrame.Navigate(new AreaDifferencesGraphPage(selectedElement));
            }
        }
    }
}

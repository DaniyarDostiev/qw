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
    /// Логика взаимодействия для ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
        private Заказчик customer;
        public ProjectPage(Заказчик customer)
        {
            InitializeComponent();

            this.customer = customer;

            comboBoxSort.Items.Add("Сортировка");
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;

            /* важно инициализацию полей ставить перед 
             * заполнением textbox и combobox чтобы все 
             * поля используемые в методе update были заранее присвоены
             */
        }

        private void updateElementList()
        {
            List<Проект> listBoxItems = DbWorker.GetContext().Проект.
                Where(x => x.название.Contains(textBoxFinder.Text) && x.удален != true && x.id_заказчика == customer.id).ToList();

            switch (comboBoxSort.SelectedIndex)
            {
                case 0:; break;
                case 1: listBoxItems = listBoxItems.OrderBy(x => x.название).ToList(); break;
                case 2: listBoxItems = listBoxItems.OrderByDescending(x => x.название).ToList(); break;
            }

            ListBoxOfEntries.ItemsSource = listBoxItems;
        }

        private void textBoxFinderChanged(object sender, TextChangedEventArgs e)
        {
            updateElementList();
        }

        private void comboBoxSortChanged(object sender, SelectionChangedEventArgs e)
        {
            updateElementList();
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new Customer());
        }
        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Проект;
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

        private void editButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Проект;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                AppFrame.mainFrame.Navigate(new ProjectEditPage(customer, selectedElement));
            }
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProjectEditPage(customer, new Проект()));
        }

        private void showDeletedEntries()
        {
            ListBoxOfEntries.ItemsSource = DbWorker.GetContext().Проект.Where(x => x.удален == true).ToList();
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
            var selectedElement = ListBoxOfEntries.SelectedItem as Проект;
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

        private void nextButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

using qw.application_pages.views;
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

namespace qw.application_pages.is_deleted
{
    /// <summary>
    /// Логика взаимодействия для DeletedCustomers.xaml
    /// </summary>
    public partial class DeletedCustomers : Page
    {
        public DeletedCustomers()
        {
            InitializeComponent();

            comboBoxSort.Items.Add("Сортировка");
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;
        }

        private void updateElementList()
        {
            List<Заказчик> customers = DbWorker.GetContext().Заказчик.
                Where(x => x.имя.Contains(textBoxFinder.Text) && x.удален != false).ToList();

            switch (comboBoxSort.SelectedIndex)
            {
                case 0:; break;
                case 1: customers = customers.OrderBy(x => x.имя).ToList(); break;
                case 2: customers = customers.OrderByDescending(x => x.имя).ToList(); break;
            }

            ListBoxCustomers.ItemsSource = customers;
        }

        private void comboBoxSortChanged(object sender, SelectionChangedEventArgs e)
        {
            updateElementList();
        }

        private void textBoxFinderChanged(object sender, TextChangedEventArgs e)
        {
            updateElementList();
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new Customer());
        }

        private void recoverEntryButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxCustomers.SelectedItem as Заказчик;
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
                    AppFrame.mainFrame.Navigate(new Customer());
                }
            }
        }
    }
}


using qw.application_pages.edits;
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

namespace qw.application_pages.additional_views
{
    /// <summary>
    /// Логика взаимодействия для EquipmentCrudPage.xaml
    /// </summary>
    public partial class EquipmentCrudPage : Page
    {
        private Профиль profile;
        public EquipmentCrudPage(Профиль profile)
        {
            InitializeComponent();
            this.profile = profile;
            showNonDeletedEntries();
        }

        private void showNonDeletedEntries()
        {
            dataGridOfEntries.ItemsSource = DbWorker.GetContext().Измерительное_оборудование.Where(x => x.удален != true).ToList();
        }

        private void saveChanges()
        {
            try
            {
                var selectedItem = dataGridOfEntries.SelectedItem as Измерительное_оборудование;
                if (selectedItem != null)
                {
                    if (selectedItem.дата_добавления_записи != null)
                    {
                        selectedItem.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    }
                    else
                    {
                        selectedItem.дата_добавления_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                        selectedItem.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                        selectedItem.удален = false;
                    }

                    // Добавляем новый элемент в контекст Entity Framework, если он не был добавлен ранее
                    if (DbWorker.GetContext().Измерительное_оборудование.Local.Contains(selectedItem) == false)
                    {
                        DbWorker.GetContext().Измерительное_оборудование.Add(selectedItem);
                    }

                    DbWorker.GetContext().SaveChanges();
                    int selectedString = dataGridOfEntries.SelectedIndex;//Сохраняем индекс текущей выделенной строки
                    showNonDeletedEntries();
                    dataGridOfEntries.SelectedIndex = selectedString; //Выделяем строку
                }
                else
                {
                    MessageBox.Show("выберите элемент из списка");
                }

            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");
                showNonDeletedEntries();
            }
        }

        private void showDeletedEntries()
        {
            dataGridOfEntries.ItemsSource = DbWorker.GetContext().Измерительное_оборудование.Where(x => x.удален == true).ToList();
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new MethodologyCrudPage(profile));
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Измерительное_оборудование;
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
                    showNonDeletedEntries();
                }
            }
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            saveChanges();
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
            showNonDeletedEntries();
        }

        private void recoverEntryButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Измерительное_оборудование;
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
    }
}

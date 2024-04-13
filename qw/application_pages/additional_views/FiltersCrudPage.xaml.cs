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
    /// Логика взаимодействия для FiltersCrudPage.xaml
    /// </summary>
    public partial class FiltersCrudPage : Page
    {
        private Обработки_на_профиле processing;
        private Профиль profile;
        private Сотрудник employee;
        public FiltersCrudPage(Обработки_на_профиле processing, Профиль profile, Сотрудник employee)
        {
            InitializeComponent();

            this.processing = processing;
            this.profile = profile;
            this.employee = employee;

            showNonDeletedEntries();
        }

        private void showNonDeletedEntries()
        {
            List<Фильтры_обработок> allEntries = DbWorker.GetContext().Список_фильтров_над_обработкой
                .Where(x => x.id_обработки_на_профиле == processing.id)
                .Select(x => x.Фильтры_обработок)
                .Where(x => x.удален != true)
                .ToList();

            dataGridOfEntries.ItemsSource = allEntries;
        }

        private void saveChanges()
        {
            try
            {
                var selectedItem = dataGridOfEntries.SelectedItem as Фильтры_обработок;
                if (selectedItem != null)
                {
                    if (selectedItem.дата_добавления_записи != null)
                    {
                        // изменение сущесвтующей записи
                        selectedItem.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    }
                    else
                    {
                        // добавление новой записи
                        selectedItem.дата_добавления_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                        selectedItem.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                        selectedItem.удален = false;

                        var linkingEntry = new Список_фильтров_над_обработкой();
                        linkingEntry.Обработки_на_профиле = processing;
                        linkingEntry.Фильтры_обработок = selectedItem;
                        DbWorker.GetContext().Список_фильтров_над_обработкой.Add(linkingEntry);
                    }

                    // Добавляем новый элемент в контекст Entity Framework, если он не был добавлен ранее
                    if (DbWorker.GetContext().Фильтры_обработок.Local.Contains(selectedItem) == false)
                    {
                        DbWorker.GetContext().Фильтры_обработок.Add(selectedItem);
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
            List<Фильтры_обработок> allEntries = DbWorker.GetContext().Список_фильтров_над_обработкой
                .Where(x => x.id_обработки_на_профиле == processing.id)
                .Select(x => x.Фильтры_обработок)
                .Where(x => x.удален == true)
                .ToList();

            dataGridOfEntries.ItemsSource = allEntries;
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new ProfileProcessingPage(profile, employee));
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Фильтры_обработок;
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
            var selectedElement = dataGridOfEntries.SelectedItem as Фильтры_обработок;
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

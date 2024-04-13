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
    /// Логика взаимодействия для TelemetricPage.xaml
    /// </summary>
    public partial class TelemetricPage : Page
    {
        private Профиль profile;
        public TelemetricPage(Профиль profile)
        {
            InitializeComponent();
            this.profile = profile;
            showNonDeletedEntries();
        }

        private void showNonDeletedEntries()
        {
            List<Телеметрические_измерения> allEntries = DbWorker.GetContext().Профиль_ТелеметрическиеИзмерения
                .Where(x => x.id_профиля == profile.id)
                .Select(x => x.Телеметрические_измерения)
                .Where(x => x.удален != true)
                .ToList();

            dataGridOfEntries.ItemsSource = allEntries;
        }

        private void saveChanges()
        {
            try
            {
                var selectedItem = dataGridOfEntries.SelectedItem as Телеметрические_измерения;
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

                        var linkingEntry = new Профиль_ТелеметрическиеИзмерения();
                        linkingEntry.Профиль = profile;
                        linkingEntry.Телеметрические_измерения = selectedItem;
                        DbWorker.GetContext().Профиль_ТелеметрическиеИзмерения.Add(linkingEntry);
                    }

                    // Добавляем новый элемент в контекст Entity Framework, если он не был добавлен ранее
                    if (DbWorker.GetContext().Телеметрические_измерения.Local.Contains(selectedItem) == false)
                    {
                        DbWorker.GetContext().Телеметрические_измерения.Add(selectedItem);
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
            List<Телеметрические_измерения> allEntries = DbWorker.GetContext().Профиль_ТелеметрическиеИзмерения
                .Where(x => x.id_профиля == profile.id)
                .Select(x => x.Телеметрические_измерения)
                .Where(x => x.удален == true)
                .ToList();

            dataGridOfEntries.ItemsSource = allEntries;
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            var linkingEntry = DbWorker.GetContext().Площадь.FirstOrDefault(x => x.id == profile.id_площади);
            AppFrame.mainFrame.Navigate(new ProfileEditPage(linkingEntry, profile));
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Телеметрические_измерения;
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
            var selectedElement = dataGridOfEntries.SelectedItem as Телеметрические_измерения;
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

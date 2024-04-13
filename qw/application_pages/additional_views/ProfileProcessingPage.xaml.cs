﻿using qw.application_pages.edits;
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
    /// Логика взаимодействия для ProfileProcessingPage.xaml
    /// </summary>
    public partial class ProfileProcessingPage : Page
    {
        private Профиль profile;
        private Сотрудник employee;
        public ProfileProcessingPage(Профиль profile, Сотрудник employee)
        {
            InitializeComponent();

            this.profile = profile;
            this.employee = employee;

            showNonDeletedEntries();
        }

        private void showNonDeletedEntries()
        {
            List<Обработки_на_профиле> allProcessing = DbWorker.GetContext().Профиль_СписокОбработок
                .Where(x => x.id_профиля == profile.id && x.id_обработчика == employee.id)
                .Select(x => x.Обработки_на_профиле)
                .Where(x => x.удален != true)
                .ToList();

            dataGridOfEntries.ItemsSource = allProcessing;
        }

        private void saveChanges()
        {
            try
            {
                var selectedItem = dataGridOfEntries.SelectedItem as Обработки_на_профиле;
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

                        var linkingEntry = new Профиль_СписокОбработок();
                        linkingEntry.Профиль = profile;
                        linkingEntry.Сотрудник = employee;
                        linkingEntry.Обработки_на_профиле = selectedItem;
                        DbWorker.GetContext().Профиль_СписокОбработок.Add(linkingEntry);
                    }

                    // Добавляем новый элемент в контекст Entity Framework, если он не был добавлен ранее
                    if (DbWorker.GetContext().Обработки_на_профиле.Local.Contains(selectedItem) == false)
                    {
                        DbWorker.GetContext().Обработки_на_профиле.Add(selectedItem);
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
            List<Обработки_на_профиле> allProcessing = DbWorker.GetContext().Профиль_СписокОбработок
                .Where(x => x.id_профиля == profile.id && x.id_обработчика == employee.id)
                .Select(x => x.Обработки_на_профиле)
                .Where(x => x.удален == true)
                .ToList();

            dataGridOfEntries.ItemsSource = allProcessing;
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            var linkingEntry = DbWorker.GetContext().Площадь.FirstOrDefault(x => x.id == profile.id_площади);
            AppFrame.mainFrame.Navigate(new ProfileEditPage(linkingEntry, profile));
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Обработки_на_профиле;
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
            var selectedElement = dataGridOfEntries.SelectedItem as Обработки_на_профиле;
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

        private void filtersPageButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Обработки_на_профиле;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                if (DbWorker.GetContext().Обработки_на_профиле.Local.Contains(selectedElement) == false)
                {
                    MessageBox.Show("выбранный элемент не был сохранен");
                }
                else
                {
                    AppFrame.mainFrame.Navigate(new FiltersCrudPage(selectedElement, profile, employee));
                }
            }
        }
    }
}

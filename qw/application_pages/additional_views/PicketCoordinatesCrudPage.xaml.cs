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
using static qw.application_pages.additional_views.ProfileStartAndEndCoordinates;

namespace qw.application_pages.additional_views
{
    /// <summary>
    /// Логика взаимодействия для PicketCoordinatesCrudPage.xaml
    /// </summary>
    public partial class PicketCoordinatesCrudPage : Page
    {
        private Пикет picket;
        public PicketCoordinatesCrudPage(Пикет picket)
        {
            InitializeComponent();
            this.picket = picket;
            showNonDeletedEntries();
        }

        private void showNonDeletedEntries()
        {
            dataGridOfEntries.ItemsSource = DbWorker.GetContext().Координаты_точки
                .Where(x => x.id == picket.id_координат_нахождения && x.удален != true).ToList();

        }

        private void saveChanges()
        {
            try
            {
                var selectedItem = dataGridOfEntries.SelectedItem as Координаты_точки;
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

                        // пикет по отношению, к которому производятся все операции
                        var linkingEntry = picket;
                        var buffer = linkingEntry.id_координат_нахождения;
                        linkingEntry.id_координат_нахождения = selectedItem.id;

                        // если при добавлении новых координат, к профилю уже были привязаны другие координаты
                        // старые координаты необходимо удалить
                        if (buffer != null)
                        {
                            var oldCoordinates = DbWorker.GetContext().Координаты_точки
                                .FirstOrDefault(x => x.id == buffer);
                            DbWorker.GetContext().Координаты_точки.Remove(oldCoordinates);
                        }
                    }

                    // Добавляем новый элемент в контекст Entity Framework, если он не был добавлен ранее
                    if (DbWorker.GetContext().Координаты_точки.Local.Contains(selectedItem) == false)
                    {
                        // добавление точки, которой до этого не было в контексте и в БД
                        DbWorker.GetContext().Координаты_точки.Add(selectedItem);
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
            dataGridOfEntries.ItemsSource = DbWorker.GetContext().Координаты_точки
                .Where(x => x.id == picket.id_координат_нахождения && x.удален == true).ToList();
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            var linkingEntry = DbWorker.GetContext().Профиль.FirstOrDefault(x => x.id == picket.id_профиля);
            AppFrame.mainFrame.Navigate(new PicketEditPage(linkingEntry, picket));
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Координаты_точки;
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
            var selectedElement = dataGridOfEntries.SelectedItem as Координаты_точки;
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
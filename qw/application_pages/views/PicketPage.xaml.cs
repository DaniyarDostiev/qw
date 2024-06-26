﻿using qw.application_pages.edits;
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
    /// Логика взаимодействия для PicketPage.xaml
    /// </summary>
    public partial class PicketPage : Page
    {
        private Профиль profile;
        public PicketPage(Профиль profile)
        {
            InitializeComponent();

            this.profile = profile;

            comboBoxSort.Items.Add("Сортировка");
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;
        }

        private void updateElementList()
        {
            List<Пикет> listBoxItems = DbWorker.GetContext().Пикет.
                Where(x => x.название_пикета.Contains(textBoxFinder.Text) && x.удален != true && x.id_профиля == profile.id).ToList();

            switch (comboBoxSort.SelectedIndex)
            {
                case 0:; break;
                case 1: listBoxItems = listBoxItems.OrderBy(x => x.название_пикета).ToList(); break;
                case 2: listBoxItems = listBoxItems.OrderByDescending(x => x.название_пикета).ToList(); break;
            }

            ListBoxOfEntries.ItemsSource = listBoxItems;
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            var linkingEntry = DbWorker.GetContext().Площадь.FirstOrDefault(x => x.id == profile.id_площади);
            AppFrame.mainFrame.Navigate(new ProfilePage(linkingEntry));
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
            List<Пикет> listBoxItems = DbWorker.GetContext().Пикет.
                Where(x => x.удален == true && x.id_профиля == profile.id).ToList();
            ListBoxOfEntries.ItemsSource = listBoxItems;
        }

        private void deletedEntriesButtonClick(object sender, RoutedEventArgs e)
        {
            crudButtonStackPanel.Visibility = Visibility.Hidden;
            deletedEntriesButtonStackPanel.Visibility = Visibility.Visible;
            navigationStackPanel.Visibility = Visibility.Hidden;
            showDeletedEntries();
        }

        private void closeDeletedEntriesButtonClick(object sender, RoutedEventArgs e)
        {
            crudButtonStackPanel.Visibility = Visibility.Visible;
            deletedEntriesButtonStackPanel.Visibility = Visibility.Hidden;
            navigationStackPanel.Visibility = Visibility.Visible;
            updateElementList();
        }

        private void recoverEntryButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Пикет;
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
            var selectedElement = ListBoxOfEntries.SelectedItem as Пикет;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                AppFrame.mainFrame.Navigate(new PicketEditPage(profile, selectedElement));
            }
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new PicketEditPage(profile, new Пикет()));
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxOfEntries.SelectedItem as Пикет;
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
    }
}

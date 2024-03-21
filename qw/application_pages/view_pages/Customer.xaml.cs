﻿using qw.application_pages.edit_pages;
using qw.database;
using qw.util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace qw.application_frames.view_frames
{
    /// <summary>
    /// Логика взаимодействия для Customer.xaml
    /// </summary>
    public partial class Customer : Page
    {
        
        public Customer()
        {
            InitializeComponent();

            comboBoxSort.Items.Add("Сортировка");
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;
        }

        private void updateElementList()
        {
            List<Заказчик> customers = DbWorker.GetContext().Заказчик.Where(x => x.имя.Contains(textBoxFinder.Text)).ToList();

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

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxCustomers.SelectedItem as Заказчик;
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
                    DbWorker.GetContext().Заказчик.Remove(selectedElement);
                    DbWorker.GetContext().SaveChanges();
                    updateElementList();
                }
            }
                
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new CustomerEditPage(new Заказчик()));
        }

        private void editButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = ListBoxCustomers.SelectedItem as Заказчик;
            if (selectedElement == null)
            {
                MessageBox.Show("выберите элемент из списка");
            }
            else
            {
                AppFrame.mainFrame.Navigate(new CustomerEditPage(selectedElement));
            }
        }

        private void projectsButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

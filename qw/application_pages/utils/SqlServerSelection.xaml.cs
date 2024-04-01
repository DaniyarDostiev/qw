﻿using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data;
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
using qw.util;
using System.Configuration;
using qw.application_pages.views;
using System.Text.RegularExpressions;

namespace qw.application_pages.utils
{
    /// <summary>
    /// Логика взаимодействия для SqlServerSelection.xaml
    /// </summary>
    public partial class SqlServerSelection : Page
    {
        public SqlServerSelection()
        {
            InitializeComponent();
            populateServerList();
            serverComboBox.SelectedIndex = 0;
        }

        private void populateServerList()
        {
            try
            {
                DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();

                foreach (DataRow row in servers.Rows)
                {
                    string serverName = row["ServerName"].ToString();
                    string instanceName = row["InstanceName"].ToString();

                    // Если у сервера есть инстанс, добавляем его к имени сервера
                    if (!string.IsNullOrEmpty(instanceName))
                        serverName += "\\" + instanceName;

                    serverComboBox.Items.Add(serverName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении списка серверов: " + ex.Message);
            }
        }

        private void chooseSqlServerButtonClick(object sender, RoutedEventArgs e)
        {
            string selectedServer = serverComboBox.SelectedItem as string;
            if (selectedServer != null)
            {
                saveConfigWithSelectedSqlServer(selectedServer);
                AppFrame.mainFrame.Navigate(new AuthPage());
            }
            else
            {
                MessageBox.Show("выберите элемент из списка");
            }
        }

        private void saveConfigWithSelectedSqlServer(String dataSource)
        {
            // Получаем текущую строку подключения из app.config
            string connectionString = ConfigurationManager.ConnectionStrings["qw2Entities"].ConnectionString;

            // Заменяем значение data source на новое значение
            string pattern = @"data source=.*?;";
            string newConnectionString = Regex.Replace(connectionString, pattern,
                $"data source={dataSource};");

            // Обновляем значение в файле app.config
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["qw2Entities"].ConnectionString = newConnectionString;
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        private void logoutButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new AuthPage());
        }
    }
}
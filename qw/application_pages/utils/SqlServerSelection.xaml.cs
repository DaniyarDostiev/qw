using System;
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

            // Загрузка последнего введенного SQL сервера при запуске приложения
            string lastSqlServer = ConfigurationManager.AppSettings["LastSqlServer"];
            if (!string.IsNullOrEmpty(lastSqlServer))
            {
                // Вставка последнего введенного SQL сервера в соответствующее поле
                serverTextBox.Text = lastSqlServer;
            }
        }

        private void chooseSqlServerButtonClick(object sender, RoutedEventArgs e)
        {
            string selectedServer = serverTextBox.Text;
            if (selectedServer != null)
            {
                saveConfigWithSelectedSqlServer(selectedServer);
                saveLastSqlServer(selectedServer);
                AppFrame.mainFrame.Navigate(new AuthPage());
            }
            else
            {
                MessageBox.Show("выберите элемент из списка");
            }
        }

        private void saveLastSqlServer(string sqlServer)
        {
            string lastSqlServer = ConfigurationManager.AppSettings["LastSqlServer"];
            if (!lastSqlServer.Equals(sqlServer))
            {
                // Сохранение последнего введенного SQL сервера в конфигурацию
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["LastSqlServer"].Value = sqlServer;
                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("appSettings");
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
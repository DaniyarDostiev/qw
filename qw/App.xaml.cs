using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Linq;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using System.Windows;

namespace qw
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static List<string> listOfServers = new List<string>();

        // Метод, который вызывается при запуске приложения
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();

                foreach (DataRow row in servers.Rows)
                {
                    string serverName = row["ServerName"].ToString();
                    string instanceName = row["InstanceName"].ToString();

                    // Если у сервера есть инстанс, добавляем его к имени сервера
                    if (!string.IsNullOrEmpty(instanceName))
                    {
                        serverName += "\\" + instanceName;
                        listOfServers.Add(serverName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении списка серверов: " + ex.Message);
            }
        }

        // Метод, который вызывается при закрытии приложения
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Очистка глобальной переменной приложения перед закрытием
            listOfServers = null;
        }
    }
}

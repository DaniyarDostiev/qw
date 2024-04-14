using qw.application_pages.additional_views;
using qw.application_pages.views;
using qw.util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

namespace qw.application_pages.utils
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void loginClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (loginTextbox.Text == "" || passwordTextbox.Text == "")
                {
                    MessageBox.Show("Заполните поля");
                }
                else
                {
                    var userObj = DbWorker.GetContext().Сотрудник
                        .Where(x => x.удален != true)
                        .FirstOrDefault(x => x.логин == loginTextbox.Text && x.пароль == passwordTextbox.Text);

                    if (userObj == null)
                    {
                        MessageBox.Show("Данные не верны, попробуйте ещё раз");
                    }
                    else
                    {
                        loginTextbox.Text = null;
                        passwordTextbox.Text = null;

                        if (userObj.Должность.название.Equals("Админ"))
                        {
                            AppFrame.mainFrame.Navigate(new EmployeeCrudPage(userObj));
                        }
                        else
                        {
                            AppFrame.mainFrame.Navigate(new Customer());
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            restartApplication();
            AppFrame.mainFrame.Navigate(new SqlServerSelection());
        }

        private void restartApplication()
        {
            // Получаем путь к текущему исполняемому файлу приложения
            string appPath = Assembly.GetEntryAssembly().Location;

            // Перезапускаем приложение
            Process.Start(new ProcessStartInfo
            {
                FileName = appPath,
                UseShellExecute = true
            });

            // Закрываем текущее приложение
            Application.Current.Shutdown();
        }
    }
}

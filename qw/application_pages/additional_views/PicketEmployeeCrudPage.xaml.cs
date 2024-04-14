using qw.application_pages.edits;
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

namespace qw.application_pages.additional_views
{
    /// <summary>
    /// Логика взаимодействия для PicketEmployeeCrudPage.xaml
    /// </summary>
    public partial class PicketEmployeeCrudPage : Page
    {
        private Пикет picket;
        public PicketEmployeeCrudPage(Пикет picket)
        {
            InitializeComponent();
            this.picket = picket;

            employeeComboBox.ItemsSource = DbWorker.GetContext().Сотрудник
                .Where(x => x.удален != true && x.Должность.название != "Админ")
                .Select(x => x.логин)
                .ToList();
            employeeComboBox.SelectedIndex = 0;

            showEntries();
        }

        private void showEntries()
        {
            List<Сотрудник> allEntries = DbWorker.GetContext().Пикет_Сотрудники
                .Where(x => x.id_пикета == picket.id)
                .Select(x => x.Сотрудник)
                .Where(x => x.удален != true)
                .ToList();

            dataGridOfEntries.ItemsSource = allEntries;
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = DbWorker.GetContext().Сотрудник
                .FirstOrDefault(x => x.логин == employeeComboBox.Text);
            var checkEmployeesOnPicket = DbWorker.GetContext().Пикет_Сотрудники
                .FirstOrDefault(x => x.id_сотрудника == selectedElement.id);

            if (checkEmployeesOnPicket == null)
            {
                var linkingEntry = new Пикет_Сотрудники();
                linkingEntry.Пикет = picket;
                linkingEntry.Сотрудник = selectedElement;
                DbWorker.GetContext().Пикет_Сотрудники.Add(linkingEntry);
                DbWorker.GetContext().SaveChanges();
            }
            else
            {
                MessageBox.Show("Сотрудник уже добавлен");
            }

            showEntries();
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Сотрудник;
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
                    var linkingEntry = DbWorker.GetContext().Пикет_Сотрудники
                        .FirstOrDefault(x => x.id_сотрудника == selectedElement.id);
                    DbWorker.GetContext().Пикет_Сотрудники.Remove(linkingEntry);
                    DbWorker.GetContext().SaveChanges();
                    showEntries();
                }
            }
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            var linkingEntry = DbWorker.GetContext().Профиль.FirstOrDefault(x => x.id == picket.id_профиля);
            AppFrame.mainFrame.Navigate(new PicketEditPage(linkingEntry, picket));
        }
    }
}

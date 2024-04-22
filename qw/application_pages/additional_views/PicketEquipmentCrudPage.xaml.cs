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
    /// Логика взаимодействия для PicketEquipmentCrudPage.xaml
    /// </summary>
    public partial class PicketEquipmentCrudPage : Page
    {
        private Пикет picket;
        public PicketEquipmentCrudPage(Пикет picket)
        {
            InitializeComponent();
            this.picket = picket;

            entriesComboBox.ItemsSource = DbWorker.GetContext().Измерительное_оборудование
                .Where(x => x.удален != true)
                .Select(x => x.название_оборудования)
                .ToList();
            entriesComboBox.SelectedIndex = 0;

            showEntries();
        }

        private void showEntries()
        {
            List<Измерительное_оборудование> allEntries = DbWorker.GetContext().Пикет_ИзмерительноеОборудование
                .Where(x => x.id_пикета == picket.id)
                .Select(x => x.Измерительное_оборудование)
                .Where(x => x.удален != true)
                .ToList();

            dataGridOfEntries.ItemsSource = allEntries;
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = DbWorker.GetContext().Измерительное_оборудование
                .FirstOrDefault(x => x.название_оборудования == entriesComboBox.Text);
            var checkEquipmentsOnPicket = DbWorker.GetContext().Пикет_ИзмерительноеОборудование
                .FirstOrDefault(x => x.id_измерительного_оборудования == selectedElement.id && x.id_пикета == picket.id);

            if (checkEquipmentsOnPicket == null)
            {
                var linkingEntry = new Пикет_ИзмерительноеОборудование();
                linkingEntry.Пикет = picket;
                linkingEntry.Измерительное_оборудование = selectedElement;
                DbWorker.GetContext().Пикет_ИзмерительноеОборудование.Add(linkingEntry);
                DbWorker.GetContext().SaveChanges();
            }
            else
            {
                MessageBox.Show("Оборудование уже добавлено");
            }

            showEntries();
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedElement = dataGridOfEntries.SelectedItem as Измерительное_оборудование;
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
                    var linkingEntry = DbWorker.GetContext().Пикет_ИзмерительноеОборудование
                        .FirstOrDefault(x => x.id_измерительного_оборудования == selectedElement.id);
                    DbWorker.GetContext().Пикет_ИзмерительноеОборудование.Remove(linkingEntry);
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

        private void equipmentPageButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new EquipmentCrudPage(picket));
        }
    }
}

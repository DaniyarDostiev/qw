using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using qw.application_pages.edits;
using qw.database;
using qw.util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для AreaPerimeterAngles.xaml
    /// </summary>
    public partial class AreaPerimeterAngles : Page
    {
        private Площадь area;
        public AreaPerimeterAngles(Площадь area)
        {
            InitializeComponent();

            this.area = area;

            showNonDeletedEntries();
            graphDisplay();
        }

        private void graphDisplay()
        {
            // Создаем модель графика
            var plotModel = new PlotModel();

            // Добавляем легенду
            plotModel.Legends.Add(new Legend()
            {
                LegendTitle = "Легенда",
                LegendPosition = LegendPosition.LeftMiddle,
            });

            // Создаем серию данных
            plotModel.Series.Add(GraphModel.areaModel(area));

            // Привязываем модель к PlotView для отображения
            plotView.Model = plotModel;
        }

        private void showNonDeletedEntries()
        {
            // Запрос для выбора всех координат точек для определенной площади
            List<Координаты_точки> allPointCoordinates = DbWorker.GetContext().Площадь_УглыПериметра
                .Where(x => x.id_площади == area.id)
                .Select(x => x.Координаты_точки)
                .Where(x => x.удален != true)
                .ToList();

            dataGridOfEntries.ItemsSource = allPointCoordinates;
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

                        var linkingEntry = new Площадь_УглыПериметра();
                        linkingEntry.Площадь = area;
                        linkingEntry.Координаты_точки = selectedItem;
                        DbWorker.GetContext().Площадь_УглыПериметра.Add(linkingEntry);
                    }

                    // Добавляем новый элемент в контекст Entity Framework, если он не был добавлен ранее
                    if (DbWorker.GetContext().Координаты_точки.Local.Contains(selectedItem) == false)
                    {
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
            var allPointCoordinates = DbWorker.GetContext().Площадь_УглыПериметра
                .Where(x => x.id_площади == area.id)
                .Select(x => x.Координаты_точки)
                .Where(x => x.удален == true)
                .ToList();

            dataGridOfEntries.ItemsSource = allPointCoordinates;
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            Проект project = DbWorker.GetContext().Проект.FirstOrDefault(x => x.id == area.id_проекта);
            AppFrame.mainFrame.Navigate(new AreaEditPage(project, area));
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
                    graphDisplay();
                }
            }
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            saveChanges();
            graphDisplay();
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
                    graphDisplay();
                }
            }
        }
    }
}

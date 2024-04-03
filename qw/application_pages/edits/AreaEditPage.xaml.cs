using qw.application_pages.additional_views;
using qw.application_pages.views;
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

namespace qw.application_pages.edits
{
    /// <summary>
    /// Логика взаимодействия для AreaEditPage.xaml
    /// </summary>
    public partial class AreaEditPage : Page
    {
        private Проект project;
        private Площадь area;
        public AreaEditPage(Проект prject, Площадь area)
        {
            InitializeComponent();
            if (area.id != 0)
            {
                importButton.Visibility = Visibility.Hidden;
                importInfoButton.Visibility = Visibility.Hidden;
            }
            else
            {
                coordinatesButton.Visibility = Visibility.Hidden;
            }
            this.project = prject;
            this.area = area;

            fieldSupervisorComboBox.ItemsSource = DbWorker.GetContext().Сотрудник.Select(x => x.логин).ToList();
            dataProcessingSupervisorComboBox.ItemsSource = DbWorker.GetContext().Сотрудник.Select(x => x.логин).ToList();

            fieldFill();
        }

        private void fieldFill()
        {
            nameTextBox.Text = area.название;
            if (area.Сотрудник == null && area.Сотрудник1 == null)
            {
                fieldSupervisorComboBox.SelectedIndex = 0;
                dataProcessingSupervisorComboBox.SelectedIndex = 0;
            }
            else
            {
                fieldSupervisorComboBox.SelectedItem = area.Сотрудник.логин;
                dataProcessingSupervisorComboBox.SelectedItem = area.Сотрудник1.логин;
            }
            perimeterLengthTextBox.Text = area.длина_периметра.ToString();
            perimeterAreaTextBox.Text = area.площадь_периметра.ToString();
            dateOfBeginTextBox.Text = area.дата_начала_работ.ToString();
            dateOfEndTextBox.Text = area.дата_окончания_работ.ToString();
            addDateTextBox.Text = area.дата_добавления_записи.ToString();
            editDateTextBox.Text = area.дата_последнего_изменения_записи.ToString();
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                area.название = nameTextBox.Text;
                area.id_проекта = project.id;
                area.id_супервайзера_полевых_работ = DbWorker.GetContext().Сотрудник
                    .FirstOrDefault(x => x.логин == fieldSupervisorComboBox.Text).id;
                area.id_супервайзера_обработки_данных = DbWorker.GetContext().Сотрудник
                    .FirstOrDefault(x => x.логин == fieldSupervisorComboBox.Text).id;
                area.длина_периметра = Double.Parse(perimeterLengthTextBox.Text);
                area.площадь_периметра = Double.Parse(perimeterAreaTextBox.Text);
                area.дата_начала_работ = DateTime.Parse(dateOfBeginTextBox.Text);
                area.дата_окончания_работ = DateTime.Parse(dateOfEndTextBox.Text);

                if (area.дата_добавления_записи != null)
                {
                    area.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                }
                else
                {
                    area.дата_добавления_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    area.дата_последнего_изменения_записи = (DateTime?)new SqlDateTime(DateTime.Now);
                    project.удален = false;
                    DbWorker.GetContext().Площадь.Add(area);
                }
                DbWorker.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены");
                AppFrame.mainFrame.Navigate(new AreaPage(project));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: " + ex.Message);
            }
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new AreaPage(project));
        }
        
        private void employeePageButtonClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new EmployeeCrudPage(area));
        }

        private void coordinatesPageClick(object sender, RoutedEventArgs e)
        {
            AppFrame.mainFrame.Navigate(new AreaPerimeterAngles(area));
        }

        private void importInfoButtonClick(object sender, RoutedEventArgs e)
        {
            new ImportWindow().Show();
        }

        private void importButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

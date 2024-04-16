using OxyPlot;
using OxyPlot.Series;
using qw.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace qw.util
{
    internal class GraphModel
    {
        public static LineSeries areaModel(Площадь area)
        {
            // Создаем серию данных (линейный график)
            var series = new LineSeries
            {
                Title = "Площадь",
                MarkerType = MarkerType.Circle // Тип маркера точек на графике
            };

            // Добавляем данные в серию
            List<Координаты_точки> allPointCoordinates = DbWorker.GetContext().Площадь_УглыПериметра
                .Where(x => x.id_площади == area.id)
                .Select(x => x.Координаты_точки)
                .Where(x => x.удален != true)
                .ToList();
            foreach (Координаты_точки point in allPointCoordinates)
            {
                series.Points.Add(new DataPoint((double)point.x, (double)point.y));
            }

            // Добавляем серию в модель
            return series;
        }

        public static LineSeries profileModel(Профиль profile)
        {
            // Создаем серию данных (линейный график)
            var series = new LineSeries
            {
                Title = "Профиль",
                MarkerType = MarkerType.Circle // Тип маркера точек на графике
            };

            // Добавляем данные в серию
            List<Координаты_точки> allPointCoordinates = DbWorker.GetContext().Профиль_ТочкиИзломов
                .Where(x => x.id_профиля == profile.id)
                .Select(x => x.Координаты_точки)
                .Where(x => x.удален != true)
                .ToList();
            allPointCoordinates.Insert(0, DbWorker.GetContext().Координаты_точки
                .Where(x => x.id == profile.id_координат_начала && x.удален != true).ToList().Single());
            allPointCoordinates.Add(DbWorker.GetContext().Координаты_точки
                .Where(x => x.id == profile.Id_координат_конца && x.удален != true).ToList().Single());

            foreach (Координаты_точки point in allPointCoordinates)
            {
                series.Points.Add(new DataPoint((double)point.x, (double)point.y));
            }

            // Добавляем серию в модель
            return series;
        }

        public static ScatterSeries picketModel(Пикет picket)
        {
            // Создаем серию данных
            var series = new ScatterSeries
            {
                Title = "Пикет",
                MarkerType = MarkerType.Circle // Тип маркера точек на графике
            };

            // Добавляем данные в серию
            List<Координаты_точки> allPointCoordinates = DbWorker.GetContext().Координаты_точки
                .Where(x => x.id == picket.id_координат_нахождения && x.удален != true).ToList();

            foreach (Координаты_точки point in allPointCoordinates)
            {
                series.Points.Add(new ScatterPoint((double)point.x, (double)point.y));
            }

            // Добавляем серию в модель
            return series;
        }
    }
}

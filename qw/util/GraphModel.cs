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
                if (point != null)
                {
                    series.Points.Add(new DataPoint((double)point.x, (double)point.y));
                }
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
                .FirstOrDefault(x => x.id == profile.id_координат_начала && x.удален != true));
            allPointCoordinates.Add(DbWorker.GetContext().Координаты_точки
                .FirstOrDefault(x => x.id == profile.Id_координат_конца && x.удален != true));

            foreach (Координаты_точки point in allPointCoordinates)
            {
                if (point != null)
                {
                    series.Points.Add(new DataPoint((double)point.x, (double)point.y));
                }
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
                if ( point != null)
                {
                    series.Points.Add(new ScatterPoint((double)point.x, (double)point.y));
                }
            }

            // Добавляем серию в модель
            return series;
        }

        public static LineSeries divergenceModelProfile(Профиль profile, string picketType, bool forArea)
        {
            // Создаем серию данных
            var series = new LineSeries
            {
                Title = picketType,
                MarkerType = MarkerType.Circle, // Тип маркера точек на графике
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline
            };
            if (forArea == true)
            {
                series.Title = profile.название_профиля;
            }

            List<Пикет> allPickets = DbWorker.GetContext().Пикет
                .Where(x => x.удален != true && x.id_профиля == profile.id && x.Виды_пикетов.название == picketType)
                .ToList();

            foreach(Пикет picket in allPickets)
            {
                List<Результаты_измерения> allMeasurements = DbWorker.GetContext().Пикет_ПромежуточныеИзмерения
                    .Where(x => x.id_пикета == picket.id)
                    .Select(x => x.Результаты_измерения)
                    .Where(x => x.удален != true)
                    .ToList();
                double averageFrequency = allMeasurements
                    .Select(x => Double.Parse(x.частота))
                    .DefaultIfEmpty()
                    .Average();
                double averageEDS = allMeasurements
                    .Select(x => Double.Parse(x.значение_ЭДС))
                    .DefaultIfEmpty()
                    .Average();
                series.Points.Add(new DataPoint(averageFrequency, averageEDS));
            }

            // Добавляем серию в модель
            return series;
        }

        public static PlotModel divergenceModelArea(Площадь area, string picketType)
        {
            var plotModel = new PlotModel();

            List<Профиль> allProfiles = DbWorker.GetContext().Профиль
                .Where(x => x.удален != true && x.id_площади == area.id)
                .ToList();
            foreach(var profile in allProfiles)
            {
                plotModel.Series.Add(divergenceModelProfile(profile, picketType, true));
            }

            return plotModel;
        }
    }
}

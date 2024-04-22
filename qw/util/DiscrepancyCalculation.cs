using qw.database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qw.util
{
    internal class DiscrepancyCalculation
    {
        public static List<double> avgOfAllPickets(Профиль profile, string pickeType)
        {
            List<Пикет> allPickets = DbWorker.GetContext().Пикет
                .Where(x => x.удален != true && x.id_профиля == profile.id && x.Виды_пикетов.название == pickeType).ToList();

            List<double> allAveragesFrequencyFromPicket = new List<double>();
            List<double> allAveragesEdsFromPicket = new List<double>();
            foreach (Пикет picket in allPickets)
            {
                List<Результаты_измерения> allMeasurements = DbWorker.GetContext().Пикет_ПромежуточныеИзмерения
                    .Where(x => x.id_пикета == picket.id)
                    .Select(x => x.Результаты_измерения)
                    .Where(x => x.удален != true)
                    .ToList();

                allAveragesFrequencyFromPicket.Add(allMeasurements
                    .Select(x => Double.Parse(x.частота))
                    .DefaultIfEmpty()
                    .Average());

                allAveragesEdsFromPicket.Add(allMeasurements
                    .Select(x => Double.Parse(x.значение_ЭДС))
                    .DefaultIfEmpty()
                    .Average());
            }

            double avgFreqPrivate = allAveragesFrequencyFromPicket.DefaultIfEmpty().Average();
            double avgEdsPrivate = allAveragesEdsFromPicket.DefaultIfEmpty().Average();

            return new List<double> { avgFreqPrivate, avgEdsPrivate };
        }

        public static List<double> avgOfOnePicketType(Профиль profile, string picketType)
        {
            // получаем все пикеты определенного типа на профиле
            List<Пикет> allPickets = DbWorker.GetContext().Пикет
                .Where(x => x.удален != true && x.id_профиля == profile.id && x.Виды_пикетов.название == picketType).ToList();

            // список для сохранения результата по парам
            // одна пара - это средние значения одного пикета
            List<double> result = new List<double>();

            foreach (Пикет picket in allPickets)
            {
                // получаем список всех измерений одного пикета
                List<Результаты_измерения> allMeasurements = DbWorker.GetContext().Пикет_ПромежуточныеИзмерения
                    .Where(x => x.id_пикета == picket.id)
                    .Select(x => x.Результаты_измерения)
                    .Where(x => x.удален != true)
                    .ToList();

                // берем все частоты одного пикета 
                // считаем среднее значение и добавляем в 
                // результирующий список
                result.Add(allMeasurements
                    .Select(x => Double.Parse(x.частота))
                    .DefaultIfEmpty()
                    .Average());

                // берем все ЭДС одного пикета 
                // считаем среднее значение и добавляем в 
                // результирующий список
                result.Add(allMeasurements
                    .Select(x => Double.Parse(x.значение_ЭДС))
                    .DefaultIfEmpty()
                    .Average());
            }

            return result;
        }
    }
}

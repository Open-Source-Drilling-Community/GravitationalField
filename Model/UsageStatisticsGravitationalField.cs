using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NORCE.Drilling.GravitationalField.Model
{
    public struct CountPerDay
    {
        public DateTime Date { get; set; }
        public ulong Count { get; set; }

        public CountPerDay(DateTime date, ulong count)
        {
            Date = date;
            Count = count;
        }
    }

    public class History
    {
        public List<CountPerDay> Data { get; set; } = new List<CountPerDay>();

        public void Increment()
        {
            if (Data == null)
            {
                Data = new List<CountPerDay>();
            }

            if (Data.Count == 0 || Data[Data.Count - 1].Date < DateTime.UtcNow.Date)
            {
                Data.Add(new CountPerDay(DateTime.UtcNow.Date, 1));
            }
            else
            {
                Data[Data.Count - 1] = new CountPerDay(Data[Data.Count - 1].Date, Data[Data.Count - 1].Count + 1);
            }
        }
    }

    public class UsageStatisticsGravitationalField
    {
        public static readonly string HOME_DIRECTORY = ".." + Path.DirectorySeparatorChar + "home" + Path.DirectorySeparatorChar;

        private static readonly object lock_ = new object();
        private static UsageStatisticsGravitationalField? instance_;

        public DateTime LastSaved { get; set; } = DateTime.MinValue;
        public TimeSpan BackUpInterval { get; set; } = TimeSpan.FromMinutes(5);

        public History GetAllGravitationalFieldIdPerDay { get; set; } = new History();
        public History GetAllGravitationalFieldMetaInfoPerDay { get; set; } = new History();
        public History GetGravitationalFieldByIdPerDay { get; set; } = new History();
        public History GetAllGravitationalFieldPerDay { get; set; } = new History();
        public History GetAllCompletedGravitationalFieldPerDay { get; set; } = new History();
        public History PostGravitationalFieldPerDay { get; set; } = new History();
        public History PutGravitationalFieldByIdPerDay { get; set; } = new History();
        public History DeleteGravitationalFieldByIdPerDay { get; set; } = new History();

        public History GetAllGravitationalFieldCalculationOrderIdPerDay { get; set; } = new History();
        public History GetAllGravitationalFieldCalculationOrderMetaInfoPerDay { get; set; } = new History();
        public History GetGravitationalFieldCalculationOrderByIdPerDay { get; set; } = new History();
        public History GetAllGravitationalFieldCalculationOrderLightPerDay { get; set; } = new History();
        public History GetAllGravitationalFieldCalculationOrderPerDay { get; set; } = new History();
        public History PostGravitationalFieldCalculationOrderPerDay { get; set; } = new History();
        public History PutGravitationalFieldCalculationOrderByIdPerDay { get; set; } = new History();
        public History DeleteGravitationalFieldCalculationOrderByIdPerDay { get; set; } = new History();

        public History GetGravitationalFieldUsageStatisticsPerDay { get; set; } = new History();

        public static UsageStatisticsGravitationalField Instance
        {
            get
            {
                if (instance_ == null)
                {
                    lock (lock_)
                    {
                        if (instance_ == null)
                        {
                            instance_ = Load() ?? new UsageStatisticsGravitationalField();
                        }
                    }
                }

                return instance_;
            }
        }

        public void IncrementGetAllGravitationalFieldIdPerDay() => IncrementHistory(() => GetAllGravitationalFieldIdPerDay, value => GetAllGravitationalFieldIdPerDay = value);
        public void IncrementGetAllGravitationalFieldMetaInfoPerDay() => IncrementHistory(() => GetAllGravitationalFieldMetaInfoPerDay, value => GetAllGravitationalFieldMetaInfoPerDay = value);
        public void IncrementGetGravitationalFieldByIdPerDay() => IncrementHistory(() => GetGravitationalFieldByIdPerDay, value => GetGravitationalFieldByIdPerDay = value);
        public void IncrementGetAllGravitationalFieldPerDay() => IncrementHistory(() => GetAllGravitationalFieldPerDay, value => GetAllGravitationalFieldPerDay = value);
        public void IncrementGetAllCompletedGravitationalFieldPerDay() => IncrementHistory(() => GetAllCompletedGravitationalFieldPerDay, value => GetAllCompletedGravitationalFieldPerDay = value);
        public void IncrementPostGravitationalFieldPerDay() => IncrementHistory(() => PostGravitationalFieldPerDay, value => PostGravitationalFieldPerDay = value);
        public void IncrementPutGravitationalFieldByIdPerDay() => IncrementHistory(() => PutGravitationalFieldByIdPerDay, value => PutGravitationalFieldByIdPerDay = value);
        public void IncrementDeleteGravitationalFieldByIdPerDay() => IncrementHistory(() => DeleteGravitationalFieldByIdPerDay, value => DeleteGravitationalFieldByIdPerDay = value);

        public void IncrementGetAllGravitationalFieldCalculationOrderIdPerDay() => IncrementHistory(() => GetAllGravitationalFieldCalculationOrderIdPerDay, value => GetAllGravitationalFieldCalculationOrderIdPerDay = value);
        public void IncrementGetAllGravitationalFieldCalculationOrderMetaInfoPerDay() => IncrementHistory(() => GetAllGravitationalFieldCalculationOrderMetaInfoPerDay, value => GetAllGravitationalFieldCalculationOrderMetaInfoPerDay = value);
        public void IncrementGetGravitationalFieldCalculationOrderByIdPerDay() => IncrementHistory(() => GetGravitationalFieldCalculationOrderByIdPerDay, value => GetGravitationalFieldCalculationOrderByIdPerDay = value);
        public void IncrementGetAllGravitationalFieldCalculationOrderLightPerDay() => IncrementHistory(() => GetAllGravitationalFieldCalculationOrderLightPerDay, value => GetAllGravitationalFieldCalculationOrderLightPerDay = value);
        public void IncrementGetAllGravitationalFieldCalculationOrderPerDay() => IncrementHistory(() => GetAllGravitationalFieldCalculationOrderPerDay, value => GetAllGravitationalFieldCalculationOrderPerDay = value);
        public void IncrementPostGravitationalFieldCalculationOrderPerDay() => IncrementHistory(() => PostGravitationalFieldCalculationOrderPerDay, value => PostGravitationalFieldCalculationOrderPerDay = value);
        public void IncrementPutGravitationalFieldCalculationOrderByIdPerDay() => IncrementHistory(() => PutGravitationalFieldCalculationOrderByIdPerDay, value => PutGravitationalFieldCalculationOrderByIdPerDay = value);
        public void IncrementDeleteGravitationalFieldCalculationOrderByIdPerDay() => IncrementHistory(() => DeleteGravitationalFieldCalculationOrderByIdPerDay, value => DeleteGravitationalFieldCalculationOrderByIdPerDay = value);

        public void IncrementGetGravitationalFieldUsageStatisticsPerDay() => IncrementHistory(() => GetGravitationalFieldUsageStatisticsPerDay, value => GetGravitationalFieldUsageStatisticsPerDay = value);

        private static UsageStatisticsGravitationalField? Load()
        {
            try
            {
                var path = Path.Combine(HOME_DIRECTORY, "history.json");
                if (!File.Exists(path))
                {
                    return null;
                }

                var jsonStr = File.ReadAllText(path);
                return string.IsNullOrWhiteSpace(jsonStr) ? null : JsonSerializer.Deserialize<UsageStatisticsGravitationalField>(jsonStr);
            }
            catch
            {
                return null;
            }
        }

        private void IncrementHistory(Func<History?> getter, Action<History> setter)
        {
            lock (lock_)
            {
                var history = getter();
                if (history == null)
                {
                    history = new History();
                    setter(history);
                }

                history.Increment();
                ManageBackup();
            }
        }

        private void ManageBackup()
        {
            if (DateTime.UtcNow <= LastSaved + BackUpInterval)
            {
                return;
            }

            LastSaved = DateTime.UtcNow;
            try
            {
                if (!Directory.Exists(HOME_DIRECTORY))
                {
                    return;
                }

                File.WriteAllText(Path.Combine(HOME_DIRECTORY, "history.json"), JsonSerializer.Serialize(this));
            }
            catch
            {
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace ContactCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            string patch = String.Empty;
            if (args.Length > 0)
            {
                patch = args[0];

            }


            Dictionary<DateTime, DaySessions> sessionsPerDay = new Dictionary<DateTime, DaySessions>();
            Dictionary<string, Dictionary<string, int>> operatorsConditionsMinutes = new Dictionary<string, Dictionary<string, int>>();

            using (StreamReader sr = new StreamReader(patch))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string[] row = sr.ReadLine().Split(';');
                    DateTime begin = DateTime.Parse(row[0]);
                    DateTime end = DateTime.Parse(row[1]);
                    HandleSession(sessionsPerDay, begin, end);
                    string operatorName = row[3];
                    string condition = row[4];
                    HandleOperator(operatorsConditionsMinutes, operatorName, condition, begin, end);
                }
            }
            OutputResultSessionsPerDay(sessionsPerDay);
            Console.WriteLine(Environment.NewLine);
            OutputResultOperatorsConditions(operatorsConditionsMinutes);

        }

        private static void OutputResultOperatorsConditions(Dictionary<string, Dictionary<string, int>> operatorsConditionsMinutes)
        {
            foreach (var operatorConditionsMinutes in operatorsConditionsMinutes)
            {
                string conditions = string.Empty;
                foreach (var conditionMinute in operatorConditionsMinutes.Value)
                {
                    conditions += $"{conditionMinute.Value} мин. в состоянии {conditionMinute.Key} ";
                }
                Console.WriteLine($"{operatorConditionsMinutes.Key} {conditions}");
            }
        }

        private static void HandleOperator(Dictionary<string, Dictionary<string, int>> operatorsConditionsMinutes, string operatorName, string condition, DateTime begin, DateTime end)
        {
            if (!operatorsConditionsMinutes.ContainsKey(operatorName))
            {
                operatorsConditionsMinutes.Add(operatorName, new Dictionary<string, int>());
            }
            if (!operatorsConditionsMinutes[operatorName].ContainsKey(condition))
            {
                operatorsConditionsMinutes[operatorName].Add(condition, 0);
            }
            if (begin.Date != end.Date || begin.Month != end.Month || begin.Year != end.Year)
            {
                throw new NotImplementedException("Calculation of sessions with switching to another day is not implemented");
            }
            int durationOfSession = (end.Hour - begin.Hour) * 60 + end.Minute - begin.Minute;

            operatorsConditionsMinutes[operatorName][condition] += durationOfSession;
        }
        private static void HandleSession(Dictionary<DateTime, DaySessions> sessionsPerDay, DateTime begin, DateTime end)
        {
            DateTime beginDate = GetDate(begin);
            DateTime endDate = GetDate(end);
            if (beginDate == endDate)
            {
                AddSession(sessionsPerDay, begin, end, beginDate);
            }
            else if (endDate == beginDate.AddDays(1))
            {
                AddSession(sessionsPerDay, begin, new DateTime(0, 0, 0, 23, 59, 0), beginDate);
                AddSession(sessionsPerDay, new DateTime(0, 0, 0, 0, 0, 0), end, beginDate);
            }
            else
            {
                throw new Exception($"There is incorrect session: begin {begin}, end {end}");
            }
        }

        private static void OutputResultSessionsPerDay(Dictionary<DateTime, DaySessions> sessionsPerDay)
        {
            foreach (var daySessions in sessionsPerDay)
            {
                Console.WriteLine($"{daySessions.Key} {daySessions.Value.MaximumNumberSessions}");
            }
        }



        private static void AddSession(Dictionary<DateTime, DaySessions> sessionsPerDay, DateTime begin, DateTime end, DateTime date)
        {
            if (!sessionsPerDay.ContainsKey(date))
            {
                sessionsPerDay.Add(date, new DaySessions());
            }
            sessionsPerDay[date].AddSession(begin, end);
        }

        private static DateTime GetDate(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}

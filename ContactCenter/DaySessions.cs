using System;

namespace ContactCenter
{
    class DaySessions
    {
        public DaySessions()
        {
            for (int i = 0; i < _minutes.Length; i++)
            {
                _minutes[i] = 0;
            }
            MaximumNumberSessions = 0;
        }
        private int[] _minutes = new int[1440];
        public int MaximumNumberSessions { get; private set; }
        public void AddSession(DateTime begin, DateTime end)
        {
            int beginMinutes = GetMinutes(begin);
            int endMinutes = GetMinutes(end);

            for (int i = beginMinutes; i <= endMinutes; i++)
            {
                _minutes[i] += 1;
                if (MaximumNumberSessions < _minutes[i])
                {
                    MaximumNumberSessions = _minutes[i];
                }
            }
        }

        private int GetMinutes(DateTime dateTime)
        {
            return dateTime.Hour * 60 + dateTime.Minute;
        }
    }
}

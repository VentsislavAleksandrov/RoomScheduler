using System;

namespace RoomSceduler.Models.Rooms
{
    public class RoomAvailableIntervalsModel
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public TimeSpan AvialebleTime { get; set; }
    }
}

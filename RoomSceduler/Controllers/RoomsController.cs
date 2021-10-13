using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RoomSceduler.Data;
using RoomSceduler.Data.Models;
using RoomSceduler.Models.Rooms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RoomSceduler.Controllers
{

    public class RoomsController: Controller
    {

        private ApplicationDbContext db;
        
        public RoomsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult CheckRoom()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckRoom(CheckRoomFormModel check)
        {
            var roomsString = System.IO.File.ReadAllText("wwwroot/Input/input.json");

            var rooms = JsonConvert.DeserializeObject<List<RoomJsonModel>>(roomsString);

            var availableRooms = rooms
                .Where(x => x.Capacity >= check.NumberOfPartisipants)
                .ToList();

            var sb = new StringBuilder();

            foreach (var room in availableRooms)
            {
                sb.AppendLine($"{room.RoomName} -");

                var date1 = false;

                foreach (var item in room.Schedule)
                {
                    var sbt = item.From.Substring(0, 10);
                    date1 = sbt == check.Date;
                    if (date1)
                    {
                        break;
                    }
                }

                //var dateExists = room.Schedule.Any(x => x.From.Contains(check.Date));


                if ( !date1 )
                {
                    var roomOpeningDatetime = DateTime
                        .Parse( $"{check.Date} T {room.AvailableFrom}");

                    var roomClosingDatetime = DateTime
                        .Parse(check.Date + "T" + room.AvailableTo);

                    while (true)
                    {
                        var FirstTimeRoomIsAvailableFrom = roomOpeningDatetime;
                        var RoomIsAvailableTo = roomOpeningDatetime
                            .AddMinutes(check.MeetingDuration);

                        var FirstTimeRoomIsAvailable = $"{FirstTimeRoomIsAvailableFrom.ToString("yyyy-MM-dd HH:mm:ss")} - {RoomIsAvailableTo.ToString("yyyy-MM-dd HH:mm:ss")} ";

                        sb.AppendLine(FirstTimeRoomIsAvailable);

                        var FirstTimeRoomIsAvailableFromPlusFifteen =
                                FirstTimeRoomIsAvailableFrom.AddMinutes(15);

                        while (true)
                        {
                            if (FirstTimeRoomIsAvailableFromPlusFifteen.AddMinutes(check.MeetingDuration) >= roomClosingDatetime)
                            {
                                break;
                            }

                            var firstTostr = FirstTimeRoomIsAvailableFromPlusFifteen.ToString("yyyy-MM-dd HH:mm:ss");

                            var firstPlus15PlusDuraion = FirstTimeRoomIsAvailableFromPlusFifteen.AddMinutes(check.MeetingDuration)
                            .ToString("yyyy-MM-dd HH:mm:ss");
                             
                            var nextTimeInterval = $"{firstTostr} - {firstPlus15PlusDuraion} ";

                            sb.AppendLine($"{nextTimeInterval} ||");
                            sb.AppendLine();
                            FirstTimeRoomIsAvailableFromPlusFifteen = FirstTimeRoomIsAvailableFromPlusFifteen.AddMinutes(15);
                        }
                        break;
                    }
                }
                else
                {
                    var roomOpeningDatetime = DateTime
                        .Parse($"{check.Date} T {room.AvailableFrom}");

                    var roomClosingDatetime = DateTime
                        .Parse(check.Date + "T" + room.AvailableTo);

                    var datetimeRoomSchedules = room.Schedule
                        .Select(x => new TimeSlotSort
                        {
                            From = DateTime.Parse(x.From),
                            To = DateTime.Parse(x.To)
                        }).ToList();

                    datetimeRoomSchedules = datetimeRoomSchedules
                        .OrderBy(x => x.From)
                        .ToList();

                    for (int i = 0; i < datetimeRoomSchedules.Count; i++)
                    {
                        var minutes = (datetimeRoomSchedules[i].From - roomOpeningDatetime);

                        if (minutes.TotalMinutes >= check.MeetingDuration)
                        {
                            while (true)
                            {
                                var FirstTimeRoomIsAvailableFrom = roomOpeningDatetime;
                                var RoomIsAvailableTo = roomOpeningDatetime
                                    .AddMinutes(check.MeetingDuration);

                                var FirstTimeRoomIsAvailable = $"{FirstTimeRoomIsAvailableFrom.ToString("yyyy-MM-dd HH:mm:ss")} - {RoomIsAvailableTo.ToString("yyyy-MM-dd HH:mm:ss")} ||";

                                sb.AppendLine(FirstTimeRoomIsAvailable);

                                var FirstTimeRoomIsAvailableFromPlusFifteen =
                                FirstTimeRoomIsAvailableFrom.AddMinutes(15);

                                while (true)
                                {
                                    if (FirstTimeRoomIsAvailableFromPlusFifteen.AddMinutes(check.MeetingDuration) > datetimeRoomSchedules[i].From)
                                    {
                                        break;
                                    }

                                    var firstTostr = FirstTimeRoomIsAvailableFromPlusFifteen.ToString("yyyy-MM-dd HH:mm:ss");

                                    var firstPlus15PlusDuraion = FirstTimeRoomIsAvailableFromPlusFifteen.AddMinutes(check.MeetingDuration)
                                    .ToString("yyyy-MM-dd HH:mm:ss");

                                    var nextTimeInterval = $"{firstTostr} - {firstPlus15PlusDuraion} ";

                                    sb.AppendLine($"{nextTimeInterval} ||");
                                    sb.AppendLine();
                                    FirstTimeRoomIsAvailableFromPlusFifteen = FirstTimeRoomIsAvailableFromPlusFifteen.AddMinutes(15);
                                }
                                break;
                            }                           
                        }
                    }                                       
                }
            }

            var model = new AvailableRoomsViewModel
            {
                AvailableRooms = sb.ToString()
            };

            return View("AvailableRooms",model);
        }

        public IActionResult InsertTimeInterval(AvailableRoomsViewModel model)
        {
            var roomsString = System.IO.File.ReadAllText("wwwroot/Input/input.json");

            var rooms = JsonConvert.DeserializeObject<List<RoomJsonModel>>(roomsString);

            var roomToModify = rooms.FirstOrDefault(x => x.RoomName == model.RoomName);

            rooms.Remove(roomToModify);

            var startHour = model.TimeInterval.Substring(0, 22);
            var endHour = model.TimeInterval.Substring(25);

            var timeslot = new TimeSlotJsonModel
            {
                From = startHour.ToString(),
                To = endHour
            };

            roomToModify.Schedule.Add(timeslot);
            rooms.Add(roomToModify);

            var jsonString = JsonConvert.SerializeObject(rooms,Formatting.Indented);

            System.IO.File.WriteAllText("wwwroot/Input/input.json", jsonString);

            return RedirectToAction("Index", "Home");
        }
    }


}

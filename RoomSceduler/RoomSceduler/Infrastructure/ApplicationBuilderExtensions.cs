using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoomSceduler.Data;
using Newtonsoft.Json;
using System.IO;
using RoomSceduler.Data.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RoomSceduler.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var serviceProvider = scopedServices.ServiceProvider;

            var data = scopedServices.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            data.Database.Migrate();
            //data.Database.EnsureCreated();
            seedRooms(data);
            return app;
        }

        public static void seedRooms(ApplicationDbContext db)
        {
            var roomsString = File.ReadAllText("wwwroot/Input/input.json");

            var rooms = JsonConvert.DeserializeObject<List<RoomJsonModel>>(roomsString);

            foreach (var room in rooms)
            {
                var curRoom = new Room
                {
                    RoomName = room.RoomName,
                    AvailableFrom = DateTime.Parse(room.AvailableFrom),
                    AvailableTo = DateTime.Parse(room.AvailableTo),
                    Capacity = room.Capacity,
                    TimeSlots = room.Schedule.Select(x => new TimeSlot
                    {
                        From = DateTime.Parse(x.From),
                        To = DateTime.Parse(x.To)
                    }).ToList()
                };

                var roomExists = db.Rooms.FirstOrDefault(x => x.RoomName == room.RoomName);

                if (roomExists == null)
                {
                    db.Rooms.Add(curRoom);
                    db.SaveChanges();
                } 
                
            }
        }
    }
}

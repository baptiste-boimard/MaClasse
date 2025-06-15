using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaClasse.Shared.Models.Scheduler;
using Service.Database.Services;
using Xunit;

namespace Tests.Unit;

public class BlockVacationServiceTests
{
    [Fact]
    public async Task GenerateWeeklyMondaysAsync_ReturnsOccurrences()
    {
        var service = new BlockVacationService(null!); // MongoDbContext not needed

        var prototype = new Appointment
        {
            Start = new DateTime(2024, 5, 20, 8, 0, 0),
            End = new DateTime(2024, 5, 20, 10, 0, 0),
            Text = "Cours",
            Color = "blue",
            IdRecurring = "A"
        };

        var vacation = new Appointment { Start = new DateTime(2024, 6, 10) };
        var result = await service.GenerateWeeklyMondaysAsync("1", prototype, vacation, new List<Appointment>());
        Assert.Equal(3, result.Count);
        Assert.Equal(new[]{new DateTime(2024,5,20), new DateTime(2024,5,27), new DateTime(2024,6,3)}, result.Select(r => r.Start.Date));
    }

    [Fact]
    public async Task GenerateWeeklyMondaysAsync_SkipsBlockedWeeks()
    {
        var service = new BlockVacationService(null!);
        var prototype = new Appointment
        {
            Start = new DateTime(2024, 5, 20, 8, 0, 0),
            End = new DateTime(2024, 5, 20, 10, 0, 0),
            Text = "Cours",
            Color = "blue",
            IdRecurring = "A"
        };
        var vacation = new Appointment { Start = new DateTime(2024, 6, 10) };
        var blocking = new List<Appointment>
        {
            new Appointment
            {
                Start = new DateTime(2024,5,27),
                End = new DateTime(2024,6,2)
            }
        };
        var result = await service.GenerateWeeklyMondaysAsync("1", prototype, vacation, blocking);
        Assert.Equal(new[]{new DateTime(2024,5,20), new DateTime(2024,6,3)}, result.Select(a=>a.Start.Date));
    }
}


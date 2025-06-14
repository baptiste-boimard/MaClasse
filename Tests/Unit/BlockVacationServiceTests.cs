using System;
using System.Collections.Generic;
using System.Linq;
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
        Assert.True(result.All(a => a.Text == prototype.Text));
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
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, a => a.Start.Date == new DateTime(2024,5,27));
    }
}


using System.Diagnostics;
using UserService.Application.Abstractions.Integrations.Models;

namespace CvManagementSystem.Infrastructure.Integrations.Utility;

public static class PriorityMapper
{
    public static string MapPriority(Priority priority)
    {
        var priorityString = priority switch
        {
            Priority.Low => "Low",
            Priority.Medium => "Medium",
            Priority.High => "High",
            _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, "Invalid priority value")
        };
        
        return priorityString;
    }
}
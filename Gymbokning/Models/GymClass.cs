using Gymbokning.Models.Attributes;

namespace Gymbokning.Models
{
    public class GymClass
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTime StartTime { get; set; }

        [MinimumClassTime(0, 1)]
        public TimeSpan Duration { get; set; }
        
        public DateTime EndTime { get { return StartTime + Duration; } }
        
        public string Description { get; set; }

        // Navigation property for attending members
        public ICollection<ApplicationUserGymClass> AttendingMembers { get; set; } = new List<ApplicationUserGymClass>();
    }
}

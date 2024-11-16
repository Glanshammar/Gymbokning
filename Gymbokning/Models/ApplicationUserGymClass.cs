namespace Gymbokning.Models
{
    public class ApplicationUserGymClass
    {
        public string ApplicationUserId { get; set; } // Foreign key for ApplicationUser
        public int GymClassId { get; set; } // Foreign key for GymClass

        // Navigation properties
        public ApplicationUser ApplicationUser { get; set; }
        public GymClass GymClass { get; set; }
    }
}

namespace Sep490_Eduseen_BE.Dtos.User
{
    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int Students { get; set; }
        public int Teachers { get; set; }
        public int Admins { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int NewUsersThisWeek { get; set; }
        public double AverageUsersPerDay { get; set; }
        public List<UserRoleCountDto> UsersByRole { get; set; } = new List<UserRoleCountDto>();
        public List<UserStatusCountDto> UsersByStatus { get; set; } = new List<UserStatusCountDto>();
        public List<UserRegistrationDto> UserRegistrationsByMonth { get; set; } = new List<UserRegistrationDto>();
    }

    public class UserRoleCountDto
    {
        public string RoleName { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class UserStatusCountDto
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class UserRegistrationDto
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }
} 
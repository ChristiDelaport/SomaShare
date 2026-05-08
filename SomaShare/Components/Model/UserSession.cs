namespace SomaShare.Components.Model
{
    public class UserSession
    {
        public string User_Id { get; set; } = string.Empty;

        // Helper to get preferred user id (Identity Id if available)
        public string UserId => CurrentUser?.Id ?? User_Id;

        // Store the current user logged in
        public User? CurrentUser { get; set; }

        public int? CurrentUserId { get; set; }


        // Store the current user's role

        public int? CurrentUserRoleId { get; set; }
        public string? CurrentUserRole { get; set; } = string.Empty;


        public bool IsSeller => CurrentUserRole == "Seller";
        public bool IsBuyer => CurrentUserRole == "Buyer";
        public bool IsAdmin => CurrentUserRole == "Admin";
    }
}

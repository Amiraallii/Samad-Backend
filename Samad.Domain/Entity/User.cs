namespace Samad.Domain.Entity
{
    public class User : IEntity<int>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NationalCode { get; set; }
        public string PasswordHash { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        #region ' Relations '
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Request> MyRequests { get; set; }
        public ICollection<CouncilReview> MyReviews { get; set; }
        #endregion ' Relations '
    }
}

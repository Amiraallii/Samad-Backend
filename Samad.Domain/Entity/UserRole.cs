namespace Samad.Domain.Entity
{
    public class UserRole
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }


        #region ' Relations '
        public User User { get; set; }
        public Role Role { get; set; }
        #endregion ' Relations '
    }
}

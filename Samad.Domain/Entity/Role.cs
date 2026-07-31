namespace Samad.Domain.Entity
{
    public class Role
    {
        public int Id { get; set; }
        public string Title { get; set; }
        #region ' Relations '
        public ICollection<UserRole> UserRoles { get; set; }
        #endregion ' Relations '
    }
}

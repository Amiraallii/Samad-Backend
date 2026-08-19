namespace Samad.Domain.Entity
{
    public class Role : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        #region ' Relations '
        public ICollection<User> Users { get; set; }
        #endregion ' Relations '
    }
}

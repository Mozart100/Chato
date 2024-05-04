namespace Chato.Server.DataAccess.Models
{
    public class EntityDbBase
    {
        public virtual string Id { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is EntityDbBase entity)
            {
                return Id.Equals(entity.Id);
            }

            return false;
        }
    }
}
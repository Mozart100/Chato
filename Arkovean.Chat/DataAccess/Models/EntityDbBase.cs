namespace Arkovean.Chat.DataAccess.Models
{
    public class EntityDbBase
    {
        public int Id { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is EntityDbBase course)
            {
                return Id.Equals(course);
            }

            return false;
        }
    }
}
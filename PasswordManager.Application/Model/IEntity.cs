namespace PasswordmanagerApp.Application.Model
{
    public interface IEntity<TKey>
    {
        public TKey Id { get; }
    }


}

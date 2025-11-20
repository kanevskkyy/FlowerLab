namespace UsersService.BLL.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, object id)
    : base($"{entityName} з ID {id} не знайдено.")
        {
        }
    }
}
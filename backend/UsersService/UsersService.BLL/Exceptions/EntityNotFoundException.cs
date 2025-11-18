namespace UsersService.BLL.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, object id) 
            : base($"The {entityName} with ID {id} was not found.")
        {
        }
    }
}
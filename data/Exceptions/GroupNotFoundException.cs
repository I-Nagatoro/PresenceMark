using System;

namespace data.Exceptions
{ 
    public class GroupNotFoundException : RepositoryException
    {
        public GroupNotFoundException(int userId)
            : base($"Группа с ID {userId} не найдена.") { }
    }
}
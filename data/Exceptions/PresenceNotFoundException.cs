using System;

namespace data.Exceptions
{
    public class PresenceNotFoundException : RepositoryException
    {
        public PresenceNotFoundException(int userId, DateOnly date, int firstLesson, int lastLesson)
            : base($"Посещаемость для пользователя ID: {userId} на дату {date.ToShortDateString()}" +
                  $" с {firstLesson} по {lastLesson} уроки не найдена.")
        { }
    }
}
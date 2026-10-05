using System;

namespace LibraryApi.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, object key)
        : base($"Сутність '{entityName}' з ключем '{key}' не знайдена в системі.")
    {
    }
}

public class BusinessRuleValidationException : Exception
{
    public BusinessRuleValidationException(string message)
        : base(message)
    {
    }
}

using System;

namespace MiniERP.Application.Exceptions;

public class ConflictException : Exception
{
    public ConflictException() : base("La operación no se pudo completar debido a un conflicto.")
    {
    }

    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
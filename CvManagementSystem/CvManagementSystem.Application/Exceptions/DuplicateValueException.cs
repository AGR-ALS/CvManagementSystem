namespace UserService.Application.Exceptions;

public class DuplicateValueException(string message) : Exception(message);
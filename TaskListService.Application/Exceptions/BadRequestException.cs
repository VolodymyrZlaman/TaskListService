namespace TaskListService.Application.Exceptions;

public class BadRequestException(string message) : Exception(message);
namespace BibliotecaApi.Exceptions;

/// <summary>Recurso solicitado não existe (HTTP 404).</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>Violação de regra de negócio causada pelos dados enviados (HTTP 400).</summary>
public class BusinessRuleException(string message) : Exception(message);

/// <summary>Conflito com o estado atual do recurso (HTTP 409).</summary>
public class ConflictException(string message) : Exception(message);

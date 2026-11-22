namespace Yemekhane.Backend.Domain.Abstractions;

public sealed class BusinessRuleException(string message) : Exception(message);

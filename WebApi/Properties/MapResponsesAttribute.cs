using MediatR;
using OneOf;

namespace WebApi.Properties;

public class MapResponsesAttribute : Attribute
{
    private readonly Type _requestType;

    public MapResponsesAttribute(Type requestType)
    {
        _requestType = requestType;
    }


    public static Type? ExtractIRequestResponseType(object? instance)
    {
        if (instance == null) return null;
        return ExtractIRequestResponseType(instance.GetType());
    }
    
    public static Type? ExtractIRequestResponseType(Type requestType)
    {
        var interfaces = requestType.GetInterfaces();
        return interfaces
            .Where(x => x.IsGenericType 
                        && x.GetGenericTypeDefinition() == (typeof(IRequest<>)) 
                        && x.GenericTypeArguments.Any())
            .SelectMany(x => x.GenericTypeArguments)
            .FirstOrDefault(x => x.IsAssignableTo(typeof(IOneOf)));
    }

    public bool ValidateResponseType()
    {
        var returnTypes = ExtractIRequestResponseType(_requestType);
        if (returnTypes is null) return false;
        if (returnTypes.IsAssignableTo(typeof(IOneOf)) is false) return false;
        
        return returnTypes.GenericTypeArguments.Any();
    }

    public IEnumerable<Type>? GetAllResponseType()
    {
        var returnTypes = ExtractIRequestResponseType(_requestType);
        if (returnTypes is null) return null;

        return returnTypes.IsAssignableTo(typeof(IOneOf)) 
            ? returnTypes.GenericTypeArguments 
            : null;
    }
}
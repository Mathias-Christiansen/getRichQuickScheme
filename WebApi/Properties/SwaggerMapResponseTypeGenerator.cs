using System.Net;
using Contracts.Errors;
using Contracts.Errors.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApi.Properties;

public class SwaggerMapResponseTypeGenerator : IApplicationModelProvider
{
    public SwaggerMapResponseTypeGenerator()
    {
    }
    
    //Attribute used to keep track of which order the IApplicationBuilders are executed. Smaller equals earlier. 
    public int Order => 0;
    
    public void OnProvidersExecuting(ApplicationModelProviderContext context)
    {
        //Do nothing while executing
        return;
    }

    public void OnProvidersExecuted(ApplicationModelProviderContext context)
    {
        //Select all endpoints
        foreach (var action in context.Result.Controllers.SelectMany(x => x.Actions))
        {
            //Add the universal response types, such as the ones handled by middleware.
            AddUniversalResponseType(action);

            //Extract the MapResponses attribute
            var responseAttribute = action.Attributes
                .Where(x => x.GetType().IsAssignableTo(typeof(MapResponsesAttribute)))
                .Cast<MapResponsesAttribute>()
                .FirstOrDefault(x => x is not null);
            if(responseAttribute is null) continue;

            //Find all possible return types of the OneOf return type
            foreach (var responseType in responseAttribute.GetAllResponseType() ?? Array.Empty<Type>())
            {
                //determine status code
                var statusCode = MapStatusCode(responseType);
                
                //Add response
                AddResponseType(action, statusCode, responseType);
            }
        }
    }

    private static void AddResponseType(IFilterModel action, HttpStatusCode statusCode, Type? returnType)
    {
        var statusCodeResult = (int)statusCode;
        if (returnType != null && returnType != typeof(Unit))
        {
            action.Filters.Add(new ProducesResponseTypeAttribute(returnType, statusCodeResult));
        }
        else if (returnType == null || returnType == typeof(Unit))
        {
            action.Filters.Add(new ProducesResponseTypeAttribute(statusCodeResult));
        }
    }

    private static void AddUniversalResponseType(ActionModel action)
    {
        //Add validation errors
        AddResponseType(action, HttpStatusCode.UnprocessableEntity, typeof(List<IError>));
        
        //Add Authorization errors
        AddResponseType(action, HttpStatusCode.Unauthorized, typeof(IPermissionError));
    }

    private static HttpStatusCode MapStatusCode(Type type)
    {
        if (type.IsAssignableTo(typeof(Unit))) return HttpStatusCode.NoContent;
        if (type.IsAssignableTo(typeof(IAlreadyExistsError))) return HttpStatusCode.Conflict;
        if (type.IsAssignableTo(typeof(IValidationError))) return HttpStatusCode.BadRequest;
        if (type.IsAssignableTo(typeof(INotFoundError))) return HttpStatusCode.NotFound;
        if (type.IsAssignableTo(typeof(IPermissionError))) return HttpStatusCode.Unauthorized;
        if (type.IsAssignableTo(typeof(IError))) return HttpStatusCode.BadRequest;
        if (type.IsAssignableTo(typeof(IEnumerable<IError>))) return HttpStatusCode.UnprocessableEntity;
        
        
        return HttpStatusCode.OK;
    }
}
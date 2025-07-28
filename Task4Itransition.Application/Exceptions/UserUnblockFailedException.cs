using System.Net;
using Task4Itransition.Application.Abstracts;

namespace Task4Itransition.Application.Exceptions;

public class UserUnblockFailedException(string message) : Exception(message), IHttpError 
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}

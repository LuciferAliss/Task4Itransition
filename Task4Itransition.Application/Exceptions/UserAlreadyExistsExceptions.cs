using System.Net;
using Task4Itransition.Application.Abstracts;

namespace Task4Itransition.Application.Exceptions;

public class UserAlreadyExistsExceptions(string email) : Exception($"User with email: {email} already exists"), IHttpError
{ 
    public HttpStatusCode StatusCode => HttpStatusCode.Conflict;
}
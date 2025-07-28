using System.Net;
using Task4Itransition.Application.Abstracts;

namespace Task4Itransition.Application.Exceptions;

public class LoginFailedException(string username) : Exception($"Invalid username: {username} or password"), IHttpError
{ 
    public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}

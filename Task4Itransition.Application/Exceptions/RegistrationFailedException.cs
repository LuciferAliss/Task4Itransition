using System.Net;
using Task4Itransition.Application.Abstracts;

namespace Task4Itransition.Application.Exceptions;

public class RegistrationFailedException(IEnumerable<string> Errors) : Exception($"Registration failed with errors: {string.Join(Environment.NewLine, Errors)}"), IHttpError
{ 
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
using System.Net;

namespace Task4Itransition.Application.Abstracts;

public interface IHttpError
{
    HttpStatusCode StatusCode { get; }
}
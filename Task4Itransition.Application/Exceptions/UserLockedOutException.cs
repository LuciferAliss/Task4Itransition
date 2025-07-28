using System;
using System.Net;
using Task4Itransition.Application.Abstracts;

namespace Task4Itransition.Application.Exceptions;

public class UserLockedOutException(string email) : Exception($"Account for {email} is locked out."), IHttpError
{
    public HttpStatusCode StatusCode => HttpStatusCode.Locked;
}

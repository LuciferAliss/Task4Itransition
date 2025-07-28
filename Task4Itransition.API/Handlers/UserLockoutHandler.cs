using System;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Task4Itransition.Application.Abstracts.Processor;
using Task4Itransition.Application.Abstracts.Services;
using Task4Itransition.Application.Exceptions;
using Task4Itransition.Domain.Entities;

namespace Task4Itransition.API.Handlers;

public class UserLockoutHandler(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext httpContext, UserManager<User> userManager, IAuthService authService)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(httpContext.User);

            if (user == null)
            {
                await _next(httpContext);
            }


            if (await userManager.IsLockedOutAsync(user))
            {
                await authService.LogoutAsync(httpContext.Request.Cookies["REFRESH_TOKEN"]);

                throw new UserBlockException("User is blocked.");
            }
        }

        await _next(httpContext);
    }
}

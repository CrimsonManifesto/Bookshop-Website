using Bookshop_Website.Models;
using Microsoft.AspNetCore.Identity;
public class ProfilePictureMiddleware
{
    private readonly RequestDelegate _next;

    public ProfilePictureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var user = await userManager.GetUserAsync(context.User); 
            var profilePictureUrl = user?.ProfilePictureUrl ?? "/images/default-profile.jpg";  

            // Save in HttpContext.Items to globally access
            context.Items["ProfilePictureUrl"] = profilePictureUrl;
        }

        // Request handle
        await _next(context);
    }
}

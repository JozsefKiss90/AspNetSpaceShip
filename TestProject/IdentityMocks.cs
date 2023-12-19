using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

public static class IdentityMocks
{
    public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);

        userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((ClaimsPrincipal principal) => null); 

        return userManager;
    }

    public static ClaimsPrincipal CreateClaimsPrincipal(string role)
    {
        var identity = new Mock<IIdentity>();
        identity.SetupGet(i => i.IsAuthenticated).Returns(true);
        identity.SetupGet(i => i.Name).Returns("testuser");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Email, "testuser@example.com"),
            new Claim(ClaimTypes.Role, role)
        };

        var principal = new Mock<ClaimsPrincipal>();
        principal.Setup(x => x.Identity).Returns(identity.Object);
        principal.Setup(x => x.Claims).Returns(claims);

        return principal.Object;
    }
}
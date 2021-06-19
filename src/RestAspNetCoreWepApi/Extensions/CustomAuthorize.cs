using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace Api.Application.Extensions
{
    // Criação da minha annotation do tipo FilterAttribute. Exemplo: [ClaimsAuthorize("Fornecedor", "Adicionar")].
    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimType, string claimValue) : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    // Método de autorização do usuário.
    // Esse método será chamado assim que passar pela annotation criada acima.
    // Sua função é validar o usuário por meio das Claims passadas em nossa annotation.
    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;

        public RequisitoClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = new UnauthorizedResult();

            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value))
                context.Result = new ForbidResult();
        }
    }

    // Método responsável por válidar que o usuário está autenticado e possui as claims necessárias dentro do nosso JWT para acessar aquela determinada controller.
    public class CustomAuthorization
    {
        public static bool ValidarClaimsUsuario(HttpContext context, string claimType, string claimValue)
        {
            return context.User.Claims.Any(c => c.Type.Equals(claimType) && c.Value.Contains(claimValue));
        }
    }
}

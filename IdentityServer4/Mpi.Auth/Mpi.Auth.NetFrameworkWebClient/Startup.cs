using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using IdentityServer3.AccessTokenValidation;
using System.Diagnostics;
using static IdentityModel.Client.OAuth2Constants;
using System.Web.Helpers;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace Mpi.Auth.NetFrameworkWebClient {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            Debug.WriteLine("* * *  HELLO!!!!! - This will be amazing!!!!!!!!!!!!!!!!!!!");
            app.UseCookieAuthentication(new CookieAuthenticationOptions() {
                AuthenticationType = "Cookies"
                
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions() {
                Authority = "http://localhost:5000",
                ClientId = "mvc",
                RedirectUri = "http://localhost:51784/signin-oidc",
                ResponseType = "id_token",
                //Scope = "email",
                SignInAsAuthenticationType = "Cookies"
            });
            //    Notifications = new OpenIdConnectAuthenticationNotifications {
            //        SecurityTokenValidated = n => {
            //            var id = n.AuthenticationTicket.Identity;

            //            // we want to keep first name, last name, subject and roles
            //            var givenName = id.FindFirst(ClaimTypes.GivenName);
            //            var familyName = id.FindFirst(ClaimTypes.Email);
            //            //var sub = id.FindFirst(ClaimTypes.Subject);
            //            var roles = id.FindAll(ClaimTypes.Role);

            //            // create new identity and set name and role claim type
            //            var nid = new ClaimsIdentity(
            //                id.AuthenticationType,
            //                ClaimTypes.GivenName,
            //                ClaimTypes.Role);

            //            nid.AddClaim(givenName);
            //            nid.AddClaim(familyName);
            //            //nid.AddClaim(sub);
            //            nid.AddClaims(roles);

            //            // add some other app specific claim
            //            nid.AddClaim(new Claim("app_specific", "some data"));

            //            n.AuthenticationTicket = new AuthenticationTicket(
            //                nid,
            //                n.AuthenticationTicket.Properties);

            //            return Task.FromResult(0);
            //        }
            //    }
            //});
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "subject";
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
        }
    }
}
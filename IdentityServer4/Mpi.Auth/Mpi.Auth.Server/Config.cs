using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mpi.Auth.Server
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources() {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        public static IEnumerable<Client> GetClients() {
            return new List<Client>
            {
                new Client {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                new Client {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },
                new Client {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit ,

                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RedirectUris = { "http://localhost:51784/signin-oidc", "http://localhost:60876/signin-oidc", "http://localhost:60876"}, // , "http://localhost:5002/signin-oidc" 
                      PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc", "http://localhost:60876", "http://localhost:60876/signout-callback-oidc", "http://localhost:51784/signout-callback-oidc" },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "rememberMe",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "api1",
                        "employee"
                    },
                    AllowOfflineAccess = true
                }
            };
        }

        public static List<TestUser> GetUsers() {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "levi",
                    Password = "password",
                    Claims = new [] {
                        new Claim("name", "Levi"),
                        new Claim("website", "https://levi.com"),
                        new Claim("address", "12345"),
                        new Claim("email", "lfuller@credera.com"),
                        new Claim("emailaddress", "lfuller@credera.com"),
                        new Claim("email_address", "lfuller@credera.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",
                    Claims = new [] {
                        new Claim("name", "Bobby"),
                        //new Claim("website", "https://bob.com"),
                        new Claim("employeenumber", "12345"),
                         new Claim("address", "testing"),
                         new Claim("website", "https://bob.com")
                    }
                }
            };
        }

        //OpenID Connect Scopes
        public static IEnumerable<IdentityResource> GetIdentityResources() {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResource {
                    Name = "rememberMe",
                    DisplayName = "Remember Me",
                    Description = "You will be remembered always and forever",
                    Required = true,
                    UserClaims = new List<string> {"rembembered"}
                },
                new IdentityResource {
                    Name = "employee",
                    DisplayName = "Employee Info",
                    Description = "All your base are belong to us now",
                    Required = true,
                    UserClaims = new List<string> { "employeenumber" }
                }
            };
        }
    }
}

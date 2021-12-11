using ApiClientApp.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiClientApp.Middleware
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClientFactory;
        public JWTMiddleware(RequestDelegate next, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory.CreateClient("StockApi");
        }
        public async Task Invoke(HttpContext context)
        {
            //var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var token = await CheckToken();

            if (token != null)
                attachAccountToContext(context, token);
            await _next(context);
        }

        private async Task<string> CheckToken()
        {
            try
            {
                HttpResponseMessage response = null;
                var ui = new UserInfo()
                {
                    Password = "Deneme123",
                    UserName = "testUser"
                };
                var json = JsonSerializer.Serialize<UserInfo>(ui);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await _httpClientFactory.PostAsync("/api/token/new", content);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void attachAccountToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                SecurityToken validatedToken = new JwtSecurityToken(token);
                //tokenHandler.ValidateToken(token2, new TokenValidationParameters
                //{
                //    ValidateIssuerSigningKey = true,
                //    IssuerSigningKey = new SymmetricSecurityKey(key),
                //    ValidateIssuer = false,
                //    ValidateAudience = false,
                //    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                //    ClockSkew = TimeSpan.Zero
                //}, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                //get the user name and role from the JWT token.
                var username = jwtToken.Claims.First(x => x.Type == "username").Value;
                var role = jwtToken.Claims.First(x => x.Type == "role").Value;
                var userClaims = new List<Claim>()
             {
                 new Claim("UserName", username),
                 new Claim("Role", role)
              };
                var userIdentity = new ClaimsIdentity(userClaims, "User Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });

                // attach account to context on successful jwt validation 
                //var user = new MVCWebApplication.Data.User();
                //user.UserName = "aa@hotmail.com";
                //context.Items["User"] = user;
                context.SignInAsync(userPrincipal);
            }
            catch (Exception ex)
            {
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
                throw new Exception(ex.Message);
            }
        }
    }
}

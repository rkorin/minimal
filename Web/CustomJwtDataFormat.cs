//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http.Authentication;
//using Microsoft.IdentityModel.Protocols;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace Web
//{
//    public class CustomJwtDataFormat : ISecureDataFormat<AuthenticationTicket>
//    {
//        private readonly string algorithm;
//        private readonly TokenValidationParameters validationParameters;

//        private readonly string _issuer = string.Empty;

//        public CustomJwtDataFormat(string issuer)
//        {
//            _issuer = issuer;
//        }
//        public CustomJwtDataFormat(string algorithm, TokenValidationParameters validationParameters)
//        {
//            this.algorithm = algorithm;
//            this.validationParameters = validationParameters;
//        }

//        public AuthenticationTicket Unprotect(string protectedText)
//            => Unprotect(protectedText, null);

//        public AuthenticationTicket Unprotect(string protectedText, string purpose)
//        {
//            var handler = new JwtSecurityTokenHandler();
//            ClaimsPrincipal principal = null;
//            SecurityToken validToken = null;

//            try
//            {
//                principal = handler.ValidateToken(protectedText, this.validationParameters, out validToken);

//                var validJwt = validToken as JwtSecurityToken;

//                if (validJwt == null)
//                {
//                    throw new ArgumentException("Invalid JWT");
//                }

//                if (!validJwt.Header.Alg.Equals(algorithm, StringComparison.Ordinal))
//                {
//                    throw new ArgumentException($"Algorithm must be '{algorithm}'");
//                }

//                // Additional custom validation of JWT claims here (if any)
//            }
//            catch (SecurityTokenValidationException)
//            {
//                return null;
//            }
//            catch (ArgumentException)
//            {
//                return null;
//            }

//            // Validation passed. Return a valid AuthenticationTicket:
//            return new AuthenticationTicket(principal, new AuthenticationProperties(), "Cookie");
//        }
         
//        // This ISecureDataFormat implementation is decode-only
//        public string Protect(AuthenticationTicket data)
//        {
//            const string secretKey = "mysupersecret_secretkey!123";
//            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
//            SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
//            const string audience = "Audience";
//            //const string issuer = "Issuer";

//            if (data == null)
//            {
//                throw new ArgumentNullException("data");
//            }

//            string audienceId = audience;              

//            var issued = data.Properties.IssuedUtc;

//            var expires = data.Properties.ExpiresUtc;

//            var token = new JwtSecurityToken(_issuer, audienceId, data.Principal.Claims ,
//                issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingCredentials);

//            var handler = new JwtSecurityTokenHandler();

//            var jwt = handler.WriteToken(token);

//            return jwt; 
//    }

//        public string Protect(AuthenticationTicket data, string purpose)
//        {
//            Protect(data);
//        }
//    }
//}

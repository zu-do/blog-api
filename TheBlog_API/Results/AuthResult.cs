using TheBlog_API.Data.Models;

namespace TheBlog_API.Results
{
    public class AuthResult
    {
        public bool IsSuccess { get; private set; }
        public ApplicationUser User { get; private set; }
        public string ErrorMessage { get; private set; }
        public string Token { get; private set; }

        private AuthResult() { }

        public static AuthResult Success(ApplicationUser user)
        {
            return new AuthResult { IsSuccess = true, User = user };
        }

        public static AuthResult Fail(string errorMessage)
        {
            return new AuthResult { IsSuccess = false, ErrorMessage = errorMessage };
        }

        public static AuthResult SuccessLogin(string token)
        {
            return new AuthResult { IsSuccess = true, Token = token };
        }
    }
}

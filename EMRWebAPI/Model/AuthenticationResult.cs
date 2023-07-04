namespace EMRWebAPI.Model
{
    namespace EMRWebAPI.Model
    {
        public class AuthenticationResult
        {
            public bool Success { get; set; }
            public IEnumerable<string> Errors { get; set; }
            public string Token { get; set; }

            public AuthenticationResult(bool success, IEnumerable<string> errors, string token)
            {
                Success = success;
                Errors = errors;
                Token = token;
            }
        }
    }

}

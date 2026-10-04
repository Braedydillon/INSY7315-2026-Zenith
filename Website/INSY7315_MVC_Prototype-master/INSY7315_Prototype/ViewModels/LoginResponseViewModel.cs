namespace INSY7315_Prototype.ViewModels
{
    public class LoginResponseViewModel
    {
        //Allows the MVC to recieve the tokens issued by the API and proves the firebase user is authenticated 
        public string IdToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public int ExpiresInSeconds { get; set; }

        public string Uid { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}

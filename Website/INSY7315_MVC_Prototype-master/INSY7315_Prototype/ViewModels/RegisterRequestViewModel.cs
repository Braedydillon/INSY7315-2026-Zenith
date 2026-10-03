namespace INSY7315_Prototype.ViewModels
{
    public class RegisterRequestViewModel
    {
        //Used to send information to API
        public string Email { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CellNo {  get; set; } = string.Empty;
        public string IdNumber {  get; set; } = string.Empty;
    }
}

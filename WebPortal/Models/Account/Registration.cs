namespace WebPortal.Models.Account
{
    public class Registration
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string RetypedPassword { get; set; }

        public string RedirectUrl { get; set; }
    }
}

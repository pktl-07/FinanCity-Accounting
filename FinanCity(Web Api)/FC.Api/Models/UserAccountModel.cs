using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FC.Api.Models
{
    public class UserAccountModel
    {
        public string CID { get; set; }
        public string UserAccountName { get; set; }
        public string Email { get; set; }
        public string SPSK { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoggedOn { get; set; }

        public string Api { get; set; }
    }

    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        [JsonIgnore]
        public string UID { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
    }

    public class ResetPasswordModel
    {
        [Display(Name = "Key")]
        public string SysKey { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class RequestResetPasswordModel
    {
        public string Email { get; set; }
    }
}
using Stock.Shared.Models.Data;

namespace XSpy.Database.Models.Views.Account
{
    public class SettingsViewModel
    {
        public UserData UserData { get; set; }

        public UserAddressData AddressData { get; set; }
    }
}
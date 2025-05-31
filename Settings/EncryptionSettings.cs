namespace EmployeeManagementApi.Settings
{
    public class EncryptionSettings
    {
        public required string Key { get; set; }
        public required string IV { get; set; }
    }
}
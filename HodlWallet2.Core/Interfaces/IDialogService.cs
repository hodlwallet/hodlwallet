namespace HodlWallet2.Core.Interfaces
{
    public interface IDialogService
    {
        bool Alert(string title, string message, string okButtonText = null, string cancelButtonText = null);
    }
}
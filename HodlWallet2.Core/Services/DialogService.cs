using HodlWallet2.Core.Interfaces;

namespace HodlWallet2.Core.Services
{
    public class DialogService : IDialogService
    {
        public bool Alert(string title, string message, string okButtonText = null, string cancelButtonText = null)
        {
            if (okButtonText == null && cancelButtonText == null)
                return NotifyAndClose(title, message);

            if (okButtonText != null && cancelButtonText == null)
                return NotifyAndConfirm(title, message, okButtonText);

            return NotifyOkayCancel(title, message, okButtonText, cancelButtonText);
        }

        bool NotifyAndClose(string title, string message, int waitInMilliSeconds = 500)
        {
            throw new System.NotImplementedException();
        }

        bool NotifyAndConfirm(string title, string message, string okButtonText = "Ok")
        {
            throw new System.NotImplementedException();
        }
        
        bool NotifyOkayCancel(string title, string message, string okButtonText = "Ok", string cancelButtonText = "Cancel")
        {
            throw new System.NotImplementedException();
        }}
}
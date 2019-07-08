using System;
using Xamarin.Forms;

namespace HodlWallet2.Core.Interfaces
{
    public interface IShareIntent
    {
        void TextShareIntent(string text);
        void QRTextShareIntent(string address);
    }
}

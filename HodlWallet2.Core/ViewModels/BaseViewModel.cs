using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace HodlWallet2.Core.ViewModels
{
    public class BaseViewModel 
        : MvxNavigationViewModel
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isForwardNavigation;
        public bool IsForwardNavigation
        {
            get => _isForwardNavigation;
            set => SetProperty(ref _isForwardNavigation, value);
        }

        protected BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        protected void LogExceptions(Exception ex)
        {
            Debug.WriteLine($"EXCEPTION LOGGED - {ex.GetType()} - {GetType().Name} - {ex.Message}");
            var currentException = ex;
            while (currentException.InnerException != null)
            {
                currentException = currentException.InnerException;
                Debug.WriteLine($"EXCEPTION LOGGED - {ex.GetType()} - {GetType().Name} - {currentException.Message}");
            }

            Debug.WriteLine(ex.StackTrace);
            //TODO insert logging framework here
        }
    }

    public abstract class BaseViewModel<TParameter> 
        : BaseViewModel, 
          IMvxViewModel<TParameter>
    {
        protected BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        public virtual void Prepare(TParameter parameter) { }
    }

    public abstract class BaseViewModelResult<TResult> 
        : BaseViewModel, 
          IMvxViewModelResult<TResult>
    {
        protected BaseViewModelResult(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        public override void ViewDisappearing()
        {
            if (CloseCompletionSource != null && !CloseCompletionSource.Task.IsCompleted && !CloseCompletionSource.Task.IsFaulted && !IsForwardNavigation)
            {
                CloseCompletionSource?.TrySetCanceled();
            }

            base.ViewDisappearing();
        }
    }

    public abstract class BaseViewModel<TParameter, TResult> 
        : BaseViewModelResult<TResult>, 
          IMvxViewModel<TParameter, TResult>
    {
        protected BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        public virtual void Prepare(TParameter parameter) { }
    }
}
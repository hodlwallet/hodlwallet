using System;
using MvvmCross;
using MvvmCross.Core;
using MvvmCross.Forms.Platforms.Ios.Core;
using MvvmCross.Logging;
using MvvmCross.Platforms.Ios.Core;

namespace HodlWallet2.iOS
{
    public class Setup : MvxFormsIosSetup<Core.App, App>
    {
        
        public override MvxLogProviderType GetDefaultLogProviderType() => MvxLogProviderType.Serilog;

        protected override void InitializeFirstChance()
        {
            base.InitializeFirstChance();          
//            Here we can register types as singletons or multiple instances
//            
//            As a variation on this, you could register a lazy singleton.
//            Every time someone needs an IFoo they will get the same one but we don't create it until someone asks for it
//
//                 Mvx.IoCProvider.RegisterSingleton<IFoo>(() => new Foo());
//
//            In this case:
//
//                - no Foo is created initially
//                - the first time any code calls Mvx.IoCProvider.Resolve<IFoo>() then a new Foo will be created and returned
//                - all subsequent calls will get the same instance that was created the first time
//
//            An alternative syntax for lazy singleton registration - especially useful when the registered type
//            requires constructor dependency injection - is:
//
//                Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IFoo, Foo>();
//
//            More info at https://www.mvvmcross.com/documentation/fundamentals/inversion-of-control-ioc
        }
    }
}
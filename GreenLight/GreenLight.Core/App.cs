/*
GLOSA Mobile. Green Light Optimal Speed Adviosry Mobile Application

Copyright © 2017 Eastpoint Software Limited

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

using MvvmCross.Platform.IoC;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

using GreenLight.Core.ViewModels;
using GreenLight.Core.Contracts;
using GreenLight.Core.Services;

namespace GreenLight.Core
{
    /// <summary>
    /// This is the Main class to launch the application
    /// </summary>
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        #region App Life-Cycle
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.LazyConstructAndRegisterSingleton<IMvxMessenger, MvxMessengerHub>();
            Mvx.LazyConstructAndRegisterSingleton<IDataAnalyticsService, DataAnalyticsService>();
			Mvx.LazyConstructAndRegisterSingleton<ILocationService, LocationService>();

            ISNTPService sntpService = new SNTPService();
            Mvx.RegisterSingleton<ISNTPService>(sntpService);

            IGLOSAWebService gLOSAWebService = new GLOSAWebService(Mvx.Resolve<IDataAnalyticsService>());
            Mvx.RegisterSingleton<IGLOSAWebService>(gLOSAWebService);

            Mvx.RegisterType<IVehicleService, VehicleService>();

            RegisterAppStart<SpeedAdvisoryViewModel>();
        }
        #endregion
    }
}

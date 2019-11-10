#region License
// Copyright (c) 2019 Hichem Kedjour
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GCloudToolbox.Core;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Kubernetes.Secrets.UI.Extensions;
using GCloudToolbox.PubSub.UI.Extensions;
using GCloudToolbox.Shell.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GCloudToolbox
{
    /// <summary>
    /// The application entry point.
    /// </summary>
    public partial class App
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            services
                .AddCore()
                .AddShell()
                .AddKubernetesSecretsUi()
                .AddPubSubUi();

            _serviceProvider = services.BuildServiceProvider();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _ = InitializeBackgroundServicesAsync();
            var shell = _serviceProvider.GetRequiredService<IShell>();
            shell.Run();
        }

        private async Task InitializeBackgroundServicesAsync()
        {
            var services = _serviceProvider.GetRequiredService<IEnumerable<IBackgroundService>>();

            foreach (var service in services)
            {
                await service.InitializeAsync();
            }
        }
    }
}

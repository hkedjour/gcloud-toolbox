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
using System.Threading.Tasks;
using GCloudToolbox.Core;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Core.Messages;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.PubSub.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GCloudToolbox.PubSub.UI.Tools
{
    /// <summary>
    /// Register the Pub/Sub subscriber tool.
    /// </summary>
    [AutoInject(InjectionScope.Singleton, RegisterAs = typeof(ITool), TryRegister = false)]
    public class PubSubSubscriberTool : ITool
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IServiceProvider _serviceProvider;

        public PubSubSubscriberTool(IMessagePublisher messagePublisher, IServiceProvider serviceProvider)
        {
            _messagePublisher = messagePublisher;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public IHeader Header { get; } = new DefaultHeader
        {
            Title = "Pub/Sub subscriber",
            IconPath = "pack://siteoforigin:,,,/Assets/subscribe.png"
        };

        /// <inheritdoc />
        public Task LaunchAsync()
        {
            IView mainView = _serviceProvider.GetRequiredService<PullMainView>();
            return _messagePublisher.PublishAsync(new AddViewMessage("Shell.ViewsHost", mainView, new Scope(_serviceProvider)));
        }
    }
}
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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GCloudToolbox.Core;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Core.Messages;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.Kubernetes.Secrets.UI.Messages;
using GCloudToolbox.Kubernetes.Secrets.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GCloudToolbox.Kubernetes.Secrets.UI
{
    [AutoInject(InjectionScope.Singleton, RegisterAs = typeof(IBackgroundService))]
    public class Coordinator : IBackgroundService
    {
        private readonly IMessageSubscriber _subscriber;
        private readonly IMessagePublisher _publisher;

        public Coordinator( IMessageSubscriber subscriber, IMessagePublisher publisher)
        {
            _subscriber = subscriber;
            _publisher = publisher;
        }

        public Task InitializeAsync()
        {
            _subscriber.Subscribe<NeedContextViewMessage>(AddViewFromMessage<ContextView>);
            _subscriber.Subscribe<NeedNamespaceViewMessage>(AddViewFromMessage<NamespaceView>);
            _subscriber.Subscribe<NeedSecretsViewMessage>(AddViewFromMessage<SecretsView>);
            _subscriber.Subscribe<NeedSecretFilesViewMessage>(AddViewFromMessage<SecretFilesView>);
            _subscriber.Subscribe<NeedSecretEditorViewMessage>(AddViewFromMessage<SecretEditorView>);
            _subscriber.Subscribe<NeedLogsViewMessage>(AddViewFromMessage<LogsView>);
            return Task.CompletedTask;
        }

        private async Task AddViewFromMessage<TView>(NeedViewMessage message) where TView : class, IView 
        {
            var view = await CreateViewAsync<TView>(message.Scope);
            if (view == null)
            {
                return;
            }

            var replay = new AddViewMessage(message.Region, view, message.Scope);
            await _publisher.PublishAsync(replay);
        }

        private Task<TView?> CreateViewAsync<TView>(IScope scope) where TView : class
        {
            var dispatcher = Application.Current?.Dispatcher;
            var applicationClosing = dispatcher == null;
            if (applicationClosing)
            {
                return Task.FromResult((TView?) null);
            }

            Debug.Assert(dispatcher != null, "dispatcher != null");
            return dispatcher.InvokeAsync(() => (TView?) scope.ScopeServiceProvider.GetRequiredService<TView>(),
                DispatcherPriority.Normal).Task;
        }
    }
}
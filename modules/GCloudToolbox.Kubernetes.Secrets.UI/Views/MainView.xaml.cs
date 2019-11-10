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
using GCloudToolbox.Kubernetes.Secrets.UI.ViewModel;
using System.Windows;
using GCloudToolbox.Core.Messages;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.Kubernetes.Secrets.UI.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace GCloudToolbox.Kubernetes.Secrets.UI.Views
{
    /// <summary>
    /// The main view of the secrets editor.
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    public partial class MainView : IView, IAsyncDisposable
    {
        private const string ContextViewRegion = "Kubernetes.Secrets.UI.MainView.Context";
        private const string NamespaceViewRegion = "Kubernetes.Secrets.UI.MainView.Namespace";
        private const string SecretsViewRegion = "Kubernetes.Secrets.UI.MainView.Secrets";
        private const string SecretFilesViewRegion = "Kubernetes.Secrets.UI.MainView.SecretFiles";
        private const string SecretEditorViewRegion = "Kubernetes.Secrets.UI.MainView.SecretEditor";
        private const string LogsViewRegion = "Kubernetes.Secrets.UI.MainView.Logs";

        public static readonly DependencyProperty ContextViewProperty =
            DependencyProperty.Register("ContextView", typeof(IView), typeof(MainView));

        public static readonly DependencyProperty NamespaceViewProperty =
            DependencyProperty.Register("NamespaceView", typeof(IView), typeof(MainView));
   
        public static readonly DependencyProperty SecretsViewProperty =
            DependencyProperty.Register("SecretsView", typeof(IView), typeof(MainView));

        public static readonly DependencyProperty SecretFilesViewProperty =
            DependencyProperty.Register("SecretFilesView", typeof(IView), typeof(MainView));

        public static readonly DependencyProperty SecretEditorViewProperty =
            DependencyProperty.Register("SecretEditorView", typeof(IView), typeof(MainView));

        public static readonly DependencyProperty LogsViewProperty =
            DependencyProperty.Register("LogsView", typeof(IView), typeof(MainView));

        private readonly MainViewModel _viewModel;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageSubscriber _messageSubscriber;
        private readonly TaskScheduler _messagesScheduler;
        private readonly IServiceScope _serviceScope;
        private readonly IScope _scope;


        public MainView(IServiceProvider serviceProvider, IMessagePublisher messagePublisher,
            IMessageSubscriber messageSubscriber)
        {
            _serviceScope = serviceProvider.CreateScope();
            _scope = Scope.From(_serviceScope);
            _viewModel = _serviceScope.ServiceProvider.GetRequiredService<MainViewModel>();
            _messagePublisher = messagePublisher;
            _messageSubscriber = messageSubscriber;
            _messagesScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            DataContext = _viewModel;
            InitializeComponent();

            Task.Run(InitializeChildViewsAsync);
        }

        /// <inheritdoc />
        public IHeader ViewHeader => _viewModel.ViewHeader;

        /// <inheritdoc />
        public object ViewContent => this;

        /// <summary>
        /// Gets or sets the current contexts view
        /// </summary>
        public IView? ContextView
        {
            get => (IView?) GetValue(ContextViewProperty);
            set => SetValue(ContextViewProperty, value);
        }

        /// <summary>
        /// Gets or sets the current namespaces view.
        /// </summary>
        public IView? NamespaceView
        {
            get => (IView?) GetValue(NamespaceViewProperty);
            set => SetValue(NamespaceViewProperty, value);
        }

        /// <summary>
        /// Gets or sets the current secrets view.
        /// </summary>
        public IView? SecretsView
        {
            get => (IView?) GetValue(SecretsViewProperty);
            set => SetValue(SecretsViewProperty, value);
        }

        /// <summary>
        /// Gets or sets the current secret files view.
        /// </summary>
        public IView? SecretFilesView
        {
            get => (IView?) GetValue(SecretFilesViewProperty);
            set => SetValue(SecretFilesViewProperty, value);
        }

        /// <summary>
        /// Gets or sets the current secret editor view
        /// </summary>
        public IView? SecretEditorView
        {
            get => (IView?) GetValue(SecretEditorViewProperty);
            set => SetValue(SecretEditorViewProperty, value);
        }

        /// <summary>
        /// Gets or sets the current logs view.
        /// </summary>
        public IView? LogsView
        {
            get => (IView?) GetValue(LogsViewProperty);
            set => SetValue(LogsViewProperty, value);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            _messageSubscriber.Unsubscribe<AddViewMessage>(OnAddView);
            return default;
        }

        private async Task InitializeChildViewsAsync()
        {
            _messageSubscriber.Subscribe<AddViewMessage>(OnAddView, _messagesScheduler);

            await _messagePublisher.PublishAsync(new NeedLogsViewMessage(_scope, LogsViewRegion));
            await _messagePublisher.PublishAsync(new NeedContextViewMessage(_scope, ContextViewRegion));
            await _messagePublisher.PublishAsync(new NeedNamespaceViewMessage(_scope, NamespaceViewRegion));
            await _messagePublisher.PublishAsync(new NeedSecretsViewMessage(_scope, SecretsViewRegion));
            await _messagePublisher.PublishAsync(new NeedSecretFilesViewMessage(_scope, SecretFilesViewRegion));
            await _messagePublisher.PublishAsync(new NeedSecretEditorViewMessage(_scope, SecretEditorViewRegion));
        }

        private Task OnAddView(AddViewMessage message)
        {
            if (!Equals(message.Scope, _scope))
            {
                return Task.CompletedTask;
            }

            switch (message.Region)
            {
                case ContextViewRegion:
                    ContextView = message.View;
                    break;
                case NamespaceViewRegion:
                    NamespaceView = message.View;
                    break;
                case SecretsViewRegion:
                    SecretsView = message.View;
                    break;
                case SecretFilesViewRegion:
                    SecretFilesView = message.View;
                    break;
                case SecretEditorViewRegion:
                    SecretEditorView = message.View;
                    break;
                case LogsViewRegion:
                    LogsView = message.View;
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
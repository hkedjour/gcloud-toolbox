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
using System.Threading.Tasks;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.Core.Wpf;
using GCloudToolbox.Kubernetes.Secrets.UI.Messages;

namespace GCloudToolbox.Kubernetes.Secrets.UI.ViewModel
{
    /// <summary>
    /// View model for <see cref="Views.SecretsView"/>.
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    public class SecretsViewModel : ViewModelBase, IAsyncDisposable
    {
        private readonly IScopedMessageSubscriber _subscriber;
        private readonly IScopedMessagePublisher _publisher;
        private readonly ISecrets _kubeSecrets;
        private IList<string> _secrets = Array.Empty<string>();
        private string? _currentContext;
        private string? _currentNamespace;
        private string? _currentSecret;

        public SecretsViewModel(IScopedMessageSubscriber subscriber, IScopedMessagePublisher publisher, ISecrets kubeSecrets)
        {
            _kubeSecrets = kubeSecrets;
            _subscriber = subscriber;
            _publisher = publisher;
            _subscriber.Subscribe<NamespaceChangedMessage>(OnNamespaceChanged);
        }

        /// <summary>
        /// Gets or sets the list of kubernetes secretes.
        /// </summary>
        public IList<string> Secrets
        {
            get => _secrets;
            set
            {
                _secrets = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected secret.
        /// </summary>
        public string? CurrentSecret
        {
            get => _currentSecret;
            set
            {
                if (_currentSecret == value)
                {
                    return;
                }

                _currentSecret = value;

                OnCurrentSecretChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            _subscriber.Unsubscribe<NamespaceChangedMessage>(OnNamespaceChanged);
            return default;
        }

        private Task OnNamespaceChanged(NamespaceChangedMessage message)
        {
            _currentContext = message.Context;
            _currentNamespace = message.Namespace;

            return RefreshSecretsAsync();
        }

        private async Task RefreshSecretsAsync()
        {
            Secrets = _currentContext == null || _currentNamespace == null
                ? Array.Empty<string>()
                : await _kubeSecrets.GetSecretsAsync(_currentNamespace, _currentContext).ConfigureAwait(false);
            CurrentSecret = null;
        }

        private void OnCurrentSecretChanged()
        {
            var message = new SecretChangedMessage(_currentSecret, _currentNamespace, _currentContext);
            _ = _publisher.PublishAsync(message);
        }
    }
}
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
    /// View model for <see cref="Views.NamespaceView"/>
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    public class NamespaceViewModel : ViewModelBase, IAsyncDisposable
    {
        private readonly IScopedMessageSubscriber _subscriber;
        private readonly IScopedMessagePublisher _publisher;
        private readonly IKubectl _kubectl;
        private IList<string> _namespaces = Array.Empty<string>();
        private string? _currentNamespace;
        private string? _currentContext;

        public NamespaceViewModel(IKubectl kubectl, IScopedMessageSubscriber subscriber, IScopedMessagePublisher publisher)
        {
            _kubectl = kubectl;
            _subscriber = subscriber;
            _publisher = publisher;
            _subscriber.Subscribe<ContextChangedMessage>(OnContextChanged);
        }

        /// <summary>
        /// Gets or sets the list of kubernetes namespaces.
        /// </summary>
        public IList<string> Namespaces
        {
            get => _namespaces;
            set
            {
                _namespaces = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current selected namespace.
        /// </summary>
        public string? CurrentNamespace
        {
            get => _currentNamespace;
            set
            {
                if (_currentNamespace == value)
                {
                    return;
                }

                _currentNamespace = value;
                OnCurrentNamespaceChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            _subscriber.Unsubscribe<ContextChangedMessage>(OnContextChanged);
            return default;
        }

        private void OnCurrentNamespaceChanged()
        {
            var message = new NamespaceChangedMessage(CurrentNamespace, _currentContext);
            _ = _publisher.PublishAsync(message);
        }

        private Task OnContextChanged(ContextChangedMessage message)
        {
            _currentContext = message.Context;
            
            return RefreshNamespacesAsync();
        }

        private async Task RefreshNamespacesAsync()
        {
            CurrentNamespace = null;

            Namespaces = _currentContext == null
                ? Array.Empty<string>()
                : await _kubectl.GetNamespacesAsync(_currentContext).ConfigureAwait(false);
        }
    }
}
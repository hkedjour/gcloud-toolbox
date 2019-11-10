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
using System.Threading.Tasks;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.Core.Wpf;
using GCloudToolbox.Kubernetes.Secrets.UI.Messages;

namespace GCloudToolbox.Kubernetes.Secrets.UI.ViewModel
{
    /// <summary>
    /// View model for <see cref="Views.SecretFilesView"/>
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    public class SecretFilesViewModel : ViewModelBase, IAsyncDisposable
    {
        private readonly IScopedMessageSubscriber _subscriber;
        private readonly IScopedMessagePublisher _publisher;
        private readonly ISecrets _secrets;
        private ISecret? _secret;
        private string? _currentFile;
        private string? _currentContext;
        private string? _currentNamesapce;
        private string? _currentSecret;

        public SecretFilesViewModel(IScopedMessageSubscriber subscriber, IScopedMessagePublisher publisher, ISecrets secrets)
        {
            _subscriber = subscriber;
            _publisher = publisher;
            _secrets = secrets;

            _subscriber.Subscribe<SecretChangedMessage>(OnSecretChanged);
        }

        /// <summary>
        /// Gets or sets the current kubernetes secret.
        /// </summary>
        public ISecret? Secret
        {
            get => _secret;
            set
            {
                if (Equals(_secret, value))
                {
                    return;
                }

                _secret = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current selected file.
        /// </summary>
        public string? CurrentFile
        {
            get => _currentFile;
            set
            {
                if (_currentFile == value)
                {
                    return;
                }

                _currentFile = value;
                OnCurrentFileChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            _subscriber.Unsubscribe<SecretChangedMessage>(OnSecretChanged);
            return default;
        }

        private Task OnSecretChanged(SecretChangedMessage message)
        {
            _currentContext = message.Context;
            _currentNamesapce = message.Namespace;
            _currentSecret = message.Secret;

            return RefreshSecretAsync();
        }

        private async Task RefreshSecretAsync()
        {
            CurrentFile = null;

            var nothingSelected = _currentContext == null || _currentNamesapce == null || _currentSecret == null;
            if (nothingSelected)
            {
                Secret = null;
                return;
            }

            Debug.Assert(_currentContext != null && _currentNamesapce != null && _currentSecret != null);

            Secret = await _secrets.GetSecretAsync(_currentSecret, _currentNamesapce, _currentContext)
                .ConfigureAwait(false);
        }

        private void OnCurrentFileChanged()
        {
            var message = new SecretFileChangedMessage(_currentFile, Secret, _currentNamesapce, _currentContext);
            _ = _publisher.PublishAsync(message);
        }
    }
}
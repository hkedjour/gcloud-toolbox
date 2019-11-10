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
using System.Windows.Input;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.Core.Wpf;
using GCloudToolbox.Core.Wpf.Commands;
using GCloudToolbox.Kubernetes.Secrets.UI.Messages;

namespace GCloudToolbox.Kubernetes.Secrets.UI.ViewModel
{
    /// <summary>
    /// View model for <see cref="Views.SecretEditorView"/>
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    public class SecretEditorViewModel : ViewModelBase, IAsyncDisposable
    {
        private readonly IScopedMessageSubscriber _subscriber;
        private readonly  ISecrets _secrets;
        private ISecret? _secret;
        private string? _secretContent;
        private string? _fileName;
        private string? _currentContext;
        private bool _saving;
        private readonly AsyncCommand<object> _saveFileCommand;

        public SecretEditorViewModel(IScopedMessageSubscriber subscriber, ISecrets secrets)
        {
            _secrets = secrets;
            _subscriber = subscriber;
            _subscriber.Subscribe<SecretFileChangedMessage>(OnSecretFileChanged);
            _saveFileCommand = new AsyncCommand<object>(OnSaveFile, CanSaveFile);

        }

        /// <summary>
        /// Gets the secret content.
        /// </summary>
        public string? SecretContent
        {
            get => _secretContent;
            set
            {
                if (_secretContent == value)
                {
                    return;
                }
                _secretContent = value;
                OnPropertyChanged();
                _ = _saveFileCommand.RaiseCanExecuteChangedAsync(true);
            }
        }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        public ICommand SaveFileCommand => _saveFileCommand;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            _subscriber.Unsubscribe<SecretFileChangedMessage>(OnSecretFileChanged);
            return default;
        }

        private bool CanSaveFile(object? _) => !_saving && SecretContent != null;

        private async Task OnSaveFile(object? _)
        {
            Debug.Assert(_fileName != null && SecretContent != null && _secret != null && _currentContext != null);

            _saving = true;
            await _saveFileCommand.RaiseCanExecuteChangedAsync(true).ConfigureAwait(false);

            _secret.SetTextFile(_fileName, SecretContent);
            await _secrets.SaveSecretAsync(_secret, _currentContext).ConfigureAwait(true);

            _saving = false;
            await _saveFileCommand.RaiseCanExecuteChangedAsync(true).ConfigureAwait(false);
        }

        private Task OnSecretFileChanged(SecretFileChangedMessage message)
        {
            _secret = message.Secret;
            _fileName = message.FileName;
            _currentContext = message.Context;

            SecretContent = _fileName == null
                ? null
                : _secret?.GetTextFile(_fileName);

            return Task.CompletedTask;
        }
    }
}
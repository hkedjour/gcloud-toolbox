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
    /// View model for <see cref="Views.ContextView"/>
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    public class ContextViewModel : ViewModelBase
    {
        private readonly IScopedMessagePublisher _publisher;
        private readonly IKubectl _kubectl;
        private IList<string> _contexts = Array.Empty<string>();
        private string? _selectedContext;

        public ContextViewModel(IKubectl kubectl, IScopedMessagePublisher publisher)
        {
            _kubectl = kubectl;
            _publisher = publisher;
        }

        /// <summary>
        /// Gets the list of kubernetes contexts.
        /// </summary>
        public IList<string> Contexts
        {
            get => _contexts;
            set
            {
                _contexts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current selected context.
        /// </summary>
        public string? SelectedContext
        {
            get => _selectedContext;
            set
            {
                if (_selectedContext == value)
                {
                    return;
                }

                _selectedContext = value;
                OnPropertyChanged();
                OnCurrentContextChanged();
            }
        }

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync() => 
            Contexts = await _kubectl.GetContextsAsync().ConfigureAwait(false);

        private void OnCurrentContextChanged()
        {
            var message = new ContextChangedMessage(SelectedContext);
            _ =  _publisher.PublishAsync(message);
        }
    }
}
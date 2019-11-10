﻿#region License
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
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.Core.Wpf;
using GCloudToolbox.Kubernetes.Messages;

namespace GCloudToolbox.Kubernetes.Secrets.UI.ViewModel
{
    /// <summary>
    /// View model for <see cref="Views.LogsView"/>
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    public class LogsViewModel : ViewModelBase, IAsyncDisposable
    {
        private readonly IScopedMessageSubscriber _subscriber;
        private readonly object _logsLock = new object();
        private string _logs = string.Empty;

        public LogsViewModel(IScopedMessageSubscriber subscriber)
        {
            _subscriber = subscriber;

            subscriber.Subscribe<CommandExecutingMessage>(OnCommandExecuting);
        }

        /// <summary>
        /// Gets or sets the current log.
        /// </summary>
        public string Logs
        {
            get => _logs;
            set
            {
                if (_logs == value)
                {
                    return;
                }

                _logs = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            _subscriber.Unsubscribe<CommandExecutingMessage>(OnCommandExecuting);
            return default;
        }

        /// <summary>
        /// Handles <see cref="CommandExecutingMessage"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        private Task OnCommandExecuting(CommandExecutingMessage message)
        {
            lock (_logsLock)
            {
                var separator = Logs.Length > 0 ? Environment.NewLine : string.Empty;
                Logs += $"{separator}> {message.Command}";
            }

            return Task.CompletedTask;
        }
    }
}
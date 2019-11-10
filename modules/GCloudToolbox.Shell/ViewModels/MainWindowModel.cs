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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GCloudToolbox.Core;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Core.Messages;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.Core.Wpf;
using GCloudToolbox.Core.Wpf.Commands;

namespace GCloudToolbox.Shell.ViewModels
{
    /// <summary>
    /// View model for the main application window.
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    public class MainWindowModel : ViewModelBase
    {
        private IView? _currentView;

        public MainWindowModel(IEnumerable<ITool> tools, IMessageSubscriber messageSubscriber)
        {
            Tools = tools.Select(t => new ToolMenuEntry(t)).ToList();
            CloseView = new AsyncCommand<IView>(OnCloseViewAsync, v => true);
            messageSubscriber.Subscribe<AddViewMessage>(OnAddView, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Gets the list of all open views.
        /// </summary>
        public ObservableCollection<IView> Views { get; } = new ObservableCollection<IView>();

        /// <summary>
        /// Gets the current selected view.
        /// </summary>
        public IView? CurrentView
        {
            get => _currentView;
            set
            {
                if (ReferenceEquals(_currentView, value))
                {
                    return;
                }

                _currentView = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the command to close a view.
        /// </summary>
        public ICommand CloseView { get; }

        /// <summary>
        /// Gets the list of all registered tools.
        /// </summary>
        public IList<ToolMenuEntry> Tools { get; }

        /// <summary>
        /// Handles the <see cref="AddViewMessage"/> message.
        /// </summary>
        /// <param name="addViewMessage">The message.</param>
        /// <returns></returns>
        private Task OnAddView(AddViewMessage addViewMessage)
        {
            if (addViewMessage.Region != "Shell.ViewsHost")
            {
                return Task.CompletedTask;
            }

            var view = addViewMessage.View;
            lock (Views)
            {
                Views.Add(view);
            }
            CurrentView = view;
            return Task.CompletedTask;
        }

        private async Task DisposeViewAsync(IView view)
        {
            if (view is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }
        }

        private Task OnCloseViewAsync(IView? view)
        {
            if (view == null)
            {
                return Task.CompletedTask;
            }

            var viewFound = RemoveView(view);
            if (!viewFound)
            {
                return Task.CompletedTask;
            }

            return DisposeViewAsync(view);
        }

        /// <summary>Removes the first occurrence of the view.</summary>
        /// <returns>
        /// <see langword="true" /> if <paramref name="view" /> is successfully removed; otherwise, <see langword="false" />.</returns>
        private bool RemoveView(IView view)
        {
            var isCurrentView = ReferenceEquals(_currentView, view);
            lock (Views)
            {
                var wasRemoved = Views.Remove(view);
                if (wasRemoved && isCurrentView)
                {
                    CurrentView = Views.FirstOrDefault();
                }
                return wasRemoved;
            }
        }
    }
}
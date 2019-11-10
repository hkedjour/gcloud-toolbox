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
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GCloudToolbox.Core.Wpf.Commands
{
    /// <summary>
    /// Sample implementation of the <see cref="ICommand"/>
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    public class Command<TParameter> : ICommand where TParameter: class
    {
        private readonly Action<TParameter?> _execute;
        private readonly Func<TParameter?, bool> _canExecute;

        public Command(Action<TParameter?> execute, Func<TParameter?, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter) => _canExecute(parameter as TParameter);

        /// <inheritdoc />
        public void Execute(object parameter) => _execute(parameter as TParameter);


        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        public Task RaiseCanExecuteChangedAsync(bool dispatchOnUiThread = true)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (!dispatchOnUiThread || dispatcher == null)
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                return Task.CompletedTask;
            }

            return dispatcher.InvokeAsync(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty), DispatcherPriority.Normal).Task;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
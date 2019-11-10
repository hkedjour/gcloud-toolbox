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

namespace GCloudToolbox.Core.Wpf.Commands
{
    /// <summary>
    /// An implementation of <see cref="IAsyncCommand{TParameter}"/>
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    public sealed class AsyncCommand<TParameter> : Command<TParameter>, IAsyncCommand<TParameter> where TParameter: class
    {
        private readonly Func<TParameter?, Task> _execute;

        public AsyncCommand(Func<TParameter?, Task> execute, Func<TParameter?, bool> canExecute)
            : base(MakeSync(execute), canExecute)
        {
            _execute = execute;
        }

        /// <inheritdoc />
        public Task ExecuteAsync(TParameter? parameter) => _execute(parameter);

        static Action<TParameter?> MakeSync(Func<TParameter?, Task> execute) => async p =>
        {
            try
            {
                await execute(p);
            }
            catch (Exception e)
            {
                MessageBox.Show($"A background command has failed with {e.Message}");
            }
        };
    }
}
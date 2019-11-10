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

using System.Threading;
using System.Threading.Tasks;
using GCloudToolbox.Core.Messaging;
using GCloudToolbox.Kubernetes.Messages;

namespace GCloudToolbox.Kubernetes
{
    /// <summary>
    /// Add logging to an <see cref="IExecuteProcess"/>.
    /// Logging is done via sending the message <see cref="CommandExecutingMessage"/>
    /// </summary>
    internal class ExecuteProcessLogDecorator : IExecuteProcess
    {
        private readonly IExecuteProcess _execute;
        private readonly IScopedMessagePublisher _publisher;

        public ExecuteProcessLogDecorator(IExecuteProcess execute, IScopedMessagePublisher publisher)
        {
            _execute = execute;
            _publisher = publisher;
        }

        /// <inheritdoc />
        public async Task<string> ExecuteAsync(string fileName, string arguments, CancellationToken cancellationToken = default)
        {
            var command = $"{fileName} {arguments}";
            var message = new CommandExecutingMessage(command);
            var publishMessageTask = _publisher.PublishAsync(message, cancellationToken);
            var executeTask = _execute.ExecuteAsync(fileName, arguments, cancellationToken);

            await publishMessageTask.ConfigureAwait(false);
            return await executeTask.ConfigureAwait(false);
        }
    }
}
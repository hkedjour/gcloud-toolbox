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

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Kubernetes.Extensions;

namespace GCloudToolbox.Kubernetes
{
    /// <summary>
    /// Implementation of <see cref="IExecuteProcess"/>
    /// </summary>
    [AutoInject(InjectionScope.Transient)]
    internal class ExecuteProcess : IExecuteProcess
    {
        /// <inheritdoc />
        public virtual async Task<string> ExecuteAsync(string fileName, string arguments, CancellationToken cancellationToken = default)
        {
            using var process = StartProcess(fileName, arguments);

            var outputTask = ReadOutputAsync(process.StandardOutput, cancellationToken);
            var errorsTask = Task.Run(() => ReadOutputAsync(process.StandardError, cancellationToken), cancellationToken);

            var exitCode = await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            var result = await outputTask.ConfigureAwait(false);
            var errors = await errorsTask.ConfigureAwait(false);

            if (exitCode != 0)
            {
                throw new ExternalProcessExecuteException(exitCode, errors);
            }

            return result;
        }

        private static Process StartProcess(string fileName, string arguments)
        {
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            return process;
        }

        /// <summary>
        /// Read the output of the process.
        /// The process will freeze if it's output buffer is full. Reading must be in parallel.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private Task<string> ReadOutputAsync(StreamReader output, CancellationToken cancellationToken)
            => Task.Run(output.ReadToEndAsync, cancellationToken);
    }
}
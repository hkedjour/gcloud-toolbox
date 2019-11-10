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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GCloudToolbox.Core.DependencyInjection;
using GCloudToolbox.Kubernetes.Extensions;

namespace GCloudToolbox.Kubernetes
{
    /// <summary>
    /// Implementation of <see cref="IKubectl"/>.
    /// </summary>
    [AutoInject(InjectionScope.Transient, RegisterAs = typeof(IKubectl))]
    internal class Kubectl : IKubectl
    {
        private readonly IExecuteProcess _executeProcess;

        public Kubectl(IExecuteProcess executeProcess)
        {
            _executeProcess = executeProcess;
        }

        /// <inheritdoc />
        public async Task<IList<string>> GetContextsAsync(CancellationToken cancellationToken = default)
        {
            var command = "config get-contexts -o name";
            var result = await ExecuteAsync(command, context: null, cancellationToken).ConfigureAwait(false);
            return result.GetLines();
        }

        /// <inheritdoc />
        public async Task<IList<string>> GetNamespacesAsync(string context,
            CancellationToken cancellationToken = default)
        {
            var command = "get namespace -o=name";
            var result = await ExecuteAsync(command, context, cancellationToken).ConfigureAwait(false);

            return ParseResult(result, true);
        }

        /// <inheritdoc />
        public async Task<IList<string>> GetAsync(string resource, string @namespace, string context, bool removePrefix = false,
            CancellationToken cancellationToken = default)
        {
            var command = $"get {resource} -o=name -n {@namespace}";
            var result = await ExecuteAsync(command, context, cancellationToken).ConfigureAwait(false);

            return ParseResult(result, removePrefix);
        }

        /// <inheritdoc />
        public Task<string> ExecuteAsync(string command, string? context, CancellationToken cancellationToken = default)
        {
            var arguments = context == null ? command : $"{command} --context={context}";

            return _executeProcess.ExecuteAsync("kubectl", arguments, cancellationToken);
        }

        private IList<string> ParseResult(string result, bool removePrefix)
        {
            var lines = result.GetLines();

            return removePrefix
                ? lines.Select(line => line.Substring(line.IndexOf('/') + 1)).ToList()
                : lines;
        }
    }
}
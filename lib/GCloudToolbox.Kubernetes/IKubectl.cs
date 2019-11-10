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
using System.Threading;
using System.Threading.Tasks;

namespace GCloudToolbox.Kubernetes
{
    /// <summary>
    /// Kubectl!
    /// </summary>
    public interface IKubectl
    {
        /// <summary>
        /// kubectl get context
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<string>> GetContextsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// kubectl get namespace --context=context
        /// </summary>
        /// <param name="context">The context to use.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<string>> GetNamespacesAsync(string context, CancellationToken cancellationToken = default);

        /// <summary>
        /// kubectl get resource -n namespace --context=context
        /// </summary>
        /// <param name="resource">The resource type to get.</param>
        /// <param name="namespace">The namespace to use.</param>
        /// <param name="context">The context to use.</param>
        /// <param name="removePrefix">Whether to automatically remove the prefix resource/ from the lines.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<string>> GetAsync(string resource, string @namespace, string context, bool removePrefix = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Execute a custom command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="context">The context to use.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> ExecuteAsync(string command, string context, CancellationToken cancellationToken = default);
    }
}
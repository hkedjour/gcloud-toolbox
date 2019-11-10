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

namespace GCloudToolbox.Kubernetes.Secrets
{
    /// <summary>
    /// Allow access to kubernetes secrets.
    /// </summary>
    public interface ISecrets
    {
        /// <summary>
        /// Gets the list of all secrets.
        /// </summary>
        /// <param name="namespace">The namespace to use.</param>
        /// <param name="context">The context to use.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<string>> GetSecretsAsync(string @namespace, string context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specific secret.
        /// </summary>
        /// <param name="name">The name of the secret.</param>
        /// <param name="namespace">The namespace to use.</param>
        /// <param name="context">The context to use.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ISecret> GetSecretAsync(string name, string @namespace, string context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves a secret back to kubernetes.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <param name="context">The context to use.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveSecretAsync(ISecret secret, string context, CancellationToken cancellationToken = default);
    }
}
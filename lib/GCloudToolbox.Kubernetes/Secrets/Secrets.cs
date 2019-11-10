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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GCloudToolbox.Core.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace GCloudToolbox.Kubernetes.Secrets
{
    /// <summary>
    /// Implementation of <see cref="ISecrets"/>
    /// </summary>
    [AutoInject(InjectionScope.Transient, RegisterAs = typeof(ISecrets))]
    internal class Secrets : ISecrets
    {
        private readonly IKubectl _kubectl;

        public Secrets(IKubectl kubectl)
        {
            _kubectl = kubectl;
        }

        /// <inheritdoc />
        public async Task<ISecret> GetSecretAsync(string name, string @namespace, string context, CancellationToken cancellationToken = default)
        {
            var command = $"get secret {name} -o=json -n {@namespace}";
            var secretJson = await _kubectl.ExecuteAsync(command, context, cancellationToken).ConfigureAwait(false);
            var secretDocument = JObject.Parse(secretJson);
            return new Secret(secretDocument);
        }

        /// <inheritdoc />
        public async Task SaveSecretAsync(ISecret secret, string context, CancellationToken cancellationToken = default)
        {
            var tempFileName = await WriteSecretToTempFileAsync(secret, cancellationToken).ConfigureAwait(false);

            var command = $"replace -f \"{tempFileName}\"";
            await _kubectl.ExecuteAsync(command, context, cancellationToken).ConfigureAwait(false);

            File.Delete(tempFileName);
        }

        /// <inheritdoc />
        public Task<IList<string>> GetSecretsAsync(string @namespace, string context, CancellationToken cancellationToken = default)
            => _kubectl.GetAsync("secret", @namespace, context, removePrefix: true, cancellationToken);

        private static async Task<string> WriteSecretToTempFileAsync(ISecret secret, CancellationToken cancellationToken)
        {
            var tempFileName = Path.GetTempFileName();
            await secret.WriteToFileAsync(tempFileName, cancellationToken);
            return tempFileName;
        }
    }
}
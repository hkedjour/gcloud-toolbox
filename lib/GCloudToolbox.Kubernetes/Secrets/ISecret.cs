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
    /// A kubernetes secret.
    /// </summary>
    public interface ISecret
    {
        /// <summary>
        /// Gets the files embedded with the secret.
        /// </summary>
        IList<string> Files { get; }

        /// <summary>
        /// Gets the content of a specific file from the secret.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The content of the file.</returns>
        string GetTextFile(string fileName);

        /// <summary>
        /// Override a file in the secret.
        /// </summary>
        /// <param name="fileName">The file name to override.</param>
        /// <param name="content">The new content of the file.</param>
        void SetTextFile(string fileName, string content);

        /// <summary>
        /// Serialize the secret to the disk file.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WriteToFileAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
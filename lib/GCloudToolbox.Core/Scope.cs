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
using Microsoft.Extensions.DependencyInjection;

namespace GCloudToolbox.Core
{
    /// <summary>
    /// Default implementation of <see cref="IScope"/>
    /// </summary>
    public sealed class Scope : IScope
    {
        public Scope(IServiceProvider scopeServiceProvider)
        {
            ScopeServiceProvider = scopeServiceProvider;
        }

        /// <summary>
        /// Creates as <see cref="IScope"/> from <see cref="IServiceScope"/>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static IScope From(IServiceScope scope) => new Scope(scope.ServiceProvider);

        /// <inheritdoc />
        public IServiceProvider ScopeServiceProvider { get; }

        /// <inheritdoc />
        public bool Equals(IScope? other)
        {
            return Equals(ScopeServiceProvider, other?.ScopeServiceProvider);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is IScope scope && Equals(scope);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ScopeServiceProvider.GetHashCode() ;
        }
    }
}
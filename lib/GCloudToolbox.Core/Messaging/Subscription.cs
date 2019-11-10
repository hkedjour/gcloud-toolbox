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

namespace GCloudToolbox.Core.Messaging
{
    /// <summary>
    /// A reference to a message handler.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    internal sealed class Subscription<TMessage> : IEquatable<Subscription<TMessage>>
    {
        public Subscription(Func<TMessage, Task> handler, TaskScheduler? scheduler = null)
        {
            Handler = handler;
            Scheduler = scheduler;
        }

        /// <summary>
        /// Gets the handler of the message.
        /// </summary>
        public Func<TMessage, Task> Handler { get; }

        /// <summary>
        /// Gets the <see cref="TaskScheduler"/> to be used for messages dispatching.
        /// </summary>
        public TaskScheduler? Scheduler { get; }

        /// <summary>
        /// Creates a new instance of <see cref="Subscription{TMessage}"/>
        /// </summary>
        /// <param name="handler">The message handler.</param>
        /// <param name="scheduler">The task scheduler.</param>
        /// <returns>A new instance of <see cref="Subscription{TMessage}"/></returns>
        public static Subscription<TMessage> From(Func<TMessage, Task> handler, TaskScheduler? scheduler = null)
            => new Subscription<TMessage>(handler, scheduler);

        /// <inheritdoc />
        public bool Equals(Subscription<TMessage>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Handler, other.Handler);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => 
            ReferenceEquals(this, obj) || obj is Subscription<TMessage> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Handler.GetHashCode();
    }
}
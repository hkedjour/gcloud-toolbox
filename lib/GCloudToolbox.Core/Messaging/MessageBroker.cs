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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GCloudToolbox.Core.DependencyInjection;

namespace GCloudToolbox.Core.Messaging
{
    /// <summary>
    /// Manage publishing and subscribing to a given message type.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    [AutoInject(InjectionScope.Scoped)]
    internal class MessageBroker<TMessage>
    {
        /// <summary>
        /// The list of currently active subscribers.
        /// </summary>
        private readonly HashSet<Subscription<TMessage>> _subscriptions = new HashSet<Subscription<TMessage>>();

        /// <inheritdoc cref="IMessageSubscriber.Subscribe{TMessage}"/>
        public void Subscribe(Func<TMessage, Task> subscription, TaskScheduler? scheduler = null)
        {
            lock (_subscriptions)
            {
                _subscriptions.Add(Subscription<TMessage>.From(subscription, scheduler));
            }
        }

        /// <inheritdoc cref="IMessageSubscriber.Unsubscribe{TMessage}"/>
        public void Unsubscribe(Func<TMessage, Task> subscription)
        {
            lock (_subscriptions)
            {
                _subscriptions.Remove(Subscription<TMessage>.From(subscription));
            }
        }

        /// <inheritdoc cref="IMessagePublisher.PublishAsync{TMessage}"/>
        public Task PublishAsync(TMessage message, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                var subscriptions = CloneSubscriptions();
                foreach (var subscription in subscriptions)
                {
                    await Task.Factory.StartNew(() => subscription.Handler(message),
                        cancellationToken, TaskCreationOptions.None, subscription.Scheduler ?? TaskScheduler.Default);
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Clones the current subscription to be used for iteration.
        /// </summary>
        private List<Subscription<TMessage>> CloneSubscriptions()
        {
            lock (_subscriptions)
            {
                return _subscriptions.ToList();
            }
        }
    }

}
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
using System.Linq;
using System.Reflection;
using GCloudToolbox.Core.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GCloudToolbox.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register core services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.TryAddSingleton<MessagesBroker>();
            services.TryAddScoped<ScopedMessagesBroker>();
            services.TryAddSingleton(typeof(IMessagePublisher), sp => sp.GetRequiredService<MessagesBroker>());
            services.TryAddSingleton(typeof(IMessageSubscriber), sp => sp.GetRequiredService<MessagesBroker>());
            services.TryAddScoped(typeof(IScopedMessagePublisher), sp => sp.GetRequiredService<ScopedMessagesBroker>());
            services.TryAddScoped(typeof(IScopedMessageSubscriber), sp => sp.GetRequiredService<ScopedMessagesBroker>());
            return services.AutoRegister(typeof(ServiceCollectionExtensions).Assembly);
        }

        /// <summary>
        /// Automatically register all classed decorated with <see cref="AutoInjectAttribute"/> for a given assembly.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">The assembly where to look for the classes.</param>
        /// <returns></returns>
        public static IServiceCollection AutoRegister(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(AutoInjectAttribute)));

            foreach (var type in types)
            {
                RegisterAutoInjectedType(services, type);
            }
                
            return services;
        }

        private static void RegisterAutoInjectedType(IServiceCollection serviceCollection, Type type)
        {
            var autoInjectAttribute = GetAutoInjectAttribute(type);
            var registerAs = autoInjectAttribute.RegisterAs ?? type;
            var scope = autoInjectAttribute.Scope;

            if (autoInjectAttribute.TryRegister)
            {
                TryRegisterAutoInjectedType(serviceCollection, type, registerAs, scope);
            }
            else
            {
                RegisterAutoInjectedType(serviceCollection, type, registerAs, scope);
            }
        }

        private static AutoInjectAttribute GetAutoInjectAttribute(Type type) => 
            (AutoInjectAttribute)Attribute.GetCustomAttribute(type, typeof(AutoInjectAttribute));

        private static void TryRegisterAutoInjectedType(IServiceCollection serviceCollection, Type type, Type registerAs, InjectionScope scope)
        {
            switch (scope)
            {
                case InjectionScope.Scoped:
                    serviceCollection.TryAddScoped(registerAs, type);
                    break;
                case InjectionScope.Singleton:
                    serviceCollection.TryAddSingleton(registerAs, type);
                    break;
                case InjectionScope.Transient:
                    serviceCollection.TryAddTransient(registerAs, type);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown auto inject scope attribute{scope}");
            }
        }
        private static void RegisterAutoInjectedType(IServiceCollection serviceCollection, Type type, Type registerAs, InjectionScope scope)
        {
            switch (scope)
            {
                case InjectionScope.Scoped:
                    serviceCollection.AddScoped(registerAs, type);
                    break;
                case InjectionScope.Singleton:
                    serviceCollection.AddSingleton(registerAs, type);
                    break;
                case InjectionScope.Transient:
                    serviceCollection.AddTransient(registerAs, type);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown auto inject scope attribute{scope}");
            }
        }
    }
}
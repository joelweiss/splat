﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using DryIoc;

namespace Splat.DryIoc
{
    /// <summary>
    /// DryIoc implementation for <see cref="IMutableDependencyResolver"/>.
    /// https://bitbucket.org/dadhi/dryioc/wiki/Home.
    /// </summary>
    /// <seealso cref="Splat.IMutableDependencyResolver" />
    public class DryIocDependencyResolver : IMutableDependencyResolver
    {
        private Container _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DryIocDependencyResolver" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public DryIocDependencyResolver(Container container = null)
        {
            _container = container ?? new Container();
        }

        /// <inheritdoc />
        public object GetService(Type serviceType, string contract = null) =>
            string.IsNullOrEmpty(contract)
                ? _container.Resolve(serviceType, IfUnresolved.ReturnDefault)
                : _container.Resolve(serviceType, contract, IfUnresolved.ReturnDefault);

        /// <inheritdoc />
        public IEnumerable<object> GetServices(Type serviceType, string contract = null) =>
            string.IsNullOrEmpty(contract)
                ? _container.ResolveMany(serviceType)
                : _container.ResolveMany(serviceType, serviceKey: contract);

        /// <inheritdoc />
        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            if (string.IsNullOrEmpty(contract))
            {
                _container.UseInstance(serviceType, factory(), IfAlreadyRegistered.AppendNewImplementation);
            }
            else
            {
                _container.UseInstance(serviceType, factory(), IfAlreadyRegistered.AppendNewImplementation, serviceKey: contract);
            }
        }

        /// <inheritdoc />
        public void UnregisterCurrent(Type serviceType, string contract = null)
        {
            if (string.IsNullOrEmpty(contract))
            {
                _container.Unregister(serviceType);
            }
            else
            {
                _container.Unregister(serviceType, contract);
            }
        }

        /// <inheritdoc />
        public void UnregisterAll(Type serviceType, string contract = null)
        {
            if (string.IsNullOrEmpty(contract))
            {
                _container.Unregister(serviceType, condition: x => x.ImplementationType == serviceType);
            }
            else
            {
                _container.Unregister(serviceType, contract, condition: x => x.ImplementationType == serviceType);
            }
        }

        /// <inheritdoc />
        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the instance.
        /// </summary>
        /// <param name="disposing">Whether or not the instance is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _container?.Dispose();
                _container = null;
            }
        }
    }
}

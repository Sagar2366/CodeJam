﻿#if LESSTHAN_NET35
// ReSharper disable all
#pragma warning disable CS1570 // XML comment on 'construct' has badly formed XML
// BASEDON: https://github.com/dotnet/corefx/blob/fe5927a0f68717223528220c891a3b02d7331829/src/System.Collections/src/System/Collections/Generic/HashSetEqualityComparer.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Collections.Generic
{

    /// <summary>
    /// Equality comparer for hashsets of hashsets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
    {
        private readonly IEqualityComparer<T> _comparer;

        public HashSetEqualityComparer()
        {
            _comparer = EqualityComparer<T>.Default;
        }

        // using m_comparer to keep equals properties in tact; don't want to choose one of the comparers
        public bool Equals(HashSet<T> x, HashSet<T> y)
        {
            return HashSet<T>.HashSetEquals(x, y, _comparer);
        }

        public int GetHashCode(HashSet<T> obj)
        {
            int hashCode = 0;
            if (obj != null)
            {
                foreach (T t in obj)
                {
                    hashCode = hashCode ^ (_comparer.GetHashCode(t) & 0x7FFFFFFF);
                }
            } // else returns hashcode of 0 for null hashsets
            return hashCode;
        }

        // Equals method for the comparer itself.
        public override bool Equals(object obj)
        {
            HashSetEqualityComparer<T> comparer = obj as HashSetEqualityComparer<T>;
            if (comparer == null)
            {
                return false;
            }
            return (_comparer == comparer._comparer);
        }

        public override int GetHashCode()
        {
            return _comparer.GetHashCode();
        }
    }
}
#endif
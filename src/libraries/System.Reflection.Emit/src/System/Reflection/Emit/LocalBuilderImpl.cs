﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Reflection.Emit
{
    internal sealed class LocalBuilderImpl : LocalBuilder
    {
        #region Private Data Members
        private readonly int _localIndex;
        private readonly Type _localType;
        private readonly MethodInfo _method;
        private readonly bool _isPinned;
        private string? _name;
        #endregion

        #region Constructor
        internal LocalBuilderImpl(int index, Type type, MethodInfo method, bool isPinned)
        {
            _isPinned = isPinned;
            _localIndex = index;
            _localType = type;
            _method = method;
        }
        #endregion

        #region Internal Members
        internal MethodInfo GetMethodBuilder() => _method;
        internal string? Name => _name;
        #endregion

        #region LocalBuilder Override
        protected override void SetLocalSymInfoCore(string name)
        {
            if (_method.DeclaringType is TypeBuilder typeBuilder && typeBuilder.IsCreated())
            {
                throw new InvalidOperationException(SR.InvalidOperation_TypeHasBeenCreated);
            }

            _name = name;
        }
        #endregion

        #region LocalVariableInfo Override
        public override bool IsPinned => _isPinned;
        public override Type LocalType => _localType;
        public override int LocalIndex => _localIndex;
        #endregion
    }
}

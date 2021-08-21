
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Xpandables.Net.Expressions;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class ScopedDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly ScopedDictionary<TKey, TValue> _previous;
    private readonly Dictionary<TKey, TValue> _map;

    public ScopedDictionary(ScopedDictionary<TKey, TValue> previous)
    {
        this._previous = previous;
        _map = new Dictionary<TKey, TValue>();
    }

    public ScopedDictionary(ScopedDictionary<TKey, TValue> previous, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        : this(previous)
    {
        _ = pairs ?? throw new ArgumentNullException(nameof(pairs));

        foreach (var p in pairs)
        {
            _map.Add(p.Key, p.Value);
        }
    }

    public void Add(TKey key, TValue value) => _map.Add(key, value);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope._previous)
        {
            if (scope._map.TryGetValue(key, out value))
                return true;
        }
        value = default;
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope._previous)
        {
            if (scope._map.ContainsKey(key))
                return true;
        }
        return false;
    }
}

/// <summary>
/// Compare two expressions to determine if they are equivalent
/// </summary>
public class ExpressionComparer
{
    private ScopedDictionary<ParameterExpression, ParameterExpression>? _parameterScope;
    private readonly Func<object?, object?, bool>? _fnCompare;

    protected ExpressionComparer(ScopedDictionary<ParameterExpression, ParameterExpression>? parameterScope, Func<object?, object?, bool>? fnCompare)
    {
        _parameterScope = parameterScope;
        _fnCompare = fnCompare;
    }

    public static bool AreEqual(Expression a, Expression b) => AreEqual(null, a, b);

    public static bool AreEqual(Expression a, Expression b, Func<object?, object?, bool> fnCompare) => AreEqual(null, a, b, fnCompare);

    public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression>? parameterScope, Expression a, Expression b)
        => new ExpressionComparer(parameterScope, null).Compare(a, b);

    public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression>? parameterScope, Expression a, Expression b, Func<object?, object?, bool> fnCompare) => new ExpressionComparer(parameterScope, fnCompare).Compare(a, b);

    protected virtual bool Compare(Expression? a, Expression? b)
    {
        if (a == b)
            return true;
        if (a == null || b == null)
            return false;
        if (a.NodeType != b.NodeType)
            return false;
        if (a.Type != b.Type)
            return false;
        return a.NodeType switch
        {
            ExpressionType.Negate or ExpressionType.NegateChecked or ExpressionType.Not or ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.ArrayLength or ExpressionType.Quote or ExpressionType.TypeAs or ExpressionType.UnaryPlus => CompareUnary((UnaryExpression)a, (UnaryExpression)b),
            ExpressionType.Add or ExpressionType.AddChecked or ExpressionType.Subtract or ExpressionType.SubtractChecked or ExpressionType.Multiply or ExpressionType.MultiplyChecked or ExpressionType.Divide or ExpressionType.Modulo or ExpressionType.And or ExpressionType.AndAlso or ExpressionType.Or or ExpressionType.OrElse or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual or ExpressionType.Equal or ExpressionType.NotEqual or ExpressionType.Coalesce or ExpressionType.ArrayIndex or ExpressionType.RightShift or ExpressionType.LeftShift or ExpressionType.ExclusiveOr or ExpressionType.Power => CompareBinary((BinaryExpression)a, (BinaryExpression)b),
            ExpressionType.TypeIs => CompareTypeIs((TypeBinaryExpression)a, (TypeBinaryExpression)b),
            ExpressionType.Conditional => CompareConditional((ConditionalExpression)a, (ConditionalExpression)b),
            ExpressionType.Constant => CompareConstant((ConstantExpression)a, (ConstantExpression)b),
            ExpressionType.Parameter => CompareParameter((ParameterExpression)a, (ParameterExpression)b),
            ExpressionType.MemberAccess => CompareMemberAccess((MemberExpression)a, (MemberExpression)b),
            ExpressionType.Call => CompareMethodCall((MethodCallExpression)a, (MethodCallExpression)b),
            ExpressionType.Lambda => CompareLambda((LambdaExpression)a, (LambdaExpression)b),
            ExpressionType.New => CompareNew((NewExpression)a, (NewExpression)b),
            ExpressionType.NewArrayInit or ExpressionType.NewArrayBounds => CompareNewArray((NewArrayExpression)a, (NewArrayExpression)b),
            ExpressionType.Invoke => CompareInvocation((InvocationExpression)a, (InvocationExpression)b),
            ExpressionType.MemberInit => CompareMemberInit((MemberInitExpression)a, (MemberInitExpression)b),
            ExpressionType.ListInit => CompareListInit((ListInitExpression)a, (ListInitExpression)b),
            _ => throw new ArgumentException($"Unhandled expression type: '{a.NodeType}'"),
        };
    }

    protected virtual bool CompareUnary(UnaryExpression a, UnaryExpression b) => a.NodeType == b.NodeType
            && a.Method == b.Method
            && a.IsLifted == b.IsLifted
            && a.IsLiftedToNull == b.IsLiftedToNull
            && Compare(a.Operand, b.Operand);

    protected virtual bool CompareBinary(BinaryExpression a, BinaryExpression b)
        => a.NodeType == b.NodeType
            && a.Method == b.Method
            && a.IsLifted == b.IsLifted
            && a.IsLiftedToNull == b.IsLiftedToNull
            && Compare(a.Left, b.Left)
            && Compare(a.Right, b.Right);

    protected virtual bool CompareTypeIs(TypeBinaryExpression a, TypeBinaryExpression b)
        => a.TypeOperand == b.TypeOperand && Compare(a.Expression, b.Expression);

    protected virtual bool CompareConditional(ConditionalExpression a, ConditionalExpression b)
        => Compare(a.Test, b.Test) && Compare(a.IfTrue, b.IfTrue) && Compare(a.IfFalse, b.IfFalse);

    protected virtual bool CompareConstant(ConstantExpression a, ConstantExpression b) => _fnCompare?.Invoke(a.Value, b.Value) ?? Equals(a.Value, b.Value);

    protected virtual bool CompareParameter(ParameterExpression a, ParameterExpression b)
    {
        if (_parameterScope != null && _parameterScope.TryGetValue(a, out var mapped))
        {
            return mapped == b;
        }
        return a == b;
    }

    protected virtual bool CompareMemberAccess(MemberExpression a, MemberExpression b)
        => a.Member == b.Member && Compare(a.Expression, b.Expression);

    protected virtual bool CompareMethodCall(MethodCallExpression a, MethodCallExpression b)
        => a.Method == b.Method && Compare(a.Object, b.Object) && CompareExpressionList(a.Arguments, b.Arguments);

    protected virtual bool CompareLambda(LambdaExpression a, LambdaExpression b)
    {
        int n = a.Parameters.Count;
        if (b.Parameters.Count != n)
            return false;
        // all must have same type
        for (int i = 0; i < n; i++)
        {
            if (a.Parameters[i].Type != b.Parameters[i].Type)
                return false;
        }
        var save = _parameterScope;
        _parameterScope = new ScopedDictionary<ParameterExpression, ParameterExpression>(_parameterScope!);
        try
        {
            for (int i = 0; i < n; i++)
            {
                _parameterScope.Add(a.Parameters[i], b.Parameters[i]);
            }
            return Compare(a.Body, b.Body);
        }
        finally
        {
            _parameterScope = save;
        }
    }

    protected virtual bool CompareNew(NewExpression a, NewExpression b)
        => a.Constructor == b.Constructor && CompareExpressionList(a.Arguments, b.Arguments) && CompareMemberList(a.Members, b.Members);

    protected virtual bool CompareExpressionList(ReadOnlyCollection<Expression> a, ReadOnlyCollection<Expression> b)
    {
        if (a == b)
            return true;
        if (a == null || b == null)
            return false;
        if (a.Count != b.Count)
            return false;
        for (int i = 0, n = a.Count; i < n; i++)
        {
            if (!this.Compare(a[i], b[i]))
                return false;
        }
        return true;
    }

    protected virtual bool CompareMemberList(ReadOnlyCollection<MemberInfo>? a, ReadOnlyCollection<MemberInfo>? b)
    {
        if (a == b)
            return true;
        if (a == null || b == null)
            return false;
        if (a.Count != b.Count)
            return false;
        for (int i = 0, n = a.Count; i < n; i++)
        {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }

    protected virtual bool CompareNewArray(NewArrayExpression a, NewArrayExpression b) => CompareExpressionList(a.Expressions, b.Expressions);

    protected virtual bool CompareInvocation(InvocationExpression a, InvocationExpression b)
        => Compare(a.Expression, b.Expression) && CompareExpressionList(a.Arguments, b.Arguments);

    protected virtual bool CompareMemberInit(MemberInitExpression a, MemberInitExpression b)
        => Compare(a.NewExpression, b.NewExpression) && CompareBindingList(a.Bindings, b.Bindings);

    protected virtual bool CompareBindingList(ReadOnlyCollection<MemberBinding> a, ReadOnlyCollection<MemberBinding> b)
    {
        if (a == b)
            return true;
        if (a == null || b == null)
            return false;
        if (a.Count != b.Count)
            return false;
        for (int i = 0, n = a.Count; i < n; i++)
        {
            if (!this.CompareBinding(a[i], b[i]))
                return false;
        }
        return true;
    }

    protected virtual bool CompareBinding(MemberBinding a, MemberBinding b)
    {
        if (a == b)
            return true;
        if (a == null || b == null)
            return false;
        if (a.BindingType != b.BindingType)
            return false;
        if (a.Member != b.Member)
            return false;
        return a.BindingType switch
        {
            MemberBindingType.Assignment => CompareMemberAssignment((MemberAssignment)a, (MemberAssignment)b),
            MemberBindingType.ListBinding => CompareMemberListBinding((MemberListBinding)a, (MemberListBinding)b),
            MemberBindingType.MemberBinding => CompareMemberMemberBinding((MemberMemberBinding)a, (MemberMemberBinding)b),
            _ => throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unhandled binding type: '{0}'", a.BindingType)),
        };
    }

    protected virtual bool CompareMemberAssignment(MemberAssignment a, MemberAssignment b)
        => a.Member == b.Member && Compare(a.Expression, b.Expression);

    protected virtual bool CompareMemberListBinding(MemberListBinding a, MemberListBinding b)
        => a.Member == b.Member && CompareElementInitList(a.Initializers, b.Initializers);

    protected virtual bool CompareMemberMemberBinding(MemberMemberBinding a, MemberMemberBinding b)
        => a.Member == b.Member && CompareBindingList(a.Bindings, b.Bindings);

    protected virtual bool CompareListInit(ListInitExpression a, ListInitExpression b)
        => Compare(a.NewExpression, b.NewExpression) && CompareElementInitList(a.Initializers, b.Initializers);

    protected virtual bool CompareElementInitList(ReadOnlyCollection<ElementInit> a, ReadOnlyCollection<ElementInit> b)
    {
        if (a == b)
            return true;
        if (a == null || b == null)
            return false;
        if (a.Count != b.Count)
            return false;
        for (int i = 0, n = a.Count; i < n; i++)
        {
            if (!this.CompareElementInit(a[i], b[i]))
                return false;
        }
        return true;
    }

    protected virtual bool CompareElementInit(ElementInit a, ElementInit b)
        => a.AddMethod == b.AddMethod && CompareExpressionList(a.Arguments, b.Arguments);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

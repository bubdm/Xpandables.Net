
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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Xpandables.Net.Expressions
{
#nullable disable
    /// <summary>
    /// Custom expression visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in sub-queries which LINQ to SQL can't otherwise handle.
    /// </summary>
    class ExpressionExpander : ExpressionVisitor
    {
        // Replacement parameters - for when invoking a lambda expression.
        readonly Dictionary<ParameterExpression, Expression> _replaceVars;

        internal ExpressionExpander() { }
        private ExpressionExpander(Dictionary<ParameterExpression, Expression> replaceVars) => _replaceVars = replaceVars;
        protected override Expression VisitParameter(ParameterExpression p) => _replaceVars != null && _replaceVars.ContainsKey(p) ? _replaceVars[p] : base.VisitParameter(p);

        /// <summary>
        /// Flatten calls to Invoke so that Entity Framework can understand it. Calls to Invoke are generated
        /// by PredicateBuilder.
        /// </summary>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            Expression target = iv.Expression;
            if (target is MemberExpression expression) target = TransformExpr(expression);
            if (target is ConstantExpression expression1) target = (expression1.Value as Expression)!;

            LambdaExpression lambda = (LambdaExpression)target;

            Dictionary<ParameterExpression, Expression> replaceVars = _replaceVars == null
                ? new Dictionary<ParameterExpression, Expression>()
                : new Dictionary<ParameterExpression, Expression>(_replaceVars);

            try
            {
                for (int i = 0; i < lambda.Parameters.Count; i++)
                    replaceVars.Add(lambda.Parameters[i], iv.Arguments[i]);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Invoke cannot be called recursively - try using a temporary variable.", ex);
            }

            return new ExpressionExpander(replaceVars).Visit(lambda.Body);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Invoke" && m.Method.DeclaringType == typeof(ExpressionExtensions))
            {
                Expression target = m.Arguments[0];
                if (target is MemberExpression expression1) target = TransformExpr(expression1);
                if (target is ConstantExpression expression2) target = (expression2.Value as Expression)!;

                LambdaExpression lambda = (LambdaExpression)target;

                Dictionary<ParameterExpression, Expression> replaceVars = _replaceVars == null
                    ? new Dictionary<ParameterExpression, Expression>()
                    : new Dictionary<ParameterExpression, Expression>(_replaceVars);
                try
                {
                    for (int i = 0; i < lambda.Parameters.Count; i++)
                        replaceVars.Add(lambda.Parameters[i], m.Arguments[i + 1]);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException("Invoke cannot be called recursively - try using a temporary variable.", ex);
                }

                return new ExpressionExpander(replaceVars).Visit(lambda.Body);
            }

            // Expand calls to an expression's Compile() method:
            if (m.Method.Name == "Compile" && m.Object is MemberExpression expression)
            {
                var me = expression;
                Expression newExpr = TransformExpr(me);
                if (newExpr != me) return newExpr;
            }

            // Strip out any nested calls to AsExpandable():
            return m.Method.Name switch
            {
                "AsExpandable" when m.Method.DeclaringType == typeof(ExpressionExtensions) => m.Arguments[0],
                _ => base.VisitMethodCall(m)
            };
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            // Strip out any references to expressions captured by outer variables - LINQ to SQL can't handle these:
            return m.Member.DeclaringType!.Name.StartsWith("<>") ? TransformExpr(m) : base.VisitMemberAccess(m);
        }

        [return: MaybeNull]
        Expression TransformExpr(MemberExpression input)
        {
            // Collapse captured outer variables
            if (input == null
                || !(input.Member is FieldInfo)
                || !input.Member.ReflectedType!.IsNestedPrivate
                || !input.Member.ReflectedType.Name.StartsWith("<>"))   // captured outer variable
                return input;

            if (input.Expression is ConstantExpression expression)
            {
                object obj = expression.Value;
                if (obj == null) return input;
                Type t = obj.GetType();
                if (!t.IsNestedPrivate || !t.Name.StartsWith("<>")) return input;
                FieldInfo fi = (FieldInfo)input.Member;
                object result = fi.GetValue(obj);
                if (result is Expression expression1) return Visit(expression1);
            }

            return input;
        }
    }
#nullable enable
}

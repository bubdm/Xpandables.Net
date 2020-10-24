
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
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Xpandables.Net.Expressions
{
#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// This comes from Matt Warren's sample:
    /// http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
    /// </summary>
    public abstract class ExpressionVisitor
    {
        public virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;

            return exp.NodeType switch
            {
                ExpressionType.Negate or ExpressionType.NegateChecked or ExpressionType.Not or ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.ArrayLength or ExpressionType.Quote or ExpressionType.TypeAs => VisitUnary((UnaryExpression)exp),
                ExpressionType.Add or ExpressionType.AddChecked or ExpressionType.Subtract or ExpressionType.SubtractChecked or ExpressionType.Multiply or ExpressionType.MultiplyChecked or ExpressionType.Divide or ExpressionType.Modulo or ExpressionType.And or ExpressionType.AndAlso or ExpressionType.Or or ExpressionType.OrElse or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual or ExpressionType.Equal or ExpressionType.NotEqual or ExpressionType.Coalesce or ExpressionType.ArrayIndex or ExpressionType.RightShift or ExpressionType.LeftShift or ExpressionType.ExclusiveOr => VisitBinary((BinaryExpression)exp),
                ExpressionType.TypeIs => VisitTypeIs((TypeBinaryExpression)exp),
                ExpressionType.Conditional => VisitConditional((ConditionalExpression)exp),
                ExpressionType.Constant => VisitConstant((ConstantExpression)exp),
                ExpressionType.Parameter => VisitParameter((ParameterExpression)exp),
                ExpressionType.MemberAccess => VisitMemberAccess((MemberExpression)exp),
                ExpressionType.Call => VisitMethodCall((MethodCallExpression)exp),
                ExpressionType.Lambda => VisitLambda((LambdaExpression)exp),
                ExpressionType.New => VisitNew((NewExpression)exp),
                ExpressionType.NewArrayInit or ExpressionType.NewArrayBounds => VisitNewArray((NewArrayExpression)exp),
                ExpressionType.Invoke => VisitInvocation((InvocationExpression)exp),
                ExpressionType.MemberInit => VisitMemberInit((MemberInitExpression)exp),
                ExpressionType.ListInit => VisitListInit((ListInitExpression)exp),
                _ => throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType)),
            };
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding) => binding.BindingType switch
        {
            MemberBindingType.Assignment => VisitMemberAssignment((MemberAssignment)binding),
            MemberBindingType.MemberBinding => VisitMemberMemberBinding((MemberMemberBinding)binding),
            MemberBindingType.ListBinding => VisitMemberListBinding((MemberListBinding)binding),
            _ => throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType)),
        };

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = VisitExpressionList(initializer.Arguments);
            return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = Visit(u.Operand)!;
            return operand != u.Operand ? Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method) : u;
        }

        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = Visit(b.Left);
            Expression right = Visit(b.Right);
            Expression conversion = Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }
            return b;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expr = Visit(b.Expression);
            return expr != b.Expression ? Expression.TypeIs(expr, b.TypeOperand) : b;
        }

        protected virtual Expression VisitConstant(ConstantExpression c) => c;

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = Visit(c.Test);
            Expression ifTrue = Visit(c.IfTrue);
            Expression ifFalse = Visit(c.IfFalse);
            return test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse ? Expression.Condition(test, ifTrue, ifFalse) : c;
        }

        protected virtual Expression VisitParameter(ParameterExpression p) => p;

        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression exp = Visit(m.Expression);
            return exp != m.Expression ? Expression.MakeMemberAccess(exp, m.Member) : m;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = Visit(m.Object);
            IEnumerable<Expression> args = VisitExpressionList(m.Arguments);
            return obj != m.Object || args != m.Arguments ? Expression.Call(obj, m.Method, args) : m;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = default;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }

            return list != null ? list.AsReadOnly() : original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = Visit(assignment.Expression);
            return e != assignment.Expression ? Expression.Bind(assignment.Member, e) : assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = VisitBindingList(binding.Bindings);
            return bindings != binding.Bindings ? Expression.MemberBind(binding.Member, bindings) : binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(binding.Initializers);
            return initializers != binding.Initializers ? Expression.ListBind(binding.Member, initializers) : binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = default;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }

            return list ?? (IEnumerable<MemberBinding>)original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = default;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }

            return list ?? (IEnumerable<ElementInit>)original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = Visit(lambda.Body);
            return body != lambda.Body ? Expression.Lambda(lambda.Type, body, lambda.Parameters) : lambda;
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = VisitExpressionList(nex.Arguments);
            return args != nex.Arguments
                ? nex.Members != null ? Expression.New(nex.Constructor!, args, nex.Members) : Expression.New(nex.Constructor!, args)
                : nex;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = VisitBindingList(init.Bindings);
            return n != init.NewExpression || bindings != init.Bindings ? Expression.MemberInit(n, bindings) : init;
        }

        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(init.Initializers);
            return n != init.NewExpression || initializers != init.Initializers ? Expression.ListInit(n, initializers) : init;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = VisitExpressionList(na.Expressions);
            return exprs != na.Expressions
                ? na.NodeType == ExpressionType.NewArrayInit
                    ? Expression.NewArrayInit(na.Type.GetElementType()!, exprs)
                    : Expression.NewArrayBounds(na.Type.GetElementType()!, exprs)
                : na;
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = VisitExpressionList(iv.Arguments);
            Expression expr = Visit(iv.Expression);
            return args != iv.Arguments || expr != iv.Expression ? Expression.Invoke(expr!, args) : iv;
        }
    }
#nullable enable
}

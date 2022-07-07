using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nest;

namespace MyLab.Search.EsAdapter
{
    class UpdateExpression<TDoc>
        where TDoc : class
    {
        private readonly Expression<Func<TDoc>> _factoryExpression;

        public UpdateExpression(Expression<Func<TDoc>> factoryExpression)
        {
            _factoryExpression = factoryExpression;
        }

        public dynamic ToUpdateModel()
        {
            var memberInitExpr = _factoryExpression.Body as MemberInitExpression;

            if (memberInitExpr == null)
                throw new NotSupportedException("Only MemberInitExpression is supported");
            if (memberInitExpr.Bindings.Count == 0)
                throw new InvalidOperationException("Object initialization must have least one property assignment");
            if (memberInitExpr.Bindings.Any(b => b.BindingType != MemberBindingType.Assignment))
                throw new NotSupportedException("Only property assignment is supported");

            var updateDoc = new ExpandoObject();
            var updateDocProperties = (IDictionary<string, Object>)updateDoc;

            foreach (var memberAssignment in memberInitExpr.Bindings.Cast<MemberAssignment>())
            {
                var mapAttr = memberAssignment.Member.GetCustomAttribute<ElasticsearchPropertyAttributeBase>();
                var memberName = mapAttr?.Name ?? memberAssignment.Member.Name;

                updateDocProperties.Add(memberName, ExpressionToValue(memberAssignment.Expression));
            }

            return updateDoc;
        }

        static object ExpressionToValue(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.New:
                    var ne = (NewExpression)expr;
                    var ctor = ne.Constructor;

                    var ctorArgs = ne.Arguments
                        .Select(ExpressionToValue)
                        .ToArray();

                    return ctor.Invoke(ctorArgs);

                case ExpressionType.Lambda:
                    return ExpressionToValue(((LambdaExpression)expr).Body);

                case ExpressionType.Call:
                {
                    var mce = (MethodCallExpression)expr;

                    object target = null;

                    if (mce.Object != null)
                    {
                        target = ExpressionToValue(mce.Object);
                    }

                    var args = mce.Arguments
                        .Select(ExpressionToValue)
                        .ToArray();

                    return mce.Method.Invoke(target, args);
                }

                case ExpressionType.MemberInit:
                {
                    var mei = (MemberInitExpression)expr;
                    var target = ExpressionToValue(mei.NewExpression);

                    foreach (var member in mei.Bindings)
                    {
                        var propInfo = member.Member as PropertyInfo;

                        if (propInfo == null)
                            throw new NotSupportedException("Only PropertyInfo assignment supported");

                        switch (member.BindingType)
                        {
                            case MemberBindingType.Assignment:

                                propInfo.SetValue(target, ExpressionToValue(((MemberAssignment)member).Expression));
                                break;
                            default:
                                throw new NotSupportedException(member.BindingType.ToString());
                        }

                    }

                    return target;
                }

                case ExpressionType.Constant:
                    return ((ConstantExpression)expr).Value;

                case ExpressionType.MemberAccess:
                {
                    var me = (MemberExpression)expr;

                    var target = ExpressionToValue(me.Expression);

                    switch (me.Member.MemberType)
                    {
                        case MemberTypes.Field:
                            return ((FieldInfo)me.Member).GetValue(target);

                        case MemberTypes.Property:
                            return ((PropertyInfo)me.Member).GetValue(target);

                        default:
                            throw new NotSupportedException(me.Member.MemberType.ToString());
                    }
                }
                default:
                    throw new NotSupportedException(expr.NodeType.ToString());
            }
        }
    }
}
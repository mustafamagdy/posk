using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;

namespace Geeky.POSK.Infrastructore.Helpers
{


  public static class ReflectionHelpers
  {
    public static Expression CallToString(this Expression exp)
    {
      return Expression.Call(
        exp,
        ReflectionHelpers.GetMethodInfo<object>(obj => obj.ToString())
        );
    }
    public static Expression CheckForNull(this Expression val, Expression ifNull, Expression ifNotNull)
    {
      return Expression.Condition(
        Expression.Equal(val, Expression.Constant(null, val.Type)),
        ifNull,
        ifNotNull,
        ifNull.Type
        );
    }
    public static MethodInfo GetMethodInfo<T>(string methodName)
    {

      return typeof(T).GetMethod(methodName);
    }

    public static MethodInfo GetMethodInfo(Delegate method)
    {
      return method.Method;
    }
    public static MethodInfo GetGenericMethodInfo(Expression<Action> call, params Type[] args)
    {
      var callExp = call.Body as MethodCallExpression;
      if (callExp == null)
        throw new Exception("Action must be a call to a method");
      if (!callExp.Method.IsGenericMethod)
        throw new Exception("Not genric method");

      return callExp.Method.GetGenericMethodDefinition().MakeGenericMethod(args);
    }
    public static bool EvaluateExpression<T>(this Expression exp, out T val)
    {
      try
      {
        val = Expression.Lambda<Func<T>>(exp).Compile()();
        return true;
      }catch
      {
        val = default(T);
        return false;
      }
    }
    public static MethodInfo GetMethodInfo<T, TArg0>(string methodName)
    {

      return typeof(T).GetMethods()
          .Where(u => u.Name == methodName)
          .Select(u => new { Method = u, Parameters = u.GetParameters() })
          .Where(u => u.Parameters.Count() == 1 && u.Parameters[0].ParameterType == typeof(TArg0))
          .Select(u => u.Method).FirstOrDefault();
    }
    public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> call)
    {
      if (call.Body.NodeType != ExpressionType.Call)
        throw new ArgumentException("call expression must result in actual method call", "call");
      return ((MethodCallExpression)call.Body).Method;
    }


    public static string GetPropertyPath(this Expression propertyAccess)
    {

      var path = "";
      var member = propertyAccess as MemberExpression;
      while (member != null)
      {

        path = member.Member.Name + (path != "" ? "." : "") + path;
        member = member.Expression as MemberExpression;
        if (member != null) { path = path + "."; }
      }
      if (path.EndsWith("."))

        path = path.Remove(path.Length - 1, 1);
      return path;
    }

    public static Tuple<Type, string> GetFirstChainPropertyType(this Expression propertyAccess)
    {
      Tuple<Type, string> result = null;

      var member = propertyAccess as MemberExpression;
    
       while ( (member?.Expression  as MemberExpression) != null )
      {
        member = member.Expression as MemberExpression;
      }
       
        var propInfo = member.Member as PropertyInfo;
        if (propInfo != null)
          result = new Tuple<Type, string>(propInfo.PropertyType, propInfo.Name);
      

      return result;
    }
    public static MethodInfo GetMethodInfo(Expression<Action> call)
    {
      if (call.Body.NodeType != ExpressionType.Call)
        throw new ArgumentException("call expression must result in actual method call", "call");
      return ((MethodCallExpression)call.Body).Method;
    }


    public static string GetPropertyPath<T>(Expression<Func<T, object>> propertyAccess)
    {
      return GetPropertyPath((Expression)propertyAccess.Body);
    }
    public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> propertyAccess)
    {
      return GetPropertyInfo((Expression)propertyAccess);
    }

    private static PropertyInfo GetPropertyInfo(Expression exp)
    {
      Expression body = (exp as LambdaExpression)?.Body ?? exp;
      if (body.NodeType == ExpressionType.Convert)
        body = ((UnaryExpression)body).Operand;
      if (body.NodeType != ExpressionType.MemberAccess)
        throw new ArgumentException("propertyAccess expression must result in actual property access", "call");
      return ((MemberExpression)body).Member as PropertyInfo;
    }

    public static PropertyInfo GetPropertyInfo<T, P>(Expression<Func<T, P>> propertyAccess)
    {
      Expression body = propertyAccess.Body;
      if (body.NodeType == ExpressionType.Convert)
        body = ((UnaryExpression)body).Operand;
      if (body.NodeType != ExpressionType.MemberAccess)
        throw new ArgumentException("propertyAccess expression must result in actual property access", "call");
      return ((MemberExpression)body).Member as PropertyInfo;
    }


    public static string GetPropertyName<T>(Expression<Func<T, object>> propertyAccess)
    {
      return GetPropertyInfo(propertyAccess).Name;
    }

    public static string GetPropertyName<T, TProp>(Expression<Func<T, TProp>> propertyAccess)
    {
      return GetPropertyInfo(propertyAccess).Name;
    }
    public static string GetPropertyName(Expression<Func<object>> propertyAccess)
    {
      return GetPropertyInfo(propertyAccess).Name;
    }

    public static string[] GetPropertyNames<T>(params Expression<Func<T, object>>[] propertyAccess)
    {

      var result = new string[propertyAccess.Count()];
      int counter = 0;
      foreach (var param in propertyAccess)
      {
        result[counter] = GetPropertyName(param);
        counter++;
      }

      return result;

    }

    public static PropertyInfo GetPropertyInfo(this Type type, string path)
    {
      var props = path.Split('.');
      PropertyInfo current = null;
      for (int i = 0; i < props.Length; i++)
      {
        current = type.GetProperty(props[i]);
        type = current.PropertyType;
      }
      return current;
    }

    public static PropertyInfo GetPropertyInfo(Expression<Func<object>> propertyAccess)
    {
      Expression body = propertyAccess.Body;
      if (body.NodeType == ExpressionType.Convert)
        body = ((UnaryExpression)body).Operand;
      if (body.NodeType != ExpressionType.MemberAccess)
        throw new ArgumentException("propertyAccess expression must result in actual property access", "call");
      return ((MemberExpression)body).Member as PropertyInfo;
    }

    public static MethodInfo GetServiceInterfaceMethoInfo(MethodBase serviceConcreteMethod)
    {
      var interfaceType = serviceConcreteMethod.DeclaringType.GetInterfaces()
         .Where(x => x.Name == $"I{serviceConcreteMethod.DeclaringType.Name}")
         .FirstOrDefault();

      if (interfaceType == null)
      {
        throw new Exception($"Coudn't find service interface for class { serviceConcreteMethod.DeclaringType.Name} ");
      }


      return interfaceType.GetMethods().Where(x => MethodInfoEquals(x, serviceConcreteMethod)).FirstOrDefault();

    }


    public static MethodInfo GetServiceConcreteMethodInfo(MethodBase serviceInterfaceMethod, Assembly ConcreteAssembly)
    {

      var concreteType = ConcreteAssembly.GetTypes().Where(x => x.Name == $"{serviceInterfaceMethod.DeclaringType.Name.Remove(0, 1)}").First();


      if (concreteType == null)
      {
        throw new Exception($"Coudn't find concrete service for interface { serviceInterfaceMethod.DeclaringType.Name} ");
      }

      return concreteType.GetMethods().Where(x => MethodInfoEquals(x, serviceInterfaceMethod)).FirstOrDefault();

    }





    private static bool ParameterInfoEquals(ParameterInfo[] p1, ParameterInfo[] p2)
    {
      if (p1.Length != p2.Length)
      {
        return false;
      }

      if (p1.Length + p2.Length == 0)
      {
        return true;
      }


      for (int i = 0; i < p1.Length; i++)
      {
        var cp1 = p1[i];
        var cp2 = p2[i];
        if (cp1.Position != cp2.Position || cp1.ParameterType.Name != cp2.ParameterType.Name)
        {
          return false;
        }
      }

      return true;
    }
    public static bool MethodInfoEquals(MethodBase m1, MethodBase m2)
    {
      if (m1.Name != m2.Name)
      {
        return false;
      }

      if (!ParameterInfoEquals(m1.GetParameters(), m2.GetParameters()))
        return false;

      if (m1.IsGenericMethod ^ m2.IsGenericMethod)
      {
        return false;
      }

      if (m1.IsGenericMethod)
      {

        var genPar1 = m1.GetGenericArguments();
        var genPar2 = m2.GetGenericArguments();

        if (genPar1.Length != genPar2.Length)
          return false;

        //TODO : (zeyada) bug for Generic Methods of <T> need test
        //for (int i = 0; i < genPar1.Length; i++)
        //{
        //  if (genPar1[i].Name != genPar2[i].Name)
        //    return false;
        //}
      }

      return true;
    }
  }
}

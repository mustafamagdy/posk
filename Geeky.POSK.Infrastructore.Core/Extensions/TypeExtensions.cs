using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CommonServiceLocator;
using System.Data;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class TypeExtensions
  {
    public static IEnumerable<Type> GetSubTypes<T>(this Assembly assembly)
    {
      var arr = assembly?.GetTypes().Where(u => u.IsClass && u.IsPublic && typeof(T).IsAssignableFrom(u)).ToArray();
      if (arr == null)
        arr = new Type[] { };
      return arr;
    }

    public static Type GetArrayElementType(this Type type)
    {
      if (type.HasElementType)
        return type.GetElementType();
      if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        return type.GetGenericArguments().First();
      return type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        .Select(x => x.GetGenericArguments().First()).FirstOrDefault();
    }

    public static Type GetArrayElementType(this Type type, bool returnTypeIfNotArray)
    {
      if (!type.IsArray())
        return type;
      else
        return type.GetArrayElementType();
    }

    public static bool IsArray(this Type arrayType)
    {
      //string is not an array???
      return typeof(string) != arrayType && (arrayType.IsArray ||
        arrayType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
      (arrayType.IsGenericType && new Type[] { typeof(IEnumerable<>), typeof(ICollection<>), typeof(ISet<>), typeof(HashSet<>) }.Contains(arrayType.GetGenericTypeDefinition())));

    }
    public static IEnumerable<Type> GetSubTypes<T>(this Type typeInAssembly)
    {
      return Assembly.GetAssembly(typeInAssembly).GetSubTypes<T>();
    }

    public static bool IsPrimitive(this Type type)
    {
      if (type.IsEnum) return true;
      Type[] primitives = new Type[] {
        typeof(byte), typeof(short), typeof(int),typeof(long),
        typeof(decimal), typeof(double), typeof(decimal), typeof(bool), typeof(string),
        typeof(byte?), typeof(short?), typeof(int?),typeof(long?),
        typeof(decimal?), typeof(double?), typeof(decimal?), typeof(bool?), typeof(Guid),typeof(Guid?),
        typeof(DateTime), typeof(DateTime?)
      };
      return primitives.Contains(type);
    }

    public static T Resolve<T>()
    {
      return ServiceLocator.Current.GetInstance<T>();
    }
    public static T Resolve<T>(this T input)
    {
      return Resolve<T>();
    }

    public static bool IsDefaultValueOfType(this object value, Type type)
    {
      return value.IsDefaultValueOfTypeInternal(type);
    }


    public static bool IsDefaultValueOfType<T>(this object value)
    {

      return value.IsDefaultValueOfTypeInternal(typeof(T));
    }

    private static bool IsDefaultValueOfTypeInternal(this object value, Type type)
    {

      if (value.GetType() != type)
      {
        throw new ArgumentException(string.Format($"targeted value is not of type {type.Name}"));
      }
      //special condition for string type
      if (type == typeof(string))
      {
        return string.IsNullOrEmpty(value.ToString());
      }
      //check if type is reference
      if (!type.IsValueType)
      {
        return (value == null);
      }
      //type is value 
      if (Activator.CreateInstance(type) == value)
      {
        return true;
      }
      return false;
    }


    public static object GenerateDefaultValue(this Type type)
    {

      if (type == typeof(string))
      {
        return string.Empty;
      }
      if (type.IsValueType)
      {
        return Activator.CreateInstance(type);
      }

      return null;
    }

    public static bool IsNumeric(this Type type)
    {
      var numerics = new Type[]
      {
        typeof(int), typeof(double), typeof(long), typeof(decimal), typeof(byte)
      };
      type = Nullable.GetUnderlyingType(type) ?? type;
      if (numerics.Contains(type))
        return true;
      return false;
    }

    /// <summary>
    /// Copy values of declared only properties of object 1 into properties of object 2 (Non deep copy);
    /// </summary>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    public static void CopyInto(this object obj1, object obj2)
    {
      var type1 = obj1.GetType();
      if (!type1.IsAssignableFrom(obj2.GetType()))
        throw new Exception("differrent types can't be copied");
      type1.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .ForEach(prop =>
        {
          prop.SetValue(obj2, prop.GetValue(obj1));
        });
    }
    public static T IfDefault<T>(this T obj, T defaultValue)
    {
      return EqualityComparer<T>.Default.Equals(obj, default(T)) ? defaultValue : obj;
    }

    
  }
}
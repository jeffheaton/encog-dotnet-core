using System;
using System.Reflection;

namespace EncogExtensions.Normalization
{
    // Source("http://www.codeproject.com/Articles/80343/Accessing-private-members.aspx")

    public static class ObjectExtensions
    {
        public static T GetPrivateField<T>(this object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            FieldInfo field = type.GetField(name, flags);
            return (T)field.GetValue(obj);
        }

        public static T GetPrivateProperty<T>(this object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            PropertyInfo field = type.GetProperty(name, flags);
            return (T)field.GetValue(obj, null);
        }

        public static void SetPrivateField(this object obj, string name, object value)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            FieldInfo field = type.GetField(name, flags);
            field.SetValue(obj, value);
        }

        public static void SetPrivateProperty(this object obj, string name, object value)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            PropertyInfo field = type.GetProperty(name, flags);
            field.SetValue(obj, value, null);
        }

        //Void No Parameters Implementation
        public static void CallPrivateMethod(this object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(name, flags);
            method.Invoke(obj,null);
        }

        //Void with Parameters Implementation
        public static void CallPrivateMethod(this object obj, string name, params object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(name, flags);
            method.Invoke(obj, param);
        }


        //No Parameters Implementation
        public static T CallPrivateMethod<T>(this object obj, string name)
        {
            return CallPrivateMethod<T>(obj, name, null);
        }

        //Parameters and Return Implementation
        public static T CallPrivateMethod<T>(this object obj, string name, params object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(name, flags);
            return (T)method.Invoke(obj, param);
        }
    }
}
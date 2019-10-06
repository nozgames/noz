using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NoZ.Reflection
{
    public class OpenDelegateBase
    {
        public static OpenDelegateBase Create(MethodInfo info)
        {
            var parameters = info.GetParameters();
            switch (parameters.Length)
            {
                case 0:
                {
                    Type genericType = typeof(OpenDelegateImpl<>).MakeGenericType(new Type[] { info.DeclaringType });
                    return (OpenDelegateBase)Activator.CreateInstance(genericType, new object[] { info });
                }

                case 1:
                {
                    Type genericType = typeof(OpenDelegateImpl<,>).MakeGenericType(new Type[] { info.DeclaringType, parameters[0].ParameterType });
                    return (OpenDelegateBase)Activator.CreateInstance(genericType, new object[] { info });
                }

                default:
                    throw new ArgumentException("methods parameter count not supported");
            }
        }
    }

    public abstract class OpenDelegate : OpenDelegateBase
    {
        public abstract void Invoke (object target);
    }

    public abstract class OpenDelegate<TArg1> : OpenDelegateBase
    {
        public abstract void Invoke(object target, TArg1 arg1);
    }

    internal class OpenDelegateImpl<TTarget> : OpenDelegate where TTarget : class
    {
        public delegate void InvokeDelegate(TTarget target);

        private InvokeDelegate _invoke;

        public OpenDelegateImpl(MethodInfo methodInfo)
        {
            _invoke = (InvokeDelegate)Delegate.CreateDelegate(typeof(InvokeDelegate), methodInfo);
        }

        public void Invoke(TTarget target) => _invoke(target);

        public override void Invoke(object target) => _invoke((TTarget)target);
    }

    public class OpenDelegateImpl<TTarget,TArg1> : OpenDelegate<TArg1> where TTarget : class
    {
        public delegate void InvokeDelegate (TTarget target, TArg1 arg1);

        private InvokeDelegate _invoke;

        public OpenDelegateImpl(MethodInfo methodInfo)
        {
            _invoke = (InvokeDelegate)Delegate.CreateDelegate(typeof(InvokeDelegate), methodInfo);
        }

        public void Invoke (TTarget target, TArg1 arg1) => _invoke(target, arg1);

        public override void Invoke (object target, TArg1 arg1) => _invoke((TTarget)target, arg1);
    }
}

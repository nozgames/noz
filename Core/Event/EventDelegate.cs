/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NoZ
{
    internal static class EventDelegate
    {
        private static Dictionary<MethodInfo, Delegate> _cache = new Dictionary<MethodInfo, Delegate>();

        public static Delegate Create(Delegate d)
        {
            if (d.Target == null)
                return d;

            if (_cache.TryGetValue(d.Method, out var cached))
                return cached;

            var parameters = d.Method.GetParameters();
            var parameterExpressions = new ParameterExpression[parameters.Length + 1];
            var callArguments = new Expression[parameters.Length];

            parameterExpressions[0] = Expression.Parameter(typeof(object)); // d.Target.GetType());

            for (int i = 0; i < callArguments.Length; i++)
            {
                parameterExpressions[i + 1] = Expression.Parameter(parameters[i].ParameterType);
                callArguments[i] = parameterExpressions[i + 1];
            }

            cached = Expression.Lambda(
                Expression.Call(
                    Expression.Convert(parameterExpressions[0], d.Method.DeclaringType),
                    d.Method,
                    callArguments),
                parameterExpressions
            ).Compile();

            _cache[d.Method] = cached;

            return cached;
        }
    }
}

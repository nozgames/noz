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
using System.Reflection;
using System.Collections.Generic;

using NoZ.Reflection;

namespace NoZ
{
    internal class StateMachineInfo
    {
        /// <summary>
        /// Cache of state machine info for each type it is used on
        /// </summary>
        private static Dictionary<Type, StateMachineInfo> _cache = new Dictionary<Type, StateMachineInfo>(ReferenceEqualityComparer<Type>.Instance);

        /// <summary>
        /// Target type 
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// All available states
        /// </summary>
        public StateInfo[] States { get; private set; }

        /// <summary>
        /// All available triggers
        /// </summary>
        public StateTriggerInfo[] Triggers { get; private set; }

        /// <summary>
        /// All available transition
        /// </summary>
        public StateTransitionInfo[] Transitions{ get; private set; }

        public StateInfo GetState(string name)
        {
            foreach (var state in States)
                if (state.Name == name)
                    return state;

            return null;
        }

        /// <summary>
        /// Create state machine info for a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static StateMachineInfo Create (Type type)
        {
            if (_cache.TryGetValue(type, out var info))
                return info;

            // Collect all info from the type
            var states = new List<StateInfo>();
            var triggers = new List<StateTriggerInfo>();
            var transitions = new List<StateTransitionInfo>();
            GetInfo(type, states, triggers, transitions);

            // Validate that all the states referenced were found
            foreach(var state in states)
                GetStateDelegate(state, type);

            info = new StateMachineInfo();
            info.TargetType = type;
            info.States = states.ToArray();
            info.Triggers = triggers.ToArray();
            info.Transitions = transitions.ToArray();
            return info;
        }

        private static void GetStateDelegate (StateInfo state, Type type)
        {
            var method = type.GetMethod(state.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(float) }, null);
            if (method != null)
            {
                state.InvokeWithTimeDelegate = (OpenDelegate<float>)OpenDelegate.Create(method as MethodInfo);
                return;
            }

            method = type.GetMethod(state.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
            if(method != null)
            {
                state.InvokeDelegate = (OpenDelegate)OpenDelegate.Create(method as MethodInfo);
                return;
            }

            if (type.BaseType != null)
                GetStateDelegate(state, type.BaseType);
        }

        private static StateInfo GetState(List<StateInfo> states, string name)
        {
            if (name == null)
                return null;

            foreach (var state in states)
                if (state.Name == name)
                    return state;

            var newState = new StateInfo { Name = name, Mask = ((ulong)1) << states.Count };
            states.Add(newState);
            return newState;
        }

        private static void GetInfo (Type type, List<StateInfo> states, List<StateTriggerInfo> triggers, List<StateTransitionInfo> transitions)
        {
            // Recurse through all parent types as well
            if (type.BaseType != null)
                GetInfo(type.BaseType, states, triggers, transitions);

            // Get all public and private methods
            var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach(var member in members)
            {
                switch(member.MemberType)
                {
                    case MemberTypes.Field:
                    case MemberTypes.Property:
                    {
                        foreach(var attr in member.GetCustomAttributes<StateTriggerAttribute>())
                        {
                            var trigger = new StateTriggerInfo();
                            trigger.From = GetState(states,attr.From);
                            trigger.To = GetState(states,attr.To);
                            trigger.TargetValue = attr.Value;
                            trigger.Getter = (FastGetter<bool>)FastGetter.Create(member);

                            if (trigger.To == null)
                                throw new InvalidOperationException("trigger to state must not be null");

                            triggers.Add(trigger);
                        }
                        break;
                    }

                    case MemberTypes.Method:
                    {
                        // Get transitions
                        foreach (var attr in member.GetCustomAttributes<StateTransitionAttribute>())
                        {
                            var trans = new StateTransitionInfo();
                            trans.From = GetState(states, attr.From);
                            trans.To = GetState(states, attr.To);
                            if (((member as MethodInfo).GetParameters()).Length == 0)
                                trans.InvokeDelegate = (OpenDelegate)OpenDelegate.Create(member as MethodInfo);
                            else
                                throw new InvalidOperationException("state transitions must return void and take no parameter or one parameter as a float");

                            if (trans.To == null)
                                throw new InvalidOperationException("transition to state must not be null");

                            transitions.Add(trans);
                        }

#if false
                        // Get states
                        foreach (var attr in member.GetCustomAttributes<StateAttribute>())
                        {
                            var state = GetState(states, attr.Name??member.Name);
                            var parameters = (member as MethodInfo).GetParameters();
                            if (parameters.Length == 0)
                                state.InvokeDelegate = (OpenDelegate)OpenDelegate.Create(member as MethodInfo);
                            else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(float))
                                state.InvokeWithTimeDelegate = (OpenDelegate<float>)OpenDelegate.Create(member as MethodInfo);
                            else
                                throw new InvalidOperationException("states must return void and take no parameter or one parameter as a float");
                        }
#endif
                        break;
                    }
                }
            }
        }
    }
}

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

namespace NoZ
{
    public class StateMachine
    {
        private static ObjectPool<StateMachine> _stateMachinePool = new NoZ.ObjectPool<StateMachine>(() => new StateMachine(), 128);

        // TODO: one list of state machines for each update mode?
        private static List<StateMachine> _stateMachines = new List<StateMachine>();

        private StateMachineInfo _sminfo;
        private string _key;
        private UpdateMode _updateMode;
        private StateInfo _state;
        private object _target;
        private float _elapsedTimeInState;

        /// <summary>
        /// Start a new state machine on the given target using the given initial state
        /// </summary>
        public static void Start(Object target, string initialState) => Start(target, initialState, null, UpdateMode.Update);

        /// <summary>
        /// Start a new state machine on the given target using the given initial state
        /// </summary>
        public static void Start(Object target, string initialState, string key, UpdateMode mode)
        {
            var sm = _stateMachinePool.Get();
            sm._sminfo = StateMachineInfo.Create(target.GetType());
            sm._key = key;
            sm._target = target;
            sm._updateMode = mode;
            sm._elapsedTimeInState = 0.0f;

            // Track all running state machines
            _stateMachines.Add(sm);

            // Set the initial state
            sm.SetState(initialState);
        }

        /// <summary>
        /// Update all state machines on the given update mode
        /// </summary>
        /// <param name="updateMode"></param>
        public static void Update(UpdateMode updateMode) 
        {
            for(int i=0; i<_stateMachines.Count; i++)
            {
                var sm = _stateMachines[i];

                // Handle node specific logic
                if (sm._target is Node node)
                {
                    // Handle destroyed nodes
                    if(node.IsDestroyed)
                    {
                        _stateMachines.RemoveAt(i);
                        _stateMachinePool.Release(sm);
                        i--;
                        continue;
                    }

                    // If the scene is paused then pause the state machine too
                    if (node.Scene == null || node.Scene.IsPaused)
                        continue;
                }

                sm._elapsedTimeInState += Time.DeltaTime;

                // Update the triggers for the current state
                sm.UpdateTriggers();

                ulong mask = 0;
                while((mask & sm._state.Mask) == 0)
                {
                    mask |= sm._state.Mask;
                    sm._state.Invoke(sm._target, sm._elapsedTimeInState);
                    sm.UpdateTriggers();
                }
            }
        }

        private bool UpdateTriggers ()
        {
            foreach(var trigger in _sminfo.Triggers)
            {
                // Trigger is only valid when in the from state
                if (trigger.From != null && trigger.From != _state)
                    continue;

                // Get the current trigger value
                var value = trigger.Getter.GetValue(_target);

                // If the value of the trigger matches then fire the trigger
                if (value == trigger.TargetValue)
                {
                    SetState(trigger.To);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Set the current state to the state with the given name
        /// </summary>
        private void SetState (string stateName)
        {
            var state = _sminfo.GetState(stateName);
            if (null == state)
                throw new ArgumentException("unknown state");

            SetState(state);
        }

        /// <summary>
        /// Set the current state using state info
        /// </summary>
        private void SetState (StateInfo state)
        {
            if (_state == state)
                return;

            // Execute all transitions
            foreach (var trans in _sminfo.Transitions)
                if (trans.To == state && (trans.From == null || trans.From == _state))
                    trans.Invoke(_target);

            _elapsedTimeInState = 0.0f;
            _state = state;
        }

        /// <summary>
        /// Set a specific state in the state machine running on a given target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="stateName"></param>
        public static void SetState (NoZ.Object target, string stateName, string key=null)
        {
            foreach (var sm in _stateMachines)
                if(sm._target == target && (key == null || key == sm._key))
                    sm.SetState(stateName);
        }

    }
}

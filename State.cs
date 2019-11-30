using System;
using System.Collections.Generic;

namespace ML_HW6
{
    public class State
    {
        public int StateNum { get; set; }
        public float InitialReward { get; set; }
        public float CurrentReward { get; set; }
        public int CurrentPolicyActionNum { get; set; }
        public List<Action> Actions { get; set; }

        public State()
        {
            Actions = new List<Action>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ML_HW6
{
    class Program
    {

        static int numStates;
        static int numActions;
        static float discount;

        static List<State> states;

        static void Main(string[] args)
        {
            ReadArgumentsIntoFields(args);

            for(int i = 1; i <= 20; i++)
            {
                Calculate(i);
            }
        }

        static void Calculate(int iteration)
        {
            Console.Write($"After iteration {iteration}:");

            foreach (State state in states)
            {
                List<float> outcomes = new List<float>();

                // For each action
                for (int i = 1; i <= numActions; i++)
                {
                    outcomes.Add(GetOutcome(state, i));
                }

                // Plus one because outcomes list is 0 based while action num are 1 based
                state.CurrentPolicyActionNum = outcomes.IndexOf(outcomes.Max()) + 1;
                state.CurrentReward = outcomes.Max();

                Console.Write($" (s{state.StateNum} a{state.CurrentPolicyActionNum} {state.CurrentReward})");
            }

            Console.Write('\n');
        }

        static float GetOutcome(State state, int actionNum)
        {
            IEnumerable<Action> outcomesForAction = state.Actions.Where(a => a.ActionNumber == actionNum);
            float summation = 0f;

            foreach (Action action in outcomesForAction)
            {
                // Using First() cause I don't want this to swallow exception and cause incorrect calc
                summation += action.Prob * states.First(s => s.StateNum == action.Dest).CurrentReward;
            }

            return summation * discount + state.InitialReward;
        }

        static bool ReadArgumentsIntoFields(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("ERROR: 4 arguments are required: num of states, num of actions, input file, and discount factor");
                return false;
            }

            if (!int.TryParse(args[0], out numStates))
            {
                Console.WriteLine("ERROR: Arg 1 must be an int");
                return false;
            }

            if (!int.TryParse(args[1], out numActions))
            {
                Console.WriteLine("ERROR: Arg 2 must be an int");
                return false;
            }

            if (!float.TryParse(args[3], out discount))
            {
                Console.WriteLine("ERROR: Arg 4 must be an float");
                return false;
            }

            return ReadFileIntoFields(args[2]);
        }

        static bool ReadFileIntoFields(string filename)
        {
            try
            {
                states = new List<State>();
                StreamReader file = new StreamReader(filename);
                string line;


                while ((line = file.ReadLine()) != null)
                {
                    State state = new State();

                    string[] parenGroups = line.Split('(');

                    // First group is name / value
                    string[] firstGroups = parenGroups[0].Split(' ');
                    float.TryParse(firstGroups[1], out float reward);
                    state.InitialReward = reward;
                    state.CurrentReward = reward;

                    int.TryParse(firstGroups[0].Substring(1), out int stateNum);
                    state.StateNum = stateNum;

                    for (int i = 1; i < parenGroups.Length; i++)
                    {
                        string[] innerGroups = parenGroups[i].Split(')')[0].Split(' ');
                        Action action = new Action();

                        int.TryParse(innerGroups[0].Substring(1), out int actionNum);
                        action.ActionNumber = actionNum;

                        int.TryParse(innerGroups[1].Substring(1), out int dest);
                        action.Dest = dest;

                        float.TryParse(innerGroups[2], out float prob);
                        action.Prob = prob;

                        state.Actions.Add(action);
                    }

                    states.Add(state);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Error reading in file");
                return false;
            }

            return true;
        }
    }
}

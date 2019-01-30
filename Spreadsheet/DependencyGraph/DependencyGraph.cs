// Skeleton implementation written by Joe Zachary for CS 3500, January 2018.
// Edited by Justin Francis, jan 2019


using System;
using System.Collections.Generic;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        //item1 = dependee list, item2 = dependent list
        private Dictionary<string, Tuple<Dictionary<string, string>, Dictionary<string, string>>> adjList;
        private int size;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            adjList = new Dictionary<string, Tuple<Dictionary<string, string>, Dictionary<string, string>>>();
            size = 0;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependents(string s)
        {
            if(adjList.TryGetValue(s, out Tuple<Dictionary<string,string>, Dictionary<string,string>> dependencies))
            {
               return dependencies.Item2.GetEnumerator().MoveNext();
            }

            return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (adjList.TryGetValue(s, out Tuple<Dictionary<string, string>, Dictionary<string, string>> dependencies))
            {
                return dependencies.Item1.GetEnumerator().MoveNext();
            }

            return false;
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (adjList.TryGetValue(s, out Tuple<Dictionary<string, string>, Dictionary<string, string>> dependencies))
            {
                return dependencies.Item2.Values;
            }

            return null;
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (adjList.TryGetValue(s, out Tuple<Dictionary<string, string>, Dictionary<string, string>> dependencies))
            {
                return dependencies.Item1.Values;
            }

            return null;
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void AddDependency(string s, string t)
        { //s = dependee, t = dependent
            bool dependentExist; //space saver
            bool dependeeExist;

            //check if the dependent does not exists in the top level container
            if ((dependentExist = adjList.TryGetValue(t, out Tuple<Dictionary<string, string>, Dictionary<string, string>> dependentDependencies)) == false)
            {
                Dictionary<string,string> dependeeDict = new Dictionary<string, string>(); //create new dependee dict for new top level dependent
                dependeeDict.Add(s, s); //add dependee to bottom level dict

                adjList.Add(t, new Tuple<Dictionary<string, string>, Dictionary<string, string>>(dependeeDict, new Dictionary<string, string>())); //add top level dependent with new dependee dict containing dependency
                size++;
            }
            //check if the dependent exists in the top level container
            else if(dependentExist == true)
            {
                //check if desired dependency does not exists in low level container
                if (!dependentDependencies.Item1.ContainsKey(s))
                {
                    dependentDependencies.Item1.Add(s, s); //add depedee dependency
                    size++;
                }
            }


            //check if the dependee does not exists in the top level container
            if ((dependeeExist = adjList.TryGetValue(s, out Tuple<Dictionary<string, string>, Dictionary<string, string>> dependeeDependencies)) == false)
            {
                Dictionary<string, string> dependentDict = new Dictionary<string, string>(); //create new dependent dict for new top level dependent
                dependentDict.Add(t, t); //add dependent to bottom level dict

                adjList.Add(s, new Tuple<Dictionary<string, string>, Dictionary<string, string>>(new Dictionary<string, string>(), dependentDict)); //add top level dependee with new dependent dict containing dependency
            }
            //check if the dependee exists in the top level container
            else if (dependeeExist == true)
            {
                //check if desired dependency does not exists in low level container
                if (!dependeeDependencies.Item2.ContainsKey(t))
                {
                    dependeeDependencies.Item2.Add(t, t); //add depedent dependency
                }
            }
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {// s = dependee, t = dependent
            bool sizeChange = false;

            //check if dependent does exist in top level container
            if(adjList.TryGetValue(t, out Tuple<Dictionary<string,string>, Dictionary<string, string>> dependentDependencies) == true)
            {
                //check if dependency exist in bottom level container
                if ((sizeChange = dependentDependencies.Item1.ContainsKey(s)) == true)
                {
                    dependentDependencies.Item1.Remove(s); //remove (s,t) dependency from bot level container
                }
            }
          
            //check if dependee exists in the top level container
            if(adjList.TryGetValue(s, out Tuple<Dictionary<string, string>, Dictionary<string, string>> dependeeDependencies))
            {
                //check if dependency exists in bot level container
                if (dependeeDependencies.Item2.ContainsKey(t))
                {
                    dependeeDependencies.Item2.Remove(t);//remove (s,t) dependency from bot level container
                }
            }

            //see if a dependency was removed
            if (sizeChange)
            {
                size--;
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        { //s = dependee, t = dependent
            List<string> oldDependents = new List<string>();

            //check if dependee exists in the top level container
            if (adjList.TryGetValue(s, out Tuple<Dictionary<string, string>, Dictionary<string, string>> dependencies))
            {
                //check if there are any dependents associated in bot level container
                if (dependencies.Item2.Count != 0)
                {
                    //add all dependencies in the form (s,r) to a reference list bc you cannot enumerate and modify a dict
                    foreach(string removeKey in dependencies.Item2.Keys)
                    {
                        oldDependents.Add(removeKey);
                    }
                    //remove each dependency in the form (s,r) from graph 
                    foreach (string r in oldDependents)
                    {
                        RemoveDependency(s, r); //this method takes care of exist/nonexist and size update
                    }
                }
            }

            //add all new dependents
            foreach(string t in newDependents)
            {
                AddDependency(s, t); //this method takes care of a non existant s in top level container, if one exists it adds dependency
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        { //s = dependee, t = dependent
            List<string> oldDependees = new List<string>();

            //check if dependent exists in the top level container
            if (adjList.TryGetValue(t, out Tuple<Dictionary<string, string>, Dictionary<string, string>> dependencies))
            {
                //check if there is a bot level container for dependees so we don't throw a nullpointer 
                if (dependencies.Item1.Count != 0)
                {
                    //add all dependees to list bc dict cannot be enumerated and modified concurrently
                    foreach(string removeKey in dependencies.Item1.Keys)
                    {
                        oldDependees.Add(removeKey);
                    }
                    //remove all dependencies in the form (r,t)
                    foreach (string r in oldDependees)
                    {
                        RemoveDependency(r, t); //this method takes care of exist/nonexist and size update
                    }
                }
            }

            //add all new dependees
            foreach (string s in newDependees)
            {
                AddDependency(s, t); //this method takes care of a non existant s in top level container, if one exists it adds dependency
            }

        }
    }
}

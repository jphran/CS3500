// Skeleton implementation written by Joe Zachary for CS 3500, January 2018.
// Edited by Justin Francis, jan 2019
// Ready for Submission, 1/31/19 

//Revised by Justin Francis, Feb 2019
//Ready for submission, 2.8.19 


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
    /// If a paramaeter to a method below is null, the method will throw ArgumentNullException with appropriate 
    /// message
    /// </summary>
    public class DependencyGraph : ICloneable
    {
        //item1 = dependee list, item2 = dependent list
        private Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> adjList;
        private int size;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            adjList = new Dictionary<string, Tuple<HashSet<string>, HashSet<string>>>();
            size = 0;
        }
        
        /// <summary>
        /// Creates a new DependencyGraph containing all dependencies contained in Dependency 
        /// Graph param
        /// </summary>
        /// <param name="dg"></param>
        public DependencyGraph(DependencyGraph dg)
        {
            this.adjList = (Dictionary<string, Tuple<HashSet<string>, HashSet<string>>>) dg.Clone();
            this.size = dg.Size;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Throws ArgumentNullException if s == null.
        /// </summary>
        public bool HasDependents(string s)
        {
            //check if param is null
            if(s == null)
            {
                throw new ArgumentNullException("HasDependents entered param is null, please revise");
            }

            //check if the string s exists as a dependee in hashmap
            if(adjList.TryGetValue(s, out Tuple<HashSet<string>, HashSet<string>> dependencies))
            {
               return dependencies.Item2.GetEnumerator().MoveNext();
            }

            return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Throw ArgumentNullException if s == null.
        /// </summary>
        public bool HasDependees(string s)
        {
            //check if param is null
            if (s == null)
            {
                throw new ArgumentNullException("HasDependees entered param is null, please revise");
            }

            //check if desired dependent exists
            if (adjList.TryGetValue(s, out Tuple<HashSet<string>, HashSet<string>> dependencies))
            {
                return dependencies.Item1.GetEnumerator().MoveNext();
            }

            return false;
        }

        /// <summary>
        /// Enumerates dependents(s).  Throws ArgumentNullException if s == null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            //check if param is null
            if (s == null)
            {
                throw new ArgumentNullException("GetDependents entered param is null, please revise");
            }

            //check if dependee exists
            if (adjList.TryGetValue(s, out Tuple<HashSet<string>, HashSet<string>> dependencies))
            {
                return dependencies.Item2;
            }

            return new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).  Throws ArgumentNullException if s == null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            //check if param is null
            if (s == null)
            {
                throw new ArgumentNullException("HasDependents entered param is null, please revise");
            }

            //check if dependent exists
            if (adjList.TryGetValue(s, out Tuple<HashSet<string>, HashSet<string>> dependencies))
            {
                return dependencies.Item1;
            }

            return new HashSet<string>();
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Throws ArgumentNullException if s == null or t == null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            //check if either param is null
            if (s == null || t == null)
            {
                throw new ArgumentNullException("AddDependency entered param is null, please revise");
            }

            //s = dependee, t = dependent
            bool dependentExist; //space saver
            bool dependeeExist;

            //check if the dependent does not exists in the top level container
            if ((dependentExist = adjList.TryGetValue(t, out Tuple<HashSet<string>, HashSet<string>> dependentDependencies)) == false)
            {
                HashSet<string> dependeeHash = new HashSet<string>
                {
                    s //add dependee to bottom level dict
                }; //create new dependee dict for new top level dependent

                adjList.Add(t, new Tuple<HashSet<string>, HashSet<string>>(dependeeHash, new HashSet<string>())); //add top level dependent with new dependee dict containing dependency
                size++;
            }
            //check if the dependent exists in the top level container
            else if(dependentExist == true)
            {
                //check if desired dependency does not exists in low level container
                if (!dependentDependencies.Item1.Contains(s))
                {
                    dependentDependencies.Item1.Add(s); //add depedee dependency
                    size++;
                }
            }


            //check if the dependee does not exists in the top level container
            if ((dependeeExist = adjList.TryGetValue(s, out Tuple<HashSet<string>, HashSet<string>> dependeeDependencies)) == false)
            {
                HashSet<string> dependentHash = new HashSet<string>(); //create new dependent dict for new top level dependent
                dependentHash.Add(t); //add dependent to bottom level dict

                adjList.Add(s, new Tuple<HashSet<string>, HashSet<string>>(new HashSet<string>(), dependentHash)); //add top level dependee with new dependent dict containing dependency
            }
            //check if the dependee exists in the top level container
            else if (dependeeExist == true)
            {
                //check if desired dependency does not exists in low level container
                if (!dependeeDependencies.Item2.Contains(t))
                {
                    dependeeDependencies.Item2.Add(t); //add depedent dependency
                }
            }
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Throws new ArgumentNullException if s == null or t == null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            //check if param is null
            if (s == null || t == null)
            {
                throw new ArgumentNullException("RemoveDependency entered param is null, please revise");
            }

            // s = dependee, t = dependent
            bool sizeChange = false;

            //check if dependent does exist in top level container
            if(adjList.TryGetValue(t, out Tuple<HashSet<string>, HashSet<string>> dependentDependencies) == true)
            {
                //check if dependency exist in bottom level container
                if ((sizeChange = dependentDependencies.Item1.Contains(s)) == true)
                {
                    dependentDependencies.Item1.Remove(s); //remove (s,t) dependency from bot level container
                }
            }
          
            //check if dependee exists in the top level container
            if(adjList.TryGetValue(s, out Tuple<HashSet<string>, HashSet<string>> dependeeDependencies))
            {
                //check if dependency exists in bot level container
                if (dependeeDependencies.Item2.Contains(t))
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
        /// Throws new ArgumentNullException if s == null or t == null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //check if param is null
            if (s == null || newDependents == null)
            {
                throw new ArgumentNullException("ReplaceDependents entered param is null, please revise");
            }

            //s = dependee, t = dependent
            List<string> oldDependents = new List<string>();

            //check if dependee exists in the top level container
            if (adjList.TryGetValue(s, out Tuple<HashSet<string>, HashSet<string>> dependencies))
            {
                //check if there are any dependents associated in bot level container
                if (dependencies.Item2.Count != 0)
                {
                    //add all dependencies in the form (s,r) to a reference list bc you cannot enumerate and modify a dict
                    foreach(string removeKey in dependencies.Item2)
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
                //check null dependent
                if(t == null)
                {
                    throw new ArgumentNullException("Null Dependent in ReplaceDependents IEnumerable, please revise");
                }

                AddDependency(s, t); //this method takes care of a non existant s in top level container, if one exists it adds dependency
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Throws new ArgumentNullException if  s == null or t == null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            //check if param is null
            if (t == null || newDependees == null)
            {
                throw new ArgumentNullException("ReplaceDependees entered param is null, please revise");
            }
            //s = dependee, t = dependent
            List<string> oldDependees = new List<string>();

            //check if dependent exists in the top level container
            if (adjList.TryGetValue(t, out Tuple<HashSet<string>, HashSet<string>> dependencies))
            {
                //check if there is a bot level container for dependees so we don't throw a nullpointer 
                if (dependencies.Item1.Count != 0)
                {
                    //add all dependees to list bc dict cannot be enumerated and modified concurrently
                    foreach(string removeKey in dependencies.Item1)
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
                //check null value
                if(s == null)
                {
                    throw new ArgumentNullException("Null dependee in ReplaceDependees IEnumerable, please revise");
                }

                AddDependency(s, t); //this method takes care of a non existant s in top level container, if one exists it adds dependency
            }

        }

        /// <summary>
        /// Creates a deep clone of dependency graph, returns type
        /// Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> </string></string>
        /// which is the underlying adjacency list
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            //hey so whoever reads this and knows a bit about this code stuff
            //is there a better way of creating and passing a clone of dependencies?
            //like can I somehow create a private method or something? I just feel like having
            //a public cloning method is bad form, PLZ LEMME KNOW CAUSE I DON'T KNOW ANYTHING
            //like I don't know how to access the underlying structure to efficiently capture all
            //key, value pairs in the underlying dictionary bc its a private variable as Dr Zachary
            //recommended
            Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> copy = new Dictionary<string, Tuple<HashSet<string>, HashSet<string>>>();
            IEnumerable<string> keys = this.adjList.Keys; //get keys from adjList

            //iterate through keys to deep clone dictionary
            foreach (string key in keys)
            {
                HashSet<string> copyDependees; 
                HashSet<string> copyDependents; 
                
                //this is just good practice apparently
                if (adjList.TryGetValue(key, out Tuple<HashSet<string>, HashSet<string>> dependencies))
                {
                    copyDependees = new HashSet<string>(dependencies.Item1); //copy dependees
                    copyDependents = new HashSet<string>(dependencies.Item2); //copy dependents
                    copy.Add(key, new Tuple<HashSet<string>, HashSet<string>>(copyDependees, copyDependents)); //add dependencies to copy
                }
            }
            return copy;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManager
{
    public class Manager : IManager
    {
        private readonly Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        private readonly Dictionary<string, HashSet<string>> dependencies = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> dependents = new Dictionary<string, HashSet<string>>();

        public void AddTask(Task task)
        {
            if (tasks.ContainsKey(task.Id))
            {
                throw new ArgumentException("Task with the same ID already exists.");
            }

            tasks[task.Id] = task;
            dependencies[task.Id] = new HashSet<string>();
            dependents[task.Id] = new HashSet<string>();
        }

        public void RemoveTask(string taskId)
        {
            if (!tasks.ContainsKey(taskId))
            {
                throw new ArgumentException("Task ID does not exist.");
            }

            foreach (var dep in dependencies[taskId])
            {
                dependents[dep].Remove(taskId);
            }

            foreach (var dep in dependents[taskId])
            {
                dependencies[dep].Remove(taskId);
            }

            tasks.Remove(taskId);
            dependencies.Remove(taskId);
            dependents.Remove(taskId);
        }

        public bool Contains(string taskId)
        {
            return tasks.ContainsKey(taskId);
        }

        public Task Get(string taskId)
        {
            if (!tasks.ContainsKey(taskId))
            {
                throw new ArgumentException("Task ID does not exist.");
            }

            return tasks[taskId];
        }

        public IEnumerable<Task> GetDependencies(string taskId)
        {
            if (!tasks.ContainsKey(taskId))
            {
                return Enumerable.Empty<Task>();
            }

            var allDependencies = new HashSet<string>();
            CollectAllDependencies(taskId, allDependencies);
            return allDependencies.Select(id => tasks[id]);
        }

        private void CollectAllDependencies(string taskId, HashSet<string> allDependencies)
        {
            foreach (var dependencyId in dependencies[taskId])
            {
                if (allDependencies.Add(dependencyId))
                {
                    CollectAllDependencies(dependencyId, allDependencies);
                }
            }
        }

        public IEnumerable<Task> GetDependents(string taskId)
        {
            if (!tasks.ContainsKey(taskId))
            {
                return Enumerable.Empty<Task>();
            }

            var allDependents = new HashSet<string>();
            CollectAllDependents(taskId, allDependents);
            return allDependents.Select(id => tasks[id]);
        }

        private void CollectAllDependents(string taskId, HashSet<string> allDependents)
        {
            foreach (var dependentId in dependents[taskId])
            {
                if (allDependents.Add(dependentId))
                {
                    CollectAllDependents(dependentId, allDependents);
                }
            }
        }

        public void AddDependency(string taskId, string dependentTaskId)
        {
            if (!tasks.ContainsKey(taskId) || !tasks.ContainsKey(dependentTaskId))
            {
                throw new ArgumentException("One or both task IDs do not exist.");
            }

            if (IsCircularDependency(taskId, dependentTaskId))
            {
                throw new ArgumentException("Circular dependency detected.");
            }

            dependencies[taskId].Add(dependentTaskId);
            dependents[dependentTaskId].Add(taskId);
        }

        private bool IsCircularDependency(string startTaskId, string newDependencyId)
        {
            var visited = new HashSet<string>();
            var stack = new Stack<string>();
            stack.Push(newDependencyId);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current == startTaskId)
                {
                    return true;
                }

                if (visited.Add(current))
                {
                    foreach (var dependency in dependencies[current])
                    {
                        stack.Push(dependency);
                    }
                }
            }

            return false;
        }

        public void RemoveDependency(string taskId, string dependentTaskId)
        {
            if (!tasks.ContainsKey(taskId) || !tasks.ContainsKey(dependentTaskId))
            {
                throw new ArgumentException("One or both task IDs do not exist.");
            }

            if (!dependencies[taskId].Contains(dependentTaskId))
            {
                throw new ArgumentException("Dependency does not exist.");
            }

            dependencies[taskId].Remove(dependentTaskId);
            dependents[dependentTaskId].Remove(taskId);
        }

        public int Size()
        {
            return tasks.Count;
        }
    }
}

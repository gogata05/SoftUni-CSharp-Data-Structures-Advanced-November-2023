using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.TaskManager
{
    public class TaskManager : ITaskManager
    {
        private Dictionary<string, Task> tasksById = new Dictionary<string, Task>();
        private LinkedList<Task> executionQueue = new LinkedList<Task>();
        private HashSet<Task> executedTasks = new HashSet<Task>();

        public void AddTask(Task task)
        {
            if (this.tasksById.ContainsKey(task.Id))
            {
                throw new ArgumentException("Task with the same ID already exists.");
            }

            this.tasksById[task.Id] = task;
            this.executionQueue.AddLast(task);
        }

        public bool Contains(Task task)
        {
            return this.tasksById.ContainsKey(task.Id);
        }

        public void DeleteTask(string taskId)
        {
            if (!this.tasksById.TryGetValue(taskId, out var task))
            {
                throw new ArgumentException("Task not found.");
            }

            this.tasksById.Remove(taskId);
            this.executionQueue.Remove(task);
            this.executedTasks.Remove(task);
        }

        public Task ExecuteTask()
        {
            if (this.executionQueue.Count == 0)
            {
                throw new InvalidOperationException("No tasks available to execute.");
            }

            var taskToExecute = this.executionQueue.First.Value;
            this.executionQueue.RemoveFirst();
            this.executedTasks.Add(taskToExecute);
            return taskToExecute;
        }

        public IEnumerable<Task> GetAllTasksOrderedByEETThenByName()
        {
            return this.executionQueue
                .Concat(this.executedTasks)
                .OrderByDescending(t => t.EstimatedExecutionTime)
                .ThenBy(t => t.Name.Length)
                .ThenBy(t => t.Name);
        }

        public IEnumerable<Task> GetDomainTasks(string domain)
        {
            return this.executionQueue
                .Where(t => t.Domain == domain)
                .Concat(this.executedTasks.Where(t => t.Domain == domain));
        }

        public Task GetTask(string taskId)
        {
            if (!this.tasksById.TryGetValue(taskId, out var task))
            {
                throw new ArgumentException("Task not found.");
            }

            return task;
        }

        public IEnumerable<Task> GetTasksInEETRange(int lowerBound, int upperBound)
        {
            return this.executionQueue
                .Where(t => t.EstimatedExecutionTime >= lowerBound && t.EstimatedExecutionTime <= upperBound)
                .Concat(this.executedTasks.Where(t => t.EstimatedExecutionTime >= lowerBound && t.EstimatedExecutionTime <= upperBound));
        }

        public void RescheduleTask(string taskId)
        {
            if (!this.tasksById.TryGetValue(taskId, out var task) || !this.executedTasks.Contains(task))
            {
                throw new ArgumentException("Task not found.");
            }

            this.executedTasks.Remove(task);
            this.executionQueue.AddLast(task);
        }

        public int Size()
        {
            return this.executionQueue.Count;
        }
    }
}

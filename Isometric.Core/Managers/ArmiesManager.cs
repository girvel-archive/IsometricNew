using System;
using System.Collections.Generic;
using System.Linq;
using Isometric.Core.Managers.Tasks;
using Isometric.Core.Time;
using Isometric.Core.Vectors;

namespace Isometric.Core.Managers
{
    public class ArmiesManager : ITimeObject
    {
        private readonly Dictionary<Army, Queue<ArmyTask>> _tasks = new Dictionary<Army, Queue<ArmyTask>>();

        private readonly object _tasksLock = new object();



        public delegate void TaskEndAction(Army army, ArmyTask previous, ArmyTask next);
        
        public TaskEndAction OnTaskFinished { get; set; }



        public void ClearQueue(Army army)
        {
            lock (_tasksLock)
            {
                _tasks.TryGetValue(army, out Queue<ArmyTask> queue);
                queue?.Clear();
            }
        }
        
        
        
        public void AddMovingTask(Army army, Vector to)
        {
            AddTask(new MovingTask {To = to, Army = army,});
        }

        public void AddAreaLootTask(Army army, Vector to, int range)
        {
            AddTask(new LootTask(army, to, range));
        }



        public void Tick(TimeSpan deltaTime)
        {
            lock (_tasksLock)
            {
                var tasksArray = _tasks.ToArray();

                foreach (var taskPair in tasksArray)
                {
                    if (!taskPair.Value.Any())
                    {
                        
                        _tasks.Remove(taskPair.Key);
                        continue;
                    };

                    if (taskPair.Value.Peek().Tick(deltaTime))
                    {
                        OnTaskFinished(
                            taskPair.Key, 
                            taskPair.Value.Dequeue(), 
                            taskPair.Value.Any() 
                                ? taskPair.Value.Peek() 
                                : null);
                    }
                }
            }
        }



        public bool IsBusy(Army army)
        {
            lock (_tasksLock)
            {
                return _tasks.Any(t => t.Key == army);
            }
        }

        public ArmyTask GetTask(Army army)
        {
            lock (_tasksLock)
            {
                Queue<ArmyTask> result;
                _tasks.TryGetValue(army, out result);

                return result?.Any() ?? false ? result.Peek() : null;
            }
        }


        
        private void AddTask(ArmyTask task)
        {
            lock (_tasksLock)
            {
                if (!_tasks.ContainsKey(task.Army))
                {
                    _tasks[task.Army] = new Queue<ArmyTask>();
                }
                
                _tasks[task.Army].Enqueue(task);
            }
        }
    }
}
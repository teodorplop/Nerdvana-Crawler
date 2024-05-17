namespace NerdvanaCrawler.Code.Utils
{
    public class TaskQueue
    {
        private class QueuedTask
        {
            private Func<Task> m_Task;
            private TaskCompletionSource m_TaskCompletion;

            public QueuedTask(Func<Task> task, TaskCompletionSource taskCompletion)
            {
                m_Task = task;
                m_TaskCompletion = taskCompletion;
            }

            public async Task Invoke()
            {
                if (m_Task != null)
                    await m_Task.Invoke();

                m_TaskCompletion?.TrySetResult();
            }
        }

        private Queue<QueuedTask> m_TaskQueue = new Queue<QueuedTask>();

        public int QueueSize => m_TaskQueue.Count;

        public async Task Enqueue(Func<Task> task)
        {
            var taskCompletion = new TaskCompletionSource();

            m_TaskQueue.Enqueue(new QueuedTask(task, taskCompletion));
            if (m_TaskQueue.Count == 1)
            {
                RunNextTask();
            }

            await taskCompletion.Task;
        }

        private async void RunNextTask()
        {
            if (!m_TaskQueue.TryPeek(out var task))
                return;

            if (task != null)
                await task.Invoke();

            m_TaskQueue.Dequeue();

            if (m_TaskQueue.Count > 0)
                RunNextTask();
        }
    }
}

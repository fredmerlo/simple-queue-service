using System.Collections.Concurrent;
using System.Linq;
using Simple.Queue;

namespace Simple.Queue
{
    public sealed class MessageQueue
    {
        private static readonly MessageQueue instance = new MessageQueue();
        private ConcurrentQueue<Message> queue;

        private MessageQueue()
        {
            queue = new ConcurrentQueue<Message>();
        }

        public static bool HasMessages { get { return !instance.queue.IsEmpty; } }

        public static void Enqueue(Message msg)
        {
            if (IsNotInQueue(msg))
            {
                instance.queue.Enqueue(msg);
            }
        }
        public static Message Dequeue()
        {
            Message msg = null;
            instance.queue.TryDequeue(out msg);

            return msg;
        }

        private static bool IsNotInQueue(Message msg)
        {
            if (msg.Payload == null || instance.queue.IsEmpty)
                return true;

            return instance.queue.ToArray().FirstOrDefault(s => s.Payload.Telephone == msg.Payload.Telephone) == null;
        }

    }
}

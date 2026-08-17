namespace Game.Core.Commands
{
    public sealed class CharacterCommandBuffer : ICharacterCommandBuffer
    {
        private readonly CommandNode[] _heap = new CommandNode[512];
        private ulong _nextSequence;

        public int Count { get; private set; }

        public bool TryEnqueue(in CharacterCommand command)
        {
            if (!command.CharacterId.IsValid)
                return false;

            if (TryCoalesce(command))
                return true;

            if (Count >= _heap.Length)
                return false;

            int index = Count++;
            _heap[index] = new CommandNode(command, _nextSequence++);
            
            SiftUp(index);
            return true;
        }

        private bool TryCoalesce(in CharacterCommand command)
        {
            if (command.Type != CharacterCommandType.Move)
                return false;

            for (int i = 0; i < Count; i++)
            {
                CharacterCommand queued = _heap[i].Command;
                if (queued.Tick != command.Tick || queued.CharacterId != command.CharacterId || queued.Type != command.Type)
                    continue;

                _heap[i] = new CommandNode(command, _heap[i].Sequence);
                return true;
            }

            return false;
        }

        public bool TryDequeueDue(uint currentTick, out CharacterCommand command)
        {
            if (Count == 0 || _heap[0].Command.Tick > currentTick)
            {
                command = default;
                return false;
            }

            command = _heap[0].Command;
            Count--;

            if (Count > 0)
            {
                _heap[0] = _heap[Count];
                SiftDown(0);
            }

            _heap[Count] = default;
            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                _heap[i] = default;
            }

            Count         = 0;
            _nextSequence = 0u;
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                
                if (!ComesBefore(_heap[index], _heap[parent]))
                    return;

                Swap(index, parent);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                int left = index * 2 + 1;
                if (left >= Count)
                    return;

                int right = left + 1;
                int next  = right < Count && ComesBefore(_heap[right], _heap[left])
                    ? right : left;

                if (!ComesBefore(_heap[next], _heap[index]))
                    return;

                Swap(index, next);
                index = next;
            }
        }

        private static bool ComesBefore(in CommandNode left, in CommandNode right)
            => left.Command.Tick < right.Command.Tick || left.Command.Tick == right.Command.Tick && left.Sequence < right.Sequence;

        private void Swap(int left, int right)
        {
            CommandNode temporary = _heap[left];
            _heap[left]  = _heap[right];
            _heap[right] = temporary;
        }

        private readonly struct CommandNode
        {
            public CharacterCommand Command  { get; }
            public ulong            Sequence { get; }

            public CommandNode(CharacterCommand command, ulong sequence)
            {
                Command  = command;
                Sequence = sequence;
            }
        }
    }
}

namespace NerdvanaCrawler.Code.ConsoleUI
{
    public class ConsoleMenuStack
    {
        private Stack<ConsoleMenu> m_Stack;

        public ConsoleMenuStack()
        {
            m_Stack = new Stack<ConsoleMenu>();
        }

        public void Push(ConsoleMenu menu)
        {
            if (m_Stack.Count > 0)
                m_Stack.Peek().Disable();

            m_Stack.Push(menu);
            Console.Clear();
            menu.Enable();
        }

        public void Pop()
        {
            if (m_Stack.Count > 0)
            {
                m_Stack.Pop().Disable();
                Console.Clear();

                if (m_Stack.Count > 0)
                    m_Stack.Peek().Enable();
            }
        }
    }
}


namespace CollabApp.mvc.Collections
{
    public class TrieNode {
        char Character { get; set; }
        public Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>();
        public bool IsEndOfWord { get; set; }

        public TrieNode(char Value)
        {
            Character = Value;
            IsEndOfWord = false;
        }
    }

    public class Trie {
        private readonly TrieNode Root;

        public Trie()
        {
            Root = new TrieNode('\0');
        }
        
        public void Insert(string word)
        {
            TrieNode currentNode = Root;
            foreach(char c in word)
            {
                if(!currentNode.Children.ContainsKey(c))
                    currentNode.Children[c] = new TrieNode(c);

                currentNode = currentNode.Children[c];
            }
            currentNode.IsEndOfWord = true;
        }

        public bool Find(string word)
        {
            TrieNode currentNode = Root;
            foreach(char c in word)
            {
                if(currentNode.Children.ContainsKey(c))
                    currentNode = currentNode.Children[c];
                else
                    return false;
            }

            return currentNode.IsEndOfWord;
        }
    }
}

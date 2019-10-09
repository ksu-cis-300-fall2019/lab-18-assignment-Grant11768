/* Dictionary.cs
 * Author: Rod Howell
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KansasStateUniversity.TreeViewer2;
using Ksu.Cis300.ImmutableBinaryTrees;

namespace Ksu.Cis300.NameLookup
{
    /// <summary>
    /// A generic dictionary in which keys must implement IComparable.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class Dictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// The keys and values in the dictionary.
        /// </summary>
        private BinaryTreeNode<KeyValuePair<TKey, TValue>> _elements = null;

        /// <summary>
        /// Gets a drawing of the underlying binary search tree.
        /// </summary>
        public TreeForm Drawing => new TreeForm(_elements, 100);

        /// <summary>
        /// Checks to see if the given key is null, and if so, throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="key">The key to check.</param>
        private static void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Finds the given key in the given binary search tree.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="t">The binary search tree.</param>
        /// <returns>The node containing key, or null if the key is not found.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Find(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t)
        {
            if (t == null)
            {
                return null;
            }
            else
            {
                int comp = key.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    return t;
                }
                else if (comp < 0)
                {
                    return Find(key, t.LeftChild);
                }
                else
                {
                    return Find(key, t.RightChild);
                }
            }
        }

        /// <summary>
        /// Builds the binary search tree that results from adding the given key and value to the given tree.
        /// If the tree already contains the given key, throws an ArgumentException.
        /// </summary>
        /// <param name="t">The binary search tree.</param>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        /// <returns>The binary search tree that results from adding k and v to t.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Add(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, TKey k, TValue v)
        {
            if (t == null)
            {
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(k, v), null, null);
            }
            else
            {
                int comp = k.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    throw new ArgumentException();
                }
                else if (comp < 0)
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Add(t.LeftChild, k, v), t.RightChild);
                }
                else
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Add(t.RightChild, k, v));
                }
            }
        }

        /// <summary>
        /// Tries to get the value associated with the given key.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k, or the default value if
        /// k is not in the dictionary.</param>
        /// <returns>Whether k was found as a key in the dictionary.</returns>
        public bool TryGetValue(TKey k, out TValue v)
        {
            CheckKey(k);
            BinaryTreeNode<KeyValuePair<TKey, TValue>> p = Find(k, _elements);
            if (p == null)
            {
                v = default(TValue);
                return false;
            }
            else
            {
                v = p.Data.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds the given key with the given associated value.
        /// If the given key is already in the dictionary, throws an
        /// InvalidOperationException.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void Add(TKey k, TValue v)
        {
            CheckKey(k);
            _elements = Add(_elements, k, v);
        }
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> RemoveMinimumKey(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out KeyValuePair<TKey, TValue> min)
        {
            if (t.LeftChild == null)
            {
                min = t.Data;
                return t.RightChild;
            }
            else
            {

                BinaryTreeNode<KeyValuePair<TKey, TValue>> temp = RemoveMinimumKey(t.LeftChild, out min);
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, temp, t.RightChild);
            }
        }

        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Remove(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out bool removed)
        {
            if(t == null)
            {
                removed = false;
                return null;
            }

            int compared = key.CompareTo(t.Data.Key);
            if(compared == 0)
            {
                if(t.LeftChild == null)
                {
                    removed = true;
                    return t.RightChild;
                }
                else if(t.RightChild == null)
                {
                    removed = true;
                    return t.LeftChild;
                }
                else
                {
                    removed = true;
                    KeyValuePair<TKey,TValue> tempKey;
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> temp = RemoveMinimumKey(t.RightChild, out tempKey);
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(tempKey, t.LeftChild, temp);
                }
            }
            if(compared < 0)
            {             
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Remove(key, t.LeftChild, out removed), t.RightChild); //if the left child is the new minimum it removes until if finds that minimum
            }
            else
            {               
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Remove(key, t.RightChild, out removed)); //otherwise removes the right child
            }
        }

        public bool Remove(TKey k)
        {
            CheckKey(k);
            _elements = Remove(k, _elements, out bool removed); //removes the key from the dictionary then returns whether this was actually removed
            return removed;
        }

    }
}

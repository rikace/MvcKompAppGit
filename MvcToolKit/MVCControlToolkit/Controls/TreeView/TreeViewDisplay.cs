using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Core.Utilities;


namespace MVCControlsToolkit.Controls.TreeView
{
    internal class TreeViewDisplay<T>: IDisplayModel, IUpdateModelState
    {
        public List<ITreeViewNodeContainer> flattened{get; set;}
        private List<ITreeViewNodeContainer> roots;
        private string displayPrefix;
        private string startPrefix;
        private System.Web.Mvc.ModelStateDictionary modelState;
        
        public object ExportToModel(Type TargetType, params object[] context)
        {
            if (flattened == null) return null;
            IDictionary<string, ITreeViewNodeContainer> treeBuilder =
                new Dictionary<string, ITreeViewNodeContainer>(flattened.Count);
            roots = new List<ITreeViewNodeContainer>();
            foreach (ITreeViewNodeContainer container in flattened)
            {
                if (container.SonNumber > flattened.Count) continue;
                if (string.IsNullOrEmpty(container.FatherOriginalId))
                    roots.Add(container);
                treeBuilder[container.OriginalId] = container;
            }
            if (roots.Count == 0) return new List<T>();
            foreach (ITreeViewNodeContainer container in flattened)
            {
                if (container.SonNumber > flattened.Count) continue;
                if (string.IsNullOrEmpty(container.FatherOriginalId)) continue;
                if (treeBuilder.ContainsKey(container.FatherOriginalId))
                {
                    treeBuilder[container.FatherOriginalId].AddSon(container);
                }
            }
            int count = 0;
            foreach (ITreeViewNodeContainer container in flattened)
            {
                if (container.SonNumber > flattened.Count) continue;
                container.AddAllMySons();
                count++;
            }
            List<T> result = new List<T>();

            foreach (ITreeViewNodeContainer container in roots)
            {
                (result as IList).Add(container.AddToBranch(null));
            }
            List<int> permutations = new List<int>();
            permutationList(roots.ToArray(), permutations);
            int[] permutationArray = Sorting.InvertPermutationsArray(
                permutations.ToArray());
            updateModelState(permutationArray);
            return result;
        }

        public void ImportFromModel(object model, params object[] context)
        {
            //do nothing at display time
        }

        public void GetCurrState(string currPrefix, int updateIndex, System.Web.Mvc.ModelStateDictionary modelState)
        {
            this.modelState=modelState;
            this.displayPrefix = currPrefix;

            startPrefix = ".$.flattened.$$";
            if (!string.IsNullOrWhiteSpace(currPrefix) && currPrefix != "display")
            {
                startPrefix = currPrefix + startPrefix;
            }
            if (startPrefix[0] == '.') startPrefix = startPrefix.Substring(1);
        }
        private void permutationList(ITreeViewNodeContainer[] preTree, List<int> permutations)
        {
            string postfix;
            if (preTree == null) return;
            foreach (ITreeViewNodeContainer container in preTree)
            {
                permutations.Add(indexOf(container.OriginalId, out postfix));
                permutationList(container.MySonContainers, permutations);
            }

        }
        private void updateModelState(int[] permutationArray)
        {
            
            List<KeyValuePair<string, System.Web.Mvc.ModelState>> register = new List<KeyValuePair<string, System.Web.Mvc.ModelState>>();
            foreach (KeyValuePair<string, System.Web.Mvc.ModelState> pair in modelState)
            {
                if (pair.Key.StartsWith(startPrefix))
                    register.Add(new KeyValuePair<string, System.Web.Mvc.ModelState>(pair.Key, pair.Value));
            }
            foreach (KeyValuePair<string, System.Web.Mvc.ModelState> pair in register)
            {
                string postFix;
                int index = indexOf(pair.Key, out postFix);
                if (index >= 0 && index < permutationArray.Length)
                {
                    modelState[itemPrefix(displayPrefix, permutationArray[index]) + postFix] = pair.Value;
                }
                

            }
        }
        private string itemPrefix(string prefix, int index)
        {
            string res = string.Format(".$.flattened.$${0}", index);
            if (!string.IsNullOrWhiteSpace(prefix) && prefix != "display")
            {
                res = prefix + res;
            }
            if (res[0] == '.') res = res.Substring(1);
            return res;
        }
        private int indexOf(string prefix, out string postFix)
        {
            string res = null;
            postFix = string.Empty;
            if (prefix.StartsWith(startPrefix))
            {
                res = prefix.Substring(startPrefix.Length);
            }
            else return -1;
            int stop = res.IndexOf('.');
            if (stop > 0)
            {
                postFix = res.Substring(stop);
                res = res.Substring(0, stop);
            }
            return int.Parse(res);

        }
        public bool MoveState
        {
            get { return true; }
        }
    }
}

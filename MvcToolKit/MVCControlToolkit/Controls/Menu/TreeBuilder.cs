using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MVCControlsToolkit.Core;
using System.Reflection;
using System.Web.Mvc;


namespace MVCControlsToolkit.Controls
{
    public class SimpleMenuItem
    {
        public string Text { get; set; }
        public string Link { get; set; }
        public string UniqueName { get; set; }
        public string Target { get; set; }
        public List<SimpleMenuItem> Children { get; set; }
    }
    public class SimpleMenuBuilder: TreeBuilder<SimpleMenuItem>
    {
        public SimpleMenuBuilder(): base(m => m.Children)
        {
        }
        public SimpleMenuBuilder Add(string Text, string link = null, string uniqueName=null, string target=null)
        {
            return Add(new SimpleMenuItem { Text = Text, Link = link, UniqueName=uniqueName, Target=target }) as SimpleMenuBuilder;
        }
        public SimpleMenuBuilder Add(bool enabled, string Text, string link = null, string uniqueName = null, string target = null)
        {
            return Add(enabled, new SimpleMenuItem { Text = Text, Link = link, UniqueName = uniqueName, Target = target }) as SimpleMenuBuilder;
        }
        public SimpleMenuBuilder Down(bool enabled=true)
        {
           return SubItems() as SimpleMenuBuilder;
        }
        public SimpleMenuBuilder Up()
        {
            return FatherItem() as SimpleMenuBuilder;
        }
    }
    public class TreeBuilder<T>
    {
        private string sonsText;
        private Stack<List<T>> levels;
        private List<T> currLevel;
        private List<T> startDisabledLevel;
        private List<T> firstLevel;
        bool levelDisabled = false;
        bool fatherDisabled = false;
        public TreeBuilder(Expression<Func<T, List<T>>> children)
        {
            if (children == null) throw (new ArgumentNullException("children"));
            currLevel = new List<T>();
            firstLevel = currLevel;
            levels = new Stack<List<T>>();
            sonsText=ExpressionHelper.GetExpressionText(children);

        }
        public List<T> Get()
        {
            return firstLevel;
        }
        public TreeBuilder<T> Add(T item)
        {
            currLevel.Add(item);
            fatherDisabled = false;
            return this;
        }
        public TreeBuilder<T> Add(bool enabled, T item)
        {
            if (enabled && !levelDisabled)
            {
                currLevel.Add(item);
                fatherDisabled = false;
            }
            else fatherDisabled = true;
            return this;
        }
        public TreeBuilder<T> SubItems()
        {
            bool enabled = !fatherDisabled;
            fatherDisabled = false;
            levels.Push(currLevel);
            List<T> newLevel= new List<T>();
            if (enabled & !levelDisabled) new PropertyAccessor(currLevel[currLevel.Count - 1], sonsText).Value = newLevel;
            currLevel=newLevel;
            if (!levelDisabled && !enabled) startDisabledLevel = currLevel;
            if (levelDisabled || !enabled) levelDisabled = true;
            else levelDisabled = false;
            return this;
        }
        public TreeBuilder<T> FatherItem()
        {
            if (levels.Count == 0) return this;
            if (startDisabledLevel == currLevel)
            {
                startDisabledLevel = null;
                levelDisabled = false;
            }
            currLevel = levels.Pop();
            return this;
        }

    }
}

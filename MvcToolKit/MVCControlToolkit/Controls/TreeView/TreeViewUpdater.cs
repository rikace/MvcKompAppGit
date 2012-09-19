using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.DataAnnotations;

namespace MVCControlsToolkit.Controls.TreeView
{
    internal class TreeViewUpdater 
    {
        public bool Deleted { get; set; }
        public bool Closed { get; set; }
        public string FatherOriginalId { get; set; }
        public int PositionAsSon { get; set; }
        public string OriginalId { get; set; }
        public string SonCollectionName { get; set; }
        public int SonNumber { get; set; }

    }

    internal class TreeViewUpdater<T> : TreeViewUpdater, IUpdateModel, IVisualState, ITreeViewNodeContainer  
    {
        PropertyAccessor collectionHandler;
        public T Item { get; set; }
        
        public TreeViewUpdater()
        {
            if (!typeof(ISafeCreation).IsAssignableFrom(typeof(T)))
                throw new NotSupportedException(string.Format(
                    ControlsResources.NotCompatibleTypes,
                    typeof(T).FullName,
                    typeof(ISafeCreation).FullName));
        }
        public TreeViewUpdater(bool deleted)
        {
            this.Deleted = deleted;
        }
        
        
        private string _ModelName;
        private System.Collections.IDictionary _Store;
        public object UpdateModel(object model, string[] fields)
        {
            if (Item == null) Deleted = true;
            if (Deleted) return model;

            _Store[Item] = Closed;
            if (model == null) model = new List<ITreeViewNodeContainer>();
            if (SonCollectionName != null)
            {

                collectionHandler =
                        new PropertyAccessor(SonCollectionName, typeof(T));
            }
            
            if (SonNumber < 0 ) return model;
            if (OriginalId == null) return model;
            (model as IList).Add(this);
            
            return model;
        }
        public void ImportFromModel(object model, object[] fields, string[] fieldNames, object[] args = null)
        {
            if (model != null)
            {
                try
                {
                    Item = (T)model;
                }
                catch { }
            }
            if (args != null && args.Length > 0)
            {
                Closed = (bool)args[0];
            }
        }
        public System.Collections.IDictionary Store
        {
            set { _Store = value; }
        }

        public string ModelName
        {
            set { _ModelName=value; }
            get { return _ModelName; }
        }
        public object AddToBranch(object obranch)
        {
            //List<T> branch = obranch as List<T>;
            //if (branch == null) branch = new List<T>();
            //branch.Add(Item);
            //return branch;
            return Item;
        }
        public void AddAllMySons()
        {
            if (MySonContainers != null && MySonContainers.Length > 0 && collectionHandler != null)
            {
                IList sons = null;
                Type listType = collectionHandler.Property.PropertyType.GetGenericArguments()[0];
                Type allListType = typeof(List<string>).GetGenericTypeDefinition().MakeGenericType(listType);
                sons = allListType.GetConstructor(new Type[0]).Invoke(new object[0]) as IList;
                foreach (ITreeViewNodeContainer sonContainer in MySonContainers)
                {
                    sons.Add(sonContainer.AddToBranch(sons));
                }
                
                collectionHandler.Property.SetValue(Item, sons, new object[0]);
            }
        }
        public void AddSon(ITreeViewNodeContainer son)
        {
            
            if (son == null) return;
            if (MySonContainers == null) MySonContainers = 
                new ITreeViewNodeContainer[SonNumber];
            if (son.PositionAsSon < 0 || son.PositionAsSon >= SonNumber) return;
            MySonContainers[son.PositionAsSon] = son;
        }
        
        public ITreeViewNodeContainer[] MySonContainers { get; set; }
       
    }
}

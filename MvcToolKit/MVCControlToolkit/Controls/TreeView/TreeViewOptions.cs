using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public enum TreeViewPersistencyMode {EditOnly, Location, Cookie} 
    public class TreeViewOptions
    {
        private string script =
        @"
            <script language='javascript' type='text/javascript'>
                $('#{0}_ItemsContainer').treeview({{
                    animated: {1},
                    unique: {2},
                    {3}
                    toggle: function() {{
   	                    MvcControlsToolkit_TreeViewToggle(this);
                    }}
                }});
            </script>
        ";
        public TreeViewOptions()
        {
            Animated = 1;
            Unique = false;
            Opacity = 1.0f;
            CanMove = true;
            CanAdd = true;
            Persist = TreeViewPersistencyMode.EditOnly;
            CookieId = "treeview";
        }
        public uint Animated { get; set; }//default 1 millisecond
        public bool Unique { get; set; } //default false
        public float Opacity { get; set; } //default 1
        public bool CanMove { get; set; } // default true
        public bool CanAdd { get; set; } // default true
        public TreeViewPersistencyMode Persist { get; set; }// default edit only
        public string CookieId { get; set; }// default "treeview"
        private string javascriptPersitency()
        {
            if(Persist == TreeViewPersistencyMode.EditOnly) return string.Empty;
            if (Persist == TreeViewPersistencyMode.Location) return
             @" persist: 'location',
            ";
            if (Persist == TreeViewPersistencyMode.Cookie && !string.IsNullOrWhiteSpace(CookieId)) return
            string.Format(
             @" persist: 'cookie',
                cookieId: '{0}',
               ", CookieId);
            return string.Empty;
        }
        internal string Render(string name)
        {
            return string.Format(script, BasicHtmlHelper.IdFromName(name), 
                Animated, 
                Unique ? "true": "false",
                javascriptPersitency()
                );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsaSQLEditor.Interfaces;
using System.Windows.Forms;
using ScintillaNET;
using WeifenLuo.WinFormsUI.Docking;

namespace ExportToHTML
{
    public class PluginMain : IPlugin2
    {
        private IPluginContext2 m_Ctxt;

        public string Name
        {
            get
            {
                return "Export to HTML";
            }
        }

        public void Initialize(IPluginContext2 context)
        {
            m_Ctxt = context;
            m_Ctxt.DockPanel.ContentAdded += DockPanel_ContentAdded;
        }

        private void DockPanel_ContentAdded(object sender, WeifenLuo.WinFormsUI.Docking.DockContentEventArgs e)
        {
            var view = (e.Content as IView);
            var dockContent = (DockContent)view;

            if (view == null)
                return;

            var editor = FindScintilla(dockContent);

            if (editor == null)
                return;

            var ctxtMenu = new ContextMenu();
            var copyAsHTML = new MenuItem("Copy as HTML");

            ctxtMenu.MenuItems.Add(copyAsHTML);
            editor.ContextMenu = ctxtMenu;

            copyAsHTML.Click += (src, evt) =>
            {
                string html = editor.ExportHtml();
                string txt = editor.Text;
                
                ClipboardHelper.CopyToClipboard(html, txt);
            };

            dockContent.FormClosed += (src, evt) =>
            {
                if (ctxtMenu != null)
                    ctxtMenu.Dispose();
            };
            
        }

        private Scintilla FindScintilla(Control ctrl)
        {
            if (ctrl is Scintilla)
                return (Scintilla)ctrl;

            foreach(Control subCtrl in ctrl.Controls)
            {
                return FindScintilla(subCtrl);
            }

            return null;
        }

        public void Unload()
        {
            m_Ctxt = null;
        }
    }
}

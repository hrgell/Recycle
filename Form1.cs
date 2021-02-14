using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

// # Add settings
// DropIntoFolder
// ExpandOnDrop
// ExpandOnDropFirst First/Last - when dropping multiple
// InsertAfter True/False - insert before or after the row we drop on.

// # Notes on dropping a source node on a NodeList possibly on a target node
//
// Dragging and dropping stuff from right to left causes the stuff dragged to be deleted from the source treeview.
// Dragging and dropping stuff from left to the right causes the stuff dragged to be added to the target treeview.
//
// Don't drop it if:
// The NodeList's immediate child nodes matches on the basename wise.
//
// Don't drop file/module (not folder) if:
// The NodeList's child nodes or parent nodes contains a node with the same basename.
// 
// # T1
//
// # T3
//
// # T4
//
//

namespace TreeViewTest
{
    public class NodeTag
    {
        public string Filename { get; set; }
        public string Basename { get; set; }
        public bool IsFolder { get; set; }

        public NodeTag(string filename, bool isfolder)
        {
            Filename = filename;
            Basename = Path.GetFileNameWithoutExtension(filename);
            IsFolder = isfolder;
        }

        public NodeTag(TreeNode node)
        {
            NodeTag tag = node.Tag as NodeTag;
            Filename = tag.Filename;
            Basename = tag.Basename;
            IsFolder = tag.IsFolder;
        }
    } // class TreeNodeTag

    public partial class Form1 : Form
    {
        long mycount = 0;
        //TODO Add these setting 
        bool DropIntoFolder = true; // drop into folder instead of after.
        bool ExpandOnDrop = true; // expand the folder the items are dopped into
        bool ExpandOnDropFirst = false; // when dropping multiple focus on first or last copied.
        bool InsertAfter = true; // insert after the row we drop on ?
        bool Insert = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Debug(string msg)
        {
            //Txt1.Text += msg + Environment.NewLine;
            Txt1.AppendText(msg + Environment.NewLine);
        }

        //public TreeNode FindParent(TreeNode node, string basename)
        //{
        //    TreeNodeTag tag = node.Tag as TreeNodeTag;
        //    if (!tag.IsFolder && tag.Basename.Equals(basename))
        //        return node;
        //    if (node.Parent == null)
        //        return null;
        //    foreach (TreeNode pnode in node.Parent.Nodes)
        //    {
        //        TreeNode found = FindParent(pnode, basename);
        //        if (found != null)
        //            return found;
        //    }
        //    return null;
        //}

        public TreeNode AddElement(TreeNodeCollection nodes, int idx, string key, bool isfolder)
        {
            NodeTag tag = new NodeTag(Path.GetFileNameWithoutExtension(key), isfolder);
            TreeNode node = nodes.Insert(idx, key, tag.Basename);
            node.Tag = tag;
            return node;
        }

        public TreeNode AddElement(TreeNodeCollection nodes, int idx, TreeNode source)
        {
            //TreeNodeTag tag = source.Tag as TreeNodeTag;
            TreeNode node = (TreeNode)source.Clone();
            nodes.Insert(idx, node);
            return node;
        }

        private void FillTreeView(TreeNodeCollection nodes, string prefix)
        {
            AddElement(nodes, nodes.Count, prefix + "1", false);
            AddElement(nodes, nodes.Count, prefix + "2", false);
            var node3 = AddElement(nodes, nodes.Count, prefix + "3", true);
            AddElement(nodes, nodes.Count, prefix + "4", false);
            AddElement(nodes, nodes.Count, prefix + "5", false);

            nodes = node3.Nodes;
            AddElement(nodes, nodes.Count, prefix + "A", false);
            AddElement(nodes, nodes.Count, prefix + "B", false);
            var nodeC = AddElement(nodes, nodes.Count, prefix + "C", true);
            AddElement(nodes, nodes.Count, prefix + "D", false);

            nodes = nodeC.Nodes;
            AddElement(nodes, nodes.Count, prefix + "X", false);
            var nodeY = AddElement(nodes, nodes.Count, prefix + "Y", false);
            var nodeZ = AddElement(nodes, nodes.Count, prefix + "Z", true);

            nodes = nodeY.Nodes;
            AddElement(nodes, nodes.Count, prefix + "W", false);
        } // FillTreeView()

        private void InitializeTreeViews()
        {
            Tv1.Nodes.Clear();
            Tv2.Nodes.Clear();
            Tv3.Nodes.Clear();
            Tv4.Nodes.Clear();
            FillTreeView(Tv1.Nodes, "test");
            FillTreeView(Tv2.Nodes, "qwer");
            FillTreeView(Tv3.Nodes, "asdf");
            FillTreeView(Tv4.Nodes, "zxcv");
        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            InitializeTreeViews();
        }

        // Only accepts clicks on the nodes
        private void Tv1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Debug(e.Node.Text);
        }

        // Only accepts clicks on node rows
        private void Tv1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //Debug("NodeMouseClick " + e.Node.Text);
            TreeView tv = (TreeView)sender;
            TreeNode node = e.Node;
            if (node == null && tv.Nodes.Count > 1)
                node = tv.Nodes[tv.Nodes.Count - 1];
            tv.SelectedNode = e.Node;
        }


        // Only accepts clicks on node rows
        private void Tv1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void Tv1_MouseDown(object sender, MouseEventArgs e)
        {
            //TreeView tv = (TreeView)sender;
            //Point targetPoint = tv.PointToClient(new Point(e.X, e.Y));
            //TreeNode node = tv.GetNodeAt(targetPoint);
            //if (node == null && tv.Nodes.Count > 0)
            //{
            //    node = tv.Nodes[tv.Nodes.Count - 1];
            //    tv.SelectedNode = node;
            //}
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Tv1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            //Debug("ItemDrag");
            if (e.Button == MouseButtons.Left)
            {
                //if(meta key is pressed)
                //    DoDragDrop(e.Item, DragDropEffects.Move);
                //else
                DoDragDrop(e.Item, DragDropEffects.Copy);
            }
        }

        private void Tv1_DragEnter(object sender, DragEventArgs e)
        {
            //Debug("DragEnter");
            //e.Effect = DragDropEffects.Move;
            e.Effect = e.AllowedEffect;
        }

        private void Tv1_DragOver(object sender, DragEventArgs e)
        {
            //Debug("DragOwer");
            TreeView tv = (TreeView)sender;
            Point targetPoint = tv.PointToClient(new Point(e.X, e.Y));
            tv.SelectedNode = tv.GetNodeAt(targetPoint);
        }

        private int GetInsertionIdx(string txt, TreeNode destination, TreeNodeCollection nodes)
        {
            if (nodes.ContainsKey(txt)) // (nodes.Find(source.Text, true).Length > 0)
                return -1;
            int idx = 0;
            if (destination != null)
            {
                if (destination.Nodes.Count > 0 && DropIntoFolder)
                    idx = destination.Nodes.Count;
                else
                    idx = (destination != null) ? destination.Index : 0;
            }
            if (InsertAfter)
                ++idx;
            return idx;
        }

        private void DropNode(TreeNode source, TreeNode destination, TreeNodeCollection nodes)
        {
            if (nodes.ContainsKey(source.Text)) // (nodes.Find(source.Text, true).Length > 0)
                return;
            TreeNode node = (TreeNode)source.Clone();
            int idx = GetInsertionIdx(source.Text, destination, nodes);
            if (idx < 0)
                return;
            AddElement(nodes, idx, node);
            if (ExpandOnDrop)
                node.EnsureVisible();
        }

        private void DropFiles(string[] files, TreeNode destination, TreeNodeCollection nodes)
        {
            // GetInsertionIdx
            if (files.Length == 0)
                return;
            int idx = (destination != null) ? destination.Index : 0;
            if (InsertAfter)
                ++idx;
            TreeNode first = null;
            TreeNode last = null;
            foreach (string filename in files)
            {
                //Debug(filename);
                if (nodes.ContainsKey(filename)) // (nodes.Find(source.Text, true).Length > 0)
                    continue;
                TreeNode node = AddElement(nodes, idx, filename, false);
                if (first == null)
                    first = node;
                last = node;
                ++idx;
            }

            if (ExpandOnDrop)
            {
                if (ExpandOnDropFirst)
                    first?.EnsureVisible();
                else
                    last?.EnsureVisible();
            }
        } // DropFiles()

        private void Tv1_DragDrop(object sender, DragEventArgs e)
        {
            //Debug("DragDrop");
            TreeNodeCollection nodes = null;
            TreeView tv = (TreeView)sender;
            Point point = tv.PointToClient(new Point(e.X, e.Y));
            TreeNode destination = tv.GetNodeAt(point);
            if (destination == null)
            {
                nodes = tv.Nodes;
                if (nodes.Count > 0)
                    destination = nodes[nodes.Count - 1];
            }
            if (nodes == null && destination != null)
            {
                if (destination.Level == 0 && destination.Nodes.Count == 0)
                    nodes = tv.Nodes;
                else if (destination.Nodes.Count > 0 && DropIntoFolder)
                    nodes = destination.Nodes;
                else if (destination.Parent != null)
                    nodes = destination.Parent.Nodes;
            }
            if (nodes == null)
                nodes = tv.Nodes;

            TreeNode source = (TreeNode)e.Data.GetData(typeof(TreeNode));
            if (source == null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    DropFiles(files, destination, nodes);
                } else
                {
                    // unknown source
                }
                return;
            }
            if (source.Equals(destination))
                return;

            TreeView tvsource = source.TreeView;

            // Dont allow nodes from Tv1 to drop on itself.
            if (tv == Tv1 && tvsource == Tv1)
                return;

            // Delete nodes dragged to the left
            if (int.Parse((string)tvsource.Tag) > int.Parse((string)tv.Tag))
            {
                source.Remove();
                return;
            }

            // Add nodes dragged to the right
            if (e.Effect == DragDropEffects.Move)
            {
                //source.Remove();
                //TreeNode node = nodes.Add(source.Text, source.Text);
                //node.EnsureVisible();
            }
            else if (e.Effect == DragDropEffects.Copy)
            {
                DropNode(source, destination, nodes);
            }
            if (destination != null)
            {
                //tv.SelectedNode = destination;
            }

        } // DragDrop()

        private void Btn2_Click(object sender, EventArgs e)
        {
            InitializeTreeViews();
        }

        private void TestTv1(string key)
        {
            //bool rst1 = Tv1.Nodes.ContainsKey(key);
            TreeNode[] nodes = Tv1.Nodes.Find(key, true);
            bool rst2 = nodes.Length > 0;
            Debug("TestTv1: " + rst2.ToString());
        }

        private TreeNode GetRootNode(TreeNode node)
        {
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return node;
        }

        private void Btn3_Click(object sender, EventArgs e)
        {
            Debug("mycount " + ++mycount);
            TestTv1("test1");
            TestTv1("testA");
        }
    } // class
} // namespace

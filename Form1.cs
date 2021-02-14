using System;
using System.Drawing;
using System.Windows.Forms;

// Add settings:
//
// ExpandOnDrop
// ExpandOnDropFirst First/Last - when dropping multiple
// InsertAfter True/False - insert before or after the row we drop on.
// DropIntoFolder

namespace TreeViewTest
{
    public partial class Form1 : Form
    {
        long mycount = 0;
        //TODO Add these setting 
        bool ExpandOnDrop = true; // expand the folder the items are dopped into
        bool ExpandOnDropFirst = false; // when dropping multiple focus on first or last copied.
        bool InsertAfter = true; // insert after the row we drop on ?
        bool DropIntoFolder = true; // drop into folder instead of after.

        public Form1()
        {
            InitializeComponent();
        }

        private void Debug(string msg)
        {
            //Txt1.Text += msg + Environment.NewLine;
            Txt1.AppendText(msg + Environment.NewLine);
        }

        private void FillTreeView(TreeNodeCollection nodes, string prefix)
        {
            nodes.Add(prefix + "1", prefix + "1");
            nodes.Add(prefix + "2", prefix + "2");
            var node = nodes.Add(prefix + "3", prefix + "3");
            nodes.Add(prefix + "4", prefix + "4");
            nodes.Add(prefix + "5", prefix + "5");

            nodes = node.Nodes;
            nodes.Add(prefix + "A", prefix + "A");
            nodes.Add(prefix + "B", prefix + "B");
            node = nodes.Add(prefix + "C", prefix + "C");
            nodes.Add(prefix + "D", prefix + "D");

            nodes = node.Nodes;
            nodes.Add(prefix + "X", prefix + "X");
            nodes.Add(prefix + "Y", prefix + "Y");
            nodes.Add(prefix + "Z", prefix + "Z");
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

        private void DropFiles(object sender, DragEventArgs e, TreeNode destination, TreeNodeCollection nodes)
        {
            int idx = (destination != null) ? destination.Index : 0;
            if (InsertAfter)
                ++idx;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            TreeNode first = null;
            TreeNode last = null;
            foreach (string filename in files)
            {
                //Debug(filename);
                if (nodes.ContainsKey(filename)) // (nodes.Find(source.Text, true).Length > 0)
                    continue;
                TreeNode node = nodes.Insert(idx, filename, filename);
                if (first == null)
                {
                    first = node;
                }
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
                    DropFiles(sender, e, destination, nodes);
                }
                return;

            }
            if (source.Equals(destination))
                return;
            //if (destination != null)
            //    tv.SelectedNode = destination;

            TreeView srcview = source.TreeView;

            // Dont allow nodes from Tv1 to drop on itself.
            if (tv == Tv1 && srcview == Tv1)
                return;
            bool remove = false;

            if(int.Parse((string)srcview.Tag) > int.Parse((string)tv.Tag))
            {
                source.Remove();
            }
            else  if (e.Effect == DragDropEffects.Move)
            {
                source.Remove();
                //TreeNode node = nodes.Add(source.Text, source.Text);
                //node.EnsureVisible();
            }
            else if (e.Effect == DragDropEffects.Copy)
            {
                if (nodes.ContainsKey(source.Text)) // (nodes.Find(source.Text, true).Length > 0)
                    return;
                TreeNode node = (TreeNode)source.Clone();
                //int idx = nodes.Add(node);
                //TreeNode node2 = nodes[idx];
                int idx = 0;
                if (destination != null && destination.Nodes.Count > 0 && DropIntoFolder)
                {
                    idx = destination.Nodes.Count;
                }
                else
                {
                    idx = (destination != null) ? destination.Index : 0;
                }
                if (InsertAfter)
                    ++idx;
                nodes.Insert(idx, node);
                if (ExpandOnDrop)
                    node.EnsureVisible();
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

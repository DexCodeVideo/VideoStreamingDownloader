using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VideoStreamingDownloader.Common.EpisodeSelector
{
    internal partial class Main : Form
    {
        private bool _firstLoad = true;
        private Items _items;
        internal HashSet<string> CheckedIds { get; private set; } = new HashSet<string>();

        internal Main(Items items)
        {
            InitializeComponent();
            _items = items;
            foreach (var item in _items)
            {
                CheckedIds.Add(item.Id);
            }
            SetLabels();
        }

        private void SetLabels()
        {
            SeleccionaCkb.Text = Resources.Translations.Strings.Select_All;
            SaveButton.Text = $"{Resources.Translations.Strings.Confirm} {Resources.Translations.Strings.Selected}";
            FilterLabel.Text = Resources.Translations.Strings.Filter;
        }

        private void EpisodeSelectorForm_Load(object sender, System.EventArgs e)
        {
            if (_firstLoad)
                BuildTree(string.Empty, false);

            _firstLoad = false;
        }

        private void BuildTree(string filter, bool expand, bool seasonCheck = true)
        {
            EpisodesTree.Nodes.Clear();
            foreach (var item in _items)
            {
                if (string.IsNullOrEmpty(filter) | item.Title.ToLower().Contains(filter.ToLower()))
                {
                    TreeNode Season = EpisodesTree.Nodes.Find(item.Season.ToString(), false).FirstOrDefault();

                    if (Season == null)
                    {
                        Season = EpisodesTree.Nodes.Add(item.Season.ToString(), item.SeasonDescription);
                        Season.Checked = seasonCheck;
                    }

                    var node = Season.Nodes.Add(item.Title);
                    node.Tag = item.Id;
                    node.Checked = CheckedIds.Contains(item.Id);
                }
            }

            if (expand)
                EpisodesTree.ExpandAll();

            SetSelectedLabel();
        }

        private void SetSelectedLabel()
        {
            SelectedLabel.Text = $"{Resources.Translations.Strings.Selected}: {CheckedIds.Count}/{_items.Count}";
        }

        private void EpisodesTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            this.EpisodesTree.AfterCheck -= new TreeViewEventHandler(this.EpisodesTree_AfterCheck);

            if (e.Node.Tag != null)
                SetNodeState(e.Node, e.Node.Checked);

            foreach (TreeNode node in e.Node.Nodes)
            {
                SetNodeState(node, e.Node.Checked);
            }

            this.EpisodesTree.AfterCheck += new TreeViewEventHandler(this.EpisodesTree_AfterCheck);

            SetSelectedLabel();
        }

        private void SetNodeState(TreeNode node, bool state)
        {
            node.Checked = state;
            if (state)
                CheckedIds.Add((string)node.Tag);
            else
                CheckedIds.Remove((string)node.Tag);
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SeleccionaCkb_CheckedChanged(object sender, System.EventArgs e)
        {
            this.EpisodesTree.AfterCheck -= new TreeViewEventHandler(this.EpisodesTree_AfterCheck);
            foreach (TreeNode parent in EpisodesTree.Nodes)
            {
                parent.Checked = SeleccionaCkb.Checked;
                foreach (TreeNode child in parent.Nodes)
                {
                    child.Checked = SeleccionaCkb.Checked;
                    if (SeleccionaCkb.Checked)
                        CheckedIds.Add((string)child.Tag);
                    else
                        CheckedIds.Remove((string)child.Tag);
                }
            }
            this.EpisodesTree.AfterCheck += new TreeViewEventHandler(this.EpisodesTree_AfterCheck);

            SetSelectedLabel();
        }

        private void FilterTextBox_TextChanged(object sender, System.EventArgs e)
        {
            if (FilterTextBox.Text.Length > 0)
                BuildTree(FilterTextBox.Text, true, false);
            else
                BuildTree(FilterTextBox.Text, false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoStreamingDownloader.Common.Downloads
{
    internal partial class Viewer : Form
    {
        HashSet<string> DisplayItems = new HashSet<string>();
        DataTable DisplayTable = new DataTable();

        internal Viewer()
        {
            InitializeComponent();
            BuildDataTable();
            DownloadStart.Enabled = Program.DownloadManager.DownloadEnable;
        }

        private void BuildDataTable()
        {
            DisplayTable.Columns.Add("Id", typeof(string));
            DisplayTable.Columns.Add("Title", typeof(string));
            DisplayTable.Columns.Add("State", typeof(int));
            DisplayTable.Columns.Add("Progress", typeof(int));
            DisplayTable.Columns.Add("StateMessage", typeof(string));

            DisplayTable.PrimaryKey = new[] { DisplayTable.Columns["Id"] };

            dataGridView1.Font = new Font("Segoe UI", 12);
            dataGridView1.RowTemplate.Height = 35;
            dataGridView1.DataSource = DisplayTable;

            dataGridView1.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns["State"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns["State"].HeaderText = Resources.Translations.Strings.State;
            dataGridView1.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns["Title"].HeaderText = Resources.Translations.Strings.Title;
            dataGridView1.Columns["Progress"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns["Progress"].HeaderText = Resources.Translations.Strings.Progress;
            dataGridView1.Columns["Progress"].CellTemplate = new ProgressBarCell();
            dataGridView1.Columns["StateMessage"].Visible = false;

            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["State"].Index && e.Value != null)
            {
                e.Value = Item.States.Description((int)e.Value);
                e.FormattingApplied = true;
            }
        }

        internal class ProgressBarCell : DataGridViewTextBoxCell
        {
            public override Type EditType => null;

            public override Type ValueType => typeof(string);

            protected override void Paint(
                Graphics graphics,
                Rectangle clipBounds,
                Rectangle cellBounds,
                int rowIndex,
                DataGridViewElementStates cellState,
                object value,
                object formattedValue,
                string errorText,
                DataGridViewCellStyle cellStyle,
                DataGridViewAdvancedBorderStyle advancedBorderStyle,
                DataGridViewPaintParts paintParts)
            {
                // Paint the normal cell background/border
                base.Paint(
                    graphics,
                    clipBounds,
                    cellBounds,
                    rowIndex,
                    cellState,
                    null,
                    null,
                    errorText,
                    cellStyle,
                    advancedBorderStyle,
                    paintParts & ~DataGridViewPaintParts.ContentForeground);

                int progress = (int)value;

                // Progress bar area
                Rectangle barBounds = new Rectangle(
                    cellBounds.X + 5,
                    cellBounds.Y + 7,
                    cellBounds.Width - 10,
                    cellBounds.Height - 14);

                var stateValue = this.DataGridView.Rows[rowIndex].Cells["State"]?.Value;
                var stateMessage = this.DataGridView.Rows[rowIndex].Cells["StateMessage"]?.Value;
                bool HasError = stateValue?.Equals(Item.States.Error) ?? false;
                if (stateValue?.Equals(Item.States.Complete) ?? false)
                {
                    progress = 100;
                    stateMessage = Resources.Translations.Strings.Complete;
                }

                string text = $"{stateMessage}: {progress}%";

                // Background
                if (HasError)
                {
                    using (var backBrush = new SolidBrush(Color.DarkSalmon))
                    {
                        graphics.FillRectangle(backBrush, barBounds);
                    }
                    text = $"{Resources.Translations.Strings.Error}: {stateMessage}";
                }
                else
                {
                    using (var backBrush = new SolidBrush(Color.LightGray))
                    {
                        graphics.FillRectangle(backBrush, barBounds);
                    }

                    Rectangle progressBounds = new Rectangle(
                    barBounds.X,
                    barBounds.Y,
                    barBounds.Width * progress / 100,
                    barBounds.Height);

                    if (progress == 100)
                    {
                        using (var progressBrush = new SolidBrush(Color.LightGreen))
                        {
                            graphics.FillRectangle(progressBrush, progressBounds);
                        }
                    }
                    else
                    {
                        using (var progressBrush = new SolidBrush(Color.DodgerBlue))
                        {
                            graphics.FillRectangle(progressBrush, progressBounds);
                        }
                    }
                }

                TextRenderer.DrawText(
                    graphics,
                    text,
                    cellStyle.Font,
                    barBounds,
                    Color.Black,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.NoPadding);
            }
        }

        private void AddDisplayTableItem(Item item)
        {
            DisplayItems.Add(item.Id);

            switch (item)
            {
                case _3CAT.Downloads.Item _3CatItem:
                    DisplayTable.Rows.Add(item.Id, _3CatItem.FileInfo.Title, item.State, 0, Resources.Translations.Strings.Pending);
                    break;
                case RTVE.Downloads.Item rtveItem:
                    DisplayTable.Rows.Add(item.Id, rtveItem.FileInfo.Title, item.State, 0, Resources.Translations.Strings.Pending);
                    break;
                case ETB.Downloads.Item etbItem:
                    DisplayTable.Rows.Add(item.Id, etbItem.FileInfo.Title, item.State, 0, Resources.Translations.Strings.Pending);
                    break;
                default:
                    DisplayTable.Rows.Add(item.Id, Resources.Translations.Strings.Unknown, item.State, 0, Resources.Translations.Strings.Pending);
                    break;
            }
        }

        private void UpdateDownloadProgress(string id, Status status)
        {
            DataRow row = DisplayTable.Rows.Find(id);

            if (row != null && row["State"].Equals(Item.States.Downloading))
            {
                row["Progress"] = status.Progress;
                row["StateMessage"] = status.Message;
            }
        }

        private void UpdateState(string id, int state)
        {
            DataRow row = DisplayTable.Rows.Find(id);

            if (row != null)
            {
                if (!row["State"].Equals(state))
                {
                    row["State"] = state;
                    if (state == Item.States.Complete)
                        row["Progress"] = 100;
                }
            }
        }

        private void LoadData()
        {
            LoadDownloadItems();
            LoadActiveDownloads();
        }

        private void LoadActiveDownloads()
        {
            foreach (var download in Program.DownloadManager.ActiveDownloads)
            {
                if (DisplayItems.Contains(download.FileId))
                    UpdateDownloadProgress(download.FileId, download.Status);
            }
        }

        private void LoadDownloadItems()
        {
            foreach (var item in Program.DownloadManager.DownloadData.DownloadItems)
            {
                if (DisplayItems.Contains(item.Key))
                {
                    UpdateState(item.Key, item.Value.State);
                }
                else
                    AddDisplayTableItem(item.Value);
            }
        }

        private void UpdatePage()
        {
            LoadData();
        }

        private async Task UpdatePageTask()
        {
            while (true)
            {
                await Task.Delay(1000);
                UpdatePage();
            }
        }

        private void DownloaderViewer_Load(object sender, System.EventArgs e)
        {
            UpdatePage();
            _ = UpdatePageTask();
        }

        private async void DownloadStart_Click(object sender, EventArgs e)
        {
            DownloadStart.Enabled = false;
            await Task.Run(() => Program.DownloadManager.StartDownload());
            DownloadStart.Enabled = Program.DownloadManager.DownloadEnable;
        }
    }
}

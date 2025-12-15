using AOI.Common.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOI.GrabProgressWinForms
{
    public partial class MainForm : Form
    {
        private readonly GrabStatusService _service;
        private readonly BindingList<GrabWorkerStatus> _workers = new();
        private HubConnection? _hub;
        private string? _selectedBatch;

        public MainForm()
        {
            InitializeComponent();

            _service = new GrabStatusService("https://localhost:7117");

            dgvWorkers.AutoGenerateColumns = false;
            dgvWorkers.DataSource = _workers;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await LoadBatchesAsync();
            await SetupSignalRAsync();
            await RefreshOnceByApiAsync();   // 初次載入一次
        }

        private async Task LoadBatchesAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var batches = await _service.GetBatchesAsync();
                cmbBatch.Items.Clear();

                foreach (var b in batches)
                    cmbBatch.Items.Add(b.BatchId);

                if (cmbBatch.Items.Count > 0)
                    cmbBatch.SelectedIndex = 0;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task SetupSignalRAsync()
        {
            _hub = new HubConnectionBuilder()
                .WithUrl("https://localhost:7117/hub/progress")
                .WithAutomaticReconnect()
                .Build();

            _hub.On<GrabProgressUpdated>("GrabProgressUpdated", (status) =>
            {
                // 非 UI 執行緒 → Invoke 回 UI
                Invoke(new Action(() =>
                {
                    if (_selectedBatch == null)
                        return;

                    if (!string.Equals(status.BatchId, _selectedBatch, StringComparison.OrdinalIgnoreCase))
                        return;

                    ApplyProgress(status);
                }));
            });

            await _hub.StartAsync();
        }

        private void ApplyProgress(GrabProgressUpdated status)
        {
            UpdateProgress(status);
        }

        private void UpdateProgress(GrabProgressUpdated status)
        {
            lblTopCount.Text = $"Top: {status.TopCompletedPanels}/{status.TopExpectedPanels}";
            lblBottomCount.Text = $"Bottom: {status.BottomCompletedPanels}/{status.BottomExpectedPanels}";

            progressTop.Maximum = Math.Max(1, status.TopExpectedPanels);
            progressBottom.Maximum = Math.Max(1, status.BottomExpectedPanels);

            progressTop.Value = Math.Min(progressTop.Maximum, status.TopCompletedPanels);
            progressBottom.Value = Math.Min(progressBottom.Maximum, status.BottomCompletedPanels);
        }

        private async void cmbBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedBatch = cmbBatch.SelectedItem as string;
            await RefreshOnceByApiAsync();
        }

        private async Task RefreshOnceByApiAsync()
        {
            if (_selectedBatch == null) return;

            var status = await _service.GetBatchStatusAsync(_selectedBatch);
            if (status != null)
                ApplyProgress(status);
        }

        private async void btnRefreshBatches_Click(object sender, EventArgs e)
        {
            await LoadBatchesAsync();
        }
    }
}

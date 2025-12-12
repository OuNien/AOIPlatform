using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOI.GrabProgressWinForms
{
    public partial class MainForm : Form
    {
        private readonly GrabStatusService _service;
        private readonly BindingList<GrabWorkerStatus> _workers = new();
        private readonly System.Windows.Forms.Timer _timer;
        private bool _isRefreshing;

        public MainForm()
        {
            InitializeComponent();

            // TODO: 如有需要，改成實際的 API URL
            _service = new GrabStatusService("https://localhost:7117");

            dgvWorkers.AutoGenerateColumns = false;
            dgvWorkers.DataSource = _workers;

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000; // 建議值：1000 ms
            _timer.Tick += async (s, e) => await RefreshStatusAsync();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await LoadBatchesAsync();
            _timer.Start();
        }

        private async Task LoadBatchesAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var batches = await _service.GetBatchesAsync();

                cmbBatch.Items.Clear();
                foreach (var b in batches)
                {
                    cmbBatch.Items.Add(b.BatchId);
                }

                if (cmbBatch.Items.Count > 0)
                {
                    cmbBatch.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"載入批號失敗: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task RefreshStatusAsync()
        {
            if (_isRefreshing)
                return;

            var batchId = cmbBatch.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(batchId))
                return;

            try
            {
                _isRefreshing = true;
                var status = await _service.GetBatchStatusAsync(batchId);

                if (status == null)
                    return;

                BuildWorkers(status);
                UpdateProgress(status);
                UpdateWorkers(status);
            }
            catch (Exception ex)
            {
                // 可以視需要暫時關掉 timer 或只在第一次錯誤時顯示
                Console.WriteLine($"Refresh error: {ex}");
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private void UpdateProgress(GrabStatusResponse status)
        {
            lblTopCount.Text = $"Top: {status.TopCompletedPanels}/{status.TopExpectedPanels}";
            lblBottomCount.Text = $"Bottom: {status.BottomCompletedPanels}/{status.BottomExpectedPanels}";

            progressTop.Maximum = Math.Max(1, status.TopExpectedPanels);
            progressBottom.Maximum = Math.Max(1, status.BottomExpectedPanels);

            progressTop.Value = Math.Min(progressTop.Maximum, status.TopCompletedPanels);
            progressBottom.Value = Math.Min(progressBottom.Maximum, status.BottomCompletedPanels);
        }

        private void UpdateWorkers(GrabStatusResponse status)
        {
            _workers.RaiseListChangedEvents = false;
            _workers.Clear();
            
            foreach (var w in status.Workers.OrderBy(w => w.Name))
            {
                _workers.Add(w);
            }
            _workers.RaiseListChangedEvents = true;
            _workers.ResetBindings();
        }

        private void BuildWorkers(GrabStatusResponse status)
        {
            status.Workers.Clear();

            // Top Workers
            if (status.TopStationLastPanel != null)
            {
                foreach (var kv in status.TopStationLastPanel)
                {
                    var name = kv.Key;
                    var lastPanel = kv.Value;

                    status.TopStationLastSeen?.TryGetValue(name, out string? seen);

                    status.Workers.Add(new GrabWorkerStatus
                    {
                        Name = name,
                        Side = "Top",
                        Status = "Done",
                        Frames = lastPanel,
                    });
                }
            }

            // Bottom Workers
            if (status.BottomStationLastPanel != null)
            {
                foreach (var kv in status.BottomStationLastPanel)
                {
                    var name = kv.Key;
                    var lastPanel = kv.Value;

                    status.BottomStationLastSeen?.TryGetValue(name, out string? seen);

                    status.Workers.Add(new GrabWorkerStatus
                    {
                        Name = name,
                        Side = "Bottom",
                        Status = "Done",
                        Frames = lastPanel,
                    });
                }
            }
        }


        private async void btnRefreshBatches_Click(object sender, EventArgs e)
        {
            await LoadBatchesAsync();
        }
    }
}

using System.Windows.Forms;

namespace AOI.GrabProgressWinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private ComboBox cmbBatch;
        private Button btnRefreshBatches;
        private Label lblBatchLabel;
        private GroupBox grpTop;
        private GroupBox grpBottom;
        private ProgressBar progressTop;
        private ProgressBar progressBottom;
        private Label lblTopCount;
        private Label lblBottomCount;
        private DataGridView dgvWorkers;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cmbBatch = new ComboBox();
            this.btnRefreshBatches = new Button();
            this.lblBatchLabel = new Label();
            this.grpTop = new GroupBox();
            this.progressTop = new ProgressBar();
            this.lblTopCount = new Label();
            this.grpBottom = new GroupBox();
            this.progressBottom = new ProgressBar();
            this.lblBottomCount = new Label();
            this.dgvWorkers = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkers)).BeginInit();
            this.grpTop.SuspendLayout();
            this.grpBottom.SuspendLayout();
            this.SuspendLayout();

            // 
            // lblBatchLabel
            // 
            this.lblBatchLabel.AutoSize = true;
            this.lblBatchLabel.Location = new System.Drawing.Point(20, 20);
            this.lblBatchLabel.Name = "lblBatchLabel";
            this.lblBatchLabel.Size = new System.Drawing.Size(56, 15);
            this.lblBatchLabel.TabIndex = 0;
            this.lblBatchLabel.Text = "批號 (Lot)";

            // 
            // cmbBatch
            // 
            this.cmbBatch.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBatch.FormattingEnabled = true;
            this.cmbBatch.Location = new System.Drawing.Point(90, 16);
            this.cmbBatch.Name = "cmbBatch";
            this.cmbBatch.Size = new System.Drawing.Size(200, 23);
            this.cmbBatch.TabIndex = 1;

            // 
            // btnRefreshBatches
            // 
            this.btnRefreshBatches.Location = new System.Drawing.Point(310, 15);
            this.btnRefreshBatches.Name = "btnRefreshBatches";
            this.btnRefreshBatches.Size = new System.Drawing.Size(90, 25);
            this.btnRefreshBatches.TabIndex = 2;
            this.btnRefreshBatches.Text = "更新批號";
            this.btnRefreshBatches.UseVisualStyleBackColor = true;
            this.btnRefreshBatches.Click += new System.EventHandler(this.btnRefreshBatches_Click);

            // 
            // grpTop
            // 
            this.grpTop.Controls.Add(this.progressTop);
            this.grpTop.Controls.Add(this.lblTopCount);
            this.grpTop.Location = new System.Drawing.Point(20, 55);
            this.grpTop.Name = "grpTop";
            this.grpTop.Size = new System.Drawing.Size(380, 70);
            this.grpTop.TabIndex = 3;
            this.grpTop.TabStop = false;
            this.grpTop.Text = "Top 面進度";

            // 
            // progressTop
            // 
            this.progressTop.Location = new System.Drawing.Point(15, 30);
            this.progressTop.Name = "progressTop";
            this.progressTop.Size = new System.Drawing.Size(260, 23);
            this.progressTop.TabIndex = 0;

            // 
            // lblTopCount
            // 
            this.lblTopCount.AutoSize = true;
            this.lblTopCount.Location = new System.Drawing.Point(285, 34);
            this.lblTopCount.Name = "lblTopCount";
            this.lblTopCount.Size = new System.Drawing.Size(60, 15);
            this.lblTopCount.TabIndex = 1;
            this.lblTopCount.Text = "Top: 0/0";

            // 
            // grpBottom
            // 
            this.grpBottom.Controls.Add(this.progressBottom);
            this.grpBottom.Controls.Add(this.lblBottomCount);
            this.grpBottom.Location = new System.Drawing.Point(20, 135);
            this.grpBottom.Name = "grpBottom";
            this.grpBottom.Size = new System.Drawing.Size(380, 70);
            this.grpBottom.TabIndex = 4;
            this.grpBottom.TabStop = false;
            this.grpBottom.Text = "Bottom 面進度";

            // 
            // progressBottom
            // 
            this.progressBottom.Location = new System.Drawing.Point(15, 30);
            this.progressBottom.Name = "progressBottom";
            this.progressBottom.Size = new System.Drawing.Size(260, 23);
            this.progressBottom.TabIndex = 0;

            // 
            // lblBottomCount
            // 
            this.lblBottomCount.AutoSize = true;
            this.lblBottomCount.Location = new System.Drawing.Point(285, 34);
            this.lblBottomCount.Name = "lblBottomCount";
            this.lblBottomCount.Size = new System.Drawing.Size(80, 15);
            this.lblBottomCount.TabIndex = 1;
            this.lblBottomCount.Text = "Bottom: 0/0";

            // 
            // dgvWorkers
            // 
            this.dgvWorkers.AllowUserToAddRows = false;
            this.dgvWorkers.AllowUserToDeleteRows = false;
            this.dgvWorkers.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom) 
                                    | AnchorStyles.Left) 
                                    | AnchorStyles.Right));
            this.dgvWorkers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWorkers.Location = new System.Drawing.Point(20, 220);
            this.dgvWorkers.Name = "dgvWorkers";
            this.dgvWorkers.ReadOnly = true;
            this.dgvWorkers.RowTemplate.Height = 25;
            this.dgvWorkers.Size = new System.Drawing.Size(560, 220);
            this.dgvWorkers.TabIndex = 5;

            // 設定欄位
            var colName = new DataGridViewTextBoxColumn
            {
                HeaderText = "Worker",
                DataPropertyName = "Name",
                Name = "colName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colSide = new DataGridViewTextBoxColumn
            {
                HeaderText = "Side",
                DataPropertyName = "Side",
                Name = "colSide",
                Width = 80
            };
            var colStatus = new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = "Status",
                Name = "colStatus",
                Width = 120
            };
            var colFrames = new DataGridViewTextBoxColumn
            {
                HeaderText = "Frames",
                DataPropertyName = "Frames",
                Name = "colFrames",
                Width = 80
            };

            this.dgvWorkers.Columns.AddRange(new DataGridViewColumn[]
            {
                colName, colSide, colStatus, colFrames
            });

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 460);
            this.Controls.Add(this.dgvWorkers);
            this.Controls.Add(this.grpBottom);
            this.Controls.Add(this.grpTop);
            this.Controls.Add(this.btnRefreshBatches);
            this.Controls.Add(this.cmbBatch);
            this.Controls.Add(this.lblBatchLabel);
            this.Name = "MainForm";
            this.Text = "Grab Progress Monitor (WinForms)";
            this.Load += new System.EventHandler(this.MainForm_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkers)).EndInit();
            this.grpTop.ResumeLayout(false);
            this.grpTop.PerformLayout();
            this.grpBottom.ResumeLayout(false);
            this.grpBottom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

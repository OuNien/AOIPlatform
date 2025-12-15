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
            cmbBatch = new ComboBox();
            btnRefreshBatches = new Button();
            lblBatchLabel = new Label();
            grpTop = new GroupBox();
            progressTop = new ProgressBar();
            lblTopCount = new Label();
            grpBottom = new GroupBox();
            progressBottom = new ProgressBar();
            lblBottomCount = new Label();
            dgvWorkers = new DataGridView();
            colName = new DataGridViewTextBoxColumn();
            colSide = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colFrames = new DataGridViewTextBoxColumn();
            grpTop.SuspendLayout();
            grpBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvWorkers).BeginInit();
            SuspendLayout();
            // 
            // cmbBatch
            // 
            cmbBatch.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBatch.FormattingEnabled = true;
            cmbBatch.Location = new System.Drawing.Point(90, 16);
            cmbBatch.Name = "cmbBatch";
            cmbBatch.Size = new System.Drawing.Size(200, 23);
            cmbBatch.TabIndex = 1;
            cmbBatch.SelectedIndexChanged += cmbBatch_SelectedIndexChanged;
            // 
            // btnRefreshBatches
            // 
            btnRefreshBatches.Location = new System.Drawing.Point(310, 15);
            btnRefreshBatches.Name = "btnRefreshBatches";
            btnRefreshBatches.Size = new System.Drawing.Size(90, 25);
            btnRefreshBatches.TabIndex = 2;
            btnRefreshBatches.Text = "更新批號";
            btnRefreshBatches.UseVisualStyleBackColor = true;
            btnRefreshBatches.Click += btnRefreshBatches_Click;
            // 
            // lblBatchLabel
            // 
            lblBatchLabel.AutoSize = true;
            lblBatchLabel.Location = new System.Drawing.Point(20, 20);
            lblBatchLabel.Name = "lblBatchLabel";
            lblBatchLabel.Size = new System.Drawing.Size(60, 15);
            lblBatchLabel.TabIndex = 0;
            lblBatchLabel.Text = "批號 (Lot)";
            // 
            // grpTop
            // 
            grpTop.Controls.Add(progressTop);
            grpTop.Controls.Add(lblTopCount);
            grpTop.Location = new System.Drawing.Point(20, 55);
            grpTop.Name = "grpTop";
            grpTop.Size = new System.Drawing.Size(380, 70);
            grpTop.TabIndex = 3;
            grpTop.TabStop = false;
            grpTop.Text = "Top 面進度";
            // 
            // progressTop
            // 
            progressTop.Location = new System.Drawing.Point(15, 30);
            progressTop.Name = "progressTop";
            progressTop.Size = new System.Drawing.Size(260, 23);
            progressTop.TabIndex = 0;
            // 
            // lblTopCount
            // 
            lblTopCount.AutoSize = true;
            lblTopCount.Location = new System.Drawing.Point(285, 34);
            lblTopCount.Name = "lblTopCount";
            lblTopCount.Size = new System.Drawing.Size(55, 15);
            lblTopCount.TabIndex = 1;
            lblTopCount.Text = "Top: 0/0";
            // 
            // grpBottom
            // 
            grpBottom.Controls.Add(progressBottom);
            grpBottom.Controls.Add(lblBottomCount);
            grpBottom.Location = new System.Drawing.Point(20, 135);
            grpBottom.Name = "grpBottom";
            grpBottom.Size = new System.Drawing.Size(380, 70);
            grpBottom.TabIndex = 4;
            grpBottom.TabStop = false;
            grpBottom.Text = "Bottom 面進度";
            // 
            // progressBottom
            // 
            progressBottom.Location = new System.Drawing.Point(15, 30);
            progressBottom.Name = "progressBottom";
            progressBottom.Size = new System.Drawing.Size(260, 23);
            progressBottom.TabIndex = 0;
            // 
            // lblBottomCount
            // 
            lblBottomCount.AutoSize = true;
            lblBottomCount.Location = new System.Drawing.Point(285, 34);
            lblBottomCount.Name = "lblBottomCount";
            lblBottomCount.Size = new System.Drawing.Size(74, 15);
            lblBottomCount.TabIndex = 1;
            lblBottomCount.Text = "Bottom: 0/0";
            // 
            // dgvWorkers
            // 
            dgvWorkers.AllowUserToAddRows = false;
            dgvWorkers.AllowUserToDeleteRows = false;
            dgvWorkers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvWorkers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvWorkers.Columns.AddRange(new DataGridViewColumn[] { colName, colSide, colStatus, colFrames });
            dgvWorkers.Location = new System.Drawing.Point(20, 220);
            dgvWorkers.Name = "dgvWorkers";
            dgvWorkers.ReadOnly = true;
            dgvWorkers.Size = new System.Drawing.Size(560, 220);
            dgvWorkers.TabIndex = 5;
            // 
            // colName
            // 
            colName.Name = "colName";
            colName.ReadOnly = true;
            // 
            // colSide
            // 
            colSide.Name = "colSide";
            colSide.ReadOnly = true;
            // 
            // colStatus
            // 
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // colFrames
            // 
            colFrames.Name = "colFrames";
            colFrames.ReadOnly = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(600, 460);
            Controls.Add(dgvWorkers);
            Controls.Add(grpBottom);
            Controls.Add(grpTop);
            Controls.Add(btnRefreshBatches);
            Controls.Add(cmbBatch);
            Controls.Add(lblBatchLabel);
            Name = "MainForm";
            Text = "Grab Progress Monitor (WinForms)";
            Load += MainForm_Load;
            grpTop.ResumeLayout(false);
            grpTop.PerformLayout();
            grpBottom.ResumeLayout(false);
            grpBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvWorkers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colSide;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colFrames;
    }
}

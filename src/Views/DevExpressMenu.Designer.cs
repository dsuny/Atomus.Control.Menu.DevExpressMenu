namespace Atomus.Control.Menu
{
    partial class DevExpressMenu
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bnt_CollapseAll = new DevExpress.XtraEditors.SimpleButton();
            this.bnt_Refresh = new DevExpress.XtraEditors.SimpleButton();
            this.bnt_ExpendAll = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.bnt_CollapseAll, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.bnt_Refresh, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.bnt_ExpendAll, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(542, 35);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // bnt_CollapseAll
            // 
            this.bnt_CollapseAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bnt_CollapseAll.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.bnt_CollapseAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bnt_CollapseAll.Location = new System.Drawing.Point(61, 4);
            this.bnt_CollapseAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnt_CollapseAll.Name = "bnt_CollapseAll";
            this.bnt_CollapseAll.Size = new System.Drawing.Size(23, 27);
            this.bnt_CollapseAll.TabIndex = 4;
            this.bnt_CollapseAll.Click += new System.EventHandler(this.Bnt_CollapseAll_Click);
            // 
            // bnt_Refresh
            // 
            this.bnt_Refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bnt_Refresh.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.bnt_Refresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bnt_Refresh.Location = new System.Drawing.Point(3, 4);
            this.bnt_Refresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnt_Refresh.Name = "bnt_Refresh";
            this.bnt_Refresh.Size = new System.Drawing.Size(23, 27);
            this.bnt_Refresh.TabIndex = 2;
            this.bnt_Refresh.Click += new System.EventHandler(this.Bnt_Refresh_Click);
            // 
            // bnt_ExpendAll
            // 
            this.bnt_ExpendAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bnt_ExpendAll.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.bnt_ExpendAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bnt_ExpendAll.Location = new System.Drawing.Point(32, 4);
            this.bnt_ExpendAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnt_ExpendAll.Name = "bnt_ExpendAll";
            this.bnt_ExpendAll.Size = new System.Drawing.Size(23, 27);
            this.bnt_ExpendAll.TabIndex = 3;
            this.bnt_ExpendAll.Click += new System.EventHandler(this.Bnt_ExpendAll_Click);
            // 
            // DevExpressMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "DevExpressMenu";
            this.Size = new System.Drawing.Size(542, 523);
            this.Load += new System.EventHandler(this.DefaultMenu_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton bnt_Refresh;
        private DevExpress.XtraEditors.SimpleButton bnt_ExpendAll;
        private DevExpress.XtraEditors.SimpleButton bnt_CollapseAll;

    }
}

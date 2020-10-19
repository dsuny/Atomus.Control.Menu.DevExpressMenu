using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atomus.Diagnostics;
using Atomus.Control.Menu.Controllers;
using Atomus.Control.Menu.Models;

namespace Atomus.Control.Menu
{
    public partial class DevExpressMenu : DevExpress.XtraEditors.XtraUserControl, IAction
    {
        private AtomusControlEventHandler beforeActionEventHandler;
        private AtomusControlEventHandler afterActionEventHandler;
        private ImageList imageList;
        private decimal MENU_ID = -1;
        private decimal PARENT_MENU_ID = -1;
        private System.Windows.Forms.Control MenuControl;

        #region Init
        public DevExpressMenu()
        {
            TreeView treeView;
            DevExpress.XtraBars.Navigation.AccordionControl accordionControl;

            InitializeComponent();

            switch (this.GetAttribute("MenuType"))
            {
                case "TreeView":
                    treeView = new TreeView();
                    treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
                    treeView.Dock = System.Windows.Forms.DockStyle.Fill;

                    treeView.HotTracking = true;
                    treeView.ItemHeight = 18;
                    treeView.Location = new System.Drawing.Point(0, 35);
                    treeView.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
                    treeView.Name = "tvw_menu";
                    treeView.ShowNodeToolTips = true;
                    treeView.Size = new System.Drawing.Size(542, 488);
                    treeView.TabIndex = 1;
                    treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.Tvw_menu_NodeMouseClick);
                    treeView.DoubleClick += new System.EventHandler(this.Tvw_menu_DoubleClick);

                    this.MenuControl = treeView;
                    break;
                case "AccordionControl":
                    accordionControl = new DevExpress.XtraBars.Navigation.AccordionControl();
                    accordionControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    accordionControl.Dock = System.Windows.Forms.DockStyle.Fill;

                    //accordionControl.ItemHeight = 18;
                    accordionControl.Location = new System.Drawing.Point(0, 35);
                    accordionControl.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
                    accordionControl.Name = "accordionControl1";
                    accordionControl.ShowToolTips = true;
                    accordionControl.Size = new System.Drawing.Size(542, 488);
                    accordionControl.TabIndex = 1;

                    accordionControl.AllowItemSelection = true;

                    this.MenuControl = accordionControl;
                    break;
            }

            this.Controls.Add(this.MenuControl);
            this.MenuControl.BringToFront();
        }
        #endregion

        #region Dictionary
        #endregion

        #region Spread
        #endregion

        #region IO
        object IAction.ControlAction(ICore sender, AtomusControlArgs e)
        {
            decimal[] vs;
            try
            {
                this.beforeActionEventHandler?.Invoke(this, e);

                switch (e.Action)
                {
                    case "Search":
                        vs = (decimal[])e.Value;

                        this.MENU_ID = vs[0];
                        this.PARENT_MENU_ID = vs[1];
                        this.Bnt_Refresh_Click(this.bnt_Refresh, null);
                        return true;
                    default:
                        throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            finally
            {
                this.afterActionEventHandler?.Invoke(this, e);
            }
        }

        private async Task<bool> LoadMenu(TreeView tvw_menu, decimal START_MENU_ID, decimal ONLY_PARENT_MENU_ID)
        {
            Service.IResponse result;
            TreeNode treeNode;
            TreeNode[] treeNodes;
            string[] tmps;
            string tmp;
            string key;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                tvw_menu.BeginUpdate();//트리갱신 준비 
                tvw_menu.Nodes.Clear();

                result = await this.SearchAsync(new DevExpressMenuSearchModel()
                {
                    START_MENU_ID = START_MENU_ID,
                    ONLY_PARENT_MENU_ID = ONLY_PARENT_MENU_ID
                });

                if (result.Status == Service.Status.OK)
                {
                    foreach (DataRow dataRow in result.DataSet.Tables[1].Rows)
                    {
                        key = string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1);
                        tmp = dataRow["IMAGE_URL1"].ToString();

                        if (!this.imageList.Images.ContainsKey(key) && tmp != "")
                            this.imageList.Images.Add(key, await this.GetAttributeWebImage(new Uri(tmp)));

                        key = string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2);
                        tmp = dataRow["IMAGE_URL2"].ToString();
                        if (!this.imageList.Images.ContainsKey(key) && tmp != "")
                            this.imageList.Images.Add(key, await this.GetAttributeWebImage(new Uri(tmp)));

                        key = string.Format("{0}.{1}", dataRow["IMAGE_URL3"], 3);
                        tmp = dataRow["IMAGE_URL3"].ToString();
                        if (!this.imageList.Images.ContainsKey(key) && tmp != "")
                            this.imageList.Images.Add(key, await this.GetAttributeWebImage(new Uri(tmp)));

                        key = string.Format("{0}.{1}", dataRow["IMAGE_URL4"], 4);
                        tmp = dataRow["IMAGE_URL4"].ToString();
                        if (!this.imageList.Images.ContainsKey(key) && tmp != "")
                            this.imageList.Images.Add(key, await this.GetAttributeWebImage(new Uri(tmp)));
                    }

                    foreach (DataRow dataRow in result.DataSet.Tables[1].Rows)
                    {
                        treeNodes = tvw_menu.Nodes.Find(dataRow["PARENT_MENU_ID"].ToString(), true);

                        dataRow["NAME"] = dataRow["NAME"].ToString().Translate();

                        tmps = this.GetAttribute("ShowNamespace.RESPONSIBILITY_ID").Split(',');

                        if (tmps.Contains(Config.Client.GetAttribute("Account.RESPONSIBILITY_ID").ToString()))
                            dataRow["DESCRIPTION"] = string.Format("{0} {1}", dataRow["DESCRIPTION"].ToString().Translate(), dataRow["NAMESPACE"]);
                        else
                            dataRow["DESCRIPTION"] = dataRow["DESCRIPTION"].ToString().Translate();

                        if (treeNodes.Length == 0)//없다면
                            treeNode = tvw_menu.Nodes.Add(dataRow["MENU_ID"].ToString(), dataRow["NAME"].ToString());
                        else//있다면
                        {
                            treeNode = treeNodes[0];
                            treeNode = treeNode.Nodes.Add(dataRow["MENU_ID"].ToString(), dataRow["NAME"].ToString());
                        }

                        treeNode.ToolTipText = dataRow["DESCRIPTION"].ToString();

                        if (dataRow["ASSEMBLY_ID"].ToString().Equals(""))//폴더
                        {
                            if (this.imageList.Images[string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1)] != null)
                            {
                                treeNode.ImageIndex = -1;
                                treeNode.ImageKey = string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1);
                            }
                            else
                            {
                                treeNode.ImageKey = "";
                                treeNode.ImageIndex = 0;
                            }

                            if (this.imageList.Images[string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2)] != null)
                            {
                                treeNode.SelectedImageIndex = -1;
                                treeNode.SelectedImageKey = string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2);
                            }
                            else
                            {
                                treeNode.SelectedImageKey = "";
                                treeNode.SelectedImageIndex = 1;
                            }
                        }
                        else//화면
                        {
                            if (this.imageList.Images[string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1)] != null)
                            {
                                treeNode.ImageIndex = -1;
                                treeNode.ImageKey = string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1);
                            }
                            else
                            {
                                treeNode.ImageKey = "";
                                treeNode.ImageIndex = 2;
                            }

                            if (this.imageList.Images[string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2)] != null)
                            {
                                treeNode.SelectedImageIndex = -1;
                                treeNode.SelectedImageKey = string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2);
                            }
                            else
                            {
                                treeNode.SelectedImageKey = "";
                                treeNode.SelectedImageIndex = 3;
                            }

                            //treeNode.ImageIndex = 2;
                            //treeNode.SelectedImageIndex = 3;

                            tmps = this.GetAttribute("ShowAssemblyID.RESPONSIBILITY_ID").Split(',');

                            if (tmps.Contains(Config.Client.GetAttribute("Account.RESPONSIBILITY_ID").ToString()))
                                treeNode.Text += string.Format(" ({0}.{1})", dataRow["MENU_ID"].ToString(), dataRow["ASSEMBLY_ID"].ToString());

                            treeNode.Tag = dataRow;
                        }
                    }

                    return true;
                }
                else
                {
                    this.MessageBoxShow(this, result.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            finally
            {
                tvw_menu.EndUpdate();
                this.Cursor = Cursors.Default;
            }
        }

        private async Task<bool> LoadMenu(DevExpress.XtraBars.Navigation.AccordionControl tvw_menu, decimal START_MENU_ID, decimal ONLY_PARENT_MENU_ID)
        {
            Service.IResponse result;
            DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElementParent;
            DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement;
            string[] tmps;
            string tmp;
            string key;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                tvw_menu.BeginUpdate();//트리갱신 준비 
                tvw_menu.Clear();

                result = await this.SearchAsync(new DevExpressMenuSearchModel()
                {
                    START_MENU_ID = START_MENU_ID,
                    ONLY_PARENT_MENU_ID = ONLY_PARENT_MENU_ID
                });

                if (result.Status == Service.Status.OK)
                {
                    foreach (DataRow dataRow in result.DataSet.Tables[1].Rows)
                    {
                        key = string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1);
                        tmp = dataRow["IMAGE_URL1"].ToString();

                        if (!this.imageList.Images.ContainsKey(key) && tmp != "")
                            this.imageList.Images.Add(key, await this.GetAttributeWebImage(new Uri(tmp)));

                        key = string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2);
                        tmp = dataRow["IMAGE_URL2"].ToString();
                        if (!this.imageList.Images.ContainsKey(key) && tmp != "")
                            this.imageList.Images.Add(key, await this.GetAttributeWebImage(new Uri(tmp)));

                        key = string.Format("{0}.{1}", dataRow["IMAGE_URL3"], 3);
                        tmp = dataRow["IMAGE_URL3"].ToString();
                        if (!this.imageList.Images.ContainsKey(key) && tmp != "")
                            this.imageList.Images.Add(key, await this.GetAttributeWebImage(new Uri(tmp)));

                        key = string.Format("{0}.{1}", dataRow["IMAGE_URL4"], 4);
                        tmp = dataRow["IMAGE_URL4"].ToString();
                        if (!this.imageList.Images.ContainsKey(key) && tmp != "")
                            this.imageList.Images.Add(key, await this.GetAttributeWebImage(new Uri(tmp)));
                    }

                    foreach (DataRow dataRow in result.DataSet.Tables[1].Rows)
                    {
                        accordionControlElementParent = tvw_menu.Elements[dataRow["PARENT_MENU_ID"].ToString()];

                        tvw_menu.ForEachElement((el) => {
                            accordionControlElementParent = accordionControlElementParent == null ? el.Elements[dataRow["PARENT_MENU_ID"].ToString()] : accordionControlElementParent;
                        });


                        dataRow["NAME"] = dataRow["NAME"].ToString().Translate();

                        tmps = this.GetAttribute("ShowNamespace.RESPONSIBILITY_ID").Split(',');

                        if (tmps.Contains(Config.Client.GetAttribute("Account.RESPONSIBILITY_ID").ToString()))
                            dataRow["DESCRIPTION"] = string.Format("{0} {1}", dataRow["DESCRIPTION"].ToString().Translate(), dataRow["NAMESPACE"]);
                        else
                            dataRow["DESCRIPTION"] = dataRow["DESCRIPTION"].ToString().Translate();

                        if (accordionControlElementParent == null)//없다면
                        {
                            accordionControlElement = new DevExpress.XtraBars.Navigation.AccordionControlElement();
                            accordionControlElement.Name = dataRow["MENU_ID"].ToString();
                            accordionControlElement.Text = dataRow["NAME"].ToString();

                            tvw_menu.Elements.Add(accordionControlElement);
                        }
                        else//있다면
                        {
                            accordionControlElement = new DevExpress.XtraBars.Navigation.AccordionControlElement();
                            accordionControlElement.Name = dataRow["MENU_ID"].ToString();
                            accordionControlElement.Text = dataRow["NAME"].ToString();

                            accordionControlElementParent.Elements.Add(accordionControlElement);
                        }

                        accordionControlElement.Hint = dataRow["DESCRIPTION"].ToString();

                        if (dataRow["ASSEMBLY_ID"].ToString().Equals(""))//폴더
                        {
                            if (this.imageList.Images[string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1)] != null)
                                accordionControlElement.ImageIndex = this.imageList.Images.IndexOfKey(string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1));
                            else
                                accordionControlElement.ImageIndex = 0;
                            //if (this.imageList.Images[string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2)] != null)
                            //{
                            //    treeNode.SelectedImageIndex = -1;
                            //    treeNode.SelectedImageKey = string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2);
                            //}
                            //else
                            //{
                            //    treeNode.SelectedImageKey = "";
                            //    treeNode.SelectedImageIndex = 1;
                            //}
                            accordionControlElement.Click += this.Tvw_menu_DoubleClick;
                        }
                        else//화면
                        {
                            if (this.imageList.Images[string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1)] != null)
                                accordionControlElement.ImageIndex = this.imageList.Images.IndexOfKey(string.Format("{0}.{1}", dataRow["IMAGE_URL1"], 1));
                            else
                                accordionControlElement.ImageIndex = 2;

                            //if (this.imageList.Images[string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2)] != null)
                            //{
                            //    treeNode.SelectedImageIndex = -1;
                            //    treeNode.SelectedImageKey = string.Format("{0}.{1}", dataRow["IMAGE_URL2"], 2);
                            //}
                            //else
                            //{
                            //    treeNode.SelectedImageKey = "";
                            //    treeNode.SelectedImageIndex = 3;
                            //}

                            //treeNode.ImageIndex = 2;
                            //treeNode.SelectedImageIndex = 3;

                            accordionControlElement.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;

                            tmps = this.GetAttribute("ShowAssemblyID.RESPONSIBILITY_ID").Split(',');

                            if (tmps.Contains(Config.Client.GetAttribute("Account.RESPONSIBILITY_ID").ToString()))
                                accordionControlElement.Text += string.Format(" ({0}.{1})", dataRow["MENU_ID"].ToString(), dataRow["ASSEMBLY_ID"].ToString());

                            accordionControlElement.Tag = dataRow;

                            accordionControlElement.Click += this.Tvw_menu_DoubleClick;
                        }
                    }

                    return true;
                }
                else
                {
                    this.MessageBoxShow(this, result.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            finally
            {
                tvw_menu.EndUpdate();
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region Event
        event AtomusControlEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                this.beforeActionEventHandler += value;
            }
            remove
            {
                this.beforeActionEventHandler -= value;
            }
        }
        event AtomusControlEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                this.afterActionEventHandler += value;
            }
            remove
            {
                this.afterActionEventHandler -= value;
            }
        }

        private async void DefaultMenu_Load(object sender, EventArgs e)
        {
            ContextMenuStrip contextMenuStrip;
            ToolStripMenuItem toolStripMenuItem;
            ToolStripSeparator toolStripSeparator;
            string[] tmps;

            try
            {
                if (this.GetAttribute("VisibleResponsibilityID").Trim() != "")
                {
                    tmps = this.GetAttribute("VisibleResponsibilityID").Split(',');

                    this.Visible = tmps.Contains(Config.Client.GetAttribute("Account.RESPONSIBILITY_ID").ToString());
                }

                this.tableLayoutPanel1.DoubleBuffered(true);
                this.MenuControl.DoubleBuffered(true);
                this.bnt_Refresh.DoubleBuffered(true);
                this.bnt_ExpendAll.DoubleBuffered(true);
                this.bnt_CollapseAll.DoubleBuffered(true);

                //this.bnt_Refresh.AppearancePressed.BackColor = this.FindForm().BackColor;
                //this.bnt_ExpendAll.FlatAppearance.MouseDownBackColor = this.FindForm().BackColor;
                //this.bnt_CollapseAll.FlatAppearance.MouseDownBackColor = this.FindForm().BackColor;

                //this.bnt_Refresh.AppearanceHovered.BackColor = this.FindForm().BackColor;
                //this.bnt_ExpendAll.FlatAppearance.MouseOverBackColor = this.FindForm().BackColor;
                //this.bnt_CollapseAll.FlatAppearance.MouseOverBackColor = this.FindForm().BackColor;

                try
                {
                    this.bnt_Refresh.BackgroundImage = await this.GetAttributeWebImage("RefreshImage");
                }
                catch (Exception exception)
                {
                    DiagnosticsTool.MyTrace(exception);
                }

                try
                {
                    this.bnt_ExpendAll.BackgroundImage = await this.GetAttributeWebImage("ExpendAllImage");
                }
                catch (Exception exception)
                {
                    DiagnosticsTool.MyTrace(exception);
                }

                try
                {
                    this.bnt_CollapseAll.BackgroundImage = await this.GetAttributeWebImage("CollapseAllImage");
                }
                catch (Exception exception)
                {
                    DiagnosticsTool.MyTrace(exception);
                }

                this.imageList = new ImageList
                {
                    ImageSize = new Size(18, 18)
                };

                try
                {
                    this.imageList.Images.Add(await this.GetAttributeWebImage("FolderImage"));
                }
                catch (Exception exception)
                {
                    DiagnosticsTool.MyTrace(exception);
                }

                try
                {
                    this.imageList.Images.Add(await this.GetAttributeWebImage("FolderOpenImage"));
                }
                catch (Exception exception)
                {
                    DiagnosticsTool.MyTrace(exception);
                }

                try
                {
                    this.imageList.Images.Add(await this.GetAttributeWebImage("AssembliesImage"));
                }
                catch (Exception exception)
                {
                    DiagnosticsTool.MyTrace(exception);
                }

                try
                {
                    this.imageList.Images.Add(await this.GetAttributeWebImage("AssembliesOpenImage"));
                }
                catch (Exception exception)
                {
                    DiagnosticsTool.MyTrace(exception);
                }

                contextMenuStrip = new ContextMenuStrip();

                toolStripMenuItem = new ToolStripMenuItem("Execute", null, Tvw_menu_DoubleClick);
                contextMenuStrip.Items.Add(toolStripMenuItem);

                toolStripSeparator = new ToolStripSeparator();
                contextMenuStrip.Items.Add(toolStripSeparator);

                toolStripMenuItem = new ToolStripMenuItem("Refresh", null, Bnt_Refresh_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem);

                toolStripMenuItem = new ToolStripMenuItem("Expend", null, ToolStripMenuItem_Expend_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem);

                toolStripMenuItem = new ToolStripMenuItem("Expend all children", null, ToolStripMenuItem_Expend_Child_All_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem);

                toolStripMenuItem = new ToolStripMenuItem("Expend all", null, Bnt_ExpendAll_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem);

                toolStripMenuItem = new ToolStripMenuItem("Collapse", null, ToolStripMenuItem_Collapse_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem);

                toolStripMenuItem = new ToolStripMenuItem("Collapse all children", null, ToolStripMenuItem_Collapse_Child_All_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem);

                toolStripMenuItem = new ToolStripMenuItem("Collapse all", null, Bnt_CollapseAll_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem);

                if (this.MenuControl is TreeView)
                {
                    TreeView treeView;

                    treeView = (TreeView)this.MenuControl;

                    treeView.BeginUpdate();
                    treeView.ImageList = this.imageList;
                    treeView.EndUpdate();

                    await this.LoadMenu(treeView, this.MENU_ID, this.PARENT_MENU_ID);

                    treeView.ContextMenuStrip = contextMenuStrip;
                }

                if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                {
                    DevExpress.XtraBars.Navigation.AccordionControl accordionControl;

                    accordionControl = (DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl;

                    try
                    {
                        accordionControl.ShowFilterControl = (DevExpress.XtraBars.Navigation.ShowFilterControl)Enum.Parse(typeof(DevExpress.XtraBars.Navigation.ShowFilterControl), this.GetAttribute("ShowFilterControl"));
                    }
                    catch (Exception exception)
                    {
                        DiagnosticsTool.MyTrace(exception);
                    }

                    accordionControl.BeginUpdate();
                    accordionControl.Images = this.imageList;
                    accordionControl.EndUpdate();

                    await this.LoadMenu(accordionControl, this.MENU_ID, this.PARENT_MENU_ID);

                    accordionControl.ContextMenuStrip = contextMenuStrip;
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }
        private async void Bnt_Refresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.MenuControl is TreeView)
                    await this.LoadMenu((TreeView)this.MenuControl, this.MENU_ID, this.PARENT_MENU_ID);

                if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                    await this.LoadMenu((DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl, this.MENU_ID, this.PARENT_MENU_ID);
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }

        private void ToolStripMenuItem_Expend_Click(object sender, EventArgs e)
        {
            TreeView treeView;
            DevExpress.XtraBars.Navigation.AccordionControl accordionControl;

            try
            {
                if (this.MenuControl is TreeView)
                {
                    treeView = (TreeView)this.MenuControl;

                    if (treeView.SelectedNode != null)
                        treeView.SelectedNode.Expand();
                }

                if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                {
                    accordionControl = (DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl;

                    if (accordionControl.SelectedElement != null)
                        accordionControl.SelectedElement.Expanded = true;
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }
        private void ToolStripMenuItem_Expend_Child_All_Click(object sender, EventArgs e)
        {
            TreeView treeView;
            DevExpress.XtraBars.Navigation.AccordionControl accordionControl;

            try
            {
                if (this.MenuControl is TreeView)
                {
                    treeView = (TreeView)this.MenuControl;

                    if (treeView.SelectedNode != null)
                        treeView.SelectedNode.ExpandAll();
                }

                if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                {
                    accordionControl = (DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl;

                    if (accordionControl.Tag != null)
                    {
                        ((DevExpress.XtraBars.Navigation.AccordionControlElement)accordionControl.Tag).Expanded = true  ;

                        this.AccordionControlElementExpanded((DevExpress.XtraBars.Navigation.AccordionControlElement)accordionControl.Tag, true);
                    }


                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }
        private void AccordionControlElementExpanded(DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement, bool expanded)
        {
            if (accordionControlElement.Elements.Count == 0)
                return;

            foreach (DevExpress.XtraBars.Navigation.AccordionControlElement item in accordionControlElement.Elements)
            {
                item.Expanded = expanded;
                this.AccordionControlElementExpanded(item, expanded);
            }
        }

        private void Bnt_ExpendAll_Click(object sender, EventArgs e)
        {
            if (this.MenuControl is TreeView)
                ((TreeView)this.MenuControl).ExpandAll();

            if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                ((DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl).ExpandAll();
        }

        private void ToolStripMenuItem_Collapse_Click(object sender, EventArgs e)
        {
            TreeView treeView;
            DevExpress.XtraBars.Navigation.AccordionControl accordionControl;

            try
            {
                if (this.MenuControl is TreeView)
                {
                    treeView = (TreeView)this.MenuControl;

                    if (treeView.SelectedNode != null)
                        treeView.SelectedNode.Collapse(true);
                }

                if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                {
                    accordionControl = (DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl;

                    if (accordionControl.SelectedElement != null)
                        accordionControl.SelectedElement.Expanded = false;
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }
        private void ToolStripMenuItem_Collapse_Child_All_Click(object sender, EventArgs e)
        {
            TreeView treeView;
            DevExpress.XtraBars.Navigation.AccordionControl accordionControl;

            try
            {
                if (this.MenuControl is TreeView)
                {
                    treeView = (TreeView)this.MenuControl;

                    if (treeView.SelectedNode != null)
                        treeView.SelectedNode.Collapse(false);
                }

                if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                {
                    accordionControl = (DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl;

                    if (accordionControl.Tag != null)
                    {
                        ((DevExpress.XtraBars.Navigation.AccordionControlElement)accordionControl.Tag).Expanded = false;

                        this.AccordionControlElementExpanded((DevExpress.XtraBars.Navigation.AccordionControlElement)accordionControl.Tag, false);
                    }
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }
        private void Bnt_CollapseAll_Click(object sender, EventArgs e)
        {
            if (this.MenuControl is TreeView)
                ((TreeView)this.MenuControl).CollapseAll();

            if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                ((DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl).CollapseAll();
        }

        private void Tvw_menu_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeView treeView;

            treeView = (TreeView)sender;

            treeView.SelectedNode = e.Node;
        }

        private void Tvw_menu_DoubleClick(object sender, EventArgs e)
        {
            TreeView treeView;
            DevExpress.XtraBars.Navigation.AccordionControl accordionControl;
            DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement;
            DataRow dataRow;

            try
            {
                if (this.MenuControl is TreeView)
                {
                    if (sender is TreeView)
                        treeView = (TreeView)sender;
                    else
                        treeView = (TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;

                    if (treeView.SelectedNode != null && treeView.SelectedNode.Tag != null)
                    {
                        dataRow = (DataRow)treeView.SelectedNode.Tag;

                        if (!dataRow["ASSEMBLY_ID"].ToString().Equals(""))
                        {
                            try
                            {
                                this.Cursor = Cursors.WaitCursor;
                                treeView.Enabled = false;

                                this.afterActionEventHandler?.Invoke(this, "Menu.OpenControl", new object[] { dataRow["MENU_ID"], dataRow["ASSEMBLY_ID"], dataRow["VISIBLE_ONE"].ToString().Equals("Y") });
                            }
                            finally
                            {
                                treeView.Enabled = true;
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }

                if (this.MenuControl is DevExpress.XtraBars.Navigation.AccordionControl)
                {
                    accordionControl = (DevExpress.XtraBars.Navigation.AccordionControl)this.MenuControl;

                    if (sender is DevExpress.XtraBars.Navigation.AccordionControlElement)
                    {
                        accordionControlElement = (DevExpress.XtraBars.Navigation.AccordionControlElement)sender;
                        if (accordionControl.SelectedElement != accordionControlElement)
                            accordionControl.SelectedElement = null;
                    }
                    else
                        accordionControlElement = accordionControl.SelectedElement;

                    accordionControl.Tag = accordionControlElement;

                    if (accordionControlElement != null && accordionControlElement.Tag != null && !accordionControl.ContextMenuStrip.Visible)
                    {
                        dataRow = (DataRow)accordionControlElement.Tag;

                        if (!dataRow["ASSEMBLY_ID"].ToString().Equals(""))
                        {
                            try
                            {
                                this.Cursor = Cursors.WaitCursor;
                                accordionControl.Enabled = false;

                                this.afterActionEventHandler?.Invoke(this, "Menu.OpenControl", new object[] { dataRow["MENU_ID"], dataRow["ASSEMBLY_ID"], dataRow["VISIBLE_ONE"].ToString().Equals("Y") });
                            }
                            finally
                            {
                                accordionControl.Enabled = true;
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }
        #endregion

        #region "ETC"
        #endregion
    }
}
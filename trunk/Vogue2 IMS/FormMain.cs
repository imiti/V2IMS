﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using Vogue2_IMS.Business;
using Vogue2_IMS.Business.BusinessModel;
using Vogue2_IMS.Business.ViewModel;
using Vogue2_IMS.Common.FormBase;
using Vogue2_IMS.GoodsManager;
using Vogue2_IMS.Model.EnumModel;
using Vogue2_IMS.OrderManager;
using Vogue2_IMS.ReceiptManager;
using Vogue2_IMS.SystemManager;
using Vogue2_IMS.UserManager;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using System.Drawing;

namespace Vogue2_IMS
{
    public partial class FormMain : RibbonFormSimpleDialogBase
    {
        #region About Log In

        private ManualResetEvent waitInitializeHandler = new ManualResetEvent(false);
        private ManualResetEvent waitloginHandler = null;
        private delegate void FormShowHandler();
        public DialogResult loginResult = DialogResult.Abort;

        private bool isLoaded = false;
        private void Login()
        {
            FormLogin flogin = new FormLogin(waitInitializeHandler);
            waitloginHandler = flogin.waitLoginHandler;
            flogin.SetDialogResult += SetDialogResult;

            loginResult = flogin.ShowDialog();
        }

        private void SetDialogResult(DialogResult dialogResult)
        {
            this.loginResult = dialogResult;
        }

        private void LoginCallback(IAsyncResult ar)
        {
            try
            {
                AsyncResult result = (AsyncResult)ar;

                ((FormShowHandler)result.AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(string.Format("Method: LoginCallback \r\bMssage:{0}", ex.Message));
            }
        }

        #endregion

        public FormMain()
            : base(Common.Language.Chinese)
        {
            InitializeComponent();

            new FormShowHandler(Login).BeginInvoke(new AsyncCallback(LoginCallback), null);

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            waitInitializeHandler.WaitOne();
        }

        #region This

        /// <summary>
        /// 加载视图
        /// </summary>
        private void RegiserView()
        {
            mRibbonPageViews.Add(rPageGoodsManager, mFmGoodsMainView);//商品管理
            mRibbonPageViews.Add(rPageSystemConfig, mFmSystemConfigView);//系统设置
            mRibbonPageViews.Add(rPageUserManager, mFmUserView);//用户管理 

            ribbon.SelectedPage = null;
            ribbon.SelectedPage = rPageGoodsManager;
        }

        /// <summary>
        /// 视图当前查询条件 
        /// </summary>
        private ViewQueryGoodsInfo mCurrentQueryInfo
        {
            get
            {
                if (mFmGoodsMainView.DefaultQueryInfo == null)
                    mFmGoodsMainView.DefaultQueryInfo = GetViewDateQuery(QueryDateRange.ThisWeek);

                return mFmGoodsMainView.DefaultQueryInfo;
            }

            set { mFmGoodsMainView.DefaultQueryInfo = value; }
        }
        Dictionary<RibbonPage, Form> mRibbonPageViews = new Dictionary<RibbonPage, Form>();

        private void ribbon_SelectedPageChanged(object sender, System.EventArgs e)
        {
            if (ribbon.SelectedPage != null && mRibbonPageViews.ContainsKey(ribbon.SelectedPage))
            {
                ViewContainers.Controls.Clear();
                var view = mRibbonPageViews[ribbon.SelectedPage];
                if (view != null && view is Form)
                {
                    mRibbonPageViews[ribbon.SelectedPage].TopLevel = false;
                    mRibbonPageViews[ribbon.SelectedPage].Dock = DockStyle.Fill;
                    ViewContainers.Controls.Add(mRibbonPageViews[ribbon.SelectedPage]);
                    mRibbonPageViews[ribbon.SelectedPage].Show();
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                if (loginResult == DialogResult.OK)
                {
                    this.WindowState = FormWindowState.Maximized;
                    this.ShowInTaskbar = true;
                    this.Text += string.Format("[{0}-{1}]", SharedVariables.Instance.RoleInfos.FirstOrDefault(r => r.Id == SharedVariables.Instance.LoginUser.User.RoleId).Name, SharedVariables.Instance.LoginUser.User.Name);
                    RegiserView();
                    // 起线程取预警消息
                    SCWarning();
                    //LoadWarningMessages();
                    btnPayment.Enabled = SharedVariables.Instance.LoginUser.User.RoleId <= SharedVariables.PMRoleId;
                    this.btnAddBuyGoods.Enabled =
                    this.btnAddConsignmentGoods.Enabled = !(SharedVariables.Instance.LoginUser.User.RoleId > SharedVariables.PMRoleId);
                    btnImport.Enabled = SharedVariables.Instance.LoginUser.User.RoleId == SharedVariables.PMRoleId;
                    this.btnRollBackGoods.Enabled = SharedVariables.Instance.LoginUser.User.RoleId == SharedVariables.AdminRoleId;
                }
                else
                {
                    Application.Exit();
                }
            }
            finally
            {
                if (waitloginHandler != null)
                {
                    waitloginHandler.Set();
                }
            }
        }

        private void SCWarning()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                Thread thread = new Thread(new ThreadStart(LoadWarningMessages));
                thread.Start();
            }
        }

        #endregion

        #region System Config

        FmSystemConfigView _FmSystemConfigView = null;
        FmSystemConfigView mFmSystemConfigView
        {
            get
            {
                if (_FmSystemConfigView == null)
                {
                    _FmSystemConfigView = new FmSystemConfigView();
                    _FmSystemConfigView.TopLevel = false;
                    _FmSystemConfigView.Dock = DockStyle.Fill;
                    _FmSystemConfigView.ButtonAdd = btnAddNewConfig;
                    _FmSystemConfigView.ButtonUpdate = btnUpdateConfig;
                    _FmSystemConfigView.ButtonDelete = btnDeleteConfig;
                    _FmSystemConfigView.ButtonRefresh = btnRefreshConfigView;

                    //btnAddNewConfig.Enabled=btnUpdateConfig.Enabled=btnRefreshConfigView.Enabled
                    rPageGroupConfigManager.Visible = false;
                    _FmSystemConfigView.BtnClicked += new System.EventHandler<NavBarClickedArgs>((object sender, NavBarClickedArgs args)
                        =>
                    {
                        rPageGroupConfigManager.Visible = true;

                        rPageGroupConfigManager.Text = string.Format("[{0}]-管理", args.FunctionTypeName);
                        btnAddNewConfig.Caption = string.Format("新建[{0}]", args.FunctionTypeName);
                        btnUpdateConfig.Caption = string.Format("更新[{0}]", args.FunctionTypeName);
                        btnDeleteConfig.Caption = string.Format("删除[{0}]", args.FunctionTypeName);
                        btnRefreshConfigView.Caption = string.Format("刷新视图", args.FunctionTypeName);
                    });

                    this.rPageSystemConfig.Groups.Insert(0, this.MainThemePageGroup);

                    this.MainRibbonControl.Pages.Remove(this.MainPage);
                }

                return _FmSystemConfigView;
            }
            set { _FmSystemConfigView = value; }
        }

        #endregion

        #region User Manager

        FmUserView _FmUserView = null;
        FmUserView mFmUserView
        {
            get
            {
                if (_FmUserView == null)
                {
                    _FmUserView = new FmUserView();
                    _FmUserView.BtnAdd = BtnUserAdd;
                    _FmUserView.BtnRefResh = BtnUserViewRefresh;
                    _FmUserView.BtnResetPwd = BtnUserRestorePwd;
                    _FmUserView.BtnUpdate = BtnUserMondify;

                    BtnUserLoginName.Caption = SharedVariables.Instance.LoginUser.User.Name;
                }

                return _FmUserView;
            }
        }

        private void btnUserLogout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.Exit();
        }

        private void btnUserUpdateSelf_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            new FmUserInfo(new UserShopRoleInfo() { User = SharedVariables.Instance.LoginUser.User }).ShowDialog();
        }

        private void btnUserUpdatePwd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            new FmUpdatedPwd().ShowDialog();
        }

        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //mFmGoodsMainView.MainView.ShowPrintPreview();
        }

        #endregion

        #region Goods Manager

        List<ViewMainGoodsInfos> GoodsMainViewDataSource = new List<ViewMainGoodsInfos>();
        FmGoodsMainView _FmGoodsMainView = null;
        FmGoodsMainView mFmGoodsMainView
        {
            get
            {
                if (_FmGoodsMainView == null)
                {
                    _FmGoodsMainView = new FmGoodsMainView();
                    _FmGoodsMainView.TopLevel = false;
                    _FmGoodsMainView.Dock = DockStyle.Fill;
                    _FmGoodsMainView.GridDefaultView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(GridDefaultView_SelectionChanged);
                    _FmGoodsMainView.GridLayoutView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(GridDefaultView_SelectionChanged);
                    _FmGoodsMainView.GridAdvBandedView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(GridDefaultView_SelectionChanged);

                    _FmGoodsMainView.GridDefaultView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(GridDefaultView_RowStyle);
                    //_FmGoodsMainView.GridLayoutView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(GridDefaultView_RowStyle);
                    _FmGoodsMainView.GridAdvBandedView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(GridDefaultView_RowStyle);

                    _FmGoodsMainView.GridDefaultView.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(GridDefaultView_RowCellClick);
                    //_FmGoodsMainView.GridLayoutView.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(GridDefaultView_RowCellClick);
                    _FmGoodsMainView.GridAdvBandedView.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(GridDefaultView_RowCellClick);

                    _FmGoodsMainView.GridDefaultView.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.GridDefaultView_CustomDrawRowIndicator);
                    _FmGoodsMainView.GridAdvBandedView.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.GridDefaultView_CustomDrawRowIndicator);


                    _FmGoodsMainView.GridDefaultView.MouseDown += new MouseEventHandler(GridDefaultView_MouseDown);
                    //_FmGoodsMainView.GridLayoutView.MouseDown += new MouseEventHandler(GridDefaultView_MouseDown);
                    _FmGoodsMainView.GridAdvBandedView.MouseDown += new MouseEventHandler(GridDefaultView_MouseDown);
                    PurchaseGoodsBusiness.Instance.OnPurchaseGoodsUpdated += new EventHandler<CusEventArgs>(StartRefreshView);
                    SaleGoodsBusiness.Instance.OnSaledGoodsUpdated += new EventHandler<CusEventArgs>(StartRefreshView);

                    StartRefreshView(this, null);
                }

                return _FmGoodsMainView;
            }
            set { _FmGoodsMainView = value; }
        }

        private void StartRefreshView(object sender, CusEventArgs e)
        {
            mFmGoodsMainView.Enabled = false;

            Task task = new Task(() =>
            {
                try
                {
                    if (!isLoaded)
                    {
                        Thread.Sleep(500);
                    }
                    if (mCurrentQueryInfo.DateRange != QueryDateRange.Customer)
                    {
                        var queryDateInfo = GetViewDateQuery(mCurrentQueryInfo.DateRange);
                        //mCurrentQueryInfo.StartPurchaseDate = mCurrentQueryInfo.StartSaledDate = queryDateInfo.StartPurchaseDate;
                        //mCurrentQueryInfo.EndPurchaseDate = mCurrentQueryInfo.EndSaledDate = queryDateInfo.EndPurchaseDate;

                        mCurrentQueryInfo.StartDate = queryDateInfo.StartDate;
                        mCurrentQueryInfo.EndDate = queryDateInfo.EndDate;

                        mCurrentQueryInfo = mCurrentQueryInfo;
                    }

                    mFmGoodsMainView.DataSource = GoodsBusiness.Instance.GetGoodses(mCurrentQueryInfo);
                }
                finally
                {
                    if (mFmGoodsMainView.DataSource == null) mFmGoodsMainView.DataSource = new List<ViewMainGoodsInfos>();

                    MainViewSource_Changed();
                }
            });

            task.Start();
        }

        /// <summary>
        /// 获取（全记录）时间查询条件 
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private ViewQueryGoodsInfo GetViewDateQuery(QueryDateRange dateRange)
        {
            var dateNow = DateTime.Now;
            var startDate = DateTime.Now;

            if (dateRange == QueryDateRange.ThisWeek)
                startDate = dateNow.AddDays(1 - Convert.ToInt32(dateNow.DayOfWeek.ToString("d")));

            if (dateRange == QueryDateRange.ThisMonth)
                startDate = dateNow.AddDays(1 - dateNow.Day);

            if (dateRange == QueryDateRange.ThisQuarter)
                startDate = dateNow.AddMonths(0 - (dateNow.Month - 1) % 3).AddDays(1 - dateNow.Day);

            if (dateRange == QueryDateRange.ThisYear)
                startDate = new DateTime(dateNow.Year, 1, 1);

            if (dateRange == QueryDateRange.AllYear10)
                startDate = new DateTime(dateNow.Year - 10, 1, 1);

            return new ViewQueryGoodsInfo() { DateRange = dateRange, StartDate = startDate, EndDate = dateNow };
            //return new ViewQueryGoodsInfo() { DateRange = dateRange, StartPurchaseDate = startDate, StartSaledDate = startDate, EndPurchaseDate = dateNow, EndSaledDate = dateNow };
        }

        #region Warning Message Box

        private void LoadWarningMessages()
        {
            var warMsgDict = GoodsBusiness.Instance.GetWarningGoodsMsg(WarningType.JSWarning, (int)(DateTime.Now - DateTime.Now.AddYears(-10)).TotalDays);

            if (warMsgDict.Count == 0 || (int)warMsgDict["GoodsCount"] == 0)
            {
                return;
            }
            int goodsCount = (int)warMsgDict["GoodsCount"];
            DateTime startDate = (DateTime)warMsgDict["MinDate"];
            DateTime endDate = (DateTime)warMsgDict["MaxDate"];
            FmWarningMessageBox fmWarning = new FmWarningMessageBox(this, SharedVariables.Instance.LoginUser.User.Name, goodsCount, startDate, endDate);
            fmWarning.linkViewInfo_Clicked += new EventHandler(linkViewInfo_Click);

            warMsgDict.Clear();

            this.BeginInvoke(new MethodInvoker(delegate { fmWarning.Show(); }));
        }

        private void linkViewInfo_Click(object sender, EventArgs e)
        {
            mFmGoodsMainView.DataSource = GoodsBusiness.Instance.GetWarningGoods(WarningType.JSWarning, (int)(DateTime.Now - DateTime.Now.AddYears(-10)).TotalDays);
            MainViewSource_Changed();
        }

        #endregion

        #region Layout Event

        private void btnShowFindPanel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!mFmGoodsMainView.MainView.IsFindPanelVisible)
                mFmGoodsMainView.MainView.ShowFindPanel();
            else
                mFmGoodsMainView.MainView.HideFindPanel();
        }

        private void btnShowFilterRow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridOptionsView optionView = mFmGoodsMainView.MainView.OptionsView as GridOptionsView;

            if (optionView != null)
                optionView.ShowAutoFilterRow = !optionView.ShowAutoFilterRow;
            else
            {
                e.Link.UserPaintStyle = BarItemPaintStyle.Standard; ;//..Reset();
            }
        }

        private void btnShowGroupPanel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridOptionsView optionView = mFmGoodsMainView.MainView.OptionsView as GridOptionsView;

            if (optionView != null)
                optionView.ShowGroupPanel = !optionView.ShowGroupPanel;
            else
            {
                e.Item.Reset();
            }
        }

        private void btnSaveCustomView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("您确证要覆盖当前视图?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                mFmGoodsMainView.SaveCustomerViewLayout(false);
            }
        }

        private void btnSaveNewCustomView_ItemClick(object sender, ItemClickEventArgs e)
        {
            mFmGoodsMainView.SaveCustomerViewLayout(true);
        }

        private void btnSetToDefaultView_ItemClick(object sender, ItemClickEventArgs e)
        {
            mFmGoodsMainView.SaveDefaultMainView();
        }

        #endregion Layout Event

        #region View Event

        private List<ViewMainGoodsInfos> GetSelectedGoodsInfos()
        {
            var gridMainView = mFmGoodsMainView.MainView;
            if (gridMainView.SelectedRowsCount > 0)
            {
                List<ViewMainGoodsInfos> mainGoodsInfos = new List<ViewMainGoodsInfos>();
                int[] rowIndexs = gridMainView.GetSelectedRows();
                foreach (var rowIndex in rowIndexs)
                {
                    var mainGoodsInfo = gridMainView.GetRow(rowIndex) as ViewMainGoodsInfos;
                    mainGoodsInfos.Add(mainGoodsInfo);
                }
                return mainGoodsInfos;
            }
            return null;
        }

        private List<ViewMainGoodsInfos> GetCheckedGoodsInfos()
        {
            var gridMainView = mFmGoodsMainView.MainView;
            List<ViewMainGoodsInfos> chkedGoodsInfos = new List<ViewMainGoodsInfos>();
            for (var i = 0; i < gridMainView.RowCount; i++)
            {
                var rowObj = gridMainView.GetRow(i);
                if (rowObj == null)
                {
                    continue;
                }
                var mainGoodsInfo = gridMainView.GetRow(i) as ViewMainGoodsInfos;
                if (mainGoodsInfo.CheckEdit)
                {
                    chkedGoodsInfos.Add(mainGoodsInfo);
                }
            }
            return chkedGoodsInfos;
        }
        private void btnFindGoods_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            mFmGoodsMainView.MainView.ShowFindPanel();
        }

        private void btnUpdateGoods_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (mFmGoodsMainView.MainView.SelectedRowsCount > 0)
                {
                    var goodsInfo = mFmGoodsMainView.MainView.GetRow(mFmGoodsMainView.MainView.GetSelectedRows()[0]) as ViewMainGoodsInfos;
                    if (goodsInfo == null) return;

                    if (goodsInfo.GoodsStatus == GoodsStatus.In)
                    {
                        var fmGoodsInfo = new FmGoodsInfo(goodsInfo.GetPurchaseRecordInfo());
                        if (fmGoodsInfo.ShowDialog() == DialogResult.OK)
                        {
                            var result = fmGoodsInfo.PurchaseGoodsInfo;
                            Business.PurchaseGoodsBusiness.Instance.UpdatePurchaseGoods(result);
                        }
                    }
                    else if (goodsInfo.GoodsStatus == GoodsStatus.Catch)
                    {
                        var fmGoodsInfo = new FmGoodsSaledMondify(goodsInfo.GetSaledGoodsInfo());
                        if (fmGoodsInfo.ShowDialog() == DialogResult.OK)
                        {
                            var result = fmGoodsInfo.SaledGoodsInfo;
                            Business.SaleGoodsBusiness.Instance.UpdateSaledGoods(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(String.Format("更新失败:\r\n{0}", ex.ToString()), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPayment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var mainGoodsInfos = this.GetSelectedGoodsInfos();
            if (mainGoodsInfos == null) return;

            var tempGoodsInfos = mainGoodsInfos.Where(info =>
            {
                return info.Goods.SourceType == (int)SourceType.JiShou && info.Goods.Status == (int)GoodsStatus.In;
            }).ToList();

            if (tempGoodsInfos.Count > 0)
            {
                string noticeMsg = "所选商品包含寄售且未售出的商品:\r\n{0} 是否打款？";
                StringBuilder sb = new StringBuilder();
                tempGoodsInfos.ForEach(info =>
                {
                    sb.AppendFormat("【{0}】 {1}\r\n", info.Goods.Code, info.Goods.Name);
                });
                if (XtraMessageBox.Show(string.Format(noticeMsg, sb.ToString()), "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }
            else
            {
                if (XtraMessageBox.Show("您确定要打款？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            var purchaseGoodsInfos = mainGoodsInfos.Select(info =>
            {
                var goodsInfo = info.GetPurchaseRecordInfo();
                goodsInfo.Goods.Paid = (int)GoodsPaid.HasPaid;
                return goodsInfo;
            }).ToList();

            Business.PurchaseGoodsBusiness.Instance.BatchUpdatePurchaseGoods(purchaseGoodsInfos);

            //if (mFmGoodsMainView.MainView.SelectedRowsCount > 0)
            //{
            //    var rowIndexs = mFmGoodsMainView.MainView.GetSelectedRows();
            //    foreach (var rowIndex in rowIndexs)
            //    {
            //        var purchaseInfo = (mFmGoodsMainView.MainView.GetRow(rowIndex) as ViewMainGoodsInfos).GetPurchaseRecordInfo();
            //        if (purchaseInfo.Goods.SourceType == (int)SourceType.JiShou
            //                && purchaseInfo.Goods.Status == (int)GoodsStatus.In
            //                && XtraMessageBox.Show(string.Format("{0} {1} \r\n此寄售商品尚未售出,是否打款?"
            //               , purchaseInfo.Goods.Code, purchaseInfo.Goods.Name), "提示"
            //                 , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            //        {
            //            continue;
            //        }

            //        purchaseInfo.GoodsPaid = new GoodsPaidInfo() { Id = (int)GoodsPaid.HasPaid };//已付款

            //        Business.PurchaseGoodsBusiness.Instance.UpdatePurchaseGoods(purchaseInfo);
            //    }
            //}
        }

        private void btnRollBackGoods_ItemClick(object sender, ItemClickEventArgs e)
        {
            var chkedMainGoodsInfos = this.GetCheckedGoodsInfos();

            if (chkedMainGoodsInfos.Count > 0)
            {
                var rollBackEnabledGoods = new List<SaledGoodsInfo>();
                chkedMainGoodsInfos.ForEach(goods =>
                {
                    if (goods.Goods.Status == (int)GoodsStatus.Saled
                            || goods.Goods.Status == (int)GoodsStatus.Catch)
                        rollBackEnabledGoods.Add(goods.GetSaledGoodsInfo());
                });

                if (rollBackEnabledGoods.Count > 0 &&
                    XtraMessageBox.Show("您确定要退货?", "提示", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    rollBackEnabledGoods.ForEach(goods =>
                    {
                        goods.Goods.Status = (int)GoodsStatus.In;
                        goods.SaledRecord.Remark = string.Format("商品{0}【{1}】,售价{2},预售价{3},折扣价{4},退货时间:{5}",
                                                    goods.Goods.Name,
                                                    goods.Goods.Code,
                                                    goods.Goods.SalePrice,
                                                    goods.Goods.Prepay,
                                                    goods.Goods.Discount,
                                                    DateTime.Now.ToString("yyyy-MM-dd hh:mm"));
                        goods.SaledRecord.UserId = SharedVariables.Instance.LoginUser.User.Id;

                        goods.Goods.SalePrice = 0;
                        goods.Goods.Prepay = 0;
                        goods.Goods.Discount = 0;
                        goods.Goods.SaledDate = null;
                    });
                    Business.SaleGoodsBusiness.Instance.BatchUpdateSaledGoods(rollBackEnabledGoods);
                }
            }
        }


        private void btnJS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var fmGoodsAdd = new FmGoodsAdd(SourceType.JiShou);
            fmGoodsAdd.WindowState = FormWindowState.Maximized;

            fmGoodsAdd.ShowDialog();
        }

        private void btnJH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var fmGoodsAdd = new FmGoodsAdd(SourceType.JinHuo);
            fmGoodsAdd.WindowState = FormWindowState.Maximized;

            fmGoodsAdd.ShowDialog();
        }

        private void btnCK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var chkedMainGoodsInfos = this.GetCheckedGoodsInfos();
                List<SaledGoodsInfo> chkedGoodsInfos = chkedMainGoodsInfos.Select(goods => goods.GetSaledGoodsInfo()).ToList();
                var fmGoodsSaled = new FmGoodsSaled(chkedGoodsInfos);
                fmGoodsSaled.WindowState = FormWindowState.Maximized;

                fmGoodsSaled.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
            }

        }

        private void GridDefaultView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                var gridMainView = mFmGoodsMainView.MainView;
                var mainGoodsInfo = gridMainView.GetRow(e.RowHandle) as ViewMainGoodsInfos;

                if (mainGoodsInfo == null) return;

                if (mainGoodsInfo.GoodsStatus == GoodsStatus.Saled)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                if (mainGoodsInfo.GoodsStatus == GoodsStatus.GetOut)
                {
                    e.Appearance.ForeColor = Color.SeaGreen;
                }
                if (mainGoodsInfo.GoodsStatus == GoodsStatus.Catch)
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
            }
        }

        private void GridDefaultView_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Column.FieldName == "CheckEdit")
            {
                var gridMainView = mFmGoodsMainView.MainView;
                gridMainView.SetRowCellValue(e.RowHandle, e.Column, !(bool)(e.CellValue ?? false));

                var chkedGoodsInfos = this.GetCheckedGoodsInfos();
                // 包含取回和已售出的商品时，商品售出按钮不可用
                this.btnSaleGoods.Enabled = chkedGoodsInfos.Count(goods =>
                {
                    return goods.Goods.Status == (int)GoodsStatus.Saled || goods.Goods.Status == (int)GoodsStatus.GetOut;
                }) == 0;
            }
        }

        private void GridDefaultView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listSelectedGoods = GetSelectedGoodsInfos();
            if (listSelectedGoods == null || listSelectedGoods.Count == 0)
            {
                btnUpdateGoods.Enabled = BtnPrintJH.Enabled = BtnPrintJS.Enabled = BtnPrintXS.Enabled = true;
                return;
            }

            btnUpdateGoods.Enabled = false;
            btnPayment.Enabled = listSelectedGoods.Count(goods => goods.Goods.Code.StartsWith("ZY")) == 0 ||
                    listSelectedGoods.Count(goods => goods.Goods.Paid == (int)GoodsPaid.HasPaid) == listSelectedGoods.Count;

            if (listSelectedGoods.Count == 1)
            {
                var curGoods = listSelectedGoods.FirstOrDefault();
                btnUpdateGoods.Enabled = (curGoods.GoodsStatus == GoodsStatus.In || curGoods.GoodsStatus == GoodsStatus.Catch);
                BtnPrintXS.Enabled = curGoods.GoodsStatus != GoodsStatus.In;
                BtnPrintJH.Enabled = curGoods.Goods.SourceType == (int)SourceType.JinHuo;
                BtnPrintJS.Enabled = curGoods.Goods.SourceType == (int)SourceType.JiShou;
                return;
            }

            BtnPrintXS.Enabled = listSelectedGoods.Count == listSelectedGoods.Count(item => item.GoodsStatus != GoodsStatus.In);
            BtnPrintJH.Enabled = listSelectedGoods.Count == listSelectedGoods.Count(item => item.Goods.SourceType == (int)SourceType.JinHuo);
            BtnPrintJS.Enabled = listSelectedGoods.Count == listSelectedGoods.Count(item => item.Goods.SourceType == (int)SourceType.JiShou);
        }

        private void GridDefaultView_MouseDown(object sender, MouseEventArgs e)
        {
            var gridMainView = (GridView)sender;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hInfo = gridMainView.CalcHitInfo(new Point(e.X, e.Y));
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                //判断光标是否在行范围内  
                if (hInfo.InRow)
                {
                    if (!hInfo.InRowCell) return;
                    //取得选定行信息  
                    var goodsInfo = gridMainView.GetRow(hInfo.RowHandle) as ViewMainGoodsInfos;

                    if (goodsInfo == null) return;

                    if (goodsInfo.GoodsStatus != GoodsStatus.In && SharedVariables.Instance.LoginUser.User.RoleId == (int)SharedVariables.AdminRoleId)
                    {
                        var fmSaledGoods = new FmGoodsSaledMondify(goodsInfo.GetSaledGoodsInfo());
                        if (fmSaledGoods.ShowDialog() == DialogResult.OK)
                        {
                            var result = fmSaledGoods.SaledGoodsInfo;
                            Business.SaleGoodsBusiness.Instance.UpdateSaledGoods(result);
                        }
                        return;
                    }

                    var fmGoodsInfo = new FmGoodsInfo(goodsInfo.GetPurchaseRecordInfo());
                    if (fmGoodsInfo.ShowDialog() == DialogResult.OK)
                    {
                        var result = fmGoodsInfo.PurchaseGoodsInfo;
                        Business.PurchaseGoodsBusiness.Instance.UpdatePurchaseGoods(result);
                    }
                }
            }
        }

        private GridView curGridView = null;
        private void GridDefaultView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = Convert.ToString(e.RowHandle + 1);
            }

            var curGridView = (GridView)sender;
            curGridView.IndicatorWidth = e.Info.DisplayText.Length + 25;
        }

        /// <summary>
        /// 更新查询时间与统计项
        /// </summary>
        private void MainViewSource_Changed()
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                if (mFmGoodsMainView.DataSource != null)
                {
                    decimal sumJHPrimice = 0;
                    decimal sumJHSaled = 0;
                    decimal sumJSPrimice = 0;
                    decimal sumJSSaled = 0;

                    Dictionary<string, decimal> dictJSCharge = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> dictJHCharge = new Dictionary<string, decimal>();

                    mFmGoodsMainView.DataSource.ForEach(source =>
                    {
                        if (source.Goods.SourceType == (int)SourceType.JinHuo)
                        {
                            if (source.GoodsStatus == GoodsStatus.In)
                                sumJHPrimice += source.Goods.PrimePrice.HasValue ? source.Goods.PrimePrice.Value : 0;

                            if (source.GoodsStatus == GoodsStatus.Saled)
                            {
                                sumJHSaled += source.Goods.SalePrice.HasValue ? source.Goods.SalePrice.Value : 0;
                                // 这里有问题，因为商品销售记录可能多次，但source只取出最近一次销售记录情况，所以可能会漏掉一些
                                if (!string.IsNullOrEmpty(source.SaledRecord.BatchId) && !dictJHCharge.ContainsKey(source.SaledRecord.BatchId))
                                {
                                    dictJHCharge.Add(source.SaledRecord.BatchId,
                                                     source.SaledRecord.PayCharge.HasValue ? source.SaledRecord.PayCharge.Value : 0);
                                }
                            }


                        }
                        else
                        {
                            if (source.GoodsStatus == GoodsStatus.In)
                                sumJSPrimice += source.Goods.PrimePrice.HasValue ? source.Goods.PrimePrice.Value : 0;

                            if (source.GoodsStatus == GoodsStatus.Saled)
                            {
                                sumJSSaled += source.Goods.SalePrice.HasValue ? source.Goods.SalePrice.Value : 0;
                                // 这里有问题，因为商品销售记录可能多次，但source只取出最近一次销售记录情况，所以可能会漏掉一些
                                if (!string.IsNullOrEmpty(source.SaledRecord.BatchId) && !dictJSCharge.ContainsKey(source.SaledRecord.BatchId))
                                {
                                    dictJSCharge.Add(source.SaledRecord.BatchId,
                                                     source.SaledRecord.PayCharge.HasValue ? source.SaledRecord.PayCharge.Value : 0);
                                }
                            }
                        }
                    });

                    TxtJHPrimiceSum.EditValue = sumJHPrimice;
                    TxtJHSaledSum.EditValue = sumJHSaled;// string.Format("{0}({1})", sumJHSaled, dictJHCharge.Sum(charge => charge.Value));
                    TxtJSPrimiceSum.EditValue = sumJSPrimice;
                    TxtJSSaledSum.EditValue = sumJSSaled;// string.Format("{0}({1})", sumJSSaled, dictJSCharge.Sum(charge => charge.Value)); ;
                }

                mFmGoodsMainView.Enabled = true;
            }));
        }

        private void btnViewRefrence_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartRefreshView(this, null);
        }

        private void BtnQueryThisWeek_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            mCurrentQueryInfo.DateRange = QueryDateRange.ThisWeek;
            //DateTime.Now.DayOfWeek

            mCurrentQueryInfo = GetViewDateQuery(mCurrentQueryInfo.DateRange);

            StartRefreshView(this, null);
        }

        private void BtnQueryThisMonth_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            mCurrentQueryInfo.DateRange = QueryDateRange.ThisMonth;

            mCurrentQueryInfo = GetViewDateQuery(mCurrentQueryInfo.DateRange);

            StartRefreshView(this, null);
        }

        private void BtnQueryThisQuarter_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            mCurrentQueryInfo.DateRange = QueryDateRange.ThisQuarter;

            mCurrentQueryInfo = GetViewDateQuery(mCurrentQueryInfo.DateRange);
            StartRefreshView(this, null);
        }

        private void BtnQueryThisYear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            mCurrentQueryInfo.DateRange = QueryDateRange.ThisYear;

            mCurrentQueryInfo = GetViewDateQuery(mCurrentQueryInfo.DateRange);
            StartRefreshView(this, null);
        }

        private void BtnQueryAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            mCurrentQueryInfo.DateRange = QueryDateRange.AllYear10;

            mCurrentQueryInfo = GetViewDateQuery(mCurrentQueryInfo.DateRange);
            StartRefreshView(this, null);
        }

        private void BtnQueryCustomer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FmGoodsQueryCustomer fmQueryCustomer = new FmGoodsQueryCustomer();

            //fmQueryCustomer.StartDate = mCurrentQueryInfo.StartPurchaseDate.HasValue ? mCurrentQueryInfo.StartPurchaseDate.Value : DateTime.Now;
            //fmQueryCustomer.EndDate = mCurrentQueryInfo.EndPurchaseDate.HasValue ? mCurrentQueryInfo.EndPurchaseDate.Value : DateTime.Now;

            fmQueryCustomer.StartDate = mCurrentQueryInfo.StartDate.HasValue ? mCurrentQueryInfo.StartDate.Value : DateTime.Now;
            fmQueryCustomer.EndDate = mCurrentQueryInfo.EndDate.HasValue ? mCurrentQueryInfo.EndDate.Value : DateTime.Now;

            if (fmQueryCustomer.ShowDialog() == DialogResult.OK)
            {
                ViewQueryGoodsInfo temp = new ViewQueryGoodsInfo();
                //temp.StartPurchaseDate = temp.StartSaledDate = fmQueryCustomer.StartDate;
                //temp.EndPurchaseDate = temp.EndSaledDate = fmQueryCustomer.EndDate;

                temp.StartDate = fmQueryCustomer.StartDate;
                temp.EndDate = fmQueryCustomer.EndDate;

                mCurrentQueryInfo = temp;
                mCurrentQueryInfo.DateRange = QueryDateRange.Customer;
                StartRefreshView(this, null);
            }
        }

        private void btnCustomViewDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            mFmGoodsMainView.RemoveCustomerView();
        }

        #endregion  View Event

        #region Export

        private string CheckExportToFile(string filter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = filter;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }

            return string.Empty;
        }

        private void BtnExportXls_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var fileName = CheckExportToFile("Excel|*.xls");
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    mFmGoodsMainView.MainView.ExportToXls(fileName, true);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(string.Format("保存失败：{0}", ex.ToString()), "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnExportXlsx_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var fileName = CheckExportToFile("Excel|*.xlsx");
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    mFmGoodsMainView.MainView.ExportToXlsx(fileName,
                        new XlsxExportOptions() { ShowGridLines = true, ExportHyperlinks = true, ExportMode = XlsxExportMode.SingleFile, TextExportMode = TextExportMode.Text });
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(string.Format("保存失败：{0}", ex.ToString()), "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnExportPdf_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var fileName = CheckExportToFile("PDF|*.pdf");
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    mFmGoodsMainView.MainView.ExportToPdf(fileName, new PdfExportOptions() { ConvertImagesToJpeg = true });
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(string.Format("保存失败：{0}", ex.ToString()), "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnExportHtml_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var fileName = CheckExportToFile("HTML|*.html");
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    mFmGoodsMainView.MainView.ExportToHtml(fileName);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(string.Format("保存失败：{0}", ex.ToString()), "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnExportMhtml_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var fileName = CheckExportToFile("HTML|*.mhtml");
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    mFmGoodsMainView.MainView.ExportToMht(fileName);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(string.Format("保存失败：{0}", ex.ToString()), "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        #endregion

        #region Import

        private void btnImport_ItemClick(object sender, ItemClickEventArgs e)
        {
            new FmGoodsImport().ShowDialog();
        }

        #endregion

        #region Print

        private void BtnPrintJH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var listSelectedGoods = GetSelectedGoodsInfos();
            if (listSelectedGoods == null) return;

            if (listSelectedGoods.Count(item => item.Goods.SourceType != (int)SourceType.JinHuo) > 0)
            {
                XtraMessageBox.Show("进货回单里不能包含寄售商品!");
                return;
            }
            List<PurchaseJhGoodsOrderInfo> listJHGoods = new List<PurchaseJhGoodsOrderInfo>();
            var totalPrice = listSelectedGoods.Sum(item => item.Goods.PrimePrice);
            listSelectedGoods.ForEach(item =>
            {
                listJHGoods.Add(new PurchaseJhGoodsOrderInfo(item.GetPurchaseRecordInfo(), 1)
                {
                    TotalPrice = totalPrice.HasValue ? totalPrice.Value : (decimal)0.00
                });
                listJHGoods.Add(new PurchaseJhGoodsOrderInfo(item.GetPurchaseRecordInfo(), 2)
                {
                    TotalPrice = totalPrice.HasValue ? totalPrice.Value : (decimal)0.00
                });
            });
            var receiptView = new FmReceiptView();
            receiptView.InitializeReceiptView<PurchaseJhGoodsOrderInfo>(listJHGoods);
            receiptView.ShowDialog();
        }

        private void BtnPrintJS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var listSelectedGoods = GetSelectedGoodsInfos();
            if (listSelectedGoods == null) return;

            if (listSelectedGoods.Count(item => item.Goods.SourceType != (int)SourceType.JiShou) > 0)
            {
                XtraMessageBox.Show("寄售回单里不能包含自有商品!");
                return;
            }
            List<PurchaseJsGoodsOrderInfo> listJsGoods = new List<PurchaseJsGoodsOrderInfo>();
            var totalPrice = listSelectedGoods.Sum(item => item.Goods.PrimePrice);
            listSelectedGoods.ForEach(item =>
            {
                listJsGoods.Add(new PurchaseJsGoodsOrderInfo(item.GetPurchaseRecordInfo(), 1)
                {
                    TotalPrice = totalPrice.HasValue ? totalPrice.Value : (decimal)0.00
                });
                listJsGoods.Add(new PurchaseJsGoodsOrderInfo(item.GetPurchaseRecordInfo(), 2)
                {
                    TotalPrice = totalPrice.HasValue ? totalPrice.Value : (decimal)0.00
                });
            });
            var receiptView = new FmReceiptView();
            receiptView.InitializeReceiptView<PurchaseJsGoodsOrderInfo>(listJsGoods);
            receiptView.ShowDialog();
        }

        private void BtnPrintXS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var listSelectedGoods = GetSelectedGoodsInfos();
            if (listSelectedGoods == null) return;

            if (listSelectedGoods.Count(item => item.GoodsStatus == GoodsStatus.In) > 0)
            {
                XtraMessageBox.Show("未售出商品不能在此处打印销售回单!");
                return;
            }

            var listSaledOrderInfo = new List<SaledGoodsOrderInfo>();
            var totalMarkPrice = listSelectedGoods.Sum(record => record.Goods.MarkPrice);
            var totalPrice = listSelectedGoods.Sum(record => record.Goods.SalePrice);
            var totalDiscount = listSelectedGoods.Sum(record => record.Goods.Discount);
            listSelectedGoods.ForEach(goods =>
            {
                listSaledOrderInfo.Add(new SaledGoodsOrderInfo(goods.GetSaledGoodsInfo(), 1)
                {
                    TotalMarkPrice = totalMarkPrice.HasValue ? totalMarkPrice.Value : (decimal)0,
                    TotalPrice = totalPrice.HasValue ? totalPrice.Value : (decimal)0,
                    TotalDiscount = totalDiscount.HasValue ? totalDiscount.Value : (decimal)0
                });
                listSaledOrderInfo.Add(new SaledGoodsOrderInfo(goods.GetSaledGoodsInfo(), 2)
                {
                    TotalMarkPrice = totalMarkPrice.HasValue ? totalMarkPrice.Value : (decimal)0,
                    TotalPrice = totalPrice.HasValue ? totalPrice.Value : (decimal)0,
                    TotalDiscount = totalDiscount.HasValue ? totalDiscount.Value : (decimal)0
                });
            });

            var receiptView = new FmReceiptView();
            receiptView.InitializeReceiptView<SaledGoodsOrderInfo>(listSaledOrderInfo);
            receiptView.ShowDialog();
        }

        private void BtnPrintView_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            mFmGoodsMainView.MainView.ShowPrintPreview();
        }

        #endregion


        #endregion
    }
}
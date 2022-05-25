namespace Aisger.Reports
{
    partial class ReportReestr
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportReestr));
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.txbxH2 = new Telerik.Reporting.HtmlTextBox();
            this.txbxH1 = new Telerik.Reporting.HtmlTextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.txbxH3 = new Telerik.Reporting.HtmlTextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(3.5D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txbxH2,
            this.txbxH1,
            this.txbxH3});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.pageHeaderSection1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Cm(0D);
            // 
            // txbxH2
            // 
            this.txbxH2.Anchoring = ((Telerik.Reporting.AnchoringStyles)((((Telerik.Reporting.AnchoringStyles.Top | Telerik.Reporting.AnchoringStyles.Bottom) 
            | Telerik.Reporting.AnchoringStyles.Left) 
            | Telerik.Reporting.AnchoringStyles.Right)));
            this.txbxH2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(1.9000002145767212D), Telerik.Reporting.Drawing.Unit.Cm(1.2999999523162842D));
            this.txbxH2.Name = "txbxH2";
            this.txbxH2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(11.699999809265137D), Telerik.Reporting.Drawing.Unit.Cm(0.62999999523162842D));
            this.txbxH2.Style.Font.Name = "Times New Roman";
            this.txbxH2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txbxH2.Value = resources.GetString("txbxH2.Value");
            // 
            // txbxH1
            // 
            this.txbxH1.Anchoring = ((Telerik.Reporting.AnchoringStyles)((((Telerik.Reporting.AnchoringStyles.Top | Telerik.Reporting.AnchoringStyles.Bottom) 
            | Telerik.Reporting.AnchoringStyles.Left) 
            | Telerik.Reporting.AnchoringStyles.Right)));
            this.txbxH1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.89999997615814209D), Telerik.Reporting.Drawing.Unit.Cm(0.30000004172325134D));
            this.txbxH1.Name = "txbxH1";
            this.txbxH1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(13.899999618530273D), Telerik.Reporting.Drawing.Unit.Cm(1.1000000238418579D));
            this.txbxH1.Style.Font.Name = "Times New Roman";
            this.txbxH1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txbxH1.Value = resources.GetString("txbxH1.Value");
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Cm(10.200000762939453D);
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Times New Roman";
            this.detail.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.detail.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.detail.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.detail.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            // 
            // txbxH3
            // 
            this.txbxH3.Anchoring = ((Telerik.Reporting.AnchoringStyles)((((Telerik.Reporting.AnchoringStyles.Top | Telerik.Reporting.AnchoringStyles.Bottom) 
            | Telerik.Reporting.AnchoringStyles.Left) 
            | Telerik.Reporting.AnchoringStyles.Right)));
            this.txbxH3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(9.9961594969499856E-05D), Telerik.Reporting.Drawing.Unit.Cm(9.9961594969499856E-05D));
            this.txbxH3.Name = "txbxH3";
            this.txbxH3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(0.59980040788650513D), Telerik.Reporting.Drawing.Unit.Cm(0.62999999523162842D));
            this.txbxH3.Style.Font.Name = "Times New Roman";
            this.txbxH3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txbxH3.Value = resources.GetString("txbxH3.Value");
            // 
            // ReportReestr
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail});
            this.Name = "ReportReestr";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(25.399999618530273D), Telerik.Reporting.Drawing.Unit.Mm(25.399999618530273D), Telerik.Reporting.Drawing.Unit.Mm(25.399999618530273D), Telerik.Reporting.Drawing.Unit.Mm(25.399999618530273D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.Name = "json";
            this.ReportParameters.Add(reportParameter1);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Cm(17.920000076293945D);
            this.ItemDataBinding += new System.EventHandler(this.ReportReestr_ItemDataBinding);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        

        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.HtmlTextBox txbxH2;
        private Telerik.Reporting.HtmlTextBox txbxH1;
        private Telerik.Reporting.HtmlTextBox txbxH3;
    }
}
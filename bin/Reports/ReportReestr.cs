namespace Aisger.Reports
{
    using Aisger.Models.Repository.Reestr;
    using Aisger.Models.Repository.Report;
    using Helpers;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;


    /// <summary>
    /// Summary description for ReportReestr.
    /// </summary>
    public partial class ReportReestr : Telerik.Reporting.Report
    {
        FormsReestrRepository formsReestr = new FormsReestrRepository();
        FormsGerRepository formsGer = new FormsGerRepository();
        ReportEE2Repository formsEE2 = new ReportEE2Repository();

        double rowHeight = 0.5;

        public ReportReestr()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }


       
        private void ReportReestr_ItemDataBinding(object sender, EventArgs e)
        {

            var curReport = (Telerik.Reporting.Processing.Report)sender;
         
            
            var jsonPar = curReport.Parameters["json"].Value;
            if (jsonPar == null) return;
            var json = jsonPar.ToString();
            if (string.IsNullOrEmpty(json)) return;
            var model = JsonConvert.DeserializeObject<ReportModel>(json);
            int tableGroupIndex = 1;
            int textBoxIndex = 1;
            string lang = CultureHelper.GetCurrentCulture();
            Dictionary<string, object> item = new Dictionary<string, object>();

            if (model.reportId == 1)
                item = formsReestr.getFormsReestr(model.year, lang);
            else if (model.reportId == 2)
                item = formsReestr.getGerSubjectsReestrByObl(model.year, model.oblastId, lang);
            else if (model.reportId == 3)
                item = formsReestr.getSubjectsReestrOffReasonByRepublic(model.year, lang);
            else if (model.reportId == 4)
                item = formsReestr.getGerSubjectsReestrOffReasonByObl(model.year, model.oblastId, lang);
            else if (model.reportId == 5)
                item = formsReestr.getEnergyAudit(model.beginDate, model.endDate);
            else if (model.reportId == 6)
                item = formsReestr.getEnergyService(model.beginDate, model.endDate);
            else if (model.reportId == 7)
                item = formsReestr.getGerEsko(model.beginDate, model.endDate);
            else if (model.reportId == 8)
                item = formsGer.gerPropertyConsumption(model.year, lang);
            else if (model.reportId == 9)
                item = formsGer.gerOblastConsumption(model.year, lang);
            else if (model.reportId == 10)
                item = formsGer.gerEconActivityConsumption(model.year, lang);
            else if (model.reportId == 11)
                item = formsGer.gerConsumptionByConsumptionGroup(model.year, lang);
            else if (model.reportId == 12)
                item = formsGer.ger_econ_activity_consumption_top100(model.year, lang);
            else if (model.reportId == 13)
                item = formsGer.ger_subject_name_consumption(model.year, lang);
            else if (model.reportId == 14)
                item = formsGer.ger_consumption_by_energy_resource(model.year, lang);
            else if (model.reportId == 15)
                item = formsGer.ger_check_uncomplete_data(model.year, lang);
            else if (model.reportId == 16)
                item = formsGer.ger_check_uncomplete_data_by_oblast(model.year, lang);
            else if (model.reportId == 17)
                item = formsGer.ger_uncorrect_or_evaded_data_by_republic(model.year, lang);
            else if (model.reportId == 25)
                item = formsGer.ger_energy_consumption_compare_previous(model.year, lang);
            else if (model.reportId == 26)
                item = formsGer.ger_energy_consumption_compare_previous_by_oblast(model.year, lang);
            else if (model.reportId == 27)
                item = formsGer.ger_energy_consumption_share_by_oblast(model.year, lang);
            else if (model.reportId == 28)
                item = formsGer.ger_electricity_consumption_by_subject_top100(model.year, lang);
            else if (model.reportId == 29)
                item = formsGer.ger_heat_consumption_by_subject_top100(model.year, lang);
            else if (model.reportId == 30)
                item = formsGer.ger_gas_consumption_by_subject_top100(model.year, lang);
            else if (model.reportId == 31)
                item = formsGer.ger_coal_consumption_by_subject_top100(model.year, lang);
            else if (model.reportId == 33)
                item = formsGer.ger_average_consumption_by_oblast(model.year, lang);
            else if (model.reportId == 34)
                item = formsGer.ger_presence_of_measure_equipment_by_oblast(model.year, lang);
            else if (model.reportId == 37)
                item = formsGer.ger_water_consumption_by_oblast(model.year, lang);
            else if (model.reportId == 38)
                item = formsReestr.getAuditCount(model.year);
            else if (model.reportId == 39)
                item = formsReestr.getEnergyMapRequests(model.beginDate, model.endDate, lang);
            else if (model.reportId == 40)
                item = formsReestr.getEnergyMapProjects(model.beginDate, model.endDate, lang);
            else if (model.reportId == 41)
                item = formsEE2.getReportEE2(model.oblastId, lang);
            else if (model.reportId == 42)
                item = formsGer.report_view_by_oblast(model.year, lang);
            else if (model.reportId == 43)
                item = formsGer.EquipmentMeteringDevices(model.year, lang);
            else if (model.reportId == 44)
                item = formsGer.ReportConsumptionExpendituresByRegion(model.year, lang);
            else if (model.reportId == 45)
                item = formsGer.ReportCountOfGuIndicators(model.year, model.oblastId, lang);
            else if (model.reportId == 46)
                item = formsGer.ReportTutByRegion(model.year, lang);
            else if (model.reportId == 47)
                item = formsGer.ReportTopTutByRegion(model.year, model.oblastId, model.limit, lang);
            else if (model.reportId == 48)
                item = formsGer.ReportTop100TutByRegion(model.year, model.oblastId, lang);
            else if (model.reportId == 49)
                item = formsGer.ReportSocialObjectsByRegion(model.year, model.oblastId, lang);
            else if (model.reportId == 50)
                item = formsGer.ReportLowCostEventsPerformedSubjectsByRegion(model.year, model.oblastId, lang);
            else if (model.reportId == 51)
                item = formsGer.ReportEffectiveEventsPerformedBySubjectsGer(model.year, model.oblastId, lang);
            else if (model.reportId == 52)
                item = formsGer.RegionEnergyConsumption(model.year, model.oblastId, lang);
            else if (model.reportId == 53)
                item = formsGer.ReportCompareRegionWithOtherRegion(model.year, lang);
            else if (model.reportId == 54)
                item = formsGer.ReportConsumptionSubjectsByGer(model.year, model.oblastId, model.fscode, lang);
            else if (model.reportId == 55)
                item = formsGer.ReportSetOfActivitiesPerformedSubjectsByGER(model.year,model.oblastId.Value,lang,model.fscode.Value,model.okedId.Value);

            List<Dictionary<string, object>> lists = (List<Dictionary<string, object>>)item["ListItems"];

            var columns = ReportModel.GetColumns(model.reportId,lang);

            string[] beginDateArr = new string[2];
            string[] endDateArr = new string[2];

            if (!string.IsNullOrEmpty(model.beginDate) && !string.IsNullOrEmpty(model.endDate))
            {
                beginDateArr = model.beginDate.Split(' ');
                endDateArr = model.endDate.Split(' ');
            }

            txbxH1.Style.TextAlign = HorizontalAlign.Center;
           // txbxH1.Multiline = true;

            txbxH1.Value =ReportModel.GetH1(model.reportId,lang);
        
            if (model.reportId == 41)
            {
                string dateStr = DateTime.Now.ToString("dd.MM.yyyy");
             //   txbxH1.Multiline = true;
                txbxH1.Value += " \r\n " + dateStr;
                txbxH1.Style.TextAlign = HorizontalAlign.Center;
            }

            txbxH1.Style.TextAlign = HorizontalAlign.Center;
           
			//txbxH1.Style.Font.Name = "SansSerif";
			//txbxH1.Style.Font.Size = Unit.Cm(0.47625);

			txbxH2.Style.TextAlign = HorizontalAlign.Center;
			//txbxH2.Style.Font.Name = "SansSerif";
			//txbxH2.Style.Font.Size = Unit.Cm(0.47625);

            txbxH2.Value = ReportModel.GetH2(model.reportId, model.year, beginDateArr[0], endDateArr[0], model.oblastName,lang);

            txbxH3.Value = "R" + model.reportId.ToString();
            ((ISupportInitialize)(this)).BeginInit();

            Table tableMain2 = new Table();

            //tableMain2.Style.TextAlign = HorizontalAlign.Center;

            var columnCount = columns.Length;
            var rowCount = 0;
            for (int i = 0; i < columnCount; i++)
            {
                var column = columns[i];
                //tableMain2.Body.Columns.Add(new TableBodyColumn(new Unit(column.Width)));//Unit.Cm(column.Width / 3)
                tableMain2.Body.Columns.Add(new TableBodyColumn());//Unit.Cm(column.Width / 3)
            }

            if ((new List<int> { 1, 3, 8, 9, 10, 11, 12, 27, 33, 42, 44, 46, 47, 48, 49, 50, 53 }).Contains(model.reportId))
                rowCount = lists.Count + 2;
            else if (model.reportId == 2)
            {
                if (model.oblastId != 0)
                    rowCount = lists.Count + 3;
                else
                    rowCount = lists.Count * 2 + 1;
            }
            else if (model.reportId == 4)
            {
                if (model.oblastId != 0)
                    rowCount = lists.Count + 3;
                else
                    rowCount = 113;
            }
            else if (model.reportId == 42) //model.reportId == 25 ||
                rowCount = lists.Count + 3;
            else if (model.reportId == 25)
            {
                rowCount = lists.Count + 2;
            }
            else if (model.reportId == 26)
                rowCount = lists.Count + 34;
            else if (model.reportId == 37)
                rowCount = lists.Count + 33;
            else if ((new List<int> { 5, 6, 7, 13, 14, 15, 16, 17, 28, 29, 30, 31, 34, 38, 39, 40, 41, 43, 45, 51, 52, 54, 55 }).Contains(model.reportId))
                rowCount = lists.Count + 1;


            string[,] values = new string[rowCount, columnCount];


            /*values*/
            if (model.reportId == 25 || model.reportId == 26 || model.reportId==42)
            {

            }
            else
            {
                for (int i = 0; i < columnCount; i++)
                {
                    values[0, i] = columns[i].Name;
                }
            }

            if ((new List<int> { 1, 3, 5, 6, 7, 8, 9, 10, 
                11, 12, 13, 14, 15, 16, 17, 28, 29, 30, 
                31, 33, 34, 38, 39, 40 ,41}).Contains(model.reportId))
                for (int i = 1; i < lists.Count + 1; i++)
                {
                    values[i, 0] = i.ToString();
                }

            var rowInd = 1;
            var rowInd26 = 2;
            var oblastName = "";
            bool flag = true;

            //for report 53
            double _tut = 0;
            double _subject_count = 0;

            for (int rowIndex = 0; rowIndex < lists.Count; rowIndex++)
            {
                var listItem = lists[rowIndex];

                int colIndex = 0;
                double sum = 0;
                foreach (var dictItem in listItem)
                {
                    if (dictItem.Value != null)
                    {
                        if ((new List<int> { 1, 3, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                            16, 17, 28, 29, 30, 31, 38, 39, 40 }).Contains(model.reportId))
                        {
                            double number;
                            if (double.TryParse(dictItem.Value.ToString(), out number) && (new List<int> { 8, 9, 10, 13, 14, 28, 29, 30, 31, 33 }).Contains(model.reportId))
                            {
                                double d = Math.Round(Convert.ToDouble(dictItem.Value.ToString()));
                                values[rowIndex + 1, colIndex + 1] = d.ToString();
                            }
                            else if ((new List<int> { 12 }).Contains(model.reportId))
                            {
                                double procent = 0;
                                values[rowIndex + 1, colIndex + 1] = dictItem.Value.ToString();
                                procent = Math.Round(Convert.ToDouble(listItem["qty_subject"]) / Convert.ToDouble(listItem["all_qty"]) * 100);
                                values[rowIndex + 1, 3] = procent.ToString();
                                values[rowIndex + 1, 4] = Math.Round(Convert.ToDouble(listItem["consumption"])).ToString();
                                procent = Math.Round(Convert.ToDouble(listItem["consumption"]) / Convert.ToDouble(listItem["all_consumption"]) * 100);
                                values[rowIndex + 1, 5] = procent.ToString();

                            }
                            else
                                values[rowIndex + 1, colIndex + 1] = dictItem.Value.ToString();

                            colIndex++;
                        }
                        else if (model.reportId == 41)
                        {
                            values[rowIndex + 1, colIndex] = dictItem.Value.ToString();
                            colIndex++;
                        }
                        else if (model.reportId == 25)
                        {
                            try
                            {
                                values[rowIndex + 2, 0] = listItem["resource_name"].ToString();
                                values[rowIndex + 2, 1] = listItem["qty_state_inst"].ToString();
                                values[rowIndex + 2, 2] = listItem["qty_state_prev_prc"].ToString();
                                values[rowIndex + 2, 3] = listItem["qty_quasi_state_inst"].ToString();
                                values[rowIndex + 2, 4] = listItem["qty_quasi_prev_prc"].ToString();
                                values[rowIndex + 2, 5] = listItem["qty_jur"].ToString();
                                values[rowIndex + 2, 6] = listItem["qty_jur_prev_prc"].ToString();
                                values[rowIndex + 2, 7] = listItem["qty_ip"].ToString();
                                values[rowIndex + 2, 8] = listItem["qty_ip_prev_prc"].ToString();
                                values[rowIndex + 2, 9] = listItem["qty_sum"].ToString();
                                values[rowIndex + 2, 10] = listItem["qty_sum_prev_prc"].ToString();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else if ((new List<int> { 26 }).Contains(model.reportId))
                        {
                            #region 25,26
                            double procent = 0;
                            if (model.reportId == 25)
                            {
                                values[rowIndex + 2, colIndex] = dictItem.Value.ToString();
                                values[rowIndex + 2, 1] = Math.Round(Convert.ToDouble(listItem["qty_state_inst"])).ToString();
                                values[rowIndex + 2, 3] = Math.Round(Convert.ToDouble(listItem["qty_quasi_state_inst"])).ToString();
                                values[rowIndex + 2, 5] = Math.Round(Convert.ToDouble(listItem["qty_jur"])).ToString();
                            }
                            if (model.reportId == 26)
                            {

                                if (oblastName != listItem["oblast_name"].ToString())
                                {
                                    if (rowIndex != 0)
                                    {
                                        int index = rowIndex - 28;
                                        double count = 0;
                                        double t1count = 0;
                                        double t3count = 0;
                                        double t5count = 0;
                                        double t2count = 0;
                                        double t4count = 0;
                                        double t6count = 0;
                                        for (int m = index; m < rowIndex; m++)
                                        {
                                            var listItem26 = lists[m];
                                            t1count += Convert.ToDouble(listItem26["qty_state_inst"]);
                                            t2count += Convert.ToDouble(listItem26["qty_state_inst_prev"]);
                                            t3count += Convert.ToDouble(listItem26["qty_quasi_state_inst"]);
                                            t4count += Convert.ToDouble(listItem26["qty_quasi_state_inst_prev"]);
                                            t5count += Convert.ToDouble(listItem26["qty_jur"]);
                                            t6count += Convert.ToDouble(listItem26["qty_jur_prev"]);

                                        }
                                        values[rowInd26, 0] = ReportModel.GetSumName(model.reportId, lang);
                                        values[rowInd26, 1] = Math.Round(t1count).ToString();
                                        values[rowInd26, 3] = Math.Round(t3count).ToString();
                                        values[rowInd26, 5] = Math.Round(t5count).ToString();
                                        values[rowInd26, 7] = Math.Round(t1count + t3count + t5count).ToString();


                                        count = t1count * 100 / t2count;
                                        if (HasValue(count))
                                            values[rowInd26, 2] = Math.Round(count).ToString();
                                        else
                                            values[rowInd26, 2] = " ";

                                        count = t3count * 100 / t4count;
                                        if (HasValue(count))
                                            values[rowInd26, 4] = Math.Round(count).ToString();
                                        else
                                            values[rowInd26, 4] = " ";

                                        count = t5count * 100 / t6count;
                                        if (HasValue(count))
                                            values[rowInd26, 6] = Math.Round(count).ToString();
                                        else
                                            values[rowInd26, 6] = " ";

                                        count = (t1count + t3count + t5count) * 100 /
                                            (t2count + t4count + t6count);
                                        if (HasValue(count))
                                            values[rowInd26, 8] = Math.Round(count).ToString();
                                        else
                                            values[rowInd26, 8] = " ";
                                        rowInd26++;

                                    }
                                    values[rowInd26, 0] = "colspan=9|" + listItem["oblast_name"];
                                    oblastName = listItem["oblast_name"].ToString();
                                    rowInd26++;
                                }
                            }

                            if (model.reportId == 26)
                            {
                                values[rowInd26, 0] = listItem["resource_name"].ToString();
                                values[rowInd26, 1] = Math.Round(Convert.ToDouble(listItem["qty_state_inst"])).ToString();
                                values[rowInd26, 3] = Math.Round(Convert.ToDouble(listItem["qty_quasi_state_inst"])).ToString();
                                values[rowInd26, 5] = Math.Round(Convert.ToDouble(listItem["qty_jur"])).ToString();
                            }


                            procent = Math.Round(Convert.ToDouble(listItem["qty_state_inst"]) * 100 / Convert.ToDouble(listItem["qty_state_inst_prev"]));
                            if (HasValue(procent))
                            {
                                if (model.reportId == 25)
                                    values[rowIndex + 2, 2] = procent.ToString();
                                else
                                    values[rowInd26, 2] = procent.ToString();

                            }
                            else
                            {
                                if (model.reportId == 25)
                                    values[rowIndex + 2, 2] = " ";
                                else
                                    values[rowInd26, 2] = " ";
                            }

                            procent = Math.Round(Convert.ToDouble(listItem["qty_quasi_state_inst"]) * 100 / Convert.ToDouble(listItem["qty_quasi_state_inst_prev"]));
                            if (HasValue(procent))
                            {
                                if (model.reportId == 25)
                                    values[rowIndex + 2, 4] = procent.ToString();
                                else
                                    values[rowInd26, 4] = procent.ToString();
                            }
                            else
                            {
                                if (model.reportId == 25)
                                    values[rowIndex + 2, 4] = " ";
                                else
                                    values[rowInd26, 4] = " ";
                            }

                            procent = Math.Round(Convert.ToDouble(listItem["qty_jur"]) * 100 / Convert.ToDouble(listItem["qty_jur_prev"]));
                            if (HasValue(procent))
                            {
                                if (model.reportId == 25)
                                    values[rowIndex + 2, 6] = procent.ToString();
                                else
                                    values[rowInd26, 6] = procent.ToString();
                            }
                            else
                            {
                                if (model.reportId == 25)
                                    values[rowIndex + 2, 6] = " ";
                                else
                                    values[rowInd26, 6] = " ";
                            }

                            if (model.reportId == 25)
                                values[rowIndex + 2, 7] = Math.Round((Convert.ToDouble(listItem["qty_state_inst"]) + Convert.ToDouble(listItem["qty_quasi_state_inst"]) + Convert.ToDouble(listItem["qty_jur"]))).ToString();
                            else
                                values[rowInd26, 7] = Math.Round((Convert.ToDouble(listItem["qty_state_inst"]) + Convert.ToDouble(listItem["qty_quasi_state_inst"]) + Convert.ToDouble(listItem["qty_jur"]))).ToString();

                            procent = Math.Round((Convert.ToDouble(listItem["qty_state_inst"]) + Convert.ToDouble(listItem["qty_quasi_state_inst"]) + Convert.ToDouble(listItem["qty_jur"])) * 100 /
                                (Convert.ToDouble(listItem["qty_state_inst_prev"]) + Convert.ToDouble(listItem["qty_quasi_state_inst_prev"]) + Convert.ToDouble(listItem["qty_jur_prev"])));
                            if (HasValue(procent))
                            {
                                if (model.reportId == 25)
                                    values[rowIndex + 2, 8] = procent.ToString();
                                else
                                    values[rowInd26, 8] = procent.ToString();
                            }
                            else
                            {
                                if (model.reportId == 25)
                                    values[rowIndex + 2, 8] = " ";
                                else
                                    values[rowInd26, 8] = " ";
                            }

                            colIndex++;

                            #endregion
                        }
                        else if (model.reportId == 33)
                        {
                            #region 33

                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = (listItem["consumption_healthcare"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["consumption_healthcare"]), 3).ToString() : "0";
                            values[rowIndex + 1, 3] = (listItem["consumption_education"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["consumption_education"]), 3).ToString() : "0";
                            values[rowIndex + 1, 4] = (listItem["consumption_culture"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["consumption_culture"]), 3).ToString() : "0";

                            #endregion
                        }
                        else if (model.reportId == 34)
                        {
                            #region 34

                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = (listItem["state_inst_equipment_avg"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["state_inst_equipment_avg"]), 2).ToString() : "0";
                            values[rowIndex + 1, 3] = (listItem["quasi_state_inst_equipment_avg"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["quasi_state_inst_equipment_avg"]), 2).ToString() : "0";
                            values[rowIndex + 1, 4] = (listItem["jur_equipment_avg"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["jur_equipment_avg"]), 2).ToString() : "0";
                            values[rowIndex + 1, 5] = (listItem["ip_equipment_avg"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["ip_equipment_avg"]), 2).ToString() : "0";
                            #endregion
                        }
                        else if ((new List<int> { 42 }).Contains(model.reportId))
                        {
                            #region 42 
                            string namerus = listItem["NameRu"].ToString();
                            values[rowIndex + 2, 0] = namerus;

                            values[rowIndex + 2, 1] = Math.Round(Convert.ToDouble(listItem["gus16"])).ToString();
                            values[rowIndex + 2, 2] = Math.Round(Convert.ToDouble(listItem["kvs16"])).ToString();
                            values[rowIndex + 2, 3] = Math.Round(Convert.ToDouble(listItem["urs16"])).ToString();
                            values[rowIndex + 2, 4] = Math.Round(Convert.ToDouble(listItem["ips16"])).ToString();
                            values[rowIndex + 2, 5] = Math.Round(Convert.ToDouble(listItem["alls16"])).ToString();

                            values[rowIndex + 2, 6] = Math.Round(Convert.ToDouble(listItem["guse16"])).ToString();
                            values[rowIndex + 2, 7] = Math.Round(Convert.ToDouble(listItem["kvse16"])).ToString();
                            values[rowIndex + 2, 8] = Math.Round(Convert.ToDouble(listItem["urse16"])).ToString();
                            values[rowIndex + 2, 9] = Math.Round(Convert.ToDouble(listItem["ipse16"])).ToString();
                            values[rowIndex + 2, 10] = Math.Round(Convert.ToDouble(listItem["allse16"])).ToString();

                            values[rowIndex + 2, 11] = Math.Round(Convert.ToDouble(listItem["gusp16"])).ToString();
                            values[rowIndex + 2, 12] = Math.Round(Convert.ToDouble(listItem["kvsp16"])).ToString();
                            values[rowIndex + 2, 13] = Math.Round(Convert.ToDouble(listItem["ursp16"])).ToString();
                            values[rowIndex + 2, 14] = Math.Round(Convert.ToDouble(listItem["ipsp16"])).ToString();
                            values[rowIndex + 2, 15] = Math.Round(Convert.ToDouble(listItem["allsp16"])).ToString();

                            values[rowIndex + 2, 16] = Math.Round(Convert.ToDouble(listItem["gusu16"])).ToString();
                            values[rowIndex + 2, 17] = Math.Round(Convert.ToDouble(listItem["kvsu16"])).ToString();
                            values[rowIndex + 2, 18] = Math.Round(Convert.ToDouble(listItem["ursu16"])).ToString();
                            values[rowIndex + 2, 19] = Math.Round(Convert.ToDouble(listItem["ipsu16"])).ToString();
                            values[rowIndex + 2, 20] = Math.Round(Convert.ToDouble(listItem["allsu16"])).ToString();

                            #endregion
                        }
                        else if ((new List<int> { 43 }).Contains(model.reportId))
                        {

                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            if (Convert.ToDouble(listItem["count_subjects"]) > 0)
                            {
                                values[rowIndex + 1, 2] = Math.Round(Convert.ToDouble(listItem["electro"]) / Convert.ToDouble(listItem["count_subjects"]) * 100, 2).ToString();
                                values[rowIndex + 1, 3] = Math.Round(Convert.ToDouble(listItem["teplo"]) / Convert.ToDouble(listItem["count_subjects"]) * 100, 2).ToString();
                                values[rowIndex + 1, 4] = Math.Round(Convert.ToDouble(listItem["voda"]) / Convert.ToDouble(listItem["count_subjects"]) * 100, 2).ToString();
                                values[rowIndex + 1, 5] = Math.Round(Convert.ToDouble(listItem["gaz"]) / Convert.ToDouble(listItem["count_subjects"]) * 100, 2).ToString();
                            }
                            else
                            {
                                values[rowIndex + 1, 2] = "0";
                                values[rowIndex + 1, 3] = "0";
                                values[rowIndex + 1, 4] = "0";
                                values[rowIndex + 1, 5] = "0";
                            }

                        }
                        else if ((new List<int> { 44 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["count_subjects"].ToString();
                            values[rowIndex + 1, 3] = (listItem["notownsource"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["notownsource"]), 2).ToString() : "0";
                            values[rowIndex + 1, 4] = (listItem["expenceenergy"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["expenceenergy"]), 2).ToString() : "0";

                        }
                        else if ((new List<int> { 45 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["idk"].ToString();
                            values[rowIndex + 1, 3] = listItem["juridical_name"].ToString();
                            values[rowIndex + 1, 4] = (listItem["countofemployees"] != DBNull.Value) ? listItem["countofemployees"].ToString() : "0";
                            values[rowIndex + 1, 5] = (listItem["countofstudents"] != DBNull.Value) ? listItem["countofstudents"].ToString() : "0";
                            values[rowIndex + 1, 6] = (listItem["countofbeds"] != DBNull.Value) ? listItem["countofbeds"].ToString() : "0";
                            values[rowIndex + 1, 7] = listItem["isplan"].ToString();
                            values[rowIndex + 1, 8] = listItem["isenergymanagement_system"].ToString();

                            values[rowIndex + 1, 9] = listItem["yearofconstruction"].ToString();
                            values[rowIndex + 1, 10] = listItem["automateitem"].ToString();

                            values[rowIndex + 1, 11] = (listItem["totalareaofbuilding"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["totalareaofbuilding"]), 2).ToString() : "0";
                            values[rowIndex + 1, 12] = (listItem["heatedareaofbuilding"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["heatedareaofbuilding"]), 2).ToString() : "0";

                            values[rowIndex + 1, 13] = listItem["centralheating"].ToString();
                            values[rowIndex + 1, 14] = listItem["independentheating"].ToString();
                        }
                        else if ((new List<int> { 46 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["count_subjects"].ToString();
                            values[rowIndex + 1, 3] = (listItem["tut"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["tut"]), 2).ToString() : "0";
                            values[rowIndex + 1, 4] = (listItem["count_subjects_woexcl"] != DBNull.Value) ? listItem["count_subjects_woexcl"].ToString() : "0";
                            values[rowIndex + 1, 5] = (listItem["tut_woexcl"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["tut_woexcl"]), 2).ToString() : "0";

                        }
                        else if ((new List<int> { 47 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["usrjuridicalname"].ToString();
                            values[rowIndex + 1, 3] = (listItem["tut"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["tut"]), 2).ToString() : "0";

                        }
                        else if ((new List<int> { 48 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["usrjuridicalname"].ToString();
                            values[rowIndex + 1, 3] = (listItem["tut"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["tut"]), 2).ToString() : "0";

                        }
                        else if ((new List<int> { 49 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["usrjuridicalname"].ToString();
                            values[rowIndex + 1, 3] = (listItem["tut"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["tut"]), 2).ToString() : "0";
                        }
                        else if ((new List<int> { 50 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["usrjuridicalname"].ToString();
                            values[rowIndex + 1, 3] = listItem["EventName"].ToString();
                            values[rowIndex + 1, 4] = (listItem["factinvest"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["factinvest"]), 2).ToString() : "0";
                        }
                        else if ((new List<int> { 51 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["usrjuridicalname"].ToString();
                            values[rowIndex + 1, 3] = listItem["EventName"].ToString();
                            values[rowIndex + 1, 4] = listItem["tr_nameru"].ToString();
                            values[rowIndex + 1, 5] = (listItem["inkind"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["inkind"]), 2).ToString() : "0";
                            values[rowIndex + 1, 6] = (listItem["f2source"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["f2source"]), 2).ToString() : "0";
                            values[rowIndex + 1, 7] = (listItem["crat0"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["crat0"]), 2).ToString() : "0";
                        }
                        else if ((new List<int> { 52 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = listItem["usrjuridicalname"].ToString();
                            values[rowIndex + 1, 3] = listItem["ei_nameru"].ToString();
                            values[rowIndex + 1, 4] = (listItem["EnergyValue"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["EnergyValue"]), 2).ToString() : "0";
                            values[rowIndex + 1, 5] = (listItem["prevenergyvalue"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["prevenergyvalue"]), 2).ToString() : "0";
                            double _energyValue = (listItem["EnergyValue"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["EnergyValue"]), 2) : 0;
                            double _prevenergyValue = (listItem["prevenergyvalue"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["prevenergyvalue"]), 2) : 0;
                            double val = _energyValue - _prevenergyValue;

                            values[rowIndex + 1, 6] = Convert.ToString(val);

                            if (val < 0)
                            {
                                values[rowIndex + 1, 7] = "снижение";
                            }
                            else if (val > 0)
                            {
                                values[rowIndex + 1, 7] = "увеличение";
                            }
                            else
                            {
                                values[rowIndex + 1, 7] = "";
                            }
                        }
                        else if ((new List<int> { 53 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();

                            //var subject_count = (listItem["count_subjects"] != DBNull.Value) ? Convert.ToInt32(listItem["count_subjects"]) : 0;                         
                            //var tut = (listItem["tut"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["tut"]), 2) : 0;

                            //double compareSCount = 0;
                            //double compareTut = 0;

                            //if (rowIndex == 0)
                            //{
                            //    _tut = tut;
                            //    _subject_count = subject_count;
                            //}else
                            //{
                            //    compareSCount = Math.Round((subject_count - _subject_count) * 100 / _subject_count, 2);
                            //    compareTut = Math.Round((tut - _tut) * 100 / _tut, 2);
                            //}

                            values[rowIndex + 1, 2] = (listItem["count_subjects"] != DBNull.Value) ? listItem["count_subjects"].ToString() : "0";
                            values[rowIndex + 1, 3] = (listItem["count_sub_ukl"] != DBNull.Value) ? listItem["count_sub_ukl"].ToString() : "0";
                            values[rowIndex + 1, 4] = (listItem["count_sub_excl"] != DBNull.Value) ? listItem["count_sub_excl"].ToString() : "0";
                            values[rowIndex + 1, 5] = (listItem["tut"] != DBNull.Value) ? Convert.ToString(Math.Round(Convert.ToDouble(listItem["tut"]), 2)) : "0";

                        }
                        else if ((new List<int> { 54 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                            values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 2] = (listItem["idk"] != DBNull.Value) ? listItem["idk"].ToString() : "";
                            values[rowIndex + 1, 3] = (listItem["biniin"] != DBNull.Value) ? listItem["biniin"].ToString() : "";
                            values[rowIndex + 1, 4] = (listItem["fs_name"] != DBNull.Value) ? listItem["fs_name"].ToString() : "";

                            values[rowIndex + 1, 5] = (listItem["juridical_name"] != DBNull.Value) ? listItem["juridical_name"].ToString() : "";
                            values[rowIndex + 1, 6] = (listItem["indicator_name"] != DBNull.Value) ? listItem["indicator_name"].ToString() : "";
                            values[rowIndex + 1, 7] = (listItem["unit_measure"] != DBNull.Value) ? listItem["unit_measure"].ToString() : "";


                            values[rowIndex + 1, 8] = (listItem["calc_formula"] != DBNull.Value) ? listItem["calc_formula"].ToString() : "0";
                            values[rowIndex + 1, 9] = (listItem["energy_value"] != DBNull.Value) ? Convert.ToString(Convert.ToDecimal(listItem["energy_value"])) : "0";

                        }
                        else if ((new List<int> { 55 }).Contains(model.reportId))
                        {
                            try
                            {

                                values[rowIndex + 1, 0] = Convert.ToString(rowIndex + 1);
                                values[rowIndex + 1, 1] = listItem["oblast_name"].ToString();
                                values[rowIndex + 1, 2] = (listItem["BINIIN"] != DBNull.Value) ? listItem["BINIIN"].ToString() : "";

                                //----55add
                                values[rowIndex + 1, 3] = (listItem["oked_root"] != DBNull.Value) ? listItem["oked_root"].ToString() : "";
                                values[rowIndex + 1, 4] = (listItem["fs_short"] != DBNull.Value) ? listItem["fs_short"].ToString() : "";
                                values[rowIndex + 1, 5] = (listItem["usrjuridicalname"] != DBNull.Value) ? listItem["usrjuridicalname"].ToString() : "";

                                values[rowIndex + 1, 6] = (listItem["EventName"] != DBNull.Value) ? listItem["EventName"].ToString() : "";
                                values[rowIndex + 1, 7] = (listItem["IsPlan"] != DBNull.Value) ? (Convert.ToBoolean(listItem["IsPlan"].ToString()) == true) ? "да" : "нет" : "нет";
                                values[rowIndex + 1, 8] = (listItem["IsNotEvents"] != DBNull.Value) ? (Convert.ToBoolean(listItem["IsNotEvents"].ToString()) == true) ? "да" : "нет" : "нет";
                                values[rowIndex + 1, 9] = (listItem["IsEnergyManagementSystem"] != DBNull.Value) ? (Convert.ToBoolean(listItem["IsEnergyManagementSystem"].ToString()) == true) ? "да" : "нет" : "нет";
                                values[rowIndex + 1, 10] = (listItem["EmplPeriod"] != DBNull.Value) ? Convert.ToDateTime(listItem["EmplPeriod"]).ToString("dd.MM.yyyy") : "";
                                values[rowIndex + 1, 11] = (listItem["PlanExpend"] != DBNull.Value) ? Convert.ToString((listItem["PlanExpend"])) : "0";
                                values[rowIndex + 1, 12] = (listItem["f_actualinvest"] != DBNull.Value) ? Convert.ToString((listItem["f_actualinvest"])) : "0";
                                values[rowIndex + 1, 13] = (listItem["tc_nameru"] != DBNull.Value) ? listItem["tc_nameru"].ToString() : "";
                                values[rowIndex + 1, 14] = (listItem["f_inkind"] != DBNull.Value) ? listItem["f_inkind"].ToString() : "0";
                                values[rowIndex + 1, 15] = (listItem["f_inmoney"] != DBNull.Value) ? listItem["f_inmoney"].ToString() : "0";

                                values[rowIndex + 1, 16] = (listItem["inkind_tut"] != DBNull.Value) ? Math.Round(Convert.ToDouble(listItem["inkind_tut"]), 2).ToString() : "0";
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                        else if ((new List<int> { 27 }).Contains(model.reportId))
                        {
                            values[rowIndex + 1, 0] = listItem["oblast_name"].ToString();
                            values[rowIndex + 1, 1] = Math.Round(Convert.ToDouble(listItem["qty_state_inst"])).ToString();
                            values[rowIndex + 1, 2] = Math.Round(Convert.ToDouble(listItem["qty_quasi_state_inst"])).ToString();
                            values[rowIndex + 1, 3] = Math.Round(Convert.ToDouble(listItem["qty_jur"])).ToString();
                            values[rowIndex + 1, 4] = Math.Round(Convert.ToDouble(listItem["qty_state_inst"]) + Convert.ToDouble(listItem["qty_quasi_state_inst"]) + Convert.ToDouble(listItem["qty_jur"])).ToString();
                            double count27 = 0;
                            for (int m = 0; m < lists.Count; m++)
                            {
                                var listItem27 = lists[m];
                                count27 += Convert.ToDouble(listItem27["qty_jur"]);
                            }
                            values[rowIndex + 1, 5] = Math.Round((Math.Round(Convert.ToDouble(listItem["qty_state_inst"]) + Convert.ToDouble(listItem["qty_quasi_state_inst"]) + Convert.ToDouble(listItem["qty_jur"]))) * 100 / count27).ToString();

                        }
                        else if (model.reportId == 2)
                        {
                            if (rowInd % 2 != 0 && rowIndex % 2 == 0)
                            {
                                values[rowInd, 0] = "colspan=6|" + dictItem.Value;
                                rowInd++;
                            }
                            else
                            {
                                if (dictItem.Key != "oblast_name")
                                {
                                    values[rowInd, colIndex] = dictItem.Value.ToString();
                                    colIndex++;
                                }
                            }

                        }
                        else if (model.reportId == 4)
                        {
                            if (rowInd == 1 || (rowInd - 1) % 7 == 0)
                            {
                                values[rowInd, 0] = "colspan=5|" + dictItem.Value.ToString();
                                rowInd++;
                            }
                            else
                            {
                                if (dictItem.Key != "oblast_name")
                                {
                                    values[rowInd, colIndex] = dictItem.Value.ToString();
                                    colIndex++;
                                }
                            }
                        }
                        double num;
                        if (double.TryParse(dictItem.Value.ToString(), out num))
                        {
                            double count = Math.Round(Convert.ToDouble(dictItem.Value.ToString()));
                            sum += count;
                        }
                    }
                }
                rowInd26++;
                if (rowInd26 == 481 && model.reportId == 26)
                {
                    int index = rowIndex - 28;
                    double count = 0;
                    double t1count = 0;
                    double t3count = 0;
                    double t5count = 0;
                    double t2count = 0;
                    double t4count = 0;
                    double t6count = 0;
                    for (int m = index; m < rowIndex; m++)
                    {
                        var listItem26 = lists[m];
                        t1count += Convert.ToDouble(listItem26["qty_state_inst"]);
                        t2count += Convert.ToDouble(listItem26["qty_state_inst_prev"]);
                        t3count += Convert.ToDouble(listItem26["qty_quasi_state_inst"]);
                        t4count += Convert.ToDouble(listItem26["qty_quasi_state_inst_prev"]);
                        t5count += Convert.ToDouble(listItem26["qty_jur"]);
                        t6count += Convert.ToDouble(listItem26["qty_jur_prev"]);

                    }
                    values[rowInd26, 0] = ReportModel.GetSumName(model.reportId,lang);
                    values[rowInd26, 1] = Math.Round(t1count).ToString();
                    values[rowInd26, 3] = Math.Round(t3count).ToString();
                    values[rowInd26, 5] = Math.Round(t5count).ToString();
                    values[rowInd26, 7] = Math.Round(t1count + t3count + t5count).ToString();

                    count = t1count * 100 / t2count;
                    if (HasValue(count))
                        values[rowInd26, 2] = Math.Round(count).ToString();
                    else
                        values[rowInd26, 2] = " ";

                    count = t3count * 100 / t4count;
                    if (HasValue(count))
                        values[rowInd26, 4] = Math.Round(count).ToString();
                    else
                        values[rowInd26, 4] = " ";

                    count = t5count * 100 / t6count;
                    if (HasValue(count))
                        values[rowInd26, 6] = Math.Round(count).ToString();
                    else
                        values[rowInd26, 6] = " ";

                    count = (t1count + t3count + t5count) * 100 /
                        (t2count + t4count + t6count);
                    if (HasValue(count))
                        values[rowInd26, 8] = Math.Round(count).ToString();
                    else
                        values[rowInd26, 8] = " ";
                }
                if (model.reportId == 2 || model.reportId == 4)
                {
                    values[rowInd, columnCount - 1] = sum.ToString();
                    rowInd++;
                }

                if (model.reportId == 2 && rowIndex % 2 != 0)
                {
                    // ---------- Итого
                    for (int t = 0; t < columnCount; t++)
                    {
                        double itogo = 0;
                        if (t > 0)
                        {
                            for (int i = 0; i < rowCount; i++)
                            {
                                if (i + 2 == rowInd || i + 1 == rowInd)
                                {
                                    double num;
                                    if (double.TryParse(values[i, t], out num))
                                        itogo += Convert.ToDouble(values[i, t]);
                                }
                            }
                            values[rowInd, t] = itogo.ToString();
                        }
                    }

                    values[rowInd, 0] = ReportModel.GetSumName(model.reportId,lang);
                    rowInd++;
                }
                if (model.reportId == 4 && (rowIndex + 1) % 5 == 0)
                {
                    // ---------- Итого
                    for (int t = 0; t < columnCount; t++)
                    {
                        double itogo = 0;
                        if (t > 0)
                        {
                            for (int i = 0; i < rowCount; i++)
                            {
                                if (rowInd - i <= 5)
                                {
                                    double num;
                                    if (double.TryParse(values[i, t], out num))
                                        itogo += Convert.ToDouble(values[i, t]);
                                }
                            }
                            values[rowInd, t] = itogo.ToString();
                        }
                    }

                    values[rowInd, 0] = ReportModel.GetSumName(model.reportId,lang);
                    rowInd++;
                }

                if ((new List<int> { 1, 3, 11, 15, 16, 17 }).Contains(model.reportId))
                    values[rowIndex + 1, columnCount - 1] = sum.ToString();

            }


            // ---------- Итого
            if ((new List<int> { 1, 3, 8, 9, 10, 11, 12, 27, 33,44,46,47,48,49,50,53 }).Contains(model.reportId))
            {
                for (int t = 0; t < columnCount; t++)
                {
                    double itogo = 0;
                    if (t > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            double num;
                            if (double.TryParse(values[i, t], out num))
                            {
                                itogo += Convert.ToDouble(values[i, t]);
                            }
                        }

                        if ((new List<int> { 12 }).Contains(model.reportId))
                        {
                            if (t == 2 || t == 4)
                                values[rowCount - 1, t] = itogo.ToString();
                        }
                        else if (model.reportId == 25)
                        {
                            double count = 0;
                            double t2count = 0;
                            double t4count = 0;
                            double t6count = 0;
                            for (int rowIndex = 0; rowIndex < lists.Count; rowIndex++)
                            {
                                var listItem = lists[rowIndex];
                                t2count += Convert.ToDouble(listItem["qty_state_inst_prev"]);
                                t4count += Convert.ToDouble(listItem["qty_quasi_state_inst_prev"]);
                                t6count += Convert.ToDouble(listItem["qty_jur_prev"]);
                            }
                            if (t == 1 || t == 3 || t == 5 || t == 7)
                                values[rowCount - 1, t] = itogo.ToString();
                            if (t == 2)
                            {
                                count = Convert.ToDouble(values[rowCount - 1, 1]) * 100 / t2count;
                                if (HasValue(count))
                                    values[rowCount - 1, 2] = Math.Round(count).ToString();
                                else
                                    values[rowCount - 1, 2] = " ";
                            }
                            if (t == 4)
                            {
                                count = Convert.ToDouble(values[rowCount - 1, 3]) * 100 / t4count;
                                if (HasValue(count))
                                    values[rowCount - 1, 4] = Math.Round(count).ToString();
                                else
                                    values[rowCount - 1, 4] = " ";
                            }
                            if (t == 6)
                            {
                                count = Convert.ToDouble(values[rowCount - 1, 5]) * 100 / t6count;
                                if (HasValue(count))
                                    values[rowCount - 1, 6] = Math.Round(count).ToString();
                                else
                                    values[rowCount - 1, 6] = " ";
                            }
                            if (t == 8)
                            {
                                count = (Convert.ToDouble(values[rowCount - 1, 1]) + Convert.ToDouble(values[rowCount - 1, 3]) + Convert.ToDouble(values[rowCount - 1, 5])) * 100 /
                                    (t2count + t4count + t6count);
                                if (HasValue(count))
                                    values[rowCount - 1, 8] = Math.Round(count).ToString();
                                else
                                    values[rowCount - 1, 8] = " ";
                            }

                        }
                        else if (model.reportId == 27)
                        {
                            values[rowCount - 1, t] = itogo.ToString();
                            values[rowCount - 1, 5] = "";
                        }
                        else if (model.reportId == 47)
                        {
                            if (t > 1)
                                values[rowCount - 1, t] = itogo.ToString();
                        }
                        else if (model.reportId == 48 || model.reportId == 49)
                        {
                            if (t > 1)
                                values[rowCount - 1, t] = itogo.ToString();
                        }
                        else if (model.reportId == 50)
                        {
                            if (t == columnCount - 1)
                                values[rowCount - 1, t] = itogo.ToString();
                        }
                        else
                            values[rowCount - 1, t] = itogo.ToString();
                    }
                }

                if (model.reportId != 26)
                    if (!(new List<int> { 27,47,48,49 }.Contains(model.reportId)))
                        values[rowCount - 1, 1] = ReportModel.GetSumName(model.reportId,lang);
                    else if (model.reportId == 47 || model.reportId==48 || model.reportId==49)
                    {
                        values[rowCount - 1, 2] = ReportModel.GetSumName(model.reportId,lang);
                    }
                    else values[rowCount - 1, 0] = ReportModel.GetSumName(model.reportId,lang);

            }
            /*values*/
			//rowCount =2;
            for (int i = 0; i < rowCount; i++)
            {
                tableMain2.Body.Rows.Add(new TableBodyRow(Unit.Cm(rowHeight)));
            }

            var textBoxes = new List<ReportItemBase>();

            if (model.reportId == 26)
            {
                values[0, 0] = "rowspan=2|Наименование энергоресурса";
                values[0, 1] = "colspan=2|Гос. учреждений";
                values[0, 3] = "colspan=2|Квазигос. учреждений";
                values[0, 5] = "colspan=2|Юр. лиц";
                values[0, 7] = "colspan=2|Сумма";
                values[1, 1] = "т.у.т.";
                values[1, 2] = "%";
                values[1, 3] = "т.у.т.";
                values[1, 4] = "%";
                values[1, 5] = "т.у.т.";
                values[1, 6] = "%";
                values[1, 7] = "т.у.т.";
                values[1, 8] = "%";
            }

            if (model.reportId == 25)
            {
                values[0, 0] = "rowspan=2|Наименование энергоресурса";
                values[0, 1] = "colspan=2|Гос. учреждений";
                values[0, 3] = "colspan=2|Квазигос. учреждений";
                values[0, 5] = "colspan=2|Юр. лиц";
                values[0, 7] = "colspan=2|ИП";
                values[0, 9] = "colspan=2|Сумма";
                values[1, 1] = "т.у.т.";
                values[1, 2] = "%";
                values[1, 3] = "т.у.т.";
                values[1, 4] = "%";
                values[1, 5] = "т.у.т.";
                values[1, 6] = "%";
                values[1, 7] = "т.у.т.";
                values[1, 8] = "%";
                values[1, 9] = "т.у.т.";
                values[1, 10] = "%";
            }

            if (model.reportId == 42) {

				values[0, 0] =(lang=="kz")? "rowspan=2|Облыс" : "rowspan=2|Область";
				values[0, 1] = (lang=="kz")? "colspan=5|Барлығы" : "colspan=5|Всего";
				values[0, 6] = (lang == "kz") ? "colspan=5|Алынып тасталды" : "colspan=5|Исключено";
				values[0, 11] = (lang == "kz") ? "colspan=5|МЭТ тізімі" : "colspan=5|Перечень ГЭР";
				values[0, 16] = (lang == "kz") ? "colspan=5|Жалтарушылар" : "colspan=5|Уклонисты";

				values[1, 1] = (lang == "kz") ? "ММ" : "ГУ";
				values[1, 2] = (lang == "kz") ? "Квази	" : "Квази";
				values[1, 3] = (lang == "kz") ? "Заңды тұлға" : "ЮЛ";
				values[1, 4] = (lang == "kz") ? "ЖК" : "ИП";
				values[1, 5] = (lang == "kz") ? "Жалпы" : "Общая";

				values[1, 6] = (lang == "kz") ? "ММ" : "ГУ";
				values[1, 7] = (lang == "kz") ? "Квази" : "Квази";
				values[1, 8] = (lang == "kz") ? "Заңды тұлға" : "ЮЛ";
				values[1, 9] = (lang == "kz") ? "ЖК" : "ИП";
				values[1, 10] = (lang == "kz") ? "Жалпы" : "Исключение";

				values[1, 11] = (lang == "kz") ? "ММ" : "ГУ";
				values[1, 12] = (lang == "kz") ? "Квази" : "Квази";
				values[1, 13] = (lang == "kz") ? "Заңды тұлға" : "ЮЛ";
				values[1, 14] = (lang == "kz") ? "ЖК" : "ИП";
				values[1, 15] = (lang == "kz") ? "Жалпы" : "Общая";

				values[1, 16] = (lang == "kz") ? "ММ" : "ГУ";
				values[1, 17] = (lang == "kz") ? "Квази" : "Квази";
				values[1, 18] = (lang == "kz") ? "Заңды тұлға" : "ЮЛ";
				values[1, 19] = (lang == "kz") ? "ЖК" : "ИП";
				values[1, 20] = (lang == "kz") ? "Жалпы" : "Общая";
			}

            //if (model.reportId == 42)
            //{

            //    values[0, 1] = "colspan=2|Область";
            //    values[0, 2] = "colspan=5|Электроэнергия (%)";
            //    values[0, 3] = "colspan=5|Теплоэнергия (%)";
            //}

                var rowInd2 = 1;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    string val = values[i, j];

					if (val == null && model.reportId == 2 || val == null && model.reportId == 4 || val == null && model.reportId == 25 || val == null && model.reportId == 26 || val == null && model.reportId == 37 || val == null && model.reportId == 42 || val == null && model.reportId == 43 && model.reportId == 45 && i>=0 && i<2)  //
                        continue;
                                        
					var txbx = GetTextBox(textBoxIndex++, i,j, rowCount, model.reportId);

                    if (model.reportId == 25 || model.reportId == 26 || model.reportId==42)
                    {
                        double columnnWidth = 0;
                        if (j == 0)
                            columnnWidth = 3.5;
                        else
                            columnnWidth = 1.5;
                        txbx.Size = new SizeU(Unit.Cm(columnnWidth), Unit.Cm(0.3));
                    }
                    else
                        txbx.Size = new SizeU(Unit.Cm(columns[j].Width), Unit.Cm(0.3));


					if (model.reportId == 42)
					{
						if (i > 1 && j > 0)
							txbx.Format = "{0:N2}";
					}
					textBoxes.Add(txbx);

                    int rowspan = 1; int colspan = 1;
                    if (val != null && val.StartsWith("rowspan"))
                    {
                        rowspan = int.Parse(val.Split('|')[0].Split('=')[1]);
                        val = val.Split('|')[1];
                    }
                    else if (val != null && val.StartsWith("colspan"))
                    {
                        colspan = int.Parse(val.Split('|')[0].Split('=')[1]);
                        val = val.Split('|')[1];
                    }

                    txbx.Value = val;

					if (i == 0)
					{
						txbx.Style.Font.Bold = true;
						txbx.Style.TextAlign = HorizontalAlign.Center;
						txbx.Style.VerticalAlign = VerticalAlign.Middle;
						txbx.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12); //Telerik.Reporting.Drawing.Font(12D); 
					}
					else
					{
						
						txbx.Style.Font.Bold = false;
						txbx.Style.VerticalAlign = VerticalAlign.Middle;
						txbx.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10);
						
						if (i == 1 && (model.reportId == 42))
						{
							txbx.Style.TextAlign = HorizontalAlign.Center;
							txbx.Style.Font.Bold = true;
						}
						else txbx.Style.BackgroundColor = Color.White;
					}

                    double num;
                    if (double.TryParse(values[i, j], out num))
                    {
                        txbx.Style.TextAlign = HorizontalAlign.Right;
                 
                    }

                    tableMain2.Body.SetCellContent(i, j, txbx, rowspan, colspan);
                }
            }

			for (int i = 0; i < columnCount; i++)
			{
				TableGroup tableGroup1 = new TableGroup();
				tableGroup1.Name = "group" + tableGroupIndex++;
				tableMain2.ColumnGroups.Add(tableGroup1);
			}

            tableMain2.Items.AddRange(textBoxes.ToArray());

			for (int i = 0; i < rowCount; i++)
			{
				TableGroup tableGroup1 = new TableGroup();
				TableGroup tableGroup2 = new TableGroup();
				
				tableGroup1.Name = "group" + tableGroupIndex++;
				tableGroup2.Name = "group" + tableGroupIndex++;
				tableGroup1.ChildGroups.Add(tableGroup2);
				tableMain2.RowGroups.Add(tableGroup1);
			}

            //tableMain2.Location = PointU.Empty;
            //tableMain2.Location = new PointU(Unit.Cm(1), Unit.Cm(1));
            /*tableMain2.Anchoring = AnchoringStyles.Bottom;
            tableMain2.Anchoring = AnchoringStyles.Top;
            tableMain2.Anchoring = AnchoringStyles.Left;
            tableMain2.Anchoring = AnchoringStyles.Right;*/
            tableMain2.Name = "tableMain2";
            //tableMain2.Size = new SizeU(Unit.Cm(10), Unit.Cm(5));

            this.detail.Items.AddRange(new ReportItemBase[] { tableMain2 });
            

            ((ISupportInitialize)(this)).EndInit();
        }

        private Telerik.Reporting.TextBox GetTextBox(int index, int rowIndex,int colIndex, int rowCount, int reportId)
        {
            var txbx = new TextBox();

			if (rowIndex > 1 && colIndex > 0)
				txbx.Format = "{0:N2}";

            txbx.Name = "txbx" + index;
            txbx.Style.Padding.Top = Unit.Cm(0.1);
            txbx.Style.Padding.Bottom = Unit.Cm(0.1);
            txbx.Size = new Telerik.Reporting.Drawing.SizeU(Unit.Cm(3D), Unit.Cm(rowHeight));//.Drawing.SizeU(Unit.Cm(3D), Unit.Cm(rowHeight));
            txbx.StyleName = "";
            txbx.Style.BorderStyle.Default = BorderType.Solid;
            txbx.Style.BorderWidth.Default = new Unit(0.15d, UnitType.Mm);
            if (reportId == 25 || reportId == 26 || reportId==42)
            {
                if (reportId == 25)
                {
                    if (rowIndex == 0 || rowIndex == 1 || rowIndex == rowCount - 1)
                        txbx.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))),
                            ((int)(((byte)(225)))), ((int)(((byte)(255)))));
                }
                else
                    if (rowIndex == 0 || rowIndex == 1)
                        txbx.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))),
                            ((int)(((byte)(225)))), ((int)(((byte)(255)))));
            }
            else if (rowIndex == 0 || rowIndex == rowCount - 1 && !(new List<int> { 2, 4, 5, 6, 7, 13, 14, 15, 16, 17, 28, 29, 30, 31, 34, 37, 38, 39, 40 }).Contains(reportId))
                txbx.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(225)))), ((int)(((byte)(255)))));
            txbx.Value = "" + index;
            return txbx;
        }

        public static bool HasValue(double value)
        {
            return !System.Double.IsNaN(value) && !System.Double.IsInfinity(value);
        }
    }
}
using System.Data;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Reestr;
using OfficeOpenXml; 
using OfficeOpenXml.Style; 
using System; 
using System.Collections.Generic; 
using System.Drawing; 
using System.IO; 
using System.Linq; 
using System.Reflection; 
using System.Threading.Tasks; 
using System.Web;
using Aisger.Models.Repository.Security; 

namespace Aisger.Utils
{
    public class ExcelFileManager
    {
		public List<RST_Reestr> ReadExcel(DataSet asDataSet, int year)
		{
			var list = new List<RST_Reestr>();
			if (asDataSet.Tables.Count == 0)
			{
				return list;
			}
			var table = asDataSet.Tables[0];
			var bins = new List<string>();
			bool duplicated = false;
			foreach (DataRow dr in table.Rows)
				{
					if (string.IsNullOrWhiteSpace(dr[1].ToString()) && string.IsNullOrWhiteSpace(dr[2].ToString()) && string.IsNullOrWhiteSpace(dr[3].ToString()))
					{
						continue;
					}
					duplicated = false;

					var bin = dr[1].ToString().Trim();
					if (!string.IsNullOrEmpty(bin) && bins.Contains(bin))
					{
						duplicated = true;
						//continue;
					}

					if (!string.IsNullOrEmpty(bin))
					{
						bins.Add(bin);
					}

					var idk = dr[0].ToString().Trim();
					var reestrName = dr[2].ToString().Trim();
					var reestrAddress = dr[3].ToString().Trim();
					var reestr = new RST_Reestr
					{
						BINIIN = bin,
						IDK = idk
					};

					var check = new RstReestrRepository().GetReestrByBin(reestr.BINIIN, year);
					reestr.StatusReestr = check.StatusReestr;
					reestr.IsExistSecUser = new SecUserRepository().CheckUserByBiniin(reestr.BINIIN);

					//---- если нет биниин 
					if (string.IsNullOrWhiteSpace(bin))
					{
						reestr.StatusReestr = StatusReestr.NOT_BINIIN;
					}

					//---- если биниин дублируется
					if (check.StatusReestr == StatusReestr.EMPTY_REESTR || check.StatusReestr == StatusReestr.NEW_REESTR)
					{
						reestr.OwnerName = reestrName;
						reestr.Address = reestrAddress;

						//----check duplicated
						if (duplicated)
						{
							reestr.StatusReestr = StatusReestr.DUPLICATE_IIN;
						}
					}
					else
					{
						reestr.OwnerName = check.OwnerName;
						reestr.Address = check.Address;
						reestr.TemplOwnerName = reestrName;

						//----check duplicated
						if (duplicated)
						{
							reestr.StatusReestr = StatusReestr.DUPLICATE_IIN_EXIST_REESTR;
						}
					}

					

					list.Add(reestr);
				}

			return list;
		}


        public static FileStreamResult GenerateXlSReportReestrFilter(IEnumerable<RST_ReportReestrFilter> rstReportReestrFilters, FileInfo info ,string filename)
        {
                MemoryStream MS = new MemoryStream();
                using (ExcelPackage pck = new ExcelPackage(info))
                {
                    var ws = pck.Workbook.Worksheets.Add("Отчет");
                    ws.Cells["A1"].Value = ResourceSetting.biniinSubject;
                    ws.Cells["B1"].Value = ResourceSetting.SubPerson;
                    ws.Cells["C1"].Value = ResourceSetting.Address;
                    ws.Cells["D1"].Value = ResourceSetting.Oblast;
                    ws.Cells["E1"].Value = ResourceSetting.RstDicStatus;
                    ws.Cells["F1"].Value = ResourceSetting.ReportReason;


                    var reportReestrFilters = rstReportReestrFilters as RST_ReportReestrFilter[] ?? rstReportReestrFilters.ToArray();
                    for (var i = 0; i < reportReestrFilters.Count(); i++)
                    {
                        var index = i + 2;
                        var reestr = reportReestrFilters[i];
                        ws.Cells["A" + index].Value = reestr.BINIIN;
                        ws.Cells["B" + index].Value = reestr.OwnerName;
                        ws.Cells["C" + index].Value = reestr.Address;
                        ws.Cells["D" + index].Value = reestr.OblastName;
                        ws.Cells["E" + index].Value = reestr.StatusName;
                        ws.Cells["F" + index].Value = reestr.ReasonName;
                    }

                    pck.SaveAs(MS);
                    MS.Position = 0;
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    FileStreamResult FSR = new FileStreamResult(MS, contentType);
                    FSR.FileDownloadName = filename;
                    return FSR;
                }
        } 
    }
}
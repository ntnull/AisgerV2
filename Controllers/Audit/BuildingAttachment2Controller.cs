using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Constants;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.EnergyAudit;

namespace Aisger.Controllers.Audit
{
    public class BuildingAttachment2Controller : Controller
    {
        //
        // GET: /BuildingAttachment2/
        
        public ActionResult GetBuildingView(long preambleId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            List<EAUDIT_Building> buildings = null;

            if (preamble != null && preamble.EAUDIT_Building.Count > 0)
                buildings = preamble.EAUDIT_Building.ToList();
            else
                buildings = new List<EAUDIT_Building>()
                {
                    new EAUDIT_Building()
                };

            attachment2.Buildings = buildings;
            
            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment2.Buildings)
            {
                foreach (var pInfo in typeof(EAUDIT_Building).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.Buildings
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/Preamble/_Buildings.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm1View(long preambleId, long? buildingId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            attachment2.RefBuilding = buildingId;
            List<EAUDIT_BuildingForm1> buildingRows = null;

            if (buildingId == null && preamble.EAUDIT_Building != null)
            {
                var eauditBuilding = preamble.EAUDIT_Building.FirstOrDefault();
                if (eauditBuilding != null)
                    attachment2.RefBuilding = buildingId = eauditBuilding.Id;
            }

            if (preamble != null && preamble.EAUDIT_BuildingForm1.Count > 0 && buildingId != null)
            {
                buildingRows = preamble.EAUDIT_BuildingForm1.Where(f => f.refBuilding == buildingId).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.BuildingForm1, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment2.BuildingForm1Rows = new List<EAUDIT_BuildingForm1>();

            foreach (var type in types)
            {
                var form = new EAUDIT_BuildingForm1()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    Unit = type.DIC_Unit != null ? type.DIC_Unit.NameRu : string.Empty,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (buildingRows != null)
                {
                    foreach (var row in buildingRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        form.ParameterDesignation = row.ParameterDesignation;
                        if (!string.IsNullOrEmpty(row.Unit))
                            form.Unit = row.Unit;
                        form.Value = row.Value;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                //                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                //                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                //                {
                //                    attachment2.IndustryForm17Rows.Add(new EAUDIT_IndustryForm17()
                //                    {
                //                        EAUDIT_Preamble = preamble,
                //                        EAUDIT_DIC_TypeResource = type,
                //                        IsCommand = true,
                //                        InnerOrder = 1000
                //                    });
                //                }

                attachment2.BuildingForm1Rows.Add(form);
            }

            if (buildingRows != null)
            {
                foreach (var source in buildingRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_BuildingForm1()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        Value = source.Value,
                        ParameterDesignation = source.ParameterDesignation,
                        Unit = source.Unit,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment2.BuildingForm1Rows.Add(form);
                }
            }
            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.BuildingForm1Rows)
            {
                form.RowSpan = attachment2.BuildingForm1Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);

                foreach (var pInfo in typeof(EAUDIT_BuildingForm1).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm1
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm1/_BuildingForm1View.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm2View(long preambleId, long? buildingId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            attachment2.RefBuilding = buildingId;
            List<EAUDIT_BuildingForm2> buildingRows = null;

            if (buildingId == null)
            {
                var eauditBuilding = preamble.EAUDIT_Building.FirstOrDefault();
                if (eauditBuilding != null)
                    attachment2.RefBuilding = buildingId = eauditBuilding.Id;
            }
            if (preamble != null && preamble.EAUDIT_BuildingForm2.Count > 0 && buildingId != null)
            {
                buildingRows = preamble.EAUDIT_BuildingForm2.Where(f => f.refBuilding == buildingId).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.BuildingForm2, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment2.BuildingForm2Rows = new List<EAUDIT_BuildingForm2>();

            foreach (var type in types)
            {
                var form = new EAUDIT_BuildingForm2()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    DesignationAndUnit = type.Designation  + (type.DIC_Unit != null ? ", " + type.DIC_Unit.NameRu : string.Empty),
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (buildingRows != null)
                {
                    foreach (var row in buildingRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        if (!string.IsNullOrEmpty(row.DesignationAndUnit))
                            form.DesignationAndUnit = row.DesignationAndUnit;
                        form.ValueStandart = row.ValueStandart;
                        form.ValueEstimatedProject = row.ValueEstimatedProject;
                        form.ValueActual = row.ValueActual;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                // for command lines
                //                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                //                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                //                {
                //                    attachment2.IndustryForm17Rows.Add(new EAUDIT_IndustryForm17()
                //                    {
                //                        EAUDIT_Preamble = preamble,
                //                        EAUDIT_DIC_TypeResource = type,
                //                        IsCommand = true,
                //                        InnerOrder = 1000
                //                    });
                //                }

                attachment2.BuildingForm2Rows.Add(form);
            }

            if (buildingRows != null)
            {
                foreach (var source in buildingRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_BuildingForm2()
                    {
                        Id = source.Id,
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        ValueStandart = source.ValueStandart,
                        ValueEstimatedProject = source.ValueEstimatedProject,
                        ValueActual = source.ValueActual,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment2.BuildingForm2Rows.Add(form);
                }
            }
            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.BuildingForm2Rows)
            {
                form.RowSpan = attachment2.BuildingForm2Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_BuildingForm2).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm2
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm2/_BuildingForm2View.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm3View(long preambleId, long? buildingId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            attachment2.RefBuilding = buildingId;
            List<EAUDIT_BuildingForm3> buildingRows = null;

            if (buildingId == null && preamble.EAUDIT_Building != null)
            {
                var eauditBuilding = preamble.EAUDIT_Building.FirstOrDefault();
                if (eauditBuilding != null)
                    attachment2.RefBuilding = buildingId = eauditBuilding.Id;
            }

            if (preamble != null && preamble.EAUDIT_BuildingForm3.Count > 0 && buildingId != null)
            {
                buildingRows = preamble.EAUDIT_BuildingForm3.Where(f => f.refBuilding == buildingId).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.BuildingForm3, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment2.BuildingForm3Rows = new List<EAUDIT_BuildingForm3>();

            foreach (var type in types)
            {
                var form = new EAUDIT_BuildingForm3()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    DesignationAndUnit = type.Designation + (type.DIC_Unit != null ? ", " + type.DIC_Unit.NameRu : string.Empty),
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (buildingRows != null)
                {
                    foreach (var row in buildingRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        if (!string.IsNullOrEmpty(row.DesignationAndUnit))
                            form.DesignationAndUnit = row.DesignationAndUnit;
                        form.ValueCritic = row.ValueCritic;
                        form.ValueEstimatedProject = row.ValueEstimatedProject;
                        form.ValueActual = row.ValueActual;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                // for command lines
                //                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                //                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                //                {
                //                    attachment2.IndustryForm17Rows.Add(new EAUDIT_IndustryForm17()
                //                    {
                //                        EAUDIT_Preamble = preamble,
                //                        EAUDIT_DIC_TypeResource = type,
                //                        IsCommand = true,
                //                        InnerOrder = 1000
                //                    });
                //                }

                attachment2.BuildingForm3Rows.Add(form);
            }

            if (buildingRows != null)
            {
                foreach (var source in buildingRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_BuildingForm3()
                    {
                        Id = source.Id,
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        ValueCritic = source.ValueCritic,
                        ValueEstimatedProject = source.ValueEstimatedProject,
                        ValueActual = source.ValueActual,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment2.BuildingForm3Rows.Add(form);
                }
            }

            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.BuildingForm3Rows)
            {
                form.RowSpan = attachment2.BuildingForm3Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_BuildingForm3).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm3
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm3/_BuildingForm3View.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm4View(long preambleId, long? buildingId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            attachment2.RefBuilding = buildingId;
            List<EAUDIT_BuildingForm4> buildingRows = null;

            if (buildingId == null)
            {
                var eauditBuilding = preamble.EAUDIT_Building.FirstOrDefault();
                if (eauditBuilding != null)
                    attachment2.RefBuilding = buildingId = eauditBuilding.Id;
            }
            if (preamble != null && preamble.EAUDIT_BuildingForm4.Count > 0 && buildingId != null)
            {
                buildingRows = preamble.EAUDIT_BuildingForm4.Where(f => f.refBuilding == buildingId).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.BuildingForm4, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment2.BuildingForm4Rows = new List<EAUDIT_BuildingForm4>();

            foreach (var type in types)
            {
                var form = new EAUDIT_BuildingForm4()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    DesignationAndUnit = type.Designation + (type.DIC_Unit != null ? ", " + type.DIC_Unit.NameRu : string.Empty),
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (buildingRows != null)
                {
                    foreach (var row in buildingRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        if (!string.IsNullOrEmpty(row.DesignationAndUnit))
                            form.DesignationAndUnit = row.DesignationAndUnit;
                        form.ValueCritic = row.ValueCritic;
                        form.ValueEstimatedProject = row.ValueEstimatedProject;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                // for command lines
                //                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                //                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                //                {
                //                    attachment2.IndustryForm17Rows.Add(new EAUDIT_IndustryForm17()
                //                    {
                //                        EAUDIT_Preamble = preamble,
                //                        EAUDIT_DIC_TypeResource = type,
                //                        IsCommand = true,
                //                        InnerOrder = 1000
                //                    });
                //                }

                attachment2.BuildingForm4Rows.Add(form);
            }

            if (buildingRows != null)
            {
                foreach (var source in buildingRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_BuildingForm4()
                    {
                        Id = source.Id,
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        ValueCritic = source.ValueCritic,
                        ValueEstimatedProject = source.ValueEstimatedProject,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment2.BuildingForm4Rows.Add(form);
                }
            }

            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.BuildingForm4Rows)
            {
                form.RowSpan = attachment2.BuildingForm4Rows
                     .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_BuildingForm4).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm4
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm4/_BuildingForm4View.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm5View(long preambleId, long? buildingId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            attachment2.RefBuilding = buildingId;
            List<EAUDIT_BuildingForm5> buildingRows = null;
            
            if (buildingId == null)
            {
                var eauditBuilding = preamble.EAUDIT_Building.FirstOrDefault();
                if (eauditBuilding != null)
                    attachment2.RefBuilding = buildingId = eauditBuilding.Id;
            }

            if (preamble != null && preamble.EAUDIT_BuildingForm5.Count > 0 && buildingId != null)
            {
                buildingRows = preamble.EAUDIT_BuildingForm5.Where(f => f.refBuilding == buildingId).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.BuildingForm5, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment2.BuildingForm5Rows = new List<EAUDIT_BuildingForm5>();

            foreach (var type in types)
            {
                var form = new EAUDIT_BuildingForm5()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    DesignationAndUnit = type.Designation + (type.DIC_Unit != null ? ", " + type.DIC_Unit.NameRu : string.Empty),
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (buildingRows != null)
                {
                    foreach (var row in buildingRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        if (!string.IsNullOrEmpty(row.DesignationAndUnit))
                            form.DesignationAndUnit = row.DesignationAndUnit;
                        form.ValueCritic = row.ValueCritic;
                        form.ValueEstimatedProject = row.ValueEstimatedProject;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                // for command lines
                //                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                //                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                //                {
                //                    attachment2.IndustryForm17Rows.Add(new EAUDIT_IndustryForm17()
                //                    {
                //                        EAUDIT_Preamble = preamble,
                //                        EAUDIT_DIC_TypeResource = type,
                //                        IsCommand = true,
                //                        InnerOrder = 1000
                //                    });
                //                }

                attachment2.BuildingForm5Rows.Add(form);
            }

            if (buildingRows != null)
            {
                foreach (var source in buildingRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_BuildingForm5()
                    {
                        Id = source.Id,
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        ValueCritic = source.ValueCritic,
                        ValueEstimatedProject = source.ValueEstimatedProject,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment2.BuildingForm5Rows.Add(form);
                }
            }

            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.BuildingForm5Rows)
            {
                form.RowSpan = attachment2.BuildingForm5Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_BuildingForm5).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm5
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm5/_BuildingForm5View.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm6View(long preambleId, long? buildingId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            attachment2.RefBuilding = buildingId;
            List<EAUDIT_BuildingForm6> buildingRows = null;

            if (buildingId == null)
            {
                var eauditBuilding = preamble.EAUDIT_Building.FirstOrDefault();
                if (eauditBuilding != null)
                    attachment2.RefBuilding = buildingId = eauditBuilding.Id;
            }
            if (preamble != null && preamble.EAUDIT_BuildingForm6.Count > 0 && buildingId != null)
            {
                buildingRows = preamble.EAUDIT_BuildingForm6.Where(f => f.refBuilding == buildingId).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.BuildingForm6, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment2.BuildingForm6Rows = new List<EAUDIT_BuildingForm6>();

            foreach (var type in types)
            {
                var form = new EAUDIT_BuildingForm6()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    DesignationAndUnit = type.Designation + (type.DIC_Unit != null ? ", " + type.DIC_Unit.NameRu : string.Empty),
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (buildingRows != null)
                {
                    foreach (var row in buildingRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        if (!string.IsNullOrEmpty(row.DesignationAndUnit))
                            form.DesignationAndUnit = row.DesignationAndUnit;
                        form.ValueStandart = row.ValueStandart;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                // for command lines
                //                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                //                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                //                {
                //                    attachment2.IndustryForm17Rows.Add(new EAUDIT_IndustryForm17()
                //                    {
                //                        EAUDIT_Preamble = preamble,
                //                        EAUDIT_DIC_TypeResource = type,
                //                        IsCommand = true,
                //                        InnerOrder = 1000
                //                    });
                //                }

                attachment2.BuildingForm6Rows.Add(form);
            }

            if (buildingRows != null)
            {
                foreach (var source in buildingRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_BuildingForm6()
                    {
                        Id = source.Id,
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        ValueStandart = source.ValueStandart,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment2.BuildingForm6Rows.Add(form);
                }
            }

            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.BuildingForm6Rows)
            {
                form.RowSpan = attachment2.BuildingForm6Rows
                   .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_BuildingForm6).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm6
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm6/_BuildingForm6View.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm7View(long preambleId, long? buildingId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            attachment2.RefBuilding = buildingId;
            List<EAUDIT_BuildingForm7> buildingRows = null;
            
            if (buildingId == null)
            {
                var eauditBuilding = preamble.EAUDIT_Building.FirstOrDefault();
                if (eauditBuilding != null)
                    attachment2.RefBuilding = buildingId = eauditBuilding.Id;
            }
            if (preamble != null && preamble.EAUDIT_BuildingForm7.Count > 0 && buildingId != null)
            {
                buildingRows = preamble.EAUDIT_BuildingForm7.Where(f => f.refBuilding == buildingId).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.BuildingForm7, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment2.BuildingForm7Rows = new List<EAUDIT_BuildingForm7>();

            foreach (var type in types)
            {
                var form = new EAUDIT_BuildingForm7()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    DesignationAndUnit = type.Designation + (type.DIC_Unit != null ? ", " + type.DIC_Unit.NameRu : string.Empty),
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (buildingRows != null)
                {
                    foreach (var row in buildingRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        if (!string.IsNullOrEmpty(row.DesignationAndUnit))
                            form.DesignationAndUnit = row.DesignationAndUnit;
                        form.ValueStandart = row.ValueStandart;
                        form.Value = row.Value;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                // for command lines
                //                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                //                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                //                {
                //                    attachment2.IndustryForm17Rows.Add(new EAUDIT_IndustryForm17()
                //                    {
                //                        EAUDIT_Preamble = preamble,
                //                        EAUDIT_DIC_TypeResource = type,
                //                        IsCommand = true,
                //                        InnerOrder = 1000
                //                    });
                //                }

                attachment2.BuildingForm7Rows.Add(form);
            }

            if (buildingRows != null)
            {
                foreach (var source in buildingRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_BuildingForm7()
                    {
                        Id = source.Id,
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        ValueStandart = source.ValueStandart,
                        Value = source.Value,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment2.BuildingForm7Rows.Add(form);
                }
            }

            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.BuildingForm7Rows)
            {
                form.RowSpan = attachment2.BuildingForm7Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_BuildingForm7).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm7
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm7/_BuildingForm7View.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm8View(long preambleId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;

            List<EAUDIT_Building> buildings = null;

            if (preamble != null && preamble.EAUDIT_Building.Count > 0)
                buildings = preamble.EAUDIT_Building.OrderBy(b => b.Name).ToList();
            else
                buildings = new List<EAUDIT_Building>()
                {
                    new EAUDIT_Building()
                };

            attachment2.Buildings = buildings;
            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.Buildings)
            {
                foreach (var pInfo in typeof(EAUDIT_Building).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm8
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm8/_BuildingForm8View.cshtml", attachment2);
        }

        public ActionResult GetBuildingForm9View(long preambleId, long? buildingId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment2 = new EauditAttachment2();
            attachment2.Preamble = preamble;
            attachment2.RefBuilding = buildingId;
            List<EAUDIT_BuildingForm9> buildingRows = null;

            if (buildingId == null)
            {
                var eauditBuilding = preamble.EAUDIT_Building.FirstOrDefault();
                if (eauditBuilding != null)
                    attachment2.RefBuilding = buildingId = eauditBuilding.Id;
            }
            if (preamble != null && preamble.EAUDIT_BuildingForm9.Count > 0 && buildingId != null)
            {
                buildingRows = preamble.EAUDIT_BuildingForm9.Where(f => f.refBuilding == buildingId).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.BuildingForm9, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment2.BuildingForm9Rows = new List<EAUDIT_BuildingForm9>();

            foreach (var type in types)
            {
                var form = new EAUDIT_BuildingForm9()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble, 
                    Designation = type.Designation,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (buildingRows != null)
                {
                    foreach (var row in buildingRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        if (!string.IsNullOrEmpty(row.Designation))
                            form.Designation = row.Designation;
                        form.Value = row.Value;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                // for command lines
                //                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                //                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                //                {
                //                    attachment2.IndustryForm19Rows.Add(new EAUDIT_IndustryForm19()
                //                    {
                //                        EAUDIT_Preamble = preamble,
                //                        EAUDIT_DIC_TypeResource = type,
                //                        IsCommand = true,
                //                        InnerOrder = 1000
                //                    });
                //                }

                attachment2.BuildingForm9Rows.Add(form);
            }

            if (buildingRows != null)
            {
                foreach (var source in buildingRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_BuildingForm9()
                    {
                        Id = source.Id,
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        Value = source.Value,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment2.BuildingForm9Rows.Add(form);
                }
            }

            attachment2.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment2.BuildingForm9Rows)
            {
                form.RowSpan = attachment2.BuildingForm9Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_BuildingForm9).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.BuildingForm9
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment2.FieldComments.Add(fieldComment);
                }
            }
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment2AdditionalFields(isReadOnly, ref attachment2);

            return PartialView("~/Views/EnergyAudit/BuildingForm9/_BuildingForm9View.cshtml", attachment2);
        }
        
        public JsonResult UpdateBuilding(EAUDIT_Building formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.Buildings);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }

        public JsonResult UpdateForm1(EAUDIT_BuildingForm1 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.BuildingForm1);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }

        public JsonResult UpdateForm2(EAUDIT_BuildingForm2 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.BuildingForm2);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }

        public JsonResult UpdateForm3(EAUDIT_BuildingForm3 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.BuildingForm3);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }

        public JsonResult UpdateForm4(EAUDIT_BuildingForm4 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.BuildingForm4);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }
        public JsonResult UpdateForm5(EAUDIT_BuildingForm5 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.BuildingForm5);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }

        public JsonResult UpdateForm6(EAUDIT_BuildingForm6 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.BuildingForm6);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }

        public JsonResult UpdateForm7(EAUDIT_BuildingForm7 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.BuildingForm7);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }

        public JsonResult UpdateForm9(EAUDIT_BuildingForm9 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateBuildingForm(formBuilding, EnergyAuditFormConsts.BuildingForm9);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }
        
        public ActionResult Delete(long id, string buildingFormCode)
        {
            var repository = new EnergyAuditRepository();
            repository.DeleteBuildingForm(id, buildingFormCode);
            return Json(new { IsSuccess = true });
        }

        private void InitAttachment2AdditionalFields(bool? isReadOnly, ref EauditAttachment2 attachment2)
        {
            attachment2.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            if (Session["SignedValue"] != null)
                attachment2.SignedEauditPreamble = (EAUDIT_Preamble)Session["SignedValue"];
        }
    }
}
namespace Aisger.Utils
{
    public static class CodeConstManager
    {
       
        public const string SESSION_USER = "SESSION_USER";
        public const string SESSION_USER_OOPT_ID = "SESSION_USER_OOPT_ID";
        public const string SESSION_USER_ORG_ID = "SESSION_USER_ORG_ID";
        public const string SESSION_USER_ROLES = "SESSION_USER_ROLES";
        public const string PATTERN = @"^([1-9]|0[1-9]|1[012])[- /.]([1-9]|0[1-9]|[12][0-9]|3[01])[- /.][0-9]{4}$";
        public const int ORGANIZATION_OOPT_ID = 2;
        public const long TYPE_ROUTE_ID =1L;
        public const long REG_STATUS_REESTR_ID =1L;
        public const long EXTEND_STATUS_REESTR_ID =2L;
        public const long NEW_STATUS_REESTR_ID =3L;
        public const long OLD_STATUS_REESTR_ID =4L;
        public const long STATUS_SEND_ID =2L;
        public const long STATUS_ACCEPT_ID =3L;
        public const long STATUS_REJECT_ID =4L;
        public const long STATUS_WORK_ID =7L;
        public const long STATUS_AGREEMENT_ID = 8L;
        public const long STATUS_EVADERS_ID = 6L;

        public const long APP_TYPE_TOO = 1L;
        public const long APP_TYPE_GU = 2L;
        public const long APP_TYPE_AO = 3L;
        public const long APP_TYPE_IP = 4L;
        public const long APP_TYPE_PHYS_PERSON = 5L;


        public const long KIND_USER_SUBJECT = 1L;
        public const long KIND_USER_AUDIT = 2L;
        public const long KIND_USER_ESCO = 3L;
        public const long KIND_USER_MAP = 4L;

        public const string CODE_USER_ESCO = "3";
        public const string CODE_USER_SUBJECT = "1";
        public const string CODE_USER_AUDIT = "2";
        public const string CODE_USER_APP = "4";
        public const string CODE_USER_MAPEE2 = "5";

        public const string CODE_REPORT_REESTR = "report";
        public const string RstApplication = "RstApplication";
        public const string RstReestr = "RstReestr";
        public const string RstExluded = "RstExluded";
        public const string RstReport = "RstReport";
        public const string SubjectPage = "SubjectPage";
        public const string AccountSetting = "AccountSetting";
        public const string Users = "Users";
        public const string SecRules = "SecRules";
        public const string JurEvent = "JurEvent";
        public const string Guest = "Guest";
        public const string RegisterForm = "RegisterForm";
        public const string AuditPage = "AuditPage";
        public const string PrivateSetting = "PrivateSetting";
        public const string SubActionPlan = "SubActionPlan";
        public const string MapApplication = "MapApplication";
        public const string AppForm = "AppForm";
        public const string AppAction = "AppAction";
        public const string SubjectPerson = "SubjectPerson";
        public const string ReportAnalyse = "ReportAnalyse";
        public const string ReportShowCase = "ReportShowCase";
        public const string ReportAskuerCase = "ReportAskuerCase";
        public const string CollectorPage = "CollectorPage";
        public const string CollectorReestr = "CollectorReestr";
        public const string SubDicKindResource = "SubDicKindResource";
        public const string SubDicTypeResource = "SubDicTypeResource";
        public const string DicOrganisation = "DicOrganisation";
        public const string DicDepartment = "DicDepartment";
        public const string DicUnit = "DicUnit";
        public const string DicOked = "DicOked";
        public const string DicHolidays = "DicHolidays";
        public const string RegistredYear = "RegistredYear";
        public const string ShowAll = "ShowAll";
        public const string EscoReestr = "EscoReestr";
        public const string EscoDicProductKind = "EscoDicProductKind";
        public const string EscoSearch = "EscoSearch";
        public const string EscoPage = "EscoPage";
        public const string MapNotify = "MapNotify";
        public const string AuditReestr = "AuditReestr";
        public const string MapApp = "MapApp";
        public const string MapInpbox = "MapInpbox";
        public const string MapProject = "MapProject";
        public const string MapApplivcantReestr = "MapApplivcantReestr";
		public const string MapRegisterEE2 = "MapRegisterEE2";
		public const string SourceController = "SourceController";

		public const string MapApplicationEE2 = "MapApplicationEE2";
		public const string MapApplicationEE2Operation = "MapApplicationEE2-operation";
		public const string MapApplicationEE2Edit = "MapApplicationEE2-edit";
		public const string MapApplicationEE2Add = "MapApplicationEE2-add";
		public const string MapApplicationEE2Send = "MapApplicationEE2-send";
		public const string MapApplicationEE2Save = "MapApplicationEE2-save";

        public const string RemoveDuplicate = "RemoveDuplicate";
        public const string MapApplicationEE2Notify = "MapApplicationEE2Notify";

        public const string EnergyAudit = "EnergyAudit";
        public const string FLOAT_CODE = "float";
        public const string STRING_CODE = "string";
        public const string LONG_CODE = "long";
        public const string SUB_DIC_STATUS_NOTGIVED = "Не предоставил";
        public const string SUB_DIC_STATUS_NOTGIVED_KZ = "Тапсырмаған";

        public const string SUB_REASON_SEND = "Отправленные";
        public const string SUB_REASON_NOTSEND = "Не отправленные";
        public const string SUB_REASON_ALL = "Все";

        public const string SUB_REASON_SEND_KZ = "Жіберілгендер";
        public const string SUB_REASON_NOTSEND_KZ = "Жіберілмегендер";
        public const string SUB_REASON_ALL_KZ = "Жалпы";

        public const long SUB_REASON_SEND_ID = 1;
        public const long SUB_REASON_NOTSEND_ID = 2;
        public const long SUB_REASON_ALL_ID = 3;
        public const string ACCEPT_REPORT = "ACCEPT_REPORT";

        public const string RST_EXCLUDED_NAME = "Исключен";
        public const string RST_NOTEXCLUDED_NAME = "Не исключен";
        public const string RST_EXCLUDED_ALL = "Все";

        public const string RST_EXCLUDED_NAME_KZ = "Шығарылған";
        public const string RST_NOTEXCLUDED_NAME_KZ = "Шығарылмаған";
        public const string RST_EXCLUDED_ALL_KZ = "Жалпы";

        public const long RST_EXCLUDED_ID = 1;
        public const long RST_NOTEXCLUDED_ID = 2;
        public const long RST_EXCLUDED_ALL_ID = 3;

        public const long RST_STATUS_EXCLUDED_ID = 2;
        public const string RST_STATUS_EXCLUDED_NAME = "Исключен";

        public const string MAP_STATUS_REJECT = "REJECT";
        public const string MAP_STATUS_ACCEPT = "ACCEPT";
        public const string MAP_STATUS_FINISHED = "FINISHED";
        public const string MAP_STATUS_PROJECT = "PROJECT";

        public const string DISC_PRODUCT = "product";
        public const string DISC_POWER = "power";
        public const string DISC_IN_KIND = "inkind";
        public const string DISC_IN_VALUE_TERM = "interm";

        public const int CountDay = 10;
        public const string DEFAULT_PWD = "J@S.udKted";
        public const string TEMPLATE_APPLID = "SN000000";
        public static readonly string[] OBLAST_CODES = { "1100000000", "1500000000", "1900000000", "2300000000", "2700000000", "3100000000", "3500000000", "3900000000", "4300000000", "4700000000", "5100000000", "5500000000", "5900000000", "6300000000", "7100000001", "7500000001"};

        public const long SORT_INDEX_DATESEND = 1;
        public const string SORT_NAME_DATESEND = "Дата подачи";
        public const string SORT_NAME_DATESEND_KZ = "Тапсырған күні";

        public const long SORT_INDEX_DATEEDIT = 2;
        public const string SORT_NAME_DATEEDIT = "Дата включения";
        public const string SORT_NAME_DATEEDIT_KZ = "Енгізген күні";

		public const string GERNOTIFY = "GerNotify";  //Получать уведомление(ГЭР) 
        public const string LOGINWITHOUTECP = "LOGINWITHOUTECP";
    }
}

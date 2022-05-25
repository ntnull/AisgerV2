using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Reports
{
    [Serializable]
    public class ReportModel
    {
        public int year { get; set; }
        public int reportId { get; set; }
        public int? oblastId { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
        public int? limit { get; set; }
        public string oblastName { get; set; }
        public int? fscode { get; set; }
        public int? okedId { get; set; }
		public static ReportColumn[] GetColumns(int reportId,string lang)
		{
			switch (reportId)
			{
				case 1: return new ReportColumn[] {
                    new ReportColumn(){Name="", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекем. саны":"Количество гос. учреждений", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Квази мекем. саны":"Количество квазигос учреждений", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлға. саны":"Количество юр. лиц", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы":"Сумма", Width=2}
                };
					break;
				case 2: return new ReportColumn[] {
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=5},
                    new ReportColumn(){Name = (lang == "kz") ? "Мемлекеттік мекем. саны" : "Количество гос. учреждений", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Квази мекем. саны":"Количество квазигос учреждений", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлға. саны":"Количество юр. лиц", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Басқалары":"Прочий", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы":"Сумма", Width=2.5}
                };
					break;
				case 3: return new ReportColumn[] {
                    new ReportColumn(){Name="", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=6},
               new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекем. саны":"Количество гос. учреждений", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Квази мекем. саны":"Количество квазигос учреждений", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлға. саны":"Количество юр. лиц", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы":"Сумма", Width=2}
                };
					break;
				case 4: return new ReportColumn[] {
                            new ReportColumn(){Name="", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=6},
               new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекем. саны":"Количество гос. учреждений", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Квази мекем. саны":"Количество квазигос учреждений", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлға. саны":"Количество юр. лиц", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы":"Сумма", Width=2}
                };
					break;
				case 5: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"БСН":"БИН", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Мекен-жайы":"Адрес", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Энергоаудиторлар саны":"Количество энергоаудитов", Width=3},
                };
					break;
				case 6: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=7},
                    new ReportColumn(){Name=(lang=="kz")?"БСН":"БИН", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Мекен-жайы":"Адрес", Width=4},
                };
					break;
				case 7: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы ЭСКО":"Наименование ЭСКО", Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Өнем атауы":"Наименование продукта", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Өнемдер тобы":"Группа продукта", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Мақсатты аудитория":"Целевая аудитория", Width=3},
                };
					break;
				case 8: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Меншік нысаны":"Форма собственности", Width=6},
                    new ReportColumn(){Name=(lang=="kz")?"Субъектер саны":"Количество субъектов", Width=4},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі":"Объем потребления", Width=4}
                };
				case 9: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область", Width=8},
                    new ReportColumn(){Name=(lang=="kz")?"Субъектер саны":"Количество субъектов", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі":"Объем потребления", Width=4}
                };
					break;
				case 10: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"В":"Вид экономической деятельности", Width=9},
                    new ReportColumn(){Name=(lang=="kz")?"Субъектер саны":"Количество субъектов", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі":"Объем потребления", Width=2.5}
                };
					break;
				case 11: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=0.5},
                    new ReportColumn(){Name=(lang=="kz")?"Меншік нысаны":"Форма собственности", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"100-ге дейін":"до 100", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"100-ден 1500-ге дейін":"от 100 до 1500", Width=1.5},
                    new ReportColumn(){Name=(lang=="kz")?"1500-ден 10000-ға дейін":"от 1500 до 10000", Width=1.5},
                    new ReportColumn(){Name=(lang=="kz")?"10000-нан 25000-ға дейін":"от 10000 до 25000", Width=1.5},
                    new ReportColumn(){Name=(lang=="kz")?"25000-нан 50000-ға дейін":"от 25000 до 50000", Width=1.5},
                    new ReportColumn(){Name=(lang=="kz")?"50000-дан 75000-ға дейін":"от 50000 до 75000", Width=1.5},
                    new ReportColumn(){Name=(lang=="kz")?"75000-нан 100000-ға дейін":"от 75000 до 100000", Width=1.5},
                    new ReportColumn(){Name=(lang=="kz")?"100000 және одан жоғары":"от 100000 и выше", Width=1.5},
                    new ReportColumn(){Name=(lang=="kz")?"Барлығы":"Итого", Width=1}
                };
				case 12: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі":"Объем потребления", Width=4},
                    new ReportColumn(){Name=(lang=="kz")?"Субъектер саны":"Количество субъектов", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Жалпы санның үлесі (%)":"Доля от общего числа (%)", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну (шартты отынның тоннасы)":"Потребление (т.у.т.)", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Шартты отынның тонна тұтыну үлесі":"Доля от общего потребления (%)", Width=2.5}
                };
					break;
				case 13: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Субъекттің атауы":"Наименование субъекта", Width=7.5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі":"Объем потребления", Width=4},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область", Width=8},
                };
					break;
				case 14: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Энергоресурстың түрі":"Вид энергоресурса", Width=8},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі (ш.о.т.)":"Объем потребления (т.у.т.)", Width=5}
                };
					break;
				case 15: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекемелер саны":"Количество гос. учреждений", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Квазигос саны. мекемелер":"Количество квазигос. учреждений", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар саны":"Количество юр. лиц", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы":"Сумма", Width=2.5}
                };
					break;
				case 16: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=3.5},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекемелер саны":"Количество гос. учреждений", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Квазигос саны. мекемелер":"Количество квазигос. учреждений", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар саны":"Количество юр. лиц", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы":"Сумма", Width=2.5}
                };
					break;
				case 17: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование", Width=3.5},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекемелер саны":"Количество гос. учреждений", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Квазигос саны. мекемелер":"Количество квазигос. учреждений", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар саны":"Количество юр. лиц", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы":"Сумма", Width=2.5}
                };
					break;
				case 25: return new ReportColumn[] {
                   new ReportColumn(){Name=(lang=="kz")?"Энергоресурстардың атауы":"Наименование энергоресурса", Width=70},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекеме (шартты отынның тоннасы)":"Гос. учреждений (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекеме (%)":"Гос. учреждений (%)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Квази мекемелер":"Квазигос. учреждений (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Квази мекемелер":"Квазигос. учреждений (%)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар (шартты отынның тоннасы)":"Юр. лиц (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар (%)":"Юр. лиц (%)", Width=20},

                    new ReportColumn(){Name=(lang=="kz")?"ИП (шартты отынның тоннасы)":"ИП. (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"ИП (%)":"ИП. (%)", Width=20},

                    new ReportColumn(){Name=(lang=="kz")?"Сомасы (шартты отынның тоннасы)":"Сумма (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы (%)":"Сумма (%)", Width=20}
                };
					break;
				case 26: return new ReportColumn[] {
                    new ReportColumn(){Name=(lang=="kz")?"Энергоресурстардың атауы":"Наименование энергоресурса", Width=70},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекеме (шартты отынның тоннасы)":"Гос. учреждений (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекеме (%)":"Гос. учреждений (%)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Квази мекемелер":"Квазигос. учреждений (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Квази мекемелер":"Квазигос. учреждений (%)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар (шартты отынның тоннасы)":"Юр. лиц (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар (%)":"Юр. лиц (%)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы (шартты отынның тоннасы)":"Сумма (т.у.т.)", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы (%)":"Сумма (%)", Width=20}
                };
					break;
				case 27: return new ReportColumn[] {
                    new ReportColumn(){Name=(lang=="kz")?"Облыс атауы":"Наименование области", Width = 5},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекемелерді тұтыну (шартты отынның тоннасы)":"Потребление гос. учреждений (т.у.т.)", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Пайдалану. квазигосы. мекеме (шартты отынның тонна)":"Потреб. квазигос. учреждений (т.у.т.)", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғаларды тұтыну (шартты отынның тоннасы)":"Потребление юр.лиц (т.у.т.)", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Сомасы (шартты отынның тоннасы)":"Сумма (т.у.т.)", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"МЭТ жалпы тұтынудан үлесі (%)":"Доля от общего потребления ГЭР (%)", Width=2}
                };
					break;
				case 28: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name="№ ИДК", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"МЭТ субъектісінің атауы":"Наименование субъекта ГЭР", Width=8},
                    new ReportColumn(){Name=(lang=="kz")?"Электр энергиясын тұтыну көлемі, кВтс":"Объем потребления электроэнергии, кВтч", Width=3.5}
                };
					break;
				case 29: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name="№ ИДК", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"МЭТ субъектісінің атауы":"Наименование субъекта ГЭР", Width=8},
                    new ReportColumn(){Name=(lang=="kz")?"Электр энергиясын тұтыну көлемі, кВтс":"Объем потребления теплоэнергии, Гкал", Width=3.5}
                };
					break;
				case 30: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name="№ ИДК", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"МЭТ субъектісінің атауы":"Наименование субъекта ГЭР", Width=8},
                    new ReportColumn(){Name=(lang=="kz")?"Электр энергиясын тұтыну көлемі, кВтс":"Объем потребления природного газа, м3", Width=3.5}
                };
					break;
				case 31: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name="№ ИДК", Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"МЭТ субъектісінің атауы":"Наименование субъекта ГЭР", Width=8},
                    new ReportColumn(){Name=(lang=="kz")?"Электр энергиясын тұтыну көлемі, кВтс":"Объем потребления каменного угля, тн", Width=3.5}
                };
					break;
				case 33: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыстың, қаланың атауы":"Наименование области, города",Width=6.5},
                    new ReportColumn(){Name=(lang=="kz")?"Денсаулық сақтау объектілері":"По объектам здравоохранения, Гкал/м2", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Білім беру объектілері":"По объектам образования, Гкал/м2", Width=2.5},
                    new ReportColumn(){Name=(lang=="kz")?"Мәдени объектілір":"По объектам культуры, Гкал/м2", Width=2.5}
                };
					break;
				case 34: return new ReportColumn[] {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыстың, қаланың атауы":"Наименование области, города",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекемелер":"Государственные учреждения, %",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Квазигос мекемелер":"Квазигосударственный сектор, %",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар":"Юридические лица, %",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Жеке кәсіпкерлер, %":"Индивидуальные предприниматели, %",Width=5}
                };
					break;
				case 37: return new ReportColumn[] {
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование",Width=3.5},
                    new ReportColumn(){Name=(lang=="kz")?"Мемлекеттік мекемелер":"Государственные учреждения",Width=3.5},
                    new ReportColumn(){Name=(lang=="kz")?"Квазигос мекемелер ":"Квазигосударственный сектор",Width=3.5},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлғалар ":"Юридические лица",Width=3.5},
                    new ReportColumn(){Name=(lang=="kz")?"Барлығы":"Всего",Width=3.5}
                };
					break;
				case 38: return new ReportColumn[] {
                    new ReportColumn(){Name="ПП", Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Атауы":"Наименование",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Саны":"Количество",Width=4}
                };
					break;
				case 39: return new ReportColumn[] {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Ай/Жыл":"Месяц/Год",Width=3},
                    new ReportColumn(){Name="Всего поступило", Width=3},
                    new ReportColumn(){Name="Отклонено", Width=2.5},
                    new ReportColumn(){Name="Ожидает ЭСКО", Width=3},
                    new ReportColumn(){Name="Включено в карту ЭЭ", Width=3}
                };
					break;
				case 40: return new ReportColumn[] {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Ай/Жыл":"Месяц/Год",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Кезең ішіндегі жобалар саны":"Количество проектов за период",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Кезең ішіндегі жобалардың жалпы сомасы (млн тг)":"Общая сумма проектов за период (млн тг)",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Жобалардың жалпы саны":"Общее количество проектов",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Жобалардың жалпы құны (млн тг)":"Общая стоимость проектов (млн тг)",Width=3}
                };
					break;
				case 41: return new ReportColumn[]{
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Білім беру":"Образование",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Денсаулық сақтау":"Здравоохранение",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Басқа":"Прочие",Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"ЭҚТЖ-сыз":"Без ОКЭД",Width=2},
                    new ReportColumn(){Name=(lang=="kz")?"Барлығы":"Итого",Width=2},
                }; break;
				case 42: return new ReportColumn[] {
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=20},

                    new ReportColumn(){Name=(lang=="kz")?"ММ":"ГУ",Width=20},
                    new ReportColumn(){Name="Квази", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлға":"ЮЛ",Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"ЖК":"ИП",Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Жалпы":"Общая",Width=20},

                    new ReportColumn(){Name=(lang=="kz")?"ММ":"ГУ",Width=20},
                    new ReportColumn(){Name="Квази", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлға":"ЮЛ",Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"ЖК":"ИП",Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Алынып тасталды":"Исключение",Width=20},

                    new ReportColumn(){Name=(lang=="kz")?"ММ":"ГУ",Width=20},
                    new ReportColumn(){Name="Квази", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлға":"ЮЛ",Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"ЖК":"ИП",Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Жалпы":"Общая",Width=20},

                    new ReportColumn(){Name=(lang=="kz")?"ММ":"ГУ",Width=20},
                    new ReportColumn(){Name="Квази", Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Заңды тұлға":"ЮЛ",Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"ЖК":"ИП",Width=20},
                    new ReportColumn(){Name=(lang=="kz")?"Жалпы":"Общая",Width=20},
                }; break;
                case 43:return new ReportColumn[]
                {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Электр энергиясы (%)":"Электроэнергия (%)",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Жылу энергиясы (%)":"Теплоэнергия (%)",Width=5},
                    new ReportColumn(){Name="Газ (%)",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Су (%)":"Вода (%)",Width=5}
                };break;
                case 44:return new ReportColumn[]
                {
                    new ReportColumn(){Name="№", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Субъекттер саны":"Количество субъектов",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі (кВт)":"Объем потребления(кВт)",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтынуға арналған шығыстар (₸)":"Расходы на потребление(₸)",Width=5}
                }; break;
                case 45:return new ReportColumn[]
                 {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name="ИДК",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Мекемелердің атауы":"Наименование организации",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Штат кестесі бойынша қызметкерлердің (қызметкерлердің) саны)":"Количество сотрудников по штатному расписанию (работников)",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Оқушылар(тәрбиеленушілер) саны)":"Количество учащихся(воспитанников)",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Төсек-орын саны(бару)":"Количество койко-мест(посещений)",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Энергия аудиті өткізілді":"Энергоаудит проводился",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Энергия менеджменті жүйесі енгізілді":"Cистема энергоменеджмента внедрена",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Салынған жылы":"Год постройки",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Автоматтандырылған жылу пунктінің болуы (қою: иә / жоқ)":"Наличие автоматизированного теплового пункта (поставить: да/нет)",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Ғимараттың жалпы ауданы, м2":"Общая площадь здания, м²",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Ғимараттың жылытылатын ауданы, м2":"Отопливаемая площадь здания, м²",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Орталық жылыту":"Центральное отопление",Width=3},
                    new ReportColumn(){Name=(lang=="kz")?"Автономды жылыту":"Автономное отопление",Width=3}
                 }; break;
                case 46:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Субъекттер саны":"Количество субъектов",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі (шартты отынның тоннасы)":"Объем потребления (т.у.т)",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Шығарылғаннан кейінгі субъектілер саны":"Количество субъектов после исключения",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Алып тасталғаннан кейін субъектілерді тұтыну көлемі(Т.с. С.)т)":"Объем потребления субъектов, после исключения(т.у.т)",Width=5}                
                     }; break;
                case 47:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Субъект атауы":"Наименование субъекта",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі (шартты отынның тоннасы)":"Объем потребления (т.у.т)",Width=5}
                     }; break;
                case 48:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Субъект атауы":"Наименование субъекта",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі (шартты отынның тоннасы)":"Объем потребления (т.у.т)",Width=5}
                     }; break;
                case 49:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Субъект атауы":"Наименование субъекта",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі (шартты отынның тоннасы)":"Объем потребления (т.у.т)",Width=5}
                     }; break;         
                case 50:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name="Субъект",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Іс-шараның атауы":"Наименование мероприятия",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Есепті кезеңдегі нақты инвестициялар (ҚҚС есебімен), теңге":"Фактические инвестиции за отчетный период (с учетом НДС), тенге",Width=5}                   
                     }; break;
                case 51:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name="Субъект",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Іс-шараның атауы":"Наименование мероприятия",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Энергетикалық ресурстың атауы":"Название энергетического ресурса",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Заттай көріністегі нақты әсер":"Фактический эффект в натуральном выражении",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Потребление энергоресурсов, полученные НЕ из собственных источников":"Потребление энергоресурсов, полученные НЕ из собственных источников",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Іс-шараның әлеуеті":"Потенциал мероприятия",Width=5}
                     }; break;
                case 52:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name="Субъект",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Энергия тиімділігі көрсеткішінің атауы":"Наименование показателя энергоэффективности",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Есепті жылдағы энергия тиімділігі көрсеткішінің мәні":"Значение показателя энергоэффективности в отчетном году",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Өткен жылдағы энергия тиімділігі көрсеткішінің мәні":"Значение показателя энергоэффективности в предыдущем году",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Тиімділік көрсеткішінің мәнін өзгерту":"Измение значения показателя эффекивности",Width=5},                       
                    new ReportColumn(){Name=(lang=="kz")?"Мәртебесі":"Статус",Width=5},
                     }; break;
                case 53:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Субъекттер саны":"Количество субъектов",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Жалтарушылар саны":"Количество уклонистов",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Алынып тасталғандар саны":"Количество исключенных",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Тұтыну көлемі (шартты отынның тоннасы)":"Объем потребления (т.у.т)",Width=5}
                     }; break;
                case 54:
                    return new ReportColumn[]
                     {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name="ИДК",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"ЖСН":"БИН",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Меншік нысаны":"Форма собстенности",Width=5 },
                    new ReportColumn(){Name="Субъект",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"ЭЭ көрсеткішінің атауы":"Наименование показателя ЭЭ",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"ЭЭ коэффициенттерін өлшеу бірлігі":"Ед. измерения коэффициентов ЭЭ",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"ЭЭ көрсеткішін есептеу формуласы":"Формула расчета показателя ЭЭ",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"ЭЭ көрсеткішіннің мәндері":"Значение показателя ЭЭ",Width=5 }
                     }; break;
                case 55:return new ReportColumn[]
                {
                    new ReportColumn(){Name="ПП", Width=1},
                    new ReportColumn(){Name=(lang=="kz")?"Облыс":"Область",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"ЖСН":"БИН",Width=5 },
                    
                    new ReportColumn(){Name=(lang=="kz")?"ОКЭД":"ОКЭД",Width=5},
                    new ReportColumn(){Name=(lang=="kz")?"Меншік түрі":"Форма собственности",Width=5 },
                    
                    new ReportColumn(){Name="Субъект",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Іс-шаралар атауы":"Наименование мероприятия",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Энергоаудит жүргізілді":"Энергоаудит проводился",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Іс-шаралар жоспарлары орындалған жоқ":"Не проводились планы мероприятий",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Басқару жүйесі енгізілді":"Cистема менеджмента внедрена",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Срок реализации за год(месяц)":"Срок реализации за год(месяц)",Width=5 },

                    new ReportColumn(){Name=(lang=="kz")?"Планируемые расходы":"Планируемые расходы",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Фактические инвестиции за отчетный период (с учетом НДС), тенге":"Фактические инвестиции за отчетный период (с учетом НДС), тенге",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Название энергетического ресурса":"Название энергетического ресурса",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Фактический эффект экономии в натуральном выражении":"Фактический эффект экономии в натуральном выражении",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Фактический эффект экономии в денежном выражении (с учетом НДС), тенге":"Фактический эффект экономии в денежном выражении (с учетом НДС), тенге",Width=5 },
                    new ReportColumn(){Name=(lang=="kz")?"Фактический эффект экономии в т.у.т.":"Фактический эффект экономии в т.у.т.",Width=5 },
                };break; 
                default:
					break;
			}
			return null;
		}

        public static string GetSumName(int reportId,string lang)
        {
            switch (reportId)
            {
                case 1: return (lang=="kz")?"Барлығының саны":"Итоговое количество:";
                    break;
                case 2: return (lang=="kz")?"Барлығы":"Итого:";
                    break;
                case 3: return (lang == "kz") ? "Барлығының саны" : "Итоговое количество:";
                    break;
                case 4: return (lang == "kz") ? "Барлығының саны" : "Итоговое количество:";
                    break;
                case 8:
                case 9:
                case 10:
                case 11: return (lang == "kz") ? "Барлығының саны" : "Итоговое количество:";
                    break;
                case 12: return "";
                    break;
                case 25: return (lang=="kz")?"Республика бойынша барлығы":"Итого по республике:";
                case 26: return (lang == "kz") ? "Облыс бойынша барлығы" : "Итого по области:";
                case 27: return (lang == "kz") ? "Республика бойынша барлығы" : "Итого по республике:";
                    break;
                case 33:  return (lang=="kz")?"Барлығы":"Итого:";
                    break;
                case 37:
                    return (lang == "kz") ? "Барлығы" : "Итого:";
                    break;
                case 44:
                    return  (lang == "kz") ? "Барлығы" : "Итого:";
                    break;
                case 46:
                    return (lang == "kz") ? "Барлығы" : "Итого:";
                    break;
                case 47:
                    return (lang == "kz") ? "Барлығы" : "Итого:";
                    break;
                case 48:
                    return (lang == "kz") ? "Барлығы" : "Итого:";
                    break;
                case 49:
                    return (lang == "kz") ? "Барлығы" : "Итого:";
                    break;
                case 50:
                    return (lang == "kz") ? "Барлығы" : "Итого:";
                    break;
                case 53:
                    return (lang == "kz") ? "Барлығы" : "Итого:";
                    break;
                default:
                    break;
            }
            return "";
        }

        public static string GetH1(int reportId,string lang="ru")
        {
            switch (reportId)
            {
                case 1:
                    return (lang == "kz") ? "Республика бойынша МЭТ субъектілерінің тізілімі бойынша есептің үлгісі" : "Отчет по реестру Субъектов ГЭР по республике";
                    break;
                case 2:
                    return (lang == "kz") ? "Облыстар бөлінісінде МЭТ субъектілерінің тізілімі бойынша есептің үлгісі" : "Отчет по реестру Субъектов ГЭР в разрезе областей";
                    break;
                case 3:
                    return (lang == "kz") ? "Республика бойынша Тізілімнен шығарудың негізгі себептері" : "Основные причины исключения из реестра по республике";
                    break;
                case 4:
                    return (lang == "kz") ? "Облыстар бөлінісінде Тізілімнен шығарудың негізгі себептері" : "Основные причины исключения из реестра в разрезе областей";
                    break;
                case 5:
                    return (lang == "kz") ? " Ел бойынша жүргізілген энергия аудиттерінің саны бар кезеңдегі<br/> энергетикалық аудит жөніндегі ұйымдардың тізбесі" : "Перечень организаций по энергетическому аудиту за период <br/> с количеством проведенных энергоаудитов, по стране";
                    break;
                case 6:
                    return (lang == "kz") ? "Ел бойынша кезең ішіндегі энергия сервистік ұйымдардың тізбесі" : "Перечень энергосервисных организаций за период, по стране";
                    break;
                case 7:
                    return (lang == "kz") ? "ЭСКЕ-нің жылжытылатын өнімдерінің тізбесі" : "Перечень продвигаемых продуктов ЭСКО";
                    break;
                case 8:
                    return (lang == "kz") ? "Меншік нысандары бойынша бөле отырып, <br/> тұтыну көлемімен саны бойынша есеп" : "Отчет по количеству, с разбивкой по формам собственности";
                    break;
                case 9:
                    return (lang == "kz") ? "Облыстар бойынша бөле отырып,<br/> тұтыну көлемімен есеп" : "Отчет по областям, с объемами потребления";
                    break;
                case 10:
                    return (lang == "kz") ? "Саны, тұтыну көлемі бар Экономикалық қызмет түрлері <br/>бойынша есеп" : "Отчет по видам экономической деятельности, с количеством,<br/> с объемами потребления";
                    break;
                case 11:
                    return (lang == "kz") ? "Тұтыну топтары бойынша есеп" : "Отчет по группам потребления, с разбивкой по формам собственности, с количеством, с объемами потребления";
                    break;
                case 12:
                    return (lang == "kz") ? "Тұтыну көлемдерінің санаттары бойынша есеп" : "Отчет по категориям объемов  потребления";
                    break;
                case 13:
                    return (lang == "kz") ? "Тұтыну көлемі бойынша сұрыптаумен атаулар бойынша есеп,<br/> алғашқы 100 шығару" : "Отчет по наименованиям, с сортировкой по объемам потребления,<br/> выводить первые 100";

                    break;
                case 14:
                    return (lang == "kz") ? "Тұтыну көлемімен энергия ресурстарының түрлері бойынша есеп" : "Отчет по видам энергоресурсов, с объемами потребления";
                    break;
                case 15:
                    return (lang == "kz") ? "Республика бойынша берілген толық емес мәліметтерді<br/> тексеру туралы есеп" : "Отчет о проверке предоставленных неполных сведений<br/> по республике";
                    break;
                case 16:
                    return (lang == "kz") ? "Облыстар бойынша берілген толық емес мәліметтерді тексеру туралы есеп" : "Отчет о проверке предоставленных неполных сведений по областям";
                    break;
                case 17:
                    return (lang == "kz") ? "Республика бойынша жалтарған және/немесе дәйексіз мәліметтер берген есеп" : "Отчет по уклонившимся и/или предоставившим недостоверные сведения по субъектам";
                    break;
                case 25:
                    return (lang == "kz") ? "Шартты отын тоннасында негізгі энергия ресурстарын тұтыну көлемі<br/> және өткен кезеңнен % " : "Объемы потребления основных энергоресурсов, в т.у.т. и <br/> % от предыдущего периода";
                case 26:
                    return (lang == "kz") ? "Негізгі энергия ресурстарын тұтыну көлемі, шартты отын тоннасында<br/> және облыстар бөлінісінде өткен кезеңнен % - бен" : "Объемы потребления основных энергоресурсов, в т.у.т.<br/> и % от предыдущего периода в разрезе областей";
                case 27:
                    return (lang == "kz") ? "МЭТ субъектілерінің облыстар бойынша <br/>энергия ресурстарын тұтыну үлестері" : "Доли потребления энергоресурсов субъектами ГЭР по областям";
                    break;
                case 28:
                    return (lang == "kz") ? "Электр энергиясын ең көп тұтынатын МЭТ субъектілері, топ 100" : "Субъекты ГЭР с наибольшим потреблением электроэнергии, топ 100";
                    break;
                case 29:
                    return (lang == "kz") ? "Жылу энергиясын ең көп тұтынатын МЭТ субъектілері, топ 100" : "Субъекты ГЭР с наибольшим потреблением теплоэнергии, топ 100";
                    break;
                case 30:
                    return (lang == "kz") ? "Табиғи газды ең көп тұтынатын МЭТ субъектілері, топ 100" : "Субъекты ГЭР с наибольшим потреблением природного газа, топ 100";
                    break;
                case 31:
                    return (lang == "kz") ? "Тас көмірді ең көп тұтынатын МЭТ субъектілері, топ 100" : "Субъекты ГЭР с наибольшим потреблением каменного угля, топ 100";
                    break;
                case 33:
                    return (lang == "kz") ? "Облыстар бойынша<br/> үлестік жылу тұтынудың орташа көрсеткіштері" : "Средние показатели удельного теплопотребления по областям";
                    break;
                case 34:
                    return (lang == "kz") ? "Энергия ресурстарын тұтынуды есепке алу аспаптарымен жарақтандыру туралы ақпарат" : "Информация об оснащенности приборами учета потребления энергоресурсов";
                    break;
                case 37:
                    return (lang == "kz") ? "МЭТ субъектілерінің облыстардың тілігінде суды тұтыну көлемі (млн. м3) " : "Объемы потребления воды субъектами ГЭР (в млн. м3) в разрере областей";
                    break;
                case 38:
                    return (lang == "kz") ? "Энергоаудиторлар бойынша өткізілген энергия аудиттерінің саны" : "Количество проведенных энергоаудитов, в разрезе энергоаудиторов";
                    break;
                case 39:
                    return (lang == "kz") ? "Энергия тиімділігі картасына енгізу үшін өтініштер бойынша есеп" : "Отчет по заявлениям для включения в Карту энергоэффективности";
                    break;
                case 40:
                    return (lang == "kz") ? "Энергия тиімділігі картасы бойынша есеп" : "Отчет по Карте энергоэффективности";
                    break;
                case 41:
                    return (lang == "kz") ? "Энерготиімділік 2.0 бойынша есептеме" : "Отчет по Проекту Энергоэффективность 2.0";
                    break;
                case 42:
                    return (lang == "kz") ? "Облыстар бойынша жиынтық, МС және шығарылған" : "Свод по областям, видам собственности и исключенным";
                    break;
                case 43:
                    return (lang == "kz") ? "Есептеу аспаптарымен жарақтандырылуы" : "Оснащенность приборами учета";
                    break;
                case 44:
                    return (lang == "kz") ? "Облыстар бөлінісінде энергия тұтынуға арналған шығыстар бойынша есеп" : "Отчет по расходам на энергопотребление в разрезе областей";
                    break;
                case 45:
                    return (lang == "kz") ? "ММ сандық көрсеткіштері бойынша есеп" : "Отчет по количественным показателям ГУ";
                    break;
                case 46:
                    return (lang == "kz") ? "ҚР бойынша жалпы тұтыну бойынша есеп (шартты отын тоннасында)" : "Отчет по общему потреблению по РК (в т.у.т.)";
                    break;
                case 47:
                    return (lang == "kz") ? "Ірі субъектілерді тұтыну (шартты отын тоннасы) бойынша іріктеу" : "Выборка крупных субъектов по потреблению (т.у.т.)";
                    break;
                case 48:
                    return (lang == "kz") ? "100 өнеркәсіптік субъект" : "100 промышленных субъектов";
                    break;
                case 49:
                    return (lang == "kz") ? "Әлеуметтік нысандар" : "Социальные объекты";
                    break;
                case 50:
                    return (lang == "kz") ? "МЭТ субъектілері орындаған шығыны аз іс-шаралар тізімі" : "Список мало затратных мероприятий исполненных субъектами ГЭР";
                    break;
                case 51:
                    return (lang == "kz") ? "МЭТ субъектілері орындаған тиімді іс-шаралар тізімі" : "Список эффективных мероприятий исполненных субъектами ГЭР";
                    break;
                case 52:
                    return (lang == "kz") ? "Өнім бірлігіне энергия ресурстарын тұтыну көлемінің жыл сайынғы <br/>төмендеуін қамтамасыз етпеген МЭТ субъектілерінің тізімі (тізім)" : "Список субъектов ГЭР (список), не обеспечивших ежегодное снижение <br/>объема потребления энергоресурсов на единицу продукции";
                    break;
                case 53:
                    return (lang == "kz") ? "Облыс бойынша деректерді салыстыру" : "Сравнение данных по областям";
                    break;
                case 54:
                    return (lang == "kz") ? "МЭТ субъектілерінің энергия ресурстарын үлестік тұтынуы бойынша жиынтық" : "Свод по удельному потреблению энергоресурсов субъектами ГЭР";
                    break;
                case 55:
                    return (lang == "kz") ? "Свод  мероприятий исполненных субъектами ГЭР" : "Свод  мероприятий исполненных субъектами ГЭР";
                    break;
                default:                    
                    break;   //
            }
            return "";
        }
        public static string GetH2(int reportId, int year, string beginDate, string endDate,string oblastName="",string lang="ru")
        {
            string _period = (lang == "kz") ? "Есептеме периоды "+year+" ж.": "Отчетный период " + year + " г.";
            string _date = (lang == "kz") ? ("Басталу күні: " + beginDate + " Аяқталу күні: " + endDate) : ("Дата начала: " + beginDate + " <br/> " +
                    "Дата окончания: " + endDate + "");
            string _oblast = oblastName + "<br/> " + _period;
            string _register = (lang=="kz")? "Тіркелген "+beginDate+" - "+endDate : "Зарегистрирована с: " + beginDate + " по: " + endDate + " ";

            switch (reportId)
            {
                case 1:
                case 2:
                case 3:
                case 4: return _period;
                    break;
                case 5: return _register;
                    break;
                case 6: return _register;
                    break;
                case 7: return _date;
                    break;
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29: return _period;
                case 30:
                case 31:
                case 33:
                case 34:
                case 37:
                case 38: return _period;
                    break;
                case 39:
                case 40: return _date;//"С " + beginDate + " по: " + endDate;
                    break;
                case 41:
				case 42: return _period;
                case 43: return _period;
                case 44: return _period;
                case 45: return _period;
                case 46: return _period;
                case 47:return _oblast;//oblastName + "<br/> Отчетный период " + year + " г.";break;
                case 48: return _oblast;
                case 49: return _oblast;
                case 50: return _oblast;
                case 51: return _period; break;
                case 52: return _period; break;
                case 53: return _period; break;
                case 54: return _period; break;
                case 55: return _oblast;break;
                default:
                    break;
            }
            return "";
        }

        private int _reportType = 0;
        public int ReportType
        {
            get { return _reportType; }
            set { _reportType = value; }
        }
    }

    [Serializable]
    public class ListItem
    {
        public string f1 { get; set; }
        public string f2 { get; set; }
        public string f3 { get; set; }
        public string f4 { get; set; }
        public string f5 { get; set; }
        public string f6 { get; set; }
        public string f7 { get; set; }
        public string f8 { get; set; }
        public string f9 { get; set; }
        public string f10 { get; set; }
    }

    public class ReportColumn
    {
        public string Name { get; set; }
        public double Width { get; set; }
    }
}
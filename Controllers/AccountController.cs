using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using Aisger.Helpers;
using Aisger.Models.Entity.Dictionary;
using Aisger.Models.Entity.Security;
using Aisger.Models.Entity.Subject;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using DotNetOpenAuth.AspNet;
using log4net;
using Microsoft.Web.WebPages.OAuth;
using NPOI.OpenXml4Net.OPC;
using NPOI.SS.Formula.Functions;
using WebMatrix.WebData;
using Aisger.Filters;
using Aisger.Models;
using RegisterModel = Aisger.Models.RegisterModel;
using KalkanCryptCOMLib;

namespace Aisger.Controllers
{
    public class AccountController : Controller
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AccountController));
        private static readonly bool _IsCheckOCSP=false;
        private KalkanCryptCOM _cryptCom = null;
        public AccountController()
        {
            _cryptCom = new KalkanCryptCOM();
            _cryptCom.Init();
        }
        // GET: /Account/LogOn
        public string GetKatoString(SEC_User guest)
        {
            var builder = new StringBuilder();
            if (guest.DIC_Kato != null)
            {
                builder.Append(guest.DIC_Kato.NameRu);
            }
            if (guest.DIC_Kato1 != null)
            {
                builder.Append(", ").Append(guest.DIC_Kato1.NameRu);
            }
            if (guest.DIC_Kato2 != null)
            {
                builder.Append(", ").Append(guest.DIC_Kato2.NameRu);
            }
            if (guest.DIC_Kato3 != null)
            {
                builder.Append(", ").Append(guest.DIC_Kato3.NameRu);
            }
            return builder.ToString();
        }

        public SEC_Guest SecGuest(SEC_User user, string codeUserKind)
        {
            SEC_Guest model;
            if (user == null)
            {
                model = new SEC_Guest { TypeApplicationId = 1, Pwd = CodeConstManager.DEFAULT_PWD, ConfirmPwd = CodeConstManager.DEFAULT_PWD };
                return model;
            }

            model = new SEC_Guest
            {
                Id = user.Id,
                Address = user.Address,
                Certificate = user.Certificate,
                FactAddress = user.FactAddress,
                BINIIN = user.BINIIN,
                FirstName = user.FirstName,
                Email = user.Email,
                InternalPhone = user.InternalPhone,
                IsCvazy = user.IsCvazy,
                IsHaveGES = user.IsHaveGES,
                JuridicalName = user.JuridicalName,
                LastName = user.LastName,
                Mobile = user.Mobile,
                SecondName = user.SecondName,
                ResponceFIO = user.ResponceFIO,
                ResponcePost = user.ResponcePost,
                Post = user.Post,
                WorkPhone = user.WorkPhone,
                OkedId = user.OkedId,
                Wastes = user.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList(),
                TypeApplicationId = user.TypeApplicationId,
                Kinds = user.SEC_UserKind.Select(aquticOblast => aquticOblast.KindId.ToString()).ToList()
            };

            if (codeUserKind != null)
            {
                var kind = user.SEC_UserKind.FirstOrDefault(e => e.DIC_KindUser.Code == codeUserKind);
                if (kind != null && kind.IsBlocked != null)
                {
                    model.IsBlokced = kind.IsBlocked.Value;
                    if (kind.IsBlocked.Value)
                    {
                        model.ReasonBlocked = kind.ReasonBlocked;
                    }
                }
                else
                {
                    model.IsBlokced = false;
                }
            }
            model.TypeApplicationId = user.TypeApplicationId;

            if (user.Oblast != null)
            {
                model.Oblast = user.Oblast.Value;
            }
            if (user.Region != null)
            {
                model.Region = user.Region.Value;
            }
            model.SubRegion = user.SubRegion;
            model.Village = user.Village;
            model.FactAddress = user.FactAddress;
            model.FactOblast = user.FactOblast;
            model.FactRegion = user.FactRegion;
            model.FactSubRegion = user.FactSubRegion;
            model.FactVillage = user.FactVillage;
            model.JuridicalKato = GetKatoString(user);
            model.FactKato = GetKatoString(user);
            return model;
        }
        public ActionResult LogOn()
        {
            LogOnModel model;
            model = new LogOnModel();
            if (HttpContext.Request.IsAuthenticated)
            {
                CheckRoles();
                var roleId = MyExtensions.GetRolesId();
                if (roleId != null && roleId.Value == 4)
                {
                    return RedirectToAction("Index", "RegisterForm");
                }
                else return RedirectToAction("Index", "Home");
            }
            model.ContentMenu = System.IO.File.ReadAllText(Server.MapPath("~/Template/contentMenu.xml"));

            return View(model);
        }
        //
        // POST: /Account/LogOn

        [HttpPost]
        [GerNavigateLogger]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (new SecUserRepository().CheckAccount(model.UserName, model.Password))
                //                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    _logger.InfoFormat("User {0} logon", model.UserName);
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    HttpContext.Session.Add(CodeConstManager.LOGINWITHOUTECP, true);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }

                    var roleId = MyExtensions.GetRolesId();
                    if (roleId != null && roleId.Value == 4)
                    {
                        return RedirectToAction("Index", "RegisterForm");
                    }
                    else return RedirectToAction("Index", "Home");

                }
                _logger.InfoFormat("User {0} error logon", model.UserName);
                ModelState.AddModelError("", ResourceSetting.AccountController_LogOn_Error);
            }

            // If we got this far, something failed, redisplay form
            return View("Logon", model);
        }

        public ActionResult LogOnCertificate(LogOnModel model, string returnUrl)
        {
            bool isSuccess = false;
            string urlStr = Url.Action("Index", "Home");
            string error = string.Empty;

            _logger.InfoFormat("User {0}. IsValid {1}", model.UserName, ModelState.IsValid);
            if (ModelState.IsValid)
            {
                if (model.UserName != null && model.Certificate != null)
                {
                    bool isAuth = CertificateAuthentication(model, out error);
                    if (isAuth)
                    {
                        _logger.InfoFormat("User {0} logon", model.UserName);
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);


                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            urlStr = returnUrl;
                        }

                        var roleId = MyExtensions.GetRolesId();
                        if (roleId != null && roleId.Value == 4)
                        {
                            urlStr = Url.Action("Index", "RegisterForm");
                            //return RedirectToAction("Index", "RegisterForm");
                        }
                        else urlStr = Url.Action("Index", "Home");
                    }
                    isSuccess = isAuth;
                }
            }


            // If we got this far, something failed, redisplay form
            return Json(new { success = isSuccess, url = urlStr, errorStr = error });
        }

        private bool CertificateAuthentication(LogOnModel model, out string error)
        {
            bool isSucccess = true;
            error = string.Empty;
            try
            {
                var xmlLogin = model.Certificate;
                string certificateBase64;
                _cryptCom.GetCertFromXML(xmlLogin, 0, out certificateBase64);
                #region метод проверка первый руководитель
                //string xmldata = "";
                //_cryptCom.X509CertificateGetInfo(certificateBase64, (int)KalkanCryptNET.KALKANCRYPTCOM_CERTPROPID.KC_CERTPROP_EXT_KEY_USAGE, out xmldata);
                //string xml2 = xmldata;
                //bool isflag = false;
                //if (xml2.Contains("1.2.398.3.3.4.1.2.1"))
                //{
                //    isflag = true;
                //}
                #endregion


                _logger.Debug("Get Certificate : " + certificateBase64);
                if (!string.IsNullOrEmpty(certificateBase64))
                {
                    /*X509Certificate x509Cert = new X509Certificate(Convert.FromBase64String(certificateBase64));                 
                        //,model.Password, X509KeyStorageFlags.PersistKeySet);
                    X509Certificate2 x509Cert2 = new X509Certificate2(x509Cert);                                       
                    bool isVerify = x509Cert2.Verify();
                    */

                    bool isVerify = IsCheckOCSP(certificateBase64, ref error);
                    _logger.Debug("Certificate Verify: " + isVerify);
                    if (isVerify)
                    {
                        // [TODO] Make Didgital dignature Verify
                        // var verifyXml =  VerifyXml(x509Cert2, xmlLogin);

                        var bin = model.UserName;
                        var repository = new SecUserRepository();
                        isSucccess = repository.CheckAccountByName(bin, false);
                    }
                    else
                    {
                        error = "Сертификат отозван. Обратитесь в ЦОН для получения нового сертификата.";
                        isSucccess = false;
                    }
                }
            }
            catch (Exception e)
            {
                error = string.Format("Ошибка аутентификации по сертификату: {0}", e);
                _logger.ErrorFormat(error);
                isSucccess = false;
            }
            return isSucccess;
        }

        public bool IsCheckOCSP(string cert, ref string errorMessage)
        {
            if (_IsCheckOCSP)
            {
                string outInfo = "";
                string inCert = cert;

                string validPath = "";
                uint err;
                string errStr;
                DateTime tmpD = new DateTime();

                int validType = (int)KalkanCryptCOMLib.KALKANCRYPTCOM_VALIDTYPE.KC_USE_OCSP;
                validPath = "http://ocsp.pki.gov.kz/";

                {
                    _cryptCom.X509ValidateCertificate(inCert, validType, validPath, tmpD, out outInfo);
                }

                _cryptCom.GetLastErrorString(out errStr, out err);
                if (err > 0)
                {
                    errorMessage = " Error: 0x" + err.ToString("X8") + Environment.NewLine + errStr.Replace("\n", "\r\n"); ;
                    return false;
                }

                return true;
            }
            else return true;
        }

        public string Base64(string cert)
        {
            string certificateBase64;
            _cryptCom.GetCertFromXML(cert, 0, out certificateBase64);
            return certificateBase64;
        }

        //---- проверка подпис
        private bool VerifyXml(X509Certificate2 cert, string xmlText)
        {
            string text = null;
            byte[] signature = null;
            UnicodeEncoding encoding = new UnicodeEncoding();
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;

            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(xmlText);

            SignedXml signedXml = new SignedXml(Doc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = Doc.GetElementsByTagName("ds:Signature");

            // Load the first <signature> node.  
            signedXml.LoadXml((XmlElement)nodeList[0]);

            var s = signedXml.Signature;

            // Check the signature and return the result.
            return signedXml.CheckSignature(csp);
            return signedXml.CheckSignature(cert, false);









            return true;
            XDocument xmlDoc = XDocument.Parse(xmlText);

            var xmlNodeText = xmlDoc.Elements().Descendants("timeTicket").FirstOrDefault();
            if (xmlNodeText != null)
            {
                text = xmlNodeText.Value;
            }
            XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
            var xmlNodeSign = xmlDoc.Descendants(ds + "SignatureValue").FirstOrDefault();
            if (xmlNodeSign != null)
            {
                var signStr = xmlNodeSign.Value;
                signature = encoding.GetBytes(signStr);
            }
            if (text == null || signature == null)
                return false;

            byte[] data = encoding.GetBytes(text);

            // Verify the signature with the hash
            return csp.VerifyHash(data, CryptoConfig.MapNameToOID("SHA1"), signature) || csp.VerifyHash(data, CryptoConfig.MapNameToOID("MD5"), signature);
        }

        private void CheckRoles()
        {
            string name = HttpContext.User.Identity.Name;
            _logger.InfoFormat("User {0} authenthicated by cookie", name);
            new SecUserRepository().CheckAccountByName(name, true);
        }

        public bool IsVerifyXML(string cert, ref string errorMessage)
        {
            bool result = false;
            string outVerifyInfo = " ";
            uint err;
            string errStr = " ";

            _cryptCom.VerifyXML(" ", 0, cert, out outVerifyInfo);
            _cryptCom.GetLastErrorString(out errStr, out err);
            if (err > 0)
            {
                result = false;
                errorMessage = " Error: 0x" + err.ToString("X8") + Environment.NewLine;
                if (outVerifyInfo != null)
                    errorMessage += outVerifyInfo.Replace("\n", "\r\n");
                errorMessage += Environment.NewLine + errStr.Replace("\n", "\r\n");
            }
            else
            {
                result = true;
                errorMessage = outVerifyInfo.Replace("\n", "\r\n");
                cert = "";
                outVerifyInfo = "";
            }

            return result;
        }

        public string GetInnByXmlCert(string xmlCert)
        {
            string sCert;
            _cryptCom.GetCertFromXML(xmlCert, 0, out sCert);
            var bytes = Convert.FromBase64String(sCert);
            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(bytes);
            string iin = cert.Subject.Substring(cert.Subject.IndexOf("SERIALNUMBER=IIN") + 16, 12);
            return iin;
        }
        //
        // GET: /Account/Login

        /*      [AllowAnonymous]
              public ActionResult Login(string returnUrl)
              {
                  ViewBag.ReturnUrl = returnUrl;
                  return View();
              }*/

        //
        // POST: /Account/Login
        /*
                [HttpPost]
                [AllowAnonymous]
                [ValidateAntiForgeryToken]
                public ActionResult Login(LoginModel model, string returnUrl)
                {
                    if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                    {
                        return RedirectToLocal(returnUrl);
                    }

                    // If we got this far, something failed, redisplay form
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    return View(model);
                }*/

        //
        // POST: /Account/LogOff

        [GerNavigateLogger]
        public ActionResult LogOff()
        {

            string currentUser = User.Identity.Name;
            FormsAuthentication.SignOut();
            _logger.InfoFormat("User {0} logout", currentUser);
            HttpContext.Session.Add(CodeConstManager.LOGINWITHOUTECP, false);
            return Redirect(Url.Content("~/"));
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }
        //
        // POST: /Account/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword iniciará una excepción en lugar de
                // devolver false en determinados escenarios de error.
                AccountRepository repository = new AccountRepository();
                bool changePasswordSucceeded;
                try
                {
                    SEC_User user = repository.GetUserFromPwd(User.Identity.Name, model.OldPassword);
                    if (user == null)
                    {
                        changePasswordSucceeded = false;
                    }
                    else
                    {
                        user.Pwd = model.NewPassword;
                        repository.UpdatePwd(user, MyExtensions.GetCurrentUserId());
                        //                        MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                        //                        changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                        changePasswordSucceeded = true;
                    }

                }
                catch (Exception ex)
                {
                    changePasswordSucceeded = false;
                    ModelState.AddModelError("", ex.Message);
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Неверный пароль или новый пароль неверен");
                }
            }

            return View(model);
        }
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }
        [HttpGet]
        public ActionResult RegistrationGuest(int kindUserId = -1)
        {
            var model = new SEC_Guest { TypeApplicationId = 1 };
            FillBagRegistrationGuest(model, kindUserId);
            return View(model);
        }

        [HttpPost]
        public ActionResult RegistrationGuest(SEC_Guest model)
        {
            FillBagRegistrationGuest(model);
            if (model.Pwd != model.ConfirmPwd)
            {
                model.IsError = true;
                model.ErrorMessage = ResourceSetting.validconfirm;
                return View(model);
            }
            long checkbin;
            if (model.BINIIN != null && (model.BINIIN.Length != 12 || !long.TryParse(model.BINIIN, out checkbin)))
            {
                model.IsError = true;
                model.ErrorMessage = ResourceSetting.binvalid;
//                ModelState.AddModelError("BINIIN", ResourceSetting.binvalid);
                return View(model);
            }

            //----найдены  одинаковое бин
            var users = new SecUserRepository().GetAll().Where(e => e.Login == model.BINIIN).ToList();
            if (users.Count > 1)
            {
                model.IsError = true;
                model.ErrorMessage = ResourceSetting.SameBin;
                return Json(model);
            }

            var user = (users.Count > 0) ? users[0] : null;
            if (user != null)
            {
                model.IsError = true;
                model.ErrorMessage = ResourceSetting.eBinAlreadyRegister;
                return View(model);
            }
            if (model.KindId == null || model.KindId == 0)
            {
                ModelState.AddModelError("KindId", ResourceSetting.NotEmpty);
            }
            if (ModelState.IsValid)
            {

                if (model.Village == 0)
                {
                    model.Village = null;
                }
                if (model.SubRegion == 0)
                {
                    model.SubRegion = null;
                }
                if (model.FactOblast == 0)
                {
                    model.FactOblast = null;
                }
                if (model.FactRegion == 0)
                {
                    model.FactRegion = null;
                }
                if (model.FactSubRegion == 0)
                {
                    model.FactSubRegion = null;
                }
                if (model.FactVillage == 0)
                {
                    model.FactVillage = null;
                }

                var repository = new SecUserRepository();
                model.Kinds = new List<string> {model.KindId.ToString()};
                repository.RegisteredUser(model, MyExtensions.GetCurrentUserId());
                return Redirect(Url.Content("~/"));
            }
            else
            {
                model.IsError = true;
                model.ErrorMessage = ResourceSetting.eRequireFillAllFields;
            }
            return View(model);
        }

        #region Развитие ГЧП в энергосбережении РК Подать заявку
        public System.Web.Mvc.JsonResult CheckBin(string bin)
        {
            Dictionary<string, object> item = new Dictionary<string, object>();
            long checkbin;
            if (bin != null && (bin.Length != 12 || !long.TryParse(bin, out checkbin)))
            {
                item["IsResult"] = -1;
                item["Message"] = ResourceSetting.binvalid;
                return Json(item);
            }

            //----найдены  одинаковое бин
            var users = new SecUserRepository().GetAll().Where(e => e.Login == bin).ToList();
            if (users.Count > 1)
            {
                item["IsResult"] = -2;
                item["Message"] = ResourceSetting.SameBin;
                string str = "<br>";
                int c = 0;
                foreach (var i in users)
                {
                    str += i.JuridicalName + "(" + i.IDK + ")";
                    if (c != users.Count - 1)
                        str += "<br>";

                    c++;
                }
                item["Users"] = str + "<br>Обратитесь к администратору системы.";

                return Json(item);
            }

            var user = (users.Count > 0) ? users[0] : null;
            if (user != null)
            {
                var kindUser = new SecUserRepository().CheckKindUser(user.Id, 5).SingleOrDefault();
                if (kindUser != null)
                {
                    item["IsResult"] = 2;
                    item["Message"] = ResourceSetting.EE2SearchInfo1; //"Заявитель уже зарегистрирован в Системе. Если забыли пароль - обратитесь к администратору системы.";
                    item["Users"] = "<br>" + user.JuridicalName;
                   
                }
                else
                {
                    item["IsResult"] = 3;
                    item["Message"] = ResourceSetting.EE2SearchInfo2;
                    item["Users"] = "<br>" + user.JuridicalName;
                    item["Login"] = user.Login;
                    item["Id"] = user.Id;
                }
            }
            else
            {
                item["IsResult"] = 1;
            }

            return Json(item);
        }
        #endregion

        public MultiSelectList GetPlants(IList<string> selectedValues)
        {
            var plants = new DicOkedRepository().GetList();
            return new MultiSelectList(plants, "Id", "FullName", selectedValues);
        }
        public MultiSelectList GetKinds(IList<string> selectedValues, int kindUserId = -1)
        {
            var list = new List<long>();
//            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);
            var plants = new DicKindUserRepository().GetList().Where(e => list.Contains(e.Id));

            if (kindUserId != -1)
            {
                List<string> bufferList = new List<string> { Convert.ToString(kindUserId) };
                selectedValues = bufferList;
            }

            return new MultiSelectList(plants, "Id", "NameRu", selectedValues);
        }
        public void FillBagRegistrationGuest(SEC_Guest model, int kindUserId=-1)
        {
          
            model.WastList = GetPlants(model.Wastes);
            model.KindList = GetKinds(model.Kinds,kindUserId);
            ViewData["TypeApplicationList"] = new SelectList(new DicTypeApplicationRepository().GetAll(), "Id",
                "ShortName" + CultureHelper.GetCurrentCulture(), model.TypeApplicationId);
            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.OkedId);
           var repository = new KatoRepository();
           var listanimal = repository.GetKatos(1, true);
           ViewData["OblastList"] = new SelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), model.Oblast);

            ViewData["RegionList"] = new SelectList(repository.GetKatos(model.Oblast, true), "Id",
                CultureHelper.GetDictionaryName("NameRu"), model.Region);

            ViewData["SubRegionList"] = new SelectList(repository.GetKatos(model.Region, false), "Id",
                CultureHelper.GetDictionaryName("NameRu"), model.SubRegion);

            ViewData["VillageList"] = new SelectList(repository.GetKatos(model.SubRegion, false), "Id",
                CultureHelper.GetDictionaryName("NameRu"), model.Village);
        }
        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }
        public ActionResult AccountSetting()
        {
            var model = new AccountRepository().GetUserById(MyExtensions.GetCurrentUserId());
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetKatos(long parentId, bool mandatory)
        {
            var founder = new KatoRepository().GetKatos(parentId, mandatory).Select(q => new
            {
                q.Id,
                q.NameRu
            });
            return Json(founder.ToArray(), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult ShowKatoDialog(long? oblast, long? region, long? subregion, long? village)
        {
            var model = new AddressEntity();
            model.Oblast = oblast;
            model.Region = region;
            model.SubRegion = subregion;
            model.Village = village;
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            ViewData["OblastList"] = new SelectList(listanimal, "Id",
                                                 "NameRu", model.Oblast);

            ViewData["RegionList"] = new SelectList(repository.GetKatos(model.Oblast, true), "Id",
                                                   "NameRu", model.Region);

            ViewData["SubRegionList"] = new SelectList(repository.GetKatos(model.Region, false), "Id",
                                                   "NameRu", model.SubRegion);

            ViewData["VillageList"] = new SelectList(repository.GetKatos(model.SubRegion, false), "Id",
                                                   "NameRu", model.Village);
            if (Request.IsAjaxRequest()) 
            {
                return PartialView(model);
            }

            return View(model);
        }

        public ActionResult ChangeCulture(string lang)
        {
            string returnUrl = Request.UrlReferrer.AbsolutePath;
            // Список культур
            List<string> cultures = CultureHelper.Cultures;
            if (!cultures.Contains(lang))
            {
                lang = CultureHelper.Ru;
            }

            Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);

            // Сохраняем выбранную культуру в куки
            HttpCookie cookie = Request.Cookies[CultureHelper.CookiesField];
            if (cookie != null)
                cookie.Value = lang;   // если куки уже установлено, то обновляем значение
            else
            {
                cookie = new HttpCookie(CultureHelper.CookiesField);
                cookie.HttpOnly = false;
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddYears(1);
                cookie.Shareable = true;
            }
            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public ActionResult AdminContactInfo()
        {
            return View();
        }
    }
}
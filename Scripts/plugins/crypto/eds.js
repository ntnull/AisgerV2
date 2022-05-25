var _storageAlias = "PKCS12";
String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
};


function chooseStoragePath() {
    var storageAlias = _storageAlias;
    var storagePath = $("#hfStoragePath").val();
    if (storageAlias !== "NONE") {
        browseKeyStore(storageAlias, "P12", storagePath, "chooseStoragePathBack");
    }
}

function chooseStoragePathBack(rw) {
    var storagePath = $("#hfStoragePath").val();

    if (rw.getErrorCode() === "NONE" && rw.result) {
        $("#dlgPasswordModal").modal();
        storagePath = rw.getResult();
        if (storagePath) {
            $("#hfStoragePath").val(storagePath);
        }
        else {
            $("#hfStoragePath").val("");
        }
    } else {
        $("#hfStoragePath").val("");
    }
}

/**
* Дата выпуска эцп
*/
function getNotBeforeBack(result) {
    if (result['errorCode'] === "NONE") {
        $("#signDateFrom").text(result.result.split(' ')[0]);
        getNotAfterCall();
    }
    else {
        if (result['errorCode'] === "WRONG_PASSWORD" && result['result'] > -1) {
            alert("Неправильный пароль! Количество оставшихся попыток: " + result['result']);
        } else if (result['errorCode'] === "WRONG_PASSWORD") {
            alert("Неправильный пароль!");
        } else {
            alert("er1"+result['errorCode']);
        }
    }
}

function getNotBeforeCall() {
    var storageAlias = _storageAlias;
    var storagePath = $("#hfStoragePath").val();
    var password = $("#passwordCert").val();
    var alias = $("#hfKeyAlias").val();
    if (storagePath !== null && storagePath !== "" && storageAlias !== null && storageAlias !== "") {
        if (password !== null && password !== "") {
            if (alias !== null && alias !== "") {
                getNotBefore(storageAlias, storagePath, alias, password, "getNotBeforeBack");
            }
            else {
                alert("Вы не выбран ключ!");
            }
        } else {
            alert("Введите пароль к хранилищу");
        }
    } else {
        alert("Не выбран хранилище!");
    }
}

/**
* Дата окончания эцп
*/
function getNotAfterBack(result) {
    
    if (result['errorCode'] === "NONE") {
        $("#signDateTo").text(result.result.split(' ')[0]);
        // for sign
        if (_doSign)
            _doSign();
    } else {
        if (result['errorCode'] === "WRONG_PASSWORD" && result['result'] > -1) {
            alert("Неправильный пароль! Количество оставшихся попыток: " + result['result']);
        } else if (result['errorCode'] === "WRONG_PASSWORD") {
            alert("Неправильный пароль!");
        } else {
            alert("er2"+result['errorCode']);
        }
    }
}

function getNotAfterCall() {
    var storageAlias = _storageAlias;
    var storagePath = $("#hfStoragePath").val();
    var password = $("#passwordCert").val();
    var alias = $("#hfKeyAlias").val();
    if (storagePath !== null && storagePath !== "" && storageAlias !== null && storageAlias !== "") {
        if (password !== null && password !== "") {
            if (alias !== null && alias !== "") {
                getNotAfter(storageAlias, storagePath, alias, password, "getNotAfterBack");
            } else {
                alert("Вы не выбрали ключ!");
            }
        } else {
            alert("Введите пароль к хранилищу");
        }
    } else {
        alert("Не выбран хранилище!");
    }
}


/**
 * @param  {object}
 * @param  {String}
 * @return {String|null}
 */
function findSubjectAttr(attrs, attr) {
    var tmp;
    var numb;

    for (numb = 0; numb < attrs.length; numb++) {
        tmp = attrs[numb].replace(/^\s\s*/, '').replace(/\s\s*$/, '');
        if (tmp.indexOf(attr + '=') === 0) {
            return tmp.substr(attr.length + 1);
        }
    }

    return null;
}

/**
 * Добавляем в html личные данные пользователя
 * @param  {Object}
 */
function fillPersonData(data) {
    var subjectDN = data.result;
    var subjectAttrs = subjectDN.split(',');
    var iin = findSubjectAttr(subjectAttrs, 'SERIALNUMBER').substr(3);
    var email = findSubjectAttr(subjectAttrs, 'E');
    var cn = findSubjectAttr(subjectAttrs, 'CN');
    cn = cn || '';
    var middleName = findSubjectAttr(subjectAttrs, 'G');
    middleName = middleName || '';
    var fullName = cn.concat(" ").concat(middleName);

    if (document.getElementById('signIIN')) {
        document.getElementById('signIIN').innerHTML = iin;
        document.getElementById('signIIN').value = iin;
    }
    if (document.getElementById('signEmail')) {
        document.getElementById('signEmail').innerHTML = email;
        document.getElementById('signEmail').value = email;
    }
    if (document.getElementById('signFIO')) {
        document.getElementById('signFIO').innerHTML = fullName;
        document.getElementById('signFIO').value = fullName;
    }
}

/**
 * Дополнительные поля для организаций - БИН и наименование
 * @param  {Object}
 */
function fillOrgData(data) {
    var subjectAttrs = data.result.split(',');
    var bin = findSubjectAttr(subjectAttrs, 'OU');
    var organizationName = findSubjectAttr(subjectAttrs, 'O');

    if (bin !== null) {
        if (bin.length > 3) {
            bin = bin.substr(3);
        }

        //document.getElementById('signBIN').style.display = '';
        //document.getElementById('signCompanyName').style.display = '';
        if (document.getElementById('signBIN')) {
            document.getElementById('signBIN').innerHTML = bin;
            document.getElementById('signBIN').value = bin;
        }
        if (document.getElementById('UserName')) {
            document.getElementById('UserName').value = bin;
        }
        if (document.getElementById('signCompanyName')) {
            document.getElementById('signCompanyName').innerHTML = organizationName.replaceAll('\\\"', '\"');
            document.getElementById('signCompanyName').value = organizationName.replaceAll('\\\"', '\"');
        }
    } else {
        //document.getElementById('signBIN').style.display = 'none';
        //document.getElementById('signCompanyName').style.display = 'none';
    }
}

function getSubDNCallBack(result) {

    if (result.errorCode === 'NONE') {
        fillPersonData(result);
        fillOrgData(result);
        //document.getElementById('personInfo').style.display = '';
        //document.getElementById('signBtnsPnl').style.display = '';
        //document.getElementById('signSelCertBtnsPnl').style.display = 'none';

        getNotBeforeCall();
    } else {
        //openNcaLayerError();
    }
}

function getSubDNCall() {
    var storageAlias = _storageAlias;
    var storagePath = $("#hfStoragePath").val();
    var password = $('#passwordCert').val();
    var alias = $("#hfKeyAlias").val();
    if (storagePath !== null && storagePath !== "" && storageAlias !== null && storageAlias !== "") {
        if (password !== null && password !== "") {
            if (alias !== null && alias !== "") {
                getSubjectDN(storageAlias, storagePath, alias, password, "getSubDNCallBack");
            } else {
                alert("Вы не выбрали ключ!");
            }
        } else {
            alert("Введите пароль к хранилищу");
        }
    } else {
        alert("Не выбран хранилище!");
    }
}

/**
 * @param  {[type]}
 * @param  {[type]}
 * @return {Boolean}
 */
var _submitCallback = null;

function signXmlBack(result) { // eslint-disable-line
    var signedData;
    if (result.errorCode === 'NONE') {
        signedData = result.getResult();
        $('#Certificate').val(signedData);
        // webSocket.close();
        
        if (_submitCallback)
            _submitCallback();
    }
    else
        if (result.errorCode === 'WRONG_PASSWORD' && result.result > -1) {
        } else if (result.errorCode === 'WRONG_PASSWORD') {
        } else {
            $('#Certificate').val('');
        }
}

/**
 * Вызываем подписку через прослойку
 *
 * @return {callback}
 *///function signXmlCall(submitCallback) {
//    _submitCallback = submitCallback;
//    var data = $('#hfXmlToSign').val();
//    var storageAlias = _storageAlias;
//    var storagePath = $('#hfStoragePath').val();
//    var password = $('#passwordCert').val();
//    var alias = $('#hfKeyAlias').val();
//    var args = [];
//    var numb;
//
//    if (storagePath && storageAlias && password && alias && data) {
//        args = [storageAlias, storagePath, alias, password, data];
//        // getData('signXml', args, callbackM);
//        signXml(storageAlias, storagePath, alias, password, data, "signXmlBack");
//
//    } else {
//        // openNcaLayerError();
//    }
//}

function signXmlCall(submitCallback, xmlValue) {
    _submitCallback = submitCallback;
    var data = xmlValue;
    var storageAlias = _storageAlias;
    var storagePath = $('#hfStoragePath').val();
    var password = $('#passwordCert').val();
    var alias = $('#hfKeyAlias').val();
    var args = [];
    var numb;
    var isSuccess = true;
    if (storagePath && storageAlias && password && alias && data) {
        args = [storageAlias, storagePath, alias, password, data];
        // getData('signXml', args, callbackM);
        signXml(storageAlias, storagePath, alias, password, data, "signXmlBack");
    } else {
        alert("Certificate Error. Trye again");
        isSuccess = false;
    }
    return isSuccess;
}

function fillKeysBack(result) {
    
    if (result['errorCode'] === "NONE") {
        var list = result['result'];
        var slotListArr = list.split("\n");
        for (var i = 0; i < slotListArr.length; i++) {
            if (slotListArr[i] === null || slotListArr[i] === "") {
                continue;
            }
            var str = slotListArr[i];
            var alias = str.split("|")[3];
            $("#hfKeyAlias").val(alias);
            getSubDNCall();
            break;
        }
    }
    else {
        if (result['errorCode'] === "WRONG_PASSWORD" && result['result'] > -1) {
            alert("Неправильный пароль! Количество оставшихся попыток: " + result['result']);
        } else if (result['errorCode'] === "WRONG_PASSWORD") {
            alert("Неправильный пароль!");
        } else {
            alert("Не верный тип сертификата. Выберите сертификат с префиксом \"AUTH...\".");
        }
    }
}

function fillKeys() {
    var storageAlias = _storageAlias;
    var storagePath = $("#hfStoragePath").val();
    var password = $('#passwordCert').val();
    //var password = passwordP;
    // AUTH 
    // SIGN
    // ALL
    var keyType = "AUTH";
    
    if (storagePath  && storageAlias) {
        if (password) {
            getKeys(storageAlias, storagePath, password, keyType, "fillKeysBack");
        } else {
            alert("Введите пароль к хранилищу");
        }
    } else {
        alert("Не выбран хранилище!");
    }
}

var _doSign = null;
function fillKeySign(doSign) {
    _doSign = doSign;
    var storageAlias = _storageAlias;
    var storagePath = $("#hfStoragePath").val();
    var password = $('#passwordCert').val();
    //var password = passwordP;
    // AUTH 
    // SIGN
    // ALL
    var keyType = "SIGN";

    if (storagePath && storageAlias) {
        if (password) {
            getKeys(storageAlias, storagePath, password, keyType, "fillKeysSignBack");
        } else {
            alert("Введите пароль к хранилищу");
        }
    } else {
        alert("Не выбран хранилище!");
    }
}

function fillKeysSignBack(result) {
    
    if (result['errorCode'] === "NONE") {
        var list = result['result'];
        var slotListArr = list.split("\n");
        for (var i = 0; i < slotListArr.length; i++) {
            if (slotListArr[i] === null || slotListArr[i] === "") {
                continue;
            }
            var str = slotListArr[i];
            var alias = str.split("|")[3];
            $("#hfKeyAlias").val(alias);
            //fillPersonData(result);
            getSubDNCall();
            
            break;
        }
    }
    else {
        if (result['errorCode'] === "WRONG_PASSWORD" && result['result'] > -1) {
            alert("Неправильный пароль! Количество оставшихся попыток: " + result['result']);
        } else if (result['errorCode'] === "WRONG_PASSWORD") {
            alert("Неправильный пароль!");
        } else {
            alert("Не верный тип сертификата. Выберите сертификат с префиксом \"GOST...\".");
        }
    }
}
﻿$(document).ready(function () {
    //douHelper.getField(douoptions.fields, 'Detail1').formatter =
    //    douHelper.getField(douoptions.fields, 'Detail2').formatter = function (v) { return v ? JSON.stringify(v) : '-'; }

    //douHelper.getField(douoptions.fields, 'Da4s').formatter = function (v) { return v ? JSON.stringify(v) : '-'; }

    var $_d1EditDataContainer = undefined;  //Deatil1編輯的容器
    //var $_d2EditDataContainer = undefined;  //Deatil2編輯的容器
    var d1options = undefined;  //Detail1 dou options
    //var d2options = undefined;  //Detail2 dou options

    //Master編輯容器加入Detail
    douoptions.afterCreateEditDataForm = function ($container, row) {
        var $_oform = $container.find(".data-edit-form-group");
        $_d1EditDataContainer = $('<div>').appendTo($_oform.parent());
    //    $_d2EditDataContainer = $('<div>').appendTo($_oform.parent());

    //    //取Detail1 dou option 並產編輯Dom
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa4/GetDataManagerOptionsJson', function (_opt) { //取model option
            ////d1options = _opt;
            ////var da4s = row.Da4s;
            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);
            //////產欄位Dom
            ////$.each(_opt.fields, function () {
            ////    if (this.visibleEdit == false)
            ////        return;
            ////    douHelper.createDataEditContent($_d1EditDataContainer, this, da4s, 3);
            ////})
        });
    //    //取Detail2 dou option 並產編輯Dom
    //    $.getJSON($.AppConfigOptions.baseurl + 'T1_1_Detail2/GetDataManagerOptionsJson', function (_opt) { //取model option
    //        d2options = _opt;
    //        var detail2 = row.Detail2;
    //        //初始options預設值
    //        douHelper.setFieldsDefaultAttribute(_opt.fields);
    //        //產欄位Dom
    //        $.each(_opt.fields, function () {
    //            if (this.visibleEdit == false)
    //                return;
    //            douHelper.createDataEditContent($_d2EditDataContainer, this, detail2, 3);
    //        })
    //    });
        //產tab
        //
        helper.bootstrap.genBootstrapTabpanel($_d1EditDataContainer.parent(), undefined, undefined, ['員工資料', '學歷'], [$_oform, $_d1EditDataContainer]);
        //helper.bootstrap.genBootstrapTabpanel($_d1EditDataContainer.parent(), undefined, undefined, ['Master', 'Detail1', 'Detail2'], [$_oform, $_d1EditDataContainer, $_d2EditDataContainer]);
    }
    //douoptions.afterEditDataConfirm = function (row, callback) {
    //    row.Detail1 = row.Detail1 || {};
    //    row.Detail2 = row.Detail2 || {};

    //    var err = [];
    //    //給detail1值
    //    $.each(d1options.fields, function () {
    //        if (this.visibleEdit === false)
    //            return;
    //        if (this.editFormtter) {
    //            row.Detail1[this.field] = douHelper.getDataEditContentValue($_d1EditDataContainer, this);
    //        }
    //        if (this.validate && this.validate(row.Detail1[this.field], row.Detail1) !== true) //驗證資料
    //            err.push(this.title + ":" + this.validate(row.Detail1[this.field], row.Detail1));
    //    })
    //    //給detail2值
    //    $.each(d2options.fields, function () {
    //        if (this.visibleEdit === false)
    //            return;
    //        if (this.editFormtter) {
    //            row.Detail2[this.field] = douHelper.getDataEditContentValue($_d2EditDataContainer, this);
    //        }
    //        if (this.validate && this.validate(row.Detail2[this.field], row.Detail2) !== true) //驗證資料
    //            err.push(this.title + ":" + this.validate(row.Detail2[this.field], row.Detail2));
    //    })
    //    callback(err);
    //}

    $("#_table").DouEditableTable(douoptions); //初始dou table
});
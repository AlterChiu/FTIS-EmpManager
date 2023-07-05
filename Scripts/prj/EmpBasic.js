$(document).ready(function () {
    //douHelper.getField(douoptions.fields, 'Da4s').formatter =
    //    douHelper.getField(douoptions.fields, 'Da1s').formatter = function (v) { return v ? JSON.stringify(v) : '-'; }

    //douHelper.getField(douoptions.fields, 'Da4s').formatter = function (v) { return v ? JSON.stringify(v) : '-'; }

    var $_d1EditDataContainer = undefined;  //Da1s編輯的容器
    var $_d4EditDataContainer = undefined;  //Da4s編輯的容器

    var d1options = undefined;  //Da1s dou options
    var d4options = undefined;  //Da4s dou options
    
    //Master編輯容器加入Detail
    douoptions.afterCreateEditDataForm = function ($container, row) {
        var oFno = row.Fno;

        var $_oform = $container.find(".data-edit-form-group");
        $_d1EditDataContainer = $('<div>').appendTo($_oform.parent());
        $_d4EditDataContainer = $('<table>').appendTo($_oform.parent());
        
        //因為F22cmmEmpData(主表)和F22cmmEmpDa1(子表)沒有建關聯，(Model：不可用set，否則找不到欄位)，因此不能用下方寫法
        //取Da1s dou option 並產編輯Dom
        ////$.getJSON($.AppConfigOptions.baseurl + 'EmpDa1/GetDataManagerOptionsJson', function (_opt) { //取model option
        ////    d1options = _opt;
        ////    var Da1s = row.Da1s;
        ////    //初始options預設值
        ////    douHelper.setFieldsDefaultAttribute(_opt.fields);
        ////    //產欄位Dom
        ////    $.each(_opt.fields, function () {
        ////        if (this.visibleEdit == false)
        ////            return;
        ////        douHelper.createDataEditContent($_d1EditDataContainer, this, Da1s, 3);
        ////    })
        ////});

        //取Da1s dou option 並產編輯Dom
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa1/GetDataManagerOptionsJson', function (_opt) { //取model option

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;
            _opt.datas = [row.Da1s];
            _opt.singleDataEdit = true;
            _opt.editformWindowStyle = $.editformWindowStyle.showEditformOnly;

            //初始options預設值
            douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.afterUpdateServerData = _opt.afterAddServerData = function (row, callback) {
                //錨點
                $('html,body').animate({ scrollTop: $container.offset().top }, "show");
                //return false;
                jspAlertMsg($("body"), { autoclose: 2000, content: '通訊方式更新完成!!', classes: 'modal-sm' },
                    function () {
                        $('html,body').animate({ scrollTop: $container.find('a[data-toggle="tab"]').offset().top }, "show");
                    });

                ////callback();
            }

            //實體Dou js                                
            var $_d1Table = $_d1EditDataContainer.douTable(_opt);
        });

        //取Da4s dou option 並產編輯Dom
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa4/GetDataManagerOptionsJson', function (_opt) { //取model option

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;
            _opt.datas = row.Da4s;            

            //初始options預設值
            douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.beforeCreateEditDataForm = function (row, callback) {
                var isAdd = JSON.stringify(row) == '{}';
                if (isAdd) {                    
                    row.Fno = oFno;                    
                }

                callback();
            };

            //實體Dou js                                
            var $_d4Table = $_d4EditDataContainer.douTable(_opt);
        });
    
        //產tab                
        helper.bootstrap.genBootstrapTabpanel($_d4EditDataContainer.parent(), undefined, undefined, ['員工資料', '通訊方式', '學歷'], [$_oform, $_d1EditDataContainer, $_d4EditDataContainer]);

        ////$container.find('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        ////    //1-1 Tab切換需要儲存異動資料(執行確定功能)
        ////    var actTab = $(e.target).html();
        ////    if (actTab == "員工資料" || actTab == "通訊方式") {
        ////        $('.modal-footer').find('.btn-primary').trigger("click");
        ////    }
        ////});
    }

    //////douoptions.afterUpdateServerData = function (row, callback) {
    //////}

    //douoptions.afterEditDataConfirm = function (row, callback) {
    //    row.Da4s = row.Da4s || {};
    //    row.Da1s = row.Da1s || {};

    //    var err = [];
    //    //給Da4s值
    //    $.each(d4options.fields, function () {
    //        if (this.visibleEdit === false)
    //            return;
    //        if (this.editFormtter) {
    //            row.Da4s[this.field] = douHelper.getDataEditContentValue($_d4EditDataContainer, this);
    //        }
    //        if (this.validate && this.validate(row.Da4s[this.field], row.Da4s) !== true) //驗證資料
    //            err.push(this.title + ":" + this.validate(row.Da4s[this.field], row.Da4s));
    //    })
    //    //給Da1s值
    //    $.each(d1options.fields, function () {
    //        if (this.visibleEdit === false)
    //            return;
    //        if (this.editFormtter) {
    //            row.Da1s[this.field] = douHelper.getDataEditContentValue($_d1EditDataContainer, this);
    //        }
    //        if (this.validate && this.validate(row.Da1s[this.field], row.Da1s) !== true) //驗證資料
    //            err.push(this.title + ":" + this.validate(row.Da1s[this.field], row.Da1s));
    //    })
    //    callback(err);
    //}

    var $_masterTable = $("#_table").DouEditableTable(douoptions); //初始dou table
});
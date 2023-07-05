$(document).ready(function () {

    var $_d1EditDataContainer = undefined;  //Da1s編輯的容器
    var $_d4EditDataContainer = undefined;  //Da4s編輯的容器

    var $_oform = $("#_tabs");
    $_d1EditDataContainer = $('<div>').appendTo($_oform.parent());
    $_d4EditDataContainer = $('<table>').appendTo($_oform.parent());

    var oFno;   //主表員編
    var oRow;   //主表Row

    //Master(EmpData) 員工資料
    douoptions.afterCreateEditDataForm = function ($container, row) {
        oRow = row;
        oFno = row.Fno;

        //因為F22cmmEmpData(主表)和F22cmmEmpDa1(子表)沒有建關聯，(Model：不可用set，否則找不到欄位)，因此不能用下方寫法
        //取Da1s dou option 並產編輯Dom
        ////$.getJSON($.AppConfigOptions.baseurl + 'EmpDa1/GetDataManagerOptionsJson', function (_opt) { //取model option       
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
    }
    var $_masterTable = $("#_table").DouEditableTable(douoptions); //初始dou table

    //1-1 Detail(EmpDa1) 通訊方式
    $.getJSON($.AppConfigOptions.baseurl + 'EmpDa1/GetDataManagerOptionsJson', function (_opt) { //取model option

        //取消自動抓後端資料
        _opt.tableOptions.url = undefined;
        _opt.datas = [oRow.Da1s];
        _opt.singleDataEdit = true;
        _opt.editformWindowStyle = $.editformWindowStyle.showEditformOnly;

        //初始options預設值
        douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

        _opt.afterUpdateServerData = _opt.afterAddServerData = function (row, callback) {
            //錨點
            $('html,body').animate({ scrollTop: $_d1EditDataContainer.offset().top }, "show");
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

    //1-n Detail(EmpDa4) 學歷
    $.getJSON($.AppConfigOptions.baseurl + 'EmpDa4/GetDataManagerOptionsJson', function (_opt) { //取model option

        //取消自動抓後端資料
        _opt.tableOptions.url = undefined;
        _opt.datas = oRow.Da4s;

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

});
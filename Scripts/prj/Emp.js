$(document).ready(function () {

    douoptions.title = '員工資料';

    var $_nowTabUI = undefined;    //當下Tab 使用的UI;
    var $_nowTable = undefined;    //當下Tab 使用的Dou實體;

    //Master(EmpData) 員工資料
    douoptions.afterCreateEditDataForm = function ($container, row) {

        var isAdd = JSON.stringify(row) == '{}';

        var $_oform = $("#_tabs");
        var $_d1EditDataContainer = $('<div>').appendTo($_oform.parent());      //Da1s編輯的容器
        var $_d4EditDataContainer = $('<table>').appendTo($_oform.parent());    //Da4s編輯的容器

        var $_d1Table = undefined;  //Da1s Dou實體
        var $_d4Table = undefined;  //Da4s Dou實體

        var isChange = false;
        var isChangeText = [];
        var oRow = row;         //主表員編
        var oFno = row.Fno;     //主表Row

        //保留確定按鈕
        $container.find('.modal-footer button').hide();
        $container.find('.modal-footer').find('.btn-primary').show();

        //1-1 Detail(EmpDa1) 通訊方式
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa1/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '通訊方式';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            oRow.Da1s = oRow.Da1s ? [oRow.Da1s] : [{}];
            _opt.datas = oRow.Da1s;

            _opt.singleDataEdit = true;
            _opt.editformWindowStyle = $.editformWindowStyle.showEditformOnly;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.afterCreateEditDataForm = function ($container, row) {
                //保留確定按鈕
                $container.find('.modal-footer button').hide();
                $container.find('.modal-footer').find('.btn-primary').show();
            }

            _opt.afterUpdateServerData = _opt.afterAddServerData = function (row, callback) {
                jspAlertMsg($("body"), { autoclose: 2000, content: '通訊方式更新成功!!', classes: 'modal-sm' },
                    function () {
                        $('html,body').animate({ scrollTop: $_d1Table.offset().top }, "show");
                    });

                //(no callback)更新dou的rowdata
                $_d1Table.instance.updateDatas(row);

                ////callback();
            }

            //實體Dou js                                
            $_d1Table = $_d1EditDataContainer.douTable(_opt);
        });

        //1-n Detail(EmpDa4) 學歷
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa4/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '學歷';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;
            oRow.Da4s = oRow.Da4s || [];
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
            $_d4Table = $_d4EditDataContainer.douTable(_opt);
        });

        //產tab
        helper.bootstrap.genBootstrapTabpanel($_d4EditDataContainer.parent(), undefined, undefined, ['員工資料', '通訊方式', '學歷'], [$_oform, $_d1EditDataContainer, $_d4EditDataContainer]);

        //預設的tab;        
        $_nowTabUI = $('#_tabs').closest('div[class=tab-content]').find('.show');

        //點選的Tab
        var jTabToggle = $('#_tabs').closest('div[class=tab-content]').siblings().find('a[data-toggle="tab"]');

        if (isAdd) {
            //tablist隱藏
            $('#_tabs').closest('div[class=tab-content]').siblings().hide();
        }

        //before tab-click
        jTabToggle.on('show.bs.tab', function (e) {
            isChange = false;
            isChangeText = [];

            //1-1 Tab切換需要儲存異動資料(trigger確定功能)
            //判斷(上一個Tab非Null且有資料)
            if ($_nowTable != null
                && $_nowTable.instance.settings.datas.find(obj => obj.Fno == oFno) != null) {
                //隱藏Bootstrap Table(多筆)找使用者挑選的Index
                var n = -1;
                $('.bootstrap-table #_table').find('.dou-field-Fno').each(function (index) {
                    if ($(this).text() == oFno) {
                        n = index;
                        return false;
                    }
                });

                //隱藏Bootstrap Table(多筆)找Index表
                var jBootstrapTable = $($('.bootstrap-table #_table').find('.dou-field-Fno')[n]).closest("tr");

                //驗證資料異動
                $_nowTabUI.find('.field-content [data-fn]').each(function (index) {
                    //欄位名稱
                    var fn = $(this).attr('data-fn');
                    var datatype = douHelper.getField($_nowTable.instance.settings.fields, fn).datatype;

                    //輸入值(UI)
                    var uiValue = "";
                    if (datatype == 'datetime' || datatype == 'date') {
                        uiValue = $(this).find('input').val();
                    }
                    else {
                        uiValue = $(this).val();
                    }

                    //輸入值(Bootstrap Table + dou實體)
                    var conValue = '';
                    if (datatype == 'textlist') {
                        //輸入提示字：UI(人名)，Bootstrap Table(人名)，X容器(員工編號)
                        conValue = jBootstrapTable.find('.dou-field-' + fn).text();
                    }
                    else {
                        //var conValue = $_nowTable.instance.settings.datas[0][fn];
                        conValue = $_nowTable.instance.settings.datas.find(obj => obj.Fno == oFno)[fn];
                    }
                    if (conValue != null) {

                        //輸入提示字：-(沒值)
                        if (uiValue == "" && conValue == "-")
                            return;

                        if (uiValue != "" && conValue != "") {
                            //日期格式比對(ui(1982-12-17), con(1982-12-17T00:00:00) => 取小統一長度)
                            if (datatype == 'datetime' || datatype == 'date') {
                                var minLength = Math.min(uiValue.length, conValue.length);
                                uiValue = uiValue.substring(0, minLength)

                                //容器(時間可能是物件) "/Date(1224043200000)/"
                                conValue = JsonDateStr2Datetime(conValue);
                                conValue = conValue.DateFormat("yyyy-MM-dd HH:mm:ss").substring(0, minLength);
                            }
                        }

                        if (uiValue != conValue) {
                            isChange = true;
                            isChangeText.push(douHelper.getField($_nowTable.instance.settings.fields, fn).title);
                            //return false;
                        }
                    }
                    else {
                        //(異動說明)DB為Null($_nowTable無欄位資料),但UI有值
                        if (uiValue != "") {
                            isChange = true;
                            isChangeText.push(douHelper.getField($_nowTable.instance.settings.fields, fn).title);
                            //return false;
                        }
                    }

                });
            }
        });

        //after tab-click
        jTabToggle.on('shown.bs.tab', function (e) {
            var nextTable = null;
            var nextTabUI = null;

            //當下切換的tab(1-1需要記錄)
            var actTab = $(e.target).html();
            if (actTab == $_masterTable.instance.settings.title) {
                nextTable = $_masterTable;
                nextTabUI = $('#_tabs').closest('div[class=tab-content]').find('.show');
            }
            else if (actTab == $_d1Table.instance.settings.title) {
                nextTable = $_d1Table;
                nextTabUI = $('#_tabs').closest('div[class=tab-content]').find('.show');
            }

            if (isChange && $_nowTable != null) {
                //異動訊息
                var content = '資料異動(' + $_nowTable.instance.settings.title + ')項目：' + '</br>'
                    + isChangeText.join(', ') + '</br>'
                    + "是否儲存";

                var isDoing = false;//confirm挑選取消(重複執行，不知原因)
                jspConfirmYesNo(nextTabUI, { content: content }, function (confrim) {
                    if (confrim) {
                        //確定
                        $_nowTabUI.find('.modal-footer').find('.btn-primary').trigger("click");
                    }
                    else {
                        //取消，還原上一個Tab編輯資料
                        if (isDoing)
                            return;

                        //取消會轉回清單，不可用
                        //$_nowTabUI.find('.modal-footer').find('.btn-default').trigger("click");

                        //隱藏Bootstrap Table(多筆)找使用者挑選的Index
                        var n = -1;
                        $('.bootstrap-table #_table').find('.dou-field-Fno').each(function (index) {
                            if ($(this).text() == oFno) {
                                n = index;
                                return false;
                            }
                        });

                        //隱藏Bootstrap Table(多筆)找Index表
                        var jBootstrapTable = $($('.bootstrap-table #_table').find('.dou-field-Fno')[n]).closest("tr");

                        //還原資料異動
                        $_nowTabUI.find('.field-content [data-fn]').each(function (index) {
                            //欄位名稱
                            var fn = $(this).attr('data-fn');
                            var datatype = douHelper.getField($_nowTable.instance.settings.fields, fn).datatype;

                            //輸入值(Bootstrap Table + dou實體)
                            var conValue = '';
                            if (datatype == 'textlist') {
                                //UI(人名)，bootstrapTable(人名)，X容器(員編)
                                conValue = jBootstrapTable.find('.dou-field-' + fn).text();
                            }
                            else {
                                //var conValue = $_nowTable.instance.settings.datas[0][fn];
                                conValue = $_nowTable.instance.settings.datas.find(obj => obj.Fno == oFno)[fn];
                            }

                            //conValue(null => DB欄位值Null) ("-" => 輸入提示字：-(沒值))
                            if (conValue == null || conValue == "-")
                                conValue = '';

                            var fn_name = douHelper.getField($_nowTable.instance.settings.fields, fn).title;
                            if (datatype == 'date') {
                                $(this).find('input').val(conValue == '' ? '' : JsonDateStr2Datetime(conValue).DateFormat("yyyy-MM-dd"));
                            }
                            else if (datatype == 'datetime') {
                                $(this).find('input').val(conValue == '' ? '' : JsonDateStr2Datetime(conValue).DateFormat("yyyy-MM-dd HH:mm"));
                            }
                            else {
                                $(this).val(conValue);
                            }
                        });
                    }
                    $_nowTable = nextTable;
                    $_nowTabUI = nextTabUI;

                    isDoing = true;
                });
            }
            else {
                $_nowTable = nextTable;
                $_nowTabUI = nextTabUI;
            }
        });


    }

    douoptions.afterUpdateServerData = function (row, callback) {
        jspAlertMsg($("body"), { autoclose: 2000, content: '員工資料更新成功!!', classes: 'modal-sm' },
            function () {
                $('html,body').animate({ scrollTop: $_masterTable.offset().top }, "show");
            });

        //(no callback)更新dou的rowdata
        $_masterTable.instance.updateDatas(row);

        //callback();
    }

    //////還原已變更Details資料
    ////douoptions.afterEditDataCancel = function (r, callback) {
    ////    if (isChange) {
    ////        $_masterTable.instance.updateDatas(r);
    ////    }
    ////}

    douoptions.afterAddServerData = function (row, callback) {        
        jspAlertMsg($("body"), { autoclose: 2000, content: '員工資料新增資料成功!!', classes: 'modal-sm' },
            function () {
                //tablist顯示
                $('#_tabs').closest('div[class=tab-content]').siblings().show();
                $('html,body').animate({ scrollTop: $_masterTable.offset().top }, "show");
            });
    }

    var $_masterTable = $("#_table").DouEditableTable(douoptions); //初始dou table

    //預設的tab;
    $_nowTable = $_masterTable;
});
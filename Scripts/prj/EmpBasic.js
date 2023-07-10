$(document).ready(function () {

    douoptions.title = '員工資料';

    //Master(EmpData) 員工資料
    douoptions.afterCreateEditDataForm = function ($container, row) {

        var $_oform = $("#_tabs");
        var $_d1EditDataContainer = $('<div>').appendTo($_oform.parent());      //Da1s編輯的容器
        var $_d4EditDataContainer = $('<table>').appendTo($_oform.parent());    //Da4s編輯的容器

        var $_d1Table = undefined;  //Da1s Dou實體
        var $_d4Table = undefined;  //Da4s Dou實體

        var $_nowTabUI = undefined;    //當下Tab 使用的UI;
        var $_nowTable = undefined;    //當下Tab 使用的Dou實體;

        var isChange = false;
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
            _opt.datas = [oRow.Da1s];
            _opt.singleDataEdit = true;
            _opt.editformWindowStyle = $.editformWindowStyle.showEditformOnly;

            //初始options預設值
            douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

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
        $_nowTabUI = $('#_tabs').closest('div[class=tab-content]').find('.active');
        
        var jTabList = $('#_tabs').closest('div[class=tab-content]').siblings().find('a[data-toggle="tab"]');
        //before tab-click
        jTabList.on('show.bs.tab', function (e) {
            isChange = false;

            //1-1 Tab切換需要儲存異動資料(執行確定功能)
            if ($_nowTable != null) {
                //驗證資料是否有異動
                $_nowTabUI.find('.field-content [data-fn]').each(function (index) {
                    //欄位名稱
                    var fn = $(this).attr('data-fn');
                    var datatype = douHelper.getField($_nowTable.instance.settings.fields, fn).datatype;


                    //UI輸入值
                    var uiValue = "";
                    if (datatype == 'datetime' || datatype == 'date') {
                        uiValue = $(this).find('input').val();
                    }
                    else {
                        uiValue = $(this).val();
                    }

                    var conValue = $_nowTable.instance.settings.datas[0][fn];
                    if (conValue != null) {

                        //日期格式比對(ui(1982-12-17), con(1982-12-17T00:00:00) => 取小統一長度)
                        if (datatype == 'datetime' || datatype == 'date') {
                            var minLength = Math.min(uiValue.length, conValue.length);
                            uiValue = uiValue.substring(0, minLength)

                            //容器(時間可能是物件) "/Date(1224043200000)/"
                            conValue = JsonDateStr2Datetime(conValue);
                            conValue = conValue.DateFormat("yyyy-MM-dd HH:mm:ss").substring(0, minLength);
                        }

                        if (uiValue != conValue) {
                            isChange = true;
                            return false;
                        }
                    }
                    else {
                        //(UI資料空值且DB為Null)或()
                        //UI有正常輸入值就是異動($_nowTable無欄位資料)
                        if (uiValue != "") {
                            isChange = true;
                            return false;
                        }
                    }

                });
            }
        });

        //after tab-click
        jTabList.on('shown.bs.tab', function (e) {            
            var nextTable = null;
            var nextTabUI = null;

            //當下切換的tab(1-1需要記錄)
            var actTab = $(e.target).html();
            if (actTab == $_masterTable.instance.settings.title) {
                nextTable = $_masterTable;
                nextTabUI = $('#_tabs').closest('div[class=tab-content]').find('.active');
            }
            else if (actTab == $_d1Table.instance.settings.title) {
                nextTable = $_d1Table;
                nextTabUI = $('#_tabs').closest('div[class=tab-content]').find('.active');
            }                         


            //1-1判斷是否有異動更新
            if (isChange && $_nowTable != null) {
                jspConfirmYesNo(nextTabUI, { content: "資料有異動，是否儲存" }, function (confrim) {
                    if (confrim) {
                        //確定
                        $_nowTabUI.find('.modal-footer').find('.btn-primary').trigger("click");
                    }
                    else {
                        //取消
                        $_nowTabUI.find('.modal-footer').find('.btn-default').trigger("click");
                    }
                    $_nowTable = nextTable;
                    $_nowTabUI = nextTabUI;
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

    var $_masterTable = $("#_table").DouEditableTable(douoptions); //初始dou table

    //預設的tab;
    $_nowTable = $_masterTable;
});
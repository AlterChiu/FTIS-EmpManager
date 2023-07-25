$(document).ready(function () {

    douoptions.title = '員工資料';

    var $_nowTabUI = undefined;    //當下Tab 使用的UI;
    var $_nowTable = undefined;    //當下Tab 使用的Dou實體;

    var $_d1EditDataContainer = undefined;      //Da1s編輯的容器
    var $_d4EditDataContainer = undefined;    //Da4s編輯的容器
    var $_d5EditDataContainer = undefined;    //Da5s編輯的容器
    var $_d6EditDataContainer = undefined;    //Da6s編輯的容器
    var $_d7EditDataContainer = undefined;    //Da7s編輯的容器
    var $_d8EditDataContainer = undefined;    //Da8s編輯的容器
    var $_d9EditDataContainer = undefined;    //Da9s編輯的容器

    var $_d1Table = undefined;  //Da1s Dou實體
    var $_d4Table = undefined;  //Da4s Dou實體
    var $_d5Table = undefined;  //Da5s Dou實體
    var $_d6Table = undefined;  //Da6s Dou實體
    var $_d7Table = undefined;  //Da7s Dou實體
    var $_d8Table = undefined;  //Da8s Dou實體
    var $_d9Table = undefined;  //Da9s Dou實體


    //主表(EmpData) 員工資料
    douoptions.afterCreateEditDataForm = function ($container, row) {

        var isAdd = JSON.stringify(row) == '{}';

        var $_oform = $("#_tabs");
        $_d1EditDataContainer = $('<div>').appendTo($_oform.parent());
        $_d4EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d5EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d6EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d7EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d8EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d9EditDataContainer = $('<table>').appendTo($_oform.parent());

        var oFno = row.Fno;
        var isChange = false;
        var isChangeText = [];

        //保留確定按鈕
        $container.find('.modal-footer button').hide();
        $container.find('.modal-footer').find('.btn-primary').show();

        if (!isAdd) {            
            //主表新增集合沒資料(預設集合)
            var iniObj = { Fno: row.Fno };

            //1-1 Detail(EmpDa1) 通訊方式            
            if (row.Da1s == undefined) {
                row.Da1s = iniObj;
            }
            SetDouEmpDa1(row.Da1s);

            //1-n Detail(EmpDa4) 學歷
            SetDouEmpDa4(row.Da4s, oFno);

            //1-n Detail(EmpDa5) 經歷
            SetDouEmpDa5(row.Da5s, oFno);

            //1-n Detail(EmpDa6) 家庭狀況
            SetDouEmpDa6(row.Da6s, oFno);

            //1-n Detail(EmpDa7) 外語檢定
            SetDouEmpDa7(row.Da7s, oFno);

            //1-n Detail(EmpDa8) 專業資格
            SetDouEmpDa8(row.Da8s, oFno);

            //1-n Detail(EmpDa9) 著作
            SetDouEmpDa9(row.Da9s, oFno);
        }

        //產tab
        ////helper.bootstrap.genBootstrapTabpanel($_d1EditDataContainer.parent(), undefined, undefined, ['員工資料', '通訊方式'], [$_oform, $_d1EditDataContainer]);
        helper.bootstrap.genBootstrapTabpanel($_d4EditDataContainer.parent(), undefined, undefined,
            ['員工資料', '通訊方式', '學歷', '經歷', '家庭狀況', '外語檢定', '專業資格', '著作'],
            [$_oform, $_d1EditDataContainer, $_d4EditDataContainer, $_d5EditDataContainer, $_d6EditDataContainer, $_d7EditDataContainer, $_d8EditDataContainer, $_d9EditDataContainer]);

        //預設的tab;        
        $_nowTabUI = $('#_tabs').closest('div[class=tab-content]').find('.show');

        //點選的Tab
        var jTabToggle = $('#_tabs').closest('div[class=tab-content]').siblings().find('a[data-toggle="tab"]');

        if (isAdd) {
            //tablist隱藏
            $('#_tabs').closest('div[class=tab-content]').siblings().hide();
        }

        //返回會員資料總表
        if ($_masterTable != undefined) {
            if (isAdd) {
                //長在footer右邊
                var btnBack = '<button id="btnBack" class="btn btn-secondary">返回會員資料總表</button>';
                $(btnBack).appendTo($container.find('.modal-footer'));
            }
            else {
                //長在Tab右邊
                var btnBack = '<li class="ms-auto"><button id="btnBack" class="btn btn-secondary">返回會員資料總表</button></li>';
                $(btnBack).appendTo($('#_tabs').closest('div[class=tab-content]').siblings());
            }
            $('#btnBack').click(function () {
                location.reload();
            });
        }

        //before tab-click
        jTabToggle.on('show.bs.tab', function (e) {
            isChange = false;
            isChangeText = [];

            //1-1 Tab切換需要儲存異動資料(trigger確定功能)
            //判斷(上一個Tab非Null且有資料)
            if ($_nowTable != null
                && $_nowTable.instance.getData().find(obj => obj.Fno == oFno) != null) {

                //停止Tab切換
                var tabStop = false;

                $.each($_nowTable.instance.settings.fields, function () {                    
                    //欄位名稱
                    var fn = this.field;                    

                    //input:輸入值(UI)
                    var $_nowContainer = $_nowTabUI.children(":first").toggleClass("data-edit-form-group");
                    var uiValue = douHelper.getDataEditContentValue($_nowContainer, this);

                    //不需輸入值(UI)
                    if (uiValue == null)
                        return; // 等於continue

                    //驗證：必填欄位
                    if (!this.allowNull) {
                        if (uiValue == '') {
                            tabStop = true;
                            return false;
                        }
                    }                    

                    //input:輸入值(dou實體)
                    var conValue = $_nowTable.instance.getData().find(obj => obj.Fno == oFno)[fn];

                    if (conValue != null) {

                        //////輸入提示字：-(沒值)
                        ////if (uiValue == "" && conValue == "-")
                        ////    return;

                        if (conValue != "") {
                            //日期格式比對(ui(1982-12-17), con(1982-12-17T00:00:00) => 取小統一長度)
                            if (this.datatype == 'datetime' || this.datatype == 'date') {
                                conValue = JsonDateStr2Datetime(conValue).DateFormat("yyyy/MM/dd HH:mm:ss");
                                var minLength = Math.min(uiValue.length, conValue.length);

                                uiValue = uiValue.substring(0, minLength)

                                //容器(時間可能是物件) "/Date(1224043200000)/"                                
                                conValue = conValue.substring(0, minLength);
                            }
                        }

                        if (uiValue != conValue) {
                            isChange = true;
                            isChangeText.push(this.title);
                            //return false;
                        }
                    }
                    else {
                        //(異動說明)DB為Null($_nowTable無欄位資料),但UI有值
                        if (uiValue != "") {
                            isChange = true;
                            isChangeText.push(this.title);
                            //return false;
                        }
                    }

                });

                //tab停止切換(原因：必填欄位....等問題)
                if (tabStop) {
                    $_nowTabUI.find('.modal-footer').find('.btn-primary').trigger("click");
                    return false;
                }
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

                        //Bootstrap Table(多筆)找編輯資料
                        var jBootstrapTable;
                        $('.bootstrap-table #_table').find('.dou-field-Fno').each(function (index) {
                            if ($(this).text() == oFno) {
                                jBootstrapTable = $(this).closest("tr");                                
                                return false;
                            }
                        });

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
                                //var conValue = $_nowTable.instance.getData()[0][fn];
                                conValue = $_nowTable.instance.getData().find(obj => obj.Fno == oFno)[fn];
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

                        //////////BootstrapRow(DB) 下拉錯誤(ex：虛擬欄位..)
                        ////////var $bootstrapTable = $_nowTabUI.find('.bootstrap-table .fixed-table-container').find('table.table-bordered');
                        ////////var $_nowBootstrapRow;
                        ////////$bootstrapTable.find('tbody tr .dou-field-Fno').each(function (index) {
                        ////////    if ($(this).html() == oFno) {
                        ////////        $_nowBootstrapRow = $(this).closest("tr");
                        ////////        return false;
                        ////////    }
                        ////////});

                        ////////$_nowTabUI.find('.field-content [data-fn]').each(function (index) {
                        ////////    //欄位名稱
                        ////////    var fn = $(this).attr('data-fn');

                        ////////    //input:輸入值(BootstrapRow) DB
                        ////////    var conValue = $_nowBootstrapRow.find('.dou-field-' + fn + '').html();

                        ////////    //不需輸入值(UI)
                        ////////    if (conValue == null)
                        ////////        return; // 等於continue

                        ////////    $(this).val(conValue);
                        ////////});
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

    var $_masterTable = $("#_table").DouEditableTable(douoptions).on($.dou.events.add, function (e, row) {
        
        //錨點
        var aPoint = $_masterTable.instance.settings.rootParentContainer;
        $('html,body').animate({ scrollTop: $(aPoint).offset().top }, "show");

        //trigger清單(新增row)編輯按鈕的，
        $_masterTable.DouEditableTable("editSpecificData", row);

    }); //初始dou table

    //預設的tab;
    $_nowTable = $_masterTable;


    function SetDouEmpDa1(datas) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa1/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '通訊方式';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? [datas] : [{}];
            _opt.datas = datas;

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
    }

    function SetDouEmpDa4(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa4/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '學歷';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            //實體Dou js
            $_d4Table = $_d4EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa5(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa5/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '經歷';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            //實體Dou js
            $_d5Table = $_d5EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa6(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa6/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '家庭狀況';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            //實體Dou js
            $_d6Table = $_d6EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa7(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa7/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '外語檢定';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            //實體Dou js
            $_d7Table = $_d7EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa8(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa8/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '專業資格';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            //實體Dou js
            $_d8Table = $_d8EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa9(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa9/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '著作';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            //實體Dou js
            $_d9Table = $_d9EditDataContainer.douTable(_opt);
        });
    };
});
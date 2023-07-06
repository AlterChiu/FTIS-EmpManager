$(document).ready(function () {

    var $_d1EditDataContainer = undefined;  //Da1s編輯的容器
    var $_d4EditDataContainer = undefined;  //Da4s編輯的容器

    var $_d1Table = undefined;  //Da1s Dou實體
    var $_d4Table = undefined;  //Da4s Dou實體

    douoptions.title = '員工資料';

    //Master(EmpData) 員工資料
    douoptions.afterCreateEditDataForm = function ($container, row) {

        var $_oform = $("#_tabs");
        $_d1EditDataContainer = $('<div>').appendTo($_oform.parent());
        $_d4EditDataContainer = $('<table>').appendTo($_oform.parent());

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
                //錨點
                jspAlertMsg($("body"), { autoclose: 2000, content: '通訊方式更新成功!!', classes: 'modal-sm' },
                    function () {
                        $('html,body').animate({ scrollTop: $_d1EditDataContainer.offset().top }, "show");
                    });

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

        //$("#_tabs").parents().closest('div[class=tab-content]').siblings()
        $_oform.parents().closest('div[class=tab-content]').siblings().find('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            //1-1 Tab切換需要儲存異動資料(執行確定功能)
            var actTab = $(e.target).html();
            if (actTab == $_masterTable.instance.settings.title || actTab == $_d1Table.instance.settings.title) {
                alert(actTab);
                //$('.modal-footer').find('.btn-primary').trigger("click");
            }
        });
    }

    douoptions.afterUpdateServerData = function (row, callback) {
        jspAlertMsg($("body"), { autoclose: 2000, content: '員工資料更新成功!!', classes: 'modal-sm' },
            function () {
                $('html,body').animate({ scrollTop: $_d1EditDataContainer.offset().top }, "show");
            });

        ////callback();
    }

    var $_masterTable = $("#_table").DouEditableTable(douoptions); //初始dou table
});
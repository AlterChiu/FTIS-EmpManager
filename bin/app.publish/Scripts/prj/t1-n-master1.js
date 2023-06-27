$(document).ready(function () {
    douoptions.tableOptions.buttonsAlign = 'none';
    douoptions.tableOptions.buttonsClass = 'default';
    douoptions.tableOptions.showExport = true;
    douoptions.tableOptions.iconsPrefix = "glyphicon";
    douoptions.tableOptions.icons = { export: 'glyphicon-export' };
    douoptions.tableOptions.exportTypes = ['csv'];
    douoptions.tableOptions.exportOptions = { fileName: '資產總表' };
    douoptions.tableOptions.formatExport = function () { return "匯出CSV" };
/*    douoptions.tableOptions.bootstrapTable = { Pagination: true, pageSize: 15, };*/

    var $_editDataContainer = undefined;
    var hasChangeDetails = false;//是否已變更Details資料

    douoptions.afterCreateEditDataForm = function ($container, row) {
        DeptoFno($container, "CustodianID", "CustodianDep", row);
        hasChangeDetails = false;
        if (row.AssetID == undefined)
            return;
        $.getJSON($.AppConfigOptions.baseurl +'UsageLog/GetDataManagerOptionsJson', function (_opt) {
            
            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;
            //20230503, edit 保管移轉紀錄編輯功能打開 by markhong
            _opt.addable = _opt.deleteable = false;
            //_opt.addable = _opt.editable = _opt.deleteable = false; 

            //給detail集合
            row.UsageLog = row.UsageLog || []; //無detail要實體參考，之後detail編輯才能跟master有關聯(前端物件)
            _opt.datas = row.UsageLog;

            //初始options預設值
            douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            //編輯後給detail.MasterId
            _opt.afterEditDataConfirm = function (drow, callback) {
                drow.AssetsAssetID = row.AssetID; //[重要]AssetsAssetID = [dbo].[Assets]+[dbo].[Assets].[AssetID]
                callback();
            }
                                                          
            //Master的編輯物件
            var $_oform = $container.find(".data-edit-form-group");
            
            //Detail的編輯物件
            $_editDataContainer = $('<div style="background-color: #FFFFf1;padding: .5rem;border-radius: .5rem;">').appendTo($_oform.parent());
            //實體Dou js
            var $_detailTable = $('<table>').appendTo($_editDataContainer).douTable(_opt).
                on([$.dou.events.add, $.dou.events.update, $.dou.events.delete].join(' '), function () {
                    hasChangeDetails = true;
                });
        });
    }

    var DeptoFno = function ($container, ef, df, row) {
        var $_depselect = $container.find('select[data-fn="' + df + '"]');
        var $_fnoselect = $container.find('select[data-fn="' + ef + '"]');
        var $_fnoptions = $_fnoselect.find('option');
        $_depselect.prop('disabled', false);
        $_fnoselect.closest('.form-group').insertAfter($_depselect.closest('.form-group')) //將員工select移置部門後
        $_depselect.on('change', function () {
            console.log('C=' + $_depselect.val());
            var dv = $_depselect.val();
            $_fnoselect.empty();
            if (dv == '')
                $_fnoptions.appendTo($_fnoselect);
            //$_fnodelect.find('option').removeClass('d-none');
            else {
                //$_fnoselect.find('option').addClass('d-none').first().removeClass('d-none'); //不隱藏第一個所有選項
                if ($_fnoptions.first().val() == '')
                    $_fnoptions.first().appendTo($_fnoselect);
                $.each($_fnoptions, function () {
                    var $_this = $(this);
                    if ($_this.attr("data-dcode") == dv)
                        $_this.appendTo($_fnoselect);
                });
                //$_fnoptions.find('[data-dcode="' + dv + '"]').appendTo($_fnoselect);//.removeClass('d-none');
            }
            //if (changeslectfist && $_fnoselect)
            //    ''
            //else
            //    $_fnoselect.val('');
            if ($_fnoselect.find('option').length > 0)
                $_fnoselect.selectIndex = 0;
        })
    }

    //還原已變更Details資料
    douoptions.afterEditDataCancel = function (r) {
        if (hasChangeDetails) 
            douoptions.updateServerData(r, function (result) {
                $_masterTable.DouEditableTable('updateDatas', result.data);//取消編輯，detail有可能已做一些改變，故重刷UI
            })
    }
    //新增Col：資訊設備
    douoptions.fields.push({
        title: "資訊資產明細", field: "ITButton", align: "center", formatter: detailbtn, visibleEdit: false
    });

    douoptions.fields.push({ title: "使用紀錄", field: "UsageLog", formatter: function (v) { console.log(v); if (v != null) return v.length }, visibleEdit: false });
    var $_masterTable = $("#_table").DouEditableTable(douoptions);

    /*douoptions.tableOptions.bootstrapTable = { Pagination: true, pageSize: 15, };*/
});

//資產盤點報表輸出
function AssetInventoryExport() {
    $.ajax({
        type: "POST",
        url: '../Asset/InventoryRpt',
        success: function (res) {
            console.log('success');
            $("#textBox1").val(res);
        },
        error: function (request) {
            alert("Error");
        }
    });
}

//展開<超連結>
function detailbtn(cellvalue, options, rowObject) {
    var vSN, vOS, vOffice, vCPU, vRam, vSSD, vHDD;
    var vIsIT = options.IsIT;
    vSN = vOS = vOffice = vCPU = vRam = vRam = vSSD = vHDD = '';
    if (options.ITAttr != "undefined" && options.ITAttr != null) {
        if (options.ITAttr[0] != "undefined" && options.ITAttr[0] != null) {
            for (let [key, value] of Object.entries(options.ITAttr[0])) {
                switch (key) {
                    case 'SN':
                        vSN = value;
                        break;
                    case 'OS':
                        vOS = value;
                        break;
                    case 'OfficeVersion':
                        vOffice = value;
                        break;
                    case 'Iserise':
                        vCPU = value;
                        break;
                    case 'RAM':
                        vRam = value;
                        break;
                    case 'SSD':
                        vSSD = value;
                        break;
                    case 'HDD':
                        vHDD = value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    var vcell = "'" + options.AssetID + "|" + vSN + "|" + vOS + "|" + vOffice + "|" + vCPU + "|" + vRam + "|" + vSSD + "|" + vHDD + "'";
    console.log(vcell);
    if (vIsIT)
        return '<a href="javascript:ShowRPT(' + vcell + ');"><i class="fas fa-list">展開</i></a>';
}

//細項popup
function ShowRPT(cellvalue) {
    const myArr = cellvalue.split("|");
    $("#AssetsAssetID").val(myArr[0]);
    $("#SN").val(myArr[1]);
    $("#OS").val(myArr[2]);
    $("#OfficeVersion").val(myArr[3]);
    $("#Iserise").val(myArr[4]);
    $("#RAM").val(myArr[5]);
    $("#SSD").val(myArr[6]);
    $("#HDD").val(myArr[7]);
    $("#ITPopUp").modal('show');
}

//細項取消
function closeDialog() {
    console.log("closed");
    $("#ITPopUp").modal('hide');
}

//細項編輯
function editInfo() {
    console.log("edit");
    var fd = new FormData();
    fd.append('AssetsAssetID', $("#AssetsAssetID").val());
    fd.append('SN', $("#SN").val());
    fd.append('OS', $("#OS").val().trim());
    fd.append('OfficeVersion', $("#OfficeVersion").val());
    fd.append('Iserise', $("#Iserise").val());
    fd.append('RAM', $("#RAM").val());
    fd.append('SSD', $("#SSD").val());
    fd.append('HDD', $("#HDD").val());
    console.log(fd);
    let confirmAction = confirm("確定要儲存嗎？");
    if (confirmAction) {
        $.ajax({
            dataType: 'json',
            url: '../ITAttributes/UpdateObject',
            //url: 'https://pj3.ftis.org.tw/AssetSys/ITAttributes/UpdateObject',
            type: "post",
            //data: new FormData($('#adj')['0']),
            data: fd,
            processData: false,
            contentType: false,
            success: function (data) {
                alert("儲存成功");
                location.reload(); 
            },

            error: function (request) {
                //alert(request.responseJSON.Message);
                alert("error");
            }
        });
    }
    else {
        alert("已取消儲存");
    }
    $("#ITPopUp").modal('hide');
}

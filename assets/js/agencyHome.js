﻿
$('divClientDetials').addClass('d-none');
$(document).ready(function () {

   
    new Clipboard(".copyEmail", {
        text: function (trigger) {

            let email = $(trigger).data('clipboard-text');
            toastr.success("Email address successfully copied to your clipboard " + email);
        
            return email;
        }
    });

    //createTwilioUser();
    AgencyDropdownPartialViewChange();

    setTimeout(function () {
        $('.currency-usd').each(function (key, value) {
            var monetary_value = $(value).text();
            if (monetary_value == '0') {
                var i = new Intl.NumberFormat('en-US', {
                    style: 'currency',
                    currency: 'USD'
                }).format(monetary_value);
                $(value).text(i);
            }
        });
    }, 3000);

    //$('#ddlGrossRevenue').change(function () {
    //    var item = $(this);
        
    //    RenderGrossRevenueChart(item.val());
    //});
    //$('#dllNetIncome').change(function () {
    //    var item = $(this);
       
    //    RenderNetIncomeChart(item.val());
    //});
    

});



function RenderGrossRevenueChart(Option) {

    var ClientID = $("#ddlclient option:selected").val();
    getAjax(`/AgencyService/GetGrossRevenueData?Option=${Option}&cType=${0}&ClientID=${ClientID}`, null, function (response) {
       
        if (response.Status == 'Success') {
            response.Xdata.shift();
            response.Ydata.shift()
            var xdata = response.Xdata.reverse(); ///['Oct', 'Nov', 'Dec'];
            var ydata = response.Ydata.reverse();//[100, 150, 200];      

            LineChart(xdata, ydata, '#container1');
        }
        else {
            var xdata = ['No Data'];
            var ydata = [0];

            LineChart(xdata, ydata, '#container1');
        }
    });

}
function Tabclick(e, type) {  
    sessionStorage.setItem('Type', type);
    if (type == 1 ) {

        $('#tabselectBank').addClass('tabselect');
        $('#tabNotinBanks').removeClass('tabselect');
    }
}
function RenderNetIncomeChart(Option) {
    var ClientID = $("#ddlclient option:selected").val();
    getAjax(`/AgencyService/GetGrossRevenueData?Option=${Option}&cType=${1}&ClientID=${ClientID}`, null, function (response) {
        
        if (response.Status == 'Success') {
            response.Xdata.shift();
            response.Ydata.shift();
            var xdata = response.Xdata.reverse(); ///['Oct', 'Nov', 'Dec'];
            var ydata = response.Ydata.reverse();//[100, 150, 200];

            LineChart(xdata, ydata, '#container2');
        }
        else {
            var xdata = ['No Data'];
            var ydata = [0];

            LineChart(xdata, ydata, '#container2');
        }

    });

}

var colorArrays = [];

var defaultChartColor = "#edf2f9";

function NeedsChart(dataArray) {
    let dom = document.getElementById("echart-needs");
    var myChart = echarts.init(dom);
    var defaultChartColor = "#edf2f9";
    var app = {};


    let options = {
        color: colorArrays,
        tooltip: {
            trigger: 'item',
            padding: [7, 10],
            backgroundColor: utils.grays.white,
            textStyle: {
                color: utils.grays.black
            },
            transitionDuration: 0,
            borderColor: utils.grays['300'],
            borderWidth: 1,
            formatter: function formatter(params) {
                if (params.data.name == "no data") {
                    return "";
                }
                else {
                    return "<strong>" + params.data.name + ":</strong> " + params.percent + "%";
                }
            }
        },
        position: function position(pos, params, dom, rect, size) {
            return getPosition(pos, params, dom, rect, size);
        },
        legend: {
            show: false
        },
        series: [{
            type: 'pie',
            radius: ['100%', '87%'],
            avoidLabelOverlap: false,
            hoverAnimation: false,
            itemStyle: {
                borderWidth: 2,
                borderColor: utils.grays.white
            },
            label: {
                normal: {
                    show: false,
                    position: 'center',
                    textStyle: {
                        fontSize: '20',
                        fontWeight: '500',
                        color: utils.grays['700']
                    }
                },
                emphasis: {
                    show: false
                }
            },
            labelLine: {
                normal: {
                    show: false
                }
            },
            data: dataArray
        }]

    };


    if (options && typeof options === 'object') {
        myChart.setOption(options);
    }
}

var color = {
    "Urgent": {
        "className": "bg-red",
        "echartColorCode": "#ff0000"
    },
    "High": {
        "className": "bg-yellow",
        "echartColorCode": "#FFDB74"
    },
    "Medium": {
        "className": "bg-dark-blue",
        "echartColorCode": "#248EA3"
    },
    "Low": {
        "className": "bg-green",
        "echartColorCode": "#00BE82"
    }
};

function KanbanCountWithIndividualPriority() {
    var ClientID = $("#ddlclient option:selected").val();
    getAjax(apiurl + `Needs/KanbanCountWithIndividualPriority?AgencyId=${ClientID}`, null, function (response) {
        let data = response.resultData;
        $("#needsCategoryDiv").children().remove();
        for (key in color) {
            var htmltext = `<div class="d-flex align-items-center">
                                                                        <span class="dot ${color[key].className}"></span>
                                                                        <span class="font-weight-semi-bold">${key}</span>
                                                                    </div>`;

            $("#needsCategoryDiv").append(htmltext);
        }

        var results = {};
        if (data != undefined && data.length > 0) {

            for (var i = 0; i < data.length; i++) {
                let KanbanTaskLabelName = data[i].kanbanTaskLabelName;
                let LabelNameCount = data[i].labelNameCount;
                let TotalTasks = 0;
                TotalTasks = data[i].totalTasks;
                $("#lblTotalTasksCount").text(TotalTasks);
                var tempObj1 = { value: LabelNameCount, name: KanbanTaskLabelName };

                results[KanbanTaskLabelName] = tempObj1;
            }
        }
        else {
            let TotalTasks = 0;
            $("#lblTotalTasksCount").text(TotalTasks);
            var tempObj = { value: 1, name: "no data" };
            results["no data"] = tempObj;

        }


        for (key in results) {

            colorArrays.push(color[key] ? color[key].echartColorCode : defaultChartColor);

        }

        NeedsChart(Object.values(results));
    });
}



var colorsArray = [];
function NotesChart(dataArray) {
    let dom = document.getElementById("echart-notes");
    var myChart = echarts.init(dom);

    var app = {};

    let options = {

        color: colorsArray,
        tooltip: {
            trigger: 'item',
            padding: [7, 10],
            backgroundColor: utils.grays.white,
            textStyle: {
                color: utils.grays.black
            },
            transitionDuration: 0,
            borderColor: utils.grays['300'],
            borderWidth: 1,
            formatter: function formatter(params) {
                if (params.data.name == "no data") {
                    return "";
                }
                else {
                    return "<strong>" + params.data.name + ":</strong> " + params.percent + "%";
                }
            }
        },
        position: function position(pos, params, dom, rect, size) {
            return getPosition(pos, params, dom, rect, size);
        },
        legend: {
            show: false
        },
        series: [{
            type: 'pie',
            radius: ['100%', '87%'],
            avoidLabelOverlap: false,
            hoverAnimation: false,
            itemStyle: {
                borderWidth: 2,
                borderColor: utils.grays.white
            },
            label: {
                normal: {
                    show: false,
                    position: 'center',
                    textStyle: {
                        fontSize: '20',
                        fontWeight: '500',
                        color: utils.grays['700']
                    }
                },
                emphasis: {
                    show: false
                }
            },
            labelLine: {
                normal: {
                    show: false
                }
            },
            data: dataArray
        }]

    };


    if (options && typeof options === 'object') {
        myChart.setOption(options);
    }
}
var colors = {
    "Income": {
        "className": "bg-red",
        "chartColorCode": "#ff0000"
    },
    "Expenses": {
        "className": "bg-orange",
        "chartColorCode": "#F68F57"
    },
    "Assets": {
        "className": "bg-green",
        "chartColorCode": "#00BE82"
    },
    "Bank Balance": {
        "className": "bg-soft-primary",
        "chartColorCode": "#bfbfbf"
    },
    "Liabilities": {
        "className": "bg-blue",
        "chartColorCode": "#235EE8"
    },
    "Equity": {
        "className": "bg-light-blue",
        "chartColorCode": "#27BCFE"
    }
};
var colorsNoteCat = {
    "Relevant": {
        "className": "turquoiseblue",
        "chartColorCode": "#05768f"
    },
    "Reliable": {
        "className": "bg-green",
        "chartColorCode": "#00BE82"
    },
    "Real Time": {
        "className": "yellow",
        "chartColorCode": "#FFDB74"
    }};

function NotesIndividualCountAndPercentageByAgencyId() {
    var ClientID = $("#ddlclient option:selected").val();
    getAjax(apiurl + `Notes/NotesIndividualCountAndPercentageByAgencyId?AgencyId=${ClientID}`, null, function (response) {
        let data = response.resultData;

        $("#notesCategoryDiv").children().remove();
        for (key in colorsNoteCat) {
            var htmltext = `<div class="d-flex align-items-center">
                                                                            <span class="dot ${colorsNoteCat[key].className}"></span>
                                                                            <span class="font-weight-semi-bold">${key}</span>
                                                                        </div>`;

            $("#notesCategoryDiv").append(htmltext);
        }
        var result = {};
        if (data != undefined && data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                let NoteCategoryName = data[i].noteCategoryName;
                let NoteCategoryCount = data[i].noteCategoryCount;
                let TotalNotes = data[i].totalNotes;
                var tempObj = { value: NoteCategoryCount, name: NoteCategoryName };
                result[NoteCategoryName] = tempObj;
                $("#lblTotalNotesCount").text(TotalNotes);
            }
        }
        else {
            let TotalNotes = 0;
            $("#lblTotalNotesCount").text(TotalNotes);         
            var tempObj = { value: 1, name: "no data" };
            result["no data"] = tempObj;
        }

        for (key in result) {

            colorsArray.push(colorsNoteCat[key] ? colorsNoteCat[key].chartColorCode : defaultChartColor)
        }

        NotesChart(Object.values(result));
    });
}



function getTeamMembersList() {
    var ClientID = $("#ddlclient option:selected").val();
    $.ajax({
        url: '/Needs/getTeamMembersList?ClientID=' + ClientID,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                var count=0;
                var number;
                // ----> Removed for loop on 07-02-2022, Reason Changed the approach.
                $.each(data.TeamMembers, function (key, object) {
                    count = object.OrderNumber
                    $('#teamMember' + count).css({ "display": "" })

                    if (object.Username != null && object.Username != '') {
                        //$('#spTeamChat' + count).removeClass("disabled-action-icons");

                        number = "tel:" + object.PhoneNumber;
                        if (number != null) {

                            $('#phoneNumber' + count).attr("href", number);
                            $('#phoneNumber' + count).removeClass("disabled-action-icons");
                        }
                    }

                    if (object.Username != null && object.Username != '') {
                        if (object.Email != null && object.Email!=undefined) {
                            $('#email' + count).data("clipboard-text", String(object.Email));
                            $('#email' + count).removeClass("disabled-action-icons");

                        }
                    }
                    if (object.LinkedInProfile != null && object.LinkedInProfile != '') {

                        $('#aLinkedInProfile' + count).attr("href", object.LinkedInProfile);
                        $('#aLinkedInProfile' + count).prop('disabled', false);
                        $('#aLinkedInProfile' + count).removeClass('disabled-action-icons');
                    }
                    else {
                        $('#aLinkedInProfile' + count)[0].className = $('#aLinkedInProfile' + count)[0].className + ' disabled-action-icons';
                        $('#aLinkedInProfile' + count).prop('disabled', true);
                    }
                    if (object.Username != null && object.Username != '' && object.Jobtitle != null && object.Jobtitle != '') {

                        if (count == 1) {
                            $('#spStaffName' + count).html(String(object.Username));
                            $('#spJobTitle' + count).html('CFO');
                        }
                        else if (count == 2) {
                            $('#spStaffName' + count).html(String(object.Username));
                            $('#spJobTitle' + count).html('Accounting Manager');
                        }
                        else if (count == 3) {
                            $('#spStaffName' + count).html(String(object.Username));
                            $('#spJobTitle' + count).html('Accountant');
                        }
                        else if (count == 4) {
                            $('#spStaffName' + count).html(String(object.Username));
                            $('#spJobTitle' + count).html('Bookkeeper');
                        }
                    }
                    else {
                        $('#spStaffName' + count).html(String(''));
                        $('#spJobTitle' + count).html(String(''));
                    }

                    if (object.Profileimage != null && object.Profileimage != '') {
                        $('#spProfileImage' + count).attr('src', object.Profileimage);
                    }
                    else {
                        $('#spProfileImage' + count).attr('src', '../assets/img/team/default-logo.png');
                    }
                    //if (object.Email != null && object.Email != '') {
                    //    $('#spTeamChat' + count).attr('href', `/Communication/Chat?WithTeamMember=${object.Email}`);
                    //}
                    //else {
                    //    $('#spTeamChat' + count).attr('href', '');
                    //}
                });
            }
        }
    });
   
}

function GetAccountOutStanding() {
    var ClientID = $("#ddlclient option:selected").val();
    $.ajax({
        url: `/AgencyService/GetAccountOutStanding?ClientID=${ClientID}`,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null && data != '') {
                
                $('#spOutSanding').html(ConvertToUDS(data.Total));
            }
            else {
                $('#spOutSanding').html(ConvertToUDS(0));
            }
        },
        error: function (e) {
            
            $('#spOutSanding').html(ConvertToUDS(0));
            console.log(e);
        }


    });
}


function AgencyDropdownPartialViewChange() {

    ShowlottieLoader(); 
    var ClientID = $("#ddlclient option:selected").val();
  

    $.ajax({
        url: '/AgencyService/GetClientDetails?id=' + ClientID,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                HidelottieLoader();
            
                MenuOptionHideAndShow(ClientID);
                GetReconcilationData();
                GetReconcilationData1();
                KanbanCountWithIndividualPriority();
                NotesIndividualCountAndPercentageByAgencyId();

                $('#roleexist').show();
                $('.spClientName').html(String(data.Name));
                $('.spEntityName').html(String(data.EntityName));
                if (data.StartDate != null && data.StartDate != '') {

                    let roughDate = Number(data.StartDate.match(/\d+/)[0]);
                    let localTime = UtcDateToLocalTime(roughDate).toDateString();

                    $('#spCreatedDate').html(localTime);
                }
                $('#spStatus').html(String(data.Status ? "Active" : "Inactive"));
                let month = moment(new Date()).diff(moment(data.StartDate), 'months', false) + 1;
                $('#spMonths').html(month);

                getTeamMembersList();
                defaultReportsWidget();

                $('.badge-soft-success').removeClass('d-none');
                $('.badge-success').removeClass('d-none');
                $('.rounded-circle').removeClass('d-none');
            }
            
            else {

                $('#roleexist').hide();
                $('.spClientName').html('');
                $('.spEntityName').html('');
                $('#spCreatedDate').html('');
                $('#spMonths').html('');
                //$('#spTeamName').html('');
                $('#spStatus').html('');
                $('#spClientAddress').html('');
                $('.badge-soft-success').addClass('d-none');
                $('.badge-success').removeClass('d-none');
                $('.rounded-circle').removeClass('d-none');
                
            }
           
        },
        error: function () {
            $('#roleexist').hide();
            $('.spClientName').html('');
            $('.spEntityName').html('');
            $('#spCreatedDate').html('');
            $('#spMonths').html('');
            //$('#spTeamName').html('');
            $('#spStaffName1').html('');
            $('#spStaffName2').html('');
            $('#spStaffName3').html('');
            $('#spStaffName4').html('');
            $('#spJobTitle1').html('');
            $('#spJobTitle2').html('');
            $('#spJobTitle3').html('');
            $('#spJobTitle4').html('');
            $('#spProfileImage1').removeAttr('src');
            $('#spProfileImage2').removeAttr('src');
            $('#spProfileImage3').removeAttr('src');
            $('#spProfileImage4').removeAttr('src');
            $('#spTeamChat1').removeAttr('href');
            $('#spTeamChat2').removeAttr('href');
            $('#spTeamChat3').removeAttr('href');
            $('#spTeamChat4').removeAttr('href');
            $('#spStatus').html('');
            $('#spClientAddress').html('');
            $('.badge-soft-success').addClass('d-none');
            $('.badge-success').addClass('d-none');
            $('.rounded-circle').addClass('d-none');
          
            
        }
     
  
    }); 
    window.onerror = function (e) {
        console.log(e);
        HidelottieLoader();
    };
}


var totalSum1;
var totalSum2;
function GetReconcilationData() {




    var ClientID = $("#ddlclient option:selected").val();
  
    var percentage = 0;
    totalSum1 = 0;
   // if (sessionStorage.getItem("NotInBanksData") == null) {
        getAjax(`/Reconciliation/GetReconciliationDashboardDataAgencyId?AgencyId=${ClientID}&type=Outstanding Payments`, null, function (response) {
            if (response.Message == "Success") {
               
                let data = response.ResultData;
                if (data != null && data.length > 0) {
                    $("#lblNotInBanksCount2").text(data[0].Count);
                    $("#lblpostiveBanksCount").text(ConvertToUDS(data[0].amountPositive).replace('-', ''));
                    $("#lblNegativeBanksCount").text(ConvertToUDS(data[0].amountNegative).replace('-', ''));
                    percentage = data[0].percentage.toFixed(0);
                    totalSum1 = data[0].Count;
                    $("#divNotInBankPercentage").html(`<div class="progress-circle" id="divNotInBankPercentage1" data-options='{"color":"url(#gradient)","progress":${percentage},"strokeWidth":5,"trailWidth":5}'></div>`)
                    utils.addProgressCircle("#divNotInBankkPercentage1");
                
                    sessionStorage.setItem("NotInBanksData", JSON.stringify(data));
                }
                else {
                    $("#lblNegativeBanksCount").text(ConvertToUDS(0).replace('-', ''));
                    $("#lblPostiveInBooksCount").text(ConvertToUDS(0).replace('-', ''));
                }
                $("#divNotInBankPercentage").html(`<div class="progress-circle" id="lblNotInBankPercentage" data-options='{"color":"url(#gradient)","progress":${percentage},"strokeWidth":5,"trailWidth":5}'></div>`)
                utils.addProgressCircle("#lblNotInBankPercentage")
                TotalSum(totalSum1, totalSum2);
            }
        })
  //  }
    //else {

    //    let data = JSON.parse(sessionStorage.getItem("NotInBanksData"));
    //    if (data != null && data.length > 0) {
    //        $("#lblNotInBanksCount2").text(data[0].Count);
    //        $("#lblpostiveBanksCount").text(ConvertToUDS(data[0].amountPositive).replace('-', ''));
    //        $("#lblNegativeBanksCount").text(ConvertToUDS(data[0].amountNegative).replace('-', ''));
    //        percentage = data[0].percentage.toFixed(0);
    //        totalSum1 = data[0].Count;            
    //    }
    //    else {
    //        $("#lblNegativeBanksCount").text(ConvertToUDS(0).replace('-', ''));
    //        $("#lblPostiveInBooksCount").text(ConvertToUDS(0).replace('-', ''));
    //    }
    //    $("#divNotInBankPercentage").html(`<div class="progress-circle" id="lblNotInBankPercentage" data-options='{"color":"url(#gradient)","progress":${percentage},"strokeWidth":5,"trailWidth":5}'></div>`)
    //    utils.addProgressCircle("#lblNotInBankPercentage")
    //    TotalSum(totalSum1, totalSum2);

    //}
    

}
function GetReconcilationData1() {
    var ClientID = $("#ddlclient option:selected").val();
 
    var totalSum = 0;
    var percentage = 0;
    totalSum2 = 0;
    //if (sessionStorage.getItem("NotInBooksData") == null) {
        getAjax(`/Reconciliation/GetReconciliationDashboardDataAgencyId?AgencyId=${ClientID}&type=Unreconciled`, null, function (response) {
            if (response.Message == "Success") {
                
                let data = response.ResultData;
                if (data != null && data.length > 0) {
                    $("#lblNotInBooksCount2").text(data[0].Count);
                    $("#lblNegativeInBooksCount").text(ConvertToUDS(data[0].amountPositive).replace('-', ''));
                    $("#lblPostiveInBooksCount").text(ConvertToUDS(data[0].amountNegative).replace('-', ''));
                    totalSum2 = data[0].Count;
                  
                    percentage = data[0].percentage;
             
                    sessionStorage.setItem("NotInBooksData", JSON.stringify(data));
                }
                else {
                    $("#lblNegativeBanksCount").text(ConvertToUDS(0).replace('-', ''));
                    $("#lblNegativeInBooksCount").text(ConvertToUDS(0).replace('-', ''));
                }
                $("#divNotInBookPercentage").html(`<div class="progress-circle" id="divNotInBookPercentage1" data-options='{"color":"url(#gradient)","progress":${percentage},"strokeWidth":5,"trailWidth":5}'></div>`)
                utils.addProgressCircle("#divNotInBookPercentage1")
                TotalSum(totalSum1, totalSum2);
            }
        })
    //}

    //else {
    //    var data = JSON.parse(sessionStorage.getItem("NotInBooksData"));
    //    if (data != null && data.length > 0) {
    //        $("#lblNotInBooksCount2").text(data[0].Count);
    //        $("#lblNegativeInBooksCount").text(ConvertToUDS(data[0].amountPositive).replace('-', ''));
    //        $("#lblPostiveInBooksCount").text(ConvertToUDS(data[0].amountNegative).replace('-', ''));
    //        totalSum2 = data[0].Count;
    //        percentage = data[0].percentage.toFixed(0);
           
    //    }
    //    else {
    //        $("#lblNegativeBanksCount").text(ConvertToUDS(0).replace('-', ''));
    //        $("#lblNegativeInBooksCount").text(ConvertToUDS(0).replace('-', ''));
    //    }
    //    $("#divNotInBookPercentage").html(`<div class="progress-circle" id="divNotInBookPercentage1" data-options='{"color":"url(#gradient)","progress":${percentage},"strokeWidth":5,"trailWidth":5}'></div>`)
    //    utils.addProgressCircle("#divNotInBookPercentage1")
    //    TotalSum(totalSum1, totalSum2);
    //}
    
    
}

function TotalSum(totalSum1, totalSum2) {
    let totalsum3 = 0;
    //if (isNaN(totalSum1 + totalSum2)) {
    //    $("#lblNotInCount").addClass('d-none');
    //}
    //else {
    //    $("#lblNotInCount").removeClass('d-none');
    //    totalsum3 = isNaN(totalSum1 + totalSum2) ? 0 : Number(totalSum1 + totalSum2);
    //    $("#lblNotInCount").text(totalsum3);

    //}
}


const camelize = (str) => {
    const text = str.replace(/[-_\s.]+(.)?/g, (_, c) =>
        c ? c.toUpperCase() : ""
    );
    return `${text.substr(0, 1).toLowerCase()}${text.substr(1)}`;
};

const getData = (el, data) => {
    try {
        return JSON.parse(el.dataset[camelize(data)]);
    } catch (e) {
        return el.dataset[camelize(data)];
    }
};
const getGrays = (dom) => ({
    white: getColor("white", dom),
    100: getColor("100", dom),
    200: getColor("200", dom),
    300: getColor("300", dom),
    400: getColor("400", dom),
    500: getColor("500", dom),
    600: getColor("600", dom),
    700: getColor("700", dom),
    800: getColor("800", dom),
    900: getColor("900", dom),
    1000: getColor("1000", dom),
    1100: getColor("1100", dom),
    black: getColor("black", dom),
});
const getColors = (dom) => ({
    primary: getColor("primary", dom),
    secondary: getColor("secondary", dom),
    success: getColor("success", dom),
    info: getColor("info", dom),
    warning: getColor("warning", dom),
    danger: getColor("danger", dom),
    light: getColor("light", dom),
    dark: getColor("dark", dom),
});
const getColor = (name, dom = document.documentElement) =>
    getComputedStyle(dom).getPropertyValue(`--falcon-${name}`).trim();
const rgbaColor = (color = "#fff", alpha = 0.5) =>
    `rgba(${hexToRgb(color)}, ${alpha})`;
const hexToRgb = (hexValue) => {
    let hex;
    hexValue.indexOf("#") === 0
        ? (hex = hexValue.substring(1))
        : (hex = hexValue);
    // Expand shorthand form (e.g. "03F") to full form (e.g. "0033FF")
    const shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(
        hex.replace(shorthandRegex, (m, r, g, b) => r + r + g + g + b + b)
    );
    return result
        ? [
            parseInt(result[1], 16),
            parseInt(result[2], 16),
            parseInt(result[3], 16),
        ]
        : null;
};
var totalSalesInit = function totalSalesInit() {
    var ECHART_LINE_TOTAL_SALES = '.echart-line-total-sales';
    var SELECT_MONTH = '.select-month';
    var $echartsLineTotalSales = document.querySelector(ECHART_LINE_TOTAL_SALES);
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    function getFormatter(params) {
        var _params$ = params[0],
            name = _params$.name,
            value = _params$.value;
        var date = new Date(name);
        return "".concat(months[0], " ").concat(date.getDate(), ", ").concat(value);
    }

    if ($echartsLineTotalSales) {
        // Get options from data attribute
        var userOptions = getData($echartsLineTotalSales, 'options');
        var chart = window.echarts.init($echartsLineTotalSales);
        var monthsnumber = [[60, 80, 60, 80, 65, 130, 120, 100, 30, 40, 30, 70], [100, 70, 80, 50, 120, 100, 130, 140, 90, 100, 40, 50], [80, 50, 60, 40, 60, 120, 100, 130, 60, 80, 50, 60], [70, 80, 100, 70, 90, 60, 80, 130, 40, 60, 50, 80], [90, 40, 80, 80, 100, 140, 100, 130, 90, 60, 70, 50], [80, 60, 80, 60, 40, 100, 120, 100, 30, 40, 30, 70], [20, 40, 20, 50, 70, 60, 110, 80, 90, 30, 50, 50], [60, 70, 30, 40, 80, 140, 80, 140, 120, 130, 100, 110], [90, 90, 40, 60, 40, 110, 90, 110, 60, 80, 60, 70], [50, 80, 50, 80, 50, 80, 120, 80, 50, 120, 110, 110], [60, 90, 60, 70, 40, 70, 100, 140, 30, 40, 30, 70], [20, 40, 20, 50, 30, 80, 120, 100, 30, 40, 30, 70]];

        var getDefaultOptions = {
            color: getGrays()['100'],
            tooltip: {
                trigger: 'axis',
                padding: [7, 10],
                backgroundColor: getGrays()['100'],
                borderColor: getGrays()['300'],
                textStyle: {
                    color: getColors().dark
                },
                borderWidth: 1,
                formatter: function formatter(params) {
                    return getFormatter(params);
                },
                transitionDuration: 0,
                position: function position(pos, params, dom, rect, size) {
                    return getPosition(pos, params, dom, rect, size);
                }
            },
            xAxis: {
                type: 'category',
                data: ['2019-01-05', '2019-01-06', '2019-01-07', '2019-01-08', '2019-01-09', '2019-01-10', '2019-01-11', '2019-01-12', '2019-01-13', '2019-01-14', '2019-01-15', '2019-01-16'],
                boundaryGap: false,
                axisPointer: {
                    lineStyle: {
                        color: getGrays()['300'],
                        type: 'dashed'
                    }
                },
                splitLine: {
                    show: false
                },
                axisLine: {
                    lineStyle: {
                        // color: utils.getGrays()['300'],
                        color: rgbaColor('#000', 0.01),
                        type: 'dashed'
                    }
                },
                axisTick: {
                    show: false
                },
                axisLabel: {
                    color: getGrays()['400'],
                    formatter: function formatter(value) {
                        var date = new Date(value);
                        return "".concat(months[date.getMonth()], " ").concat(date.getDate());
                    },
                    margin: 15
                }
            },
            yAxis: {
                type: 'value',
                axisPointer: {
                    show: false
                },
                splitLine: {
                    lineStyle: {
                        color: getGrays()['300'],
                        type: 'dashed'
                    }
                },
                boundaryGap: false,
                axisLabel: {
                    show: true,
                    color: getGrays()['400'],
                    margin: 15
                },
                axisTick: {
                    show: false
                },
                axisLine: {
                    show: false
                }
            },
            series: [{
                type: 'line',
                data: monthsnumber[0],
                lineStyle: {
                    color: getColors().primary
                },
                itemStyle: {
                    borderColor: getColors().primary,
                    borderWidth: 2
                },
                symbol: 'circle',
                symbolSize: 10,
                smooth: false,
                hoverAnimation: true,
                areaStyle: {
                    color: {
                        type: 'linear',
                        x: 0,
                        y: 0,
                        x2: 0,
                        y2: 1,
                        colorStops: [{
                            offset: 0,
                            color: rgbaColor(getColors().primary, 0.2)
                        }, {
                            offset: 1,
                            color: rgbaColor(getColors().primary, 0)
                        }]
                    }
                }
            }],
            grid: {
                right: '28px',
                left: '40px',
                bottom: '15%',
                top: '5%'
            }
        };

        if (getDefaultOptions) { // && typeof getDefaultOptions === 'object'
            chart.setOption(getDefaultOptions);
        }

        $echartsLineTotalSales.echartSetOption(chart, userOptions, getDefaultOptions); // Change chart options according to the selected month

        var monthSelect = document.querySelector(SELECT_MONTH);

        if (monthSelect) {
            monthSelect.addEventListener('change', function (e) {
                var month = e.currentTarget.value;
                var data = monthsnumber[month];
                chart.setOption({
                    tooltip: {
                        formatter: function formatter(params) {
                            var _params$2 = params[0],
                                name = _params$2.name,
                                value = _params$2.value;
                            var date = new Date(name);
                            return "".concat(months[month], " ").concat(date.getDate(), ", ").concat(value);
                        }
                    },
                    xAxis: {
                        axisLabel: {
                            formatter: function formatter(value) {
                                var date = new Date(value);
                                return "".concat(months[month], " ").concat(date.getDate());
                            },
                            margin: 15
                        }
                    },
                    series: [{
                        data: data
                    }]
                });
            });
        }
    }
};
var totalSalesInitTwo = function totalSalesInitTwo() {
    var ECHART_LINE_TOTAL_SALES = '.echart-line-total-sales-2';
    var SELECT_MONTH = '.select-month';
    var $echartsLineTotalSales = document.querySelector(ECHART_LINE_TOTAL_SALES);
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    function getFormatter(params) {
        var _params$ = params[0],
            name = _params$.name,
            value = _params$.value;
        var date = new Date(name);
        return "".concat(months[0], " ").concat(date.getDate(), ", ").concat(value);
    }

    if ($echartsLineTotalSales) {
        // Get options from data attribute
        var userOptions = getData($echartsLineTotalSales, 'options');
        var chart = window.echarts.init($echartsLineTotalSales);
        var monthsnumber = [[60, 80, 60, 80, 65, 130, 120, 100, 30, 40, 30, 70], [100, 70, 80, 50, 120, 100, 130, 140, 90, 100, 40, 50], [80, 50, 60, 40, 60, 120, 100, 130, 60, 80, 50, 60], [70, 80, 100, 70, 90, 60, 80, 130, 40, 60, 50, 80], [90, 40, 80, 80, 100, 140, 100, 130, 90, 60, 70, 50], [80, 60, 80, 60, 40, 100, 120, 100, 30, 40, 30, 70], [20, 40, 20, 50, 70, 60, 110, 80, 90, 30, 50, 50], [60, 70, 30, 40, 80, 140, 80, 140, 120, 130, 100, 110], [90, 90, 40, 60, 40, 110, 90, 110, 60, 80, 60, 70], [50, 80, 50, 80, 50, 80, 120, 80, 50, 120, 110, 110], [60, 90, 60, 70, 40, 70, 100, 140, 30, 40, 30, 70], [20, 40, 20, 50, 30, 80, 120, 100, 30, 40, 30, 70]];

        var getDefaultOptions = {
            color: getGrays()['100'],
            tooltip: {
                trigger: 'axis',
                padding: [7, 10],
                backgroundColor: getGrays()['100'],
                borderColor: getGrays()['300'],
                textStyle: {
                    color: getColors().dark
                },
                borderWidth: 1,
                formatter: function formatter(params) {
                    return getFormatter(params);
                },
                transitionDuration: 0,
                position: function position(pos, params, dom, rect, size) {
                    return getPosition(pos, params, dom, rect, size);
                }
            },
            xAxis: {
                type: 'category',
                data: ['2019-01-05', '2019-01-06', '2019-01-07', '2019-01-08', '2019-01-09', '2019-01-10', '2019-01-11', '2019-01-12', '2019-01-13', '2019-01-14', '2019-01-15', '2019-01-16'],
                boundaryGap: false,
                axisPointer: {
                    lineStyle: {
                        color: getGrays()['300'],
                        type: 'dashed'
                    }
                },
                splitLine: {
                    show: false
                },
                axisLine: {
                    lineStyle: {
                        color: getGrays()['300'],
                        //color: utils.rgbaColor('#000', 0.01),
                        type: 'dashed'
                    }
                },
                axisTick: {
                    show: false
                },
                axisLabel: {
                    //color: utils.getGrays()['400'],
                    formatter: function formatter(value) {
                        var date = new Date(value);
                        return "".concat(months[date.getMonth()], " ").concat(date.getDate());
                    },
                    margin: 15
                }
            },
            yAxis: {
                type: 'value',
                axisPointer: {
                    show: false
                },
                splitLine: {
                    lineStyle: {
                        color: getGrays()['300'],
                        type: 'dashed'
                    }
                },
                boundaryGap: false,
                axisLabel: {
                    show: true,
                    color: getGrays()['400'],
                    margin: 15
                },
                axisTick: {
                    show: false
                },
                axisLine: {
                    show: false
                }
            },
            series: [{
                type: 'line',
                data: monthsnumber[0],
                lineStyle: {
                    color: getColors().primary
                },
                itemStyle: {
                    borderColor: getColors().primary,
                    borderWidth: 2
                },
                symbol: 'circle',
                symbolSize: 10,
                smooth: false,
                hoverAnimation: true,
                areaStyle: {
                    color: {
                        type: 'linear',
                        x: 0,
                        y: 0,
                        x2: 0,
                        y2: 1,
                        colorStops: [{
                            offset: 0,
                            color: rgbaColor(getColors().primary, 0.2)
                        }, {
                            offset: 1,
                            color: rgbaColor(getColors().primary, 0)
                        }]
                    }
                }
            }],
            grid: {
                right: '28px',
                left: '40px',
                bottom: '15%',
                top: '5%'
            }
        };
        if (getDefaultOptions && typeof getDefaultOptions === 'object') {
            chart.setOption(getDefaultOptions);
        }

        var monthSelect = document.querySelector(SELECT_MONTH);

        if (monthSelect) {
            monthSelect.addEventListener('change', function (e) {
                var month = e.currentTarget.value;
                var data = monthsnumber[month];
                chart.setOption({
                    tooltip: {
                        formatter: function formatter(params) {
                            var _params$2 = params[0],
                                name = _params$2.name,
                                value = _params$2.value;
                            var date = new Date(name);
                            return "".concat(months[month], " ").concat(date.getDate(), ", ").concat(value);
                        }
                    },
                    xAxis: {
                        axisLabel: {
                            formatter: function formatter(value) {
                                var date = new Date(value);
                                return "".concat(months[month], " ").concat(date.getDate());
                            },
                            margin: 15
                        }
                    },
                    series: [{
                        data: data
                    }]
                });
            });
        }
    }
};

var defaultReportsWidget = function () {
    var agencyId = $("#ddlclient option:selected").val();
    getAjax(`/Reports/GetDashboardReports?agencyId=${agencyId}`, null, function (response) {
        var divMonthlyReports = $("#divMonthlyReports");
        var divYearlyReports = $("#divYearlyReports");
        divMonthlyReports.empty();
        divYearlyReports.empty();
        if (response.DataMonthly.length > 0) {
            for (var i = 0; i < response.DataMonthly.length; i++) {
                let reportHtml = prepareReportMedia(response.DataMonthly[i]);
                divMonthlyReports.append(reportHtml);
            }
        }
        if (response.DataYearly.length > 0) {
            for (var i = 0; i < response.DataYearly.length; i++) {
                let reportHtml = prepareReportMedia(response.DataYearly[i]);
                divYearlyReports.append(reportHtml);
            }
        }
        //if ($('#Loader').css('display') == 'none') {
        //    RenderGrossRevenueChart($('#ddlGrossRevenue').val());
        //    RenderNetIncomeChart($('#dllNetIncome').val());
        //}
    });
}
function prepareReportMedia(report) {
    let DownloadFileLink = SpecialURLEncoding(report.DownloadFileLink);
    let thumbnail = getSampleBGImageByFileExtension(report.FileExtention);    
    if (isEmptyOrBlank(thumbnail))
        thumbnail = report.FilePath;
    report.FilePath = report.FilePath.replace("~/", "../../");
    thumbnail = thumbnail.replace("~/", "../../");
    let reportHTML = `<div class="media align-items-center mb-3">
                                    <div class="avatar avatar-2xl">
                                        <a class="data-fancybox" href="${DownloadFileLink}" data-fancybox="group" data-type="iframe"><img class="rounded" src="${thumbnail}" alt="" style="height:46px;width:36px"></a>
                                    </div>
                                    <div class="media-body ml-3">
                                        <h6 class="mb-0 font-weight-semi-bold"><a class="text-900" href="${DownloadFileLink}"data-fancybox="group" data-type="iframe">${report.PeriodType} ${report.Year}</a></h6>
                                        <p class="text-500 fs--2 mb-0">Created <span class="ml-2 d-inline-block">${moment(report.CreatedDate).format("MMMM DD, YYYY")}</span></p>
                                    </div>
                                </div>`;
    return reportHTML;
}
var createTwilioUser = function () {
    postAjax("/Twilio/CreateTwilioUser", null, function () { });
}
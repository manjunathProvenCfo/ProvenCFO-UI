///import { debug } from "util";

function LineChart(XseriesData,YseriesData,chartControlerId, optioncontoleid) {
    var Events = {
        CHANGE: 'change'
    };
    const $echartsLineTotalSales = document.querySelector(chartControlerId);
    const months = [
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec",
    ];
    var ConvertToUDS = function (inputAmount) {
        var usdAmount = new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(inputAmount);
        return usdAmount;
    }
    function getFormatter(params) {
      
        const { name, value } = params[0];
        var date = new Date(name);
        return `${name} , ${ConvertToUDS(value)}`;//`${months[0]} ${date.getDate()}, ${value}`;
    }
    if ($echartsLineTotalSales) {
        const $this = $($echartsLineTotalSales);

        // Get options from data attribute
        const userOptions = $this.data("options");
        const chart = window.echarts.init($echartsLineTotalSales);
        //const monthsnumber = [
        //    [60, 80, 60, 80, 65, 130, 120, 100, 30, 40, 30, 70],
        //    [100, 70, 80, 50, 120, 100, 130, 140, 90, 100, 40, 50],
        //    [80, 50, 60, 40, 60, 120, 100, 130, 60, 80, 50, 60],
        //    [70, 80, 100, 70, 90, 60, 80, 130, 40, 60, 50, 80],
        //    [90, 40, 80, 80, 100, 140, 100, 130, 90, 60, 70, 50],
        //    [80, 60, 80, 60, 40, 100, 120, 100, 30, 40, 30, 70],
        //    [20, 40, 20, 50, 70, 60, 110, 80, 90, 30, 50, 50],
        //    [60, 70, 30, 40, 80, 140, 80, 140, 120, 130, 100, 110],
        //    [90, 90, 40, 60, 40, 110, 90, 110, 60, 80, 60, 70],
        //    [50, 80, 50, 80, 50, 80, 120, 80, 50, 120, 110, 110],
        //    [60, 90, 60, 70, 40, 70, 100, 140, 30, 40, 30, 70],
        //    [20, 40, 20, 50, 30, 80, 120, 100, 30, 40, 30, 70],
        //];
        const defaultOptions = {
            color: utils.grays.white,
            tooltip: {
                trigger: "axis",
                padding: [7, 10],
                backgroundColor: utils.grays.white,
                borderColor: utils.grays["300"],
                borderWidth: 1,
                textStyle: { color: utils.colors.dark },
                formatter(params) {
                    return getFormatter(params);
                },
                transitionDuration: 0,
                position(pos, params, dom, rect, size) {
                    return getPosition(pos, params, dom, rect, size);
                },
            },
            xAxis: {
                type: "category",
                data: XseriesData,
                boundaryGap: false,
                axisPointer: {
                    lineStyle: {
                        color: utils.grays["300"],
                        type: "dashed",
                    },
                },
                splitLine: { show: false },
                axisLine: {
                    lineStyle: {
                        // color: utils.grays['300'],
                        color: utils.rgbaColor("#000", 0.01),
                        type: "dashed",
                    },
                },
                axisTick: { show: false },
                axisLabel: {
                    color: utils.grays["400"],
                    formatter: function (value) {
                        var date = new Date(value);
                        
                        return value;//`${months[date.getMonth()]} ${date.getDate()}`;
                    },
                    margin: 15,
                },
            },
            yAxis: {
                type: "value",
                axisPointer: { show: false },
                splitLine: {
                    lineStyle: {
                        color: utils.grays["300"],
                        type: "dashed",
                    },
                },
                boundaryGap: false,
                axisLabel: {
                    show: true,
                    color: utils.grays["400"],
                    margin: -5,
                },
                axisTick: { show: false },
                axisLine: { show: false },
            },
            series: [
                {
                    type: "line",
                    data: YseriesData,
                    lineStyle: { color: utils.colors.primary },
                    itemStyle: {
                        borderColor: utils.colors.primary,
                        borderWidth: 2,
                    },
                    symbol: "circle",
                    symbolSize: 10,
                    smooth: false,
                    hoverAnimation: true,
                    areaStyle: {
                        color: {
                            type: "linear",
                            x: 0,
                            y: 0,
                            x2: 0,
                            y2: 1,
                            colorStops: [
                                {
                                    offset: 0,
                                    color: utils.rgbaColor(utils.colors.primary, 0.2),
                                },
                                {
                                    offset: 1,
                                    color: utils.rgbaColor(utils.colors.primary, 0),
                                },
                            ],
                        },
                    },
                },
            ],
            grid: { right: "28px", left: "40px", bottom: "15%", top: "5%" },
        };

        const options = window._.merge(defaultOptions, userOptions);
        chart.setOption(options);

        // Change chart options accordiong to the selected month
        //utils.$document.on(Events.CHANGE, optioncontoleid, (e) => {
        //    const $field = $(e.target);
        //    const month = $field.val();
        //   // const data = monthsnumber[month];
        //    const data = YseriesData[month];
           

        //    chart.setOption({
        //        tooltip: {
        //            formatter: function (params) {
        //                const { name, value } = params[0];
        //                var date = new Date(name);
        //                return value; //`${months[month]} ${date.getDate()}, ${value}`;
        //            },
        //        },
        //        xAxis: {
        //            axisLabel: {
        //                formatter: function (value) {
        //                    var date = new Date(value);
        //                    return `${YseriesData[$field.val()]} ${date.getDate()}`; //`${months[$field.val()]} ${date.getDate()}`;
        //                },
        //                margin: 15,
        //            },
        //        },
        //        series: [{ data: data }],
        //    });
        //});
    }

}
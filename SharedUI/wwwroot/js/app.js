function scrollContainer(direction) {
    const container = document.querySelector('.horizontal-scroll-container');
    const scrollAmount = 150; 
    container.scrollBy({
        left: direction * scrollAmount,
        behavior: 'smooth'
    });
}

window.onscroll = function () {
    var showAfter = 150;
    var element = document.getElementById('btn-back-to-top');
    if (window.scrollY > showAfter)
        element.style.display = 'block';
    else
        element.style.display = 'none';
};
async function drawmap(container, tradedata) {
    const topology = await fetch(
        '/js/world.topo.json'
    ).then(response => response.json());

    // Load the data from the HTML table and tag it with an upper case name used
    // for joining
    var data = tradedata.map(function (item) {
        return {
            key: item.title2,
            value: item.value3,
            importvalue: persianJs(item.value1.toLocaleString()).englishNumber(),
            exportvalue: persianJs(item.value2.toLocaleString()).englishNumber(),
            country: item.title1
        };
    });

    // Initialize the chart
    Highcharts.mapChart(container, {
        chart: {
            map: topology
        },
        title: {
            text: ''
        },

        subtitle: {
            text: ''
        },

        mapNavigation: {
            enabled: true,
            buttonOptions: {
                verticalAlign: 'top'
            }
        },
        lang: {
            exitFullScreen: "خروج از حالت تمام صفحه",
            viewFullscreen: "نمایش تمام صفحه",
            downloadPNG: 'دانلود png',
            downloadJPG: 'دانلود jpg',
            downloadSVG: 'دانلود svg',
            printChart: 'چاپ'
        },

        exporting: {
            fallbackToExportServer: false,
            //pdfFont: {
            //    normal:     '../fonts/Vazir-Regular.ttf',
            //    bold: '../fonts/Vazir-Bold.ttf',
            //    bolditalic: '../fonts/Vazir-Light.ttf',
            //    italic: '../fonts/Vazir-Thin.ttf'

            //        },
            buttons: {
                contextButton: {
                    menuItems: ["viewFullscreen", "separator", "downloadPNG", "downloadSVG", "downloadJPG", "separator", "printChart"]
                }, formAttributes: {
                    encoding: 'UTF-8'
                }
            }
        },
        //colors: ['rgba(19,64,117,0.05)', 'rgba(19,64,117,0.2)', 'rgba(19,64,117,0.4)',
        //    'rgba(19,64,117,0.5)', 'rgba(19,64,117,0.6)', 'rgba(19,64,117,0.8)', 'rgba(19,64,117,1)'],
        colors: ['#ff626a', '#e5c2b0', '#e9e036',
            '#9fccba', '#00b469'],
        legend: {
            title: {
                text: 'مجموع صادرات و واردات به دلار',
                style: {
                    color: ( // theme
                        Highcharts.defaultOptions &&
                        Highcharts.defaultOptions.legend &&
                        Highcharts.defaultOptions.legend.title &&
                        Highcharts.defaultOptions.legend.title.style &&
                        Highcharts.defaultOptions.legend.title.style.color
                    ) || 'black',



                }
            },

            align: 'left',
            verticalAlign: 'bottom',
            floating: true,
            layout: 'vertical',
            valueDecimals: 0,
            backgroundColor: ( // theme
                Highcharts.defaultOptions &&
                Highcharts.defaultOptions.legend &&
                Highcharts.defaultOptions.legend.backgroundColor
            ) || 'rgba(255, 255, 255, 0.85)',
            symbolRadius: 0,
            symbolHeight: 10
        },
        mapView: {
            projection: {
                name: 'LambertConformalConic'
            }
        },
        xAxis: {
            labels: {
                enabled: false
            }
        },
        tooltip: {
            useHTML: true,
            headerFormat: '',
            pointFormat:
                '<div class="d-flex flex-column justify-content-evenly map-tooltip-box"><div class="d-flex flex-row-reverse justify-content-between"><h4>{point.country}</h4><img class="rounded-pill" src="./img/countriesFlag/4x3/{point.key}.svg" style="width:4rem;height:3rem"></div><div class="d-flex flex-row-reverse text-danger"><div >صادرات به ایران : <strong>{point.importvalue}</strong></div></div><div class="d-flex flex-row-reverse text-success"><div >واردات از ایران : <strong>{point.exportvalue}</strong></div></div></div>'
            //'<div class="d-flex flex-column justify-content-evenly map-tooltip-box">' +
            //'<div class="d-flex flex-row-reverse justify-content-between text-start"><h4>' + this.point.country + '</h4>' +
            //'<img class="rounded-pill" src="https://countryflagsapi.com/svg/' + this.point.key + '" style="width:4rem;height:3rem"></div>' +
            //'<div class="d-flex flex-row-reverse text-danger text-start"><div >صادرات به ایران : <strong>' + persianJs(this.point.exportvalue.toLocaleString()).englishNumber() + '</strong></div></div>' +
            //'<div class="d-flex flex-row-reverse text-success text-end"><div >واردات از ایران : <strong>' + persianJs(this.point.importvalue.toLocaleString()).englishNumber() + '</strong></div></div></div>'
        },


        colorAxis: {
            dataClasses: [{
                to: 10000000,
                name: 'تا ده میلیون دلار',
                color: '#ffa2a7'
            },
            //{
            //from: 1000000,
            //to: 10000000,
            //name: 'یک تا ده میلیون دلار',
            //color: '#ff626a'

            //},
            {
                from: 10000000,
                to: 100000000,
                name: 'ده تا صد میلیون دلار',
                color: '#d2f4e6'

            }, {
                from: 100000000,
                to: 1000000000,
                name: 'صد میلیون تا یک میلیارد دلار',
                color: '#9fccba'

            }, {
                from: 1000000000,
                name: 'بیش از یک میلیارد دلار',
                color: '#00b469'

            }]


        },

        series: [{
            data: data,
            joinBy: ['iso-a2', 'key'],
            animation: true,
            states: {
                hover: {
                    color: '#bdbdbd'

                }
            },
            dataLabels: {
                enabled: true,
                formatter: function () {
                    return this.point.key;
                },
                style: {
                    fontWeight: 100,
                    fontSize: '9px',
                    textOutline: 'none'
                }
            }
            //,
            //tooltip: {
            //    formatter: function () {
            //        return this.point.value;
            //    }
            //}
        }]
    });
}
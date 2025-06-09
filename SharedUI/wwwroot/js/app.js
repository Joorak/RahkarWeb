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


 async function drawLeasingWorldMap(container, turnoverData) {
    //const topology = await fetch('/js/world.topo.json').then(response => response.json());
    const topology = await fetch('_content/SharedUI/js/world.topo.json').then(response => response.json());

    var data = turnoverData.map(function (item) {
        return {
            key: item.isO2,
            value: item.leasingVolume,
            percent: item.marketShare,
            country: item.name
        };
    });

    // Create the chart
     Highcharts.mapChart(container, {
        accessibility:
        {
            enabled: false
        },
        chart: {
            map: topology,
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
             buttons: {
                 contextButton: {
                     menuItems: ["viewFullscreen", "separator", "downloadPNG", "downloadSVG", "downloadJPG", "separator", "printChart"]
                 }, formAttributes: {
                     encoding: 'UTF-8'
                 }
             }
         },
        colors: ['#ff626a', '#e5c2b0', '#e9e036',
             '#9fccba', '#00b469'],
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
                 `<div class="d-flex flex-column justify-content-evenly map-tooltip-box">
                    <div class="d-flex flex-row-reverse justify-content-between">
                        <h4>{point.country}</h4>
                        <img class="rounded-pill" src="./_content/SharedUI/img/countriesFlag/4x3/{point.iso-a2}.svg" style="width:4rem;height:3rem">
                    </div>
                    <div class="d-flex flex-row-reverse text-danger">
                        <div >حجم عملیات لیزینگ : <strong>{point.value}</strong>(میلیون دلار)</div>
                    </div>
                    <div class="d-flex flex-row-reverse text-success">
                        <div >درصد به کل : <strong>{point.percent}</strong> درصد</div>
                    </div>
                </div>`
         },


         colorAxis: {
             dataClasses: [{
                 to: 10000,
                 name: 'تا ده میلیارد دلار',
                 color: '#ffa2a7'
             },
             {
                 from: 10000,
                 to: 100000,
                 name: 'ده تا صد میلیارد دلار',
                 color: '#d2f4e6'

             }, {
                 from: 100000,
                 to: 1000000,
                 name: 'صد تا هزار میلیارد دلار',
                 color: '#9fccba'

             }, {
                 from: 1000000,
                 name: 'بیش از هزار میلیارد دلار',
                 color: '#00b469'

             }]


         },
        series: [{
            name: '',
            data: data,
            joinBy: ['iso-a2', 'key'],
            animation: true,
            states: {
                hover: {
                    color: '#BADA55'
                }
            },
            dataLabels: {
                enabled: true,
                formatter: function () {
                    if(this.point.value > 0)
                        return this.point["iso-a2"];
                },
                style: {
                    fontWeight: 100,
                    fontSize: '9px',
                    textOutline: 'none'
                }
            },

            showInLegend: false

        }]
    });
}



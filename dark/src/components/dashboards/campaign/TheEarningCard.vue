<script setup lang="ts">
import { ref, computed } from 'vue';

/*Chart 1*/
const Chart1 = computed(() => {
    return {
        series: [
            {
                name: '',
                data: [4, 5, 2, 10, 9, 12, 4, 9]
            }
        ],
        colors: ['rgba(var(--v-theme-secondary))'],
        plotOptions: {
            bar: {
                horizontal: false,
                startingShape: 'flat',
                endingShape: 'flat',
                columnWidth: '95%',
                barHeight: '100%',
                borderRadius: 4
            }
        },
        chart: {
            toolbar: {
                show: false
            },
            sparkline: { enabled: true }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 4,
            colors: ['transparent']
        },
        xaxis: {
            show: false,
            labels: {
                show: false
            }
        },
        yaxis: {
            show: false
        },
        grid: {
            show: false,
            stroke: {
                show: false
            },
            padding: {
                top: 0,
                bottom: 0
            }
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            enabled: true,
            theme: 'dark',
            x: {
                show: false
            }
        }
    };
});

/*Chart 2*/
const Chart2 = computed(() => {
    return {
        series: [
            {
                name: '',
                data: [0, 5, 6, 10, 9, 12, 4, 9]
            }
        ],
        colors: ['#7460ee'],
        plotOptions: {
            bar: {
                horizontal: false,
                startingShape: 'flat',
                endingShape: 'flat',
                columnWidth: '95%',
                barHeight: '100%',
                borderRadius: 4
            }
        },
        chart: {
            toolbar: {
                show: false
            },
            sparkline: { enabled: true }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 4,
            colors: ['transparent']
        },
        xaxis: {
            show: false,
            labels: {
                show: false
            }
        },
        yaxis: {
            show: false
        },
        grid: {
            show: false,
            stroke: {
                show: false
            },
            padding: {
                top: 0,
                bottom: 0
            }
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            enabled: true,
            theme: 'dark',
            x: {
                show: false
            }
        }
    };
});

/*Chart 3*/
const Chart3 = computed(() => {
    return {
        series: [
            {
                name: '',
                data: [0, 5, 6, 10, 9, 12, 4, 9]
            }
        ],
        colors: ['rgba(var(--v-theme-secondary))'],
        plotOptions: {
            bar: {
                horizontal: false,
                startingShape: 'flat',
                endingShape: 'flat',
                columnWidth: '95%',
                barHeight: '100%',
                borderRadius: 4
            }
        },
        chart: {
            toolbar: {
                show: false
            },
            sparkline: { enabled: true }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 4,
            colors: ['transparent']
        },
        xaxis: {
            show: false,
            labels: {
                show: false
            }
        },
        yaxis: {
            show: false
        },
        grid: {
            show: false,
            stroke: {
                show: false
            },
            padding: {
                top: 0,
                bottom: 0
            }
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            enabled: true,
            theme: 'dark',
            x: {
                show: false
            }
        }
    };
});

/*Chart 4*/
const Chart4 = computed(() => {
    return {
        series: [
            {
                name: '',
                data: [0, 5, 6, 10, 9, 12, 4, 9]
            }
        ],
        colors: ['rgba(var(--v-theme-error))'],
        plotOptions: {
            bar: {
                horizontal: false,
                startingShape: 'flat',
                endingShape: 'flat',
                columnWidth: '95%',
                barHeight: '100%',
                borderRadius: 4
            }
        },
        chart: {
            toolbar: {
                show: false
            },
            sparkline: { enabled: true }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 4,
            colors: ['transparent']
        },
        xaxis: {
            show: false,
            labels: {
                show: false
            }
        },
        yaxis: {
            show: false
        },
        grid: {
            show: false,
            stroke: {
                show: false
            },
            padding: {
                top: 0,
                bottom: 0
            }
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            enabled: true,
            theme: 'dark',
            x: {
                show: false
            }
        }
    };
});
</script>
<template>
    <v-card elevation="10" class="text-center earning-cards">
        <div class="d-flex flex-wrap">
            <div class="w-25 border-e">
                <div class="py-4 pb-2">
                    <div>
                        <h3 class="text-h5 font-weight-medium">Unique Visit</h3>
                        <div class="d-flex justify-center mt-3">
                            <apexchart type="bar" height="70" width="120" :options="Chart1" :series="Chart1.series"> </apexchart>
                        </div>
                        <div class="text-center border-t mt-4 pt-2 mobile-border">
                            <h4 class="mb-0 text-h5">12456</h4>
                        </div>
                    </div>
                </div>
            </div>
            <div class="w-25 border-e">
                <div class="py-4 pb-2">
                    <div>
                        <h3 class="text-h5 font-weight-medium">Total Visit</h3>
                        <div class="d-flex justify-center mt-3">
                            <apexchart type="bar" height="70" width="120" :options="Chart2" :series="Chart2.series"> </apexchart>
                        </div>
                        <div class="text-center border-t mt-4 pt-2 mobile-border">
                            <h4 class="mb-0 text-h5">11456</h4>
                        </div>
                    </div>
                </div>
            </div>
            <div class="w-25 border-e">
                <div class="py-4 pb-2">
                    <div>
                        <h3 class="text-h5 font-weight-medium">Bounce rate</h3>
                        <div class="d-flex justify-center mt-3">
                            <apexchart type="bar" height="70" width="120" :options="Chart3" :series="Chart3.series"> </apexchart>
                        </div>
                        <div class="text-center border-t mt-4 pt-2 mobile-border">
                            <h4 class="mb-0 text-h5">12456</h4>
                        </div>
                    </div>
                </div>
            </div>
            <div class="w-25">
                <div class="py-4 pb-2">
                    <div>
                        <h3 class="text-h5 font-weight-medium">Page Views</h3>
                        <div class="d-flex justify-center mt-3">
                            <apexchart type="bar" height="70" width="120" :options="Chart4" :series="Chart4.series"> </apexchart>
                        </div>
                        <div class="text-center border-t mt-4 pt-2">
                            <h4 class="mb-0 text-h5">13456</h4>
                        </div>
                    </div>
                </div>
            </div>
            
        </div>
    </v-card>
</template>

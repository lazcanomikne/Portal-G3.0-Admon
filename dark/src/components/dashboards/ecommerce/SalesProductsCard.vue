<script setup lang="ts">
import { ref, computed } from 'vue';
/*Chart*/
const chartOptions = computed(() => {
    return {
        series: [
            {
                name: '',
                data: [4, 5, 2, 10, 9, 12, 4, 9, 4, 5, 3, 10]
            }
        ],
        chart: {
            type: 'bar',
            fontFamily: 'inherit',
            height: 60,
            toolbar: {
                show: false
            },
            sparkline: {
                enabled: true
            }
        },
        colors: ['rgba(var(--v-theme-primary))'],
        grid: {
            show: false
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '65%',
                borderRadius: 2,
                endingShape: 'rounded'
            }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 6,
            colors: ['transparent']
        },
        xaxis: {
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            labels: {
                show: false
            }
        },
        yaxis: {
            labels: {
                show: false
            }
        },
        axisBorder: {
            show: false
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            theme: 'dark',
            style: {
                fontSize: '12px',
                fontFamily: 'inherit'
            },
            x: {
                show: false
            },
            y: {
                formatter: undefined
            }
        }
    };
});

const chartOptions2 = computed(() => {
    return {
        series: [
            {
                name: '',
                data: [4, 5, 2, 10, 9, 12, 4, 9, 4, 5, 3, 10]
            }
        ],
        chart: {
            type: 'bar',
            fontFamily: 'inherit',
            height: 60,
            toolbar: {
                show: false
            },
            sparkline: {
                enabled: true
            }
        },
        colors: ['rgba(var(--v-theme-secondary))'],
        grid: {
            show: false
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '65%',
                borderRadius: 2,
                endingShape: 'rounded'
            }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 6,
            colors: ['transparent']
        },
        xaxis: {
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            labels: {
                show: false
            }
        },
        yaxis: {
            labels: {
                show: false
            }
        },
        axisBorder: {
            show: false
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            theme: 'dark',
            style: {
                fontSize: '12px',
                fontFamily: 'inherit'
            },
            x: {
                show: false
            },
            y: {
                formatter: undefined
            }
        }
    };
});

const chartOptions3 = computed(() => {
    return {
        series: [
            {
                name: '',
                data: [4, 5, 2, 10, 9, 12, 4, 9, 4, 5, 3, 10]
            }
        ],
        chart: {
            type: 'bar',
            fontFamily: 'inherit',
            height: 60,
            toolbar: {
                show: false
            },
            sparkline: {
                enabled: true
            }
        },
        colors: ['rgba(var(--v-theme-error))'],
        grid: {
            show: false
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '65%',
                borderRadius: 2,
                endingShape: 'rounded'
            }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 6,
            colors: ['transparent']
        },
        xaxis: {
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            labels: {
                show: false
            }
        },
        yaxis: {
            labels: {
                show: false
            }
        },
        axisBorder: {
            show: false
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            theme: 'dark',
            style: {
                fontSize: '12px',
                fontFamily: 'inherit'
            },
            x: {
                show: false
            },
            y: {
                formatter: undefined
            }
        }
    };
});
</script>
<template>

        <v-col cols="12" lg="4">
            <VCard elevation="10" class="overflow-hidden">
                <v-card-text>
                    <h3 class="card-title mb-2">Product A Sales</h3>
                    <div class="d-flex ga-4">
                        <div>
                            <p class="textSecondary text-15">Monthly</p>
                            <h6 class="font-weight-medium text-subtitle-1">20.40%</h6>
                        </div>
                        <div>
                            <p class="textSecondary text-15">Daily</p>
                            <h6 class="font-weight-medium text-subtitle-1">5.40%</h6>
                        </div>
                    </div>

                    <div class="mt-6 text-center w-100">
                        <apexchart
                            type="bar"
                            height="60"
                            :options="chartOptions"
                            :series="chartOptions.series"
                            class="d-flex justify-center"
                        ></apexchart>
                    </div>
                </v-card-text>
            </VCard>
        </v-col>
        <v-col cols="12" lg="4">
            <VCard elevation="10" class="overflow-hidden">
                <v-card-text>
                    <h3 class="card-title mb-2">Product B Sales</h3>
                    <div class="d-flex ga-4">
                        <div>
                            <p class="textSecondary text-15">Monthly</p>
                            <h6 class="font-weight-medium text-subtitle-1">20.40%</h6>
                        </div>
                        <div>
                            <p class="textSecondary text-15">Daily</p>
                            <h6 class="font-weight-medium text-subtitle-1">5.40%</h6>
                        </div>
                    </div>

                    <div class="mt-6 text-center w-100">
                        <apexchart
                            type="bar"
                            height="60"
                            :options="chartOptions2"
                            :series="chartOptions2.series"
                            class="d-flex justify-center"
                        ></apexchart>
                    </div>
                </v-card-text>
            </VCard>
        </v-col>
        <v-col cols="12" lg="4">
            <VCard elevation="10" class="overflow-hidden">
                <v-card-text>
                    <h3 class="card-title mb-2">Product C Sales</h3>
                    <div class="d-flex ga-4">
                        <div>
                            <p class="textSecondary text-15">Monthly</p>
                            <h6 class="font-weight-medium text-subtitle-1">20.40%</h6>
                        </div>
                        <div>
                            <p class="textSecondary text-15">Daily</p>
                            <h6 class="font-weight-medium text-subtitle-1">5.40%</h6>
                        </div>
                    </div>

                    <div class="mt-6 text-center w-100">
                        <apexchart
                            type="bar"
                            height="60"
                            :options="chartOptions3"
                            :series="chartOptions3.series"
                            class="d-flex justify-center"
                        ></apexchart>
                    </div>
                </v-card-text>
            </VCard>
        </v-col>


</template>

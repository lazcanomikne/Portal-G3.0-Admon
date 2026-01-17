<script setup lang="ts">
import { ChartBarIcon, ChartLineIcon } from 'vue-tabler-icons';
import { ref, computed } from 'vue';

/*Chart*/
const chartOptions = computed(() => {
    return {
        series: [
            {
                name: 'Growth ',
                data: [0, 1, 1, 10, 24, 6, 12, 4, 21, 15, 44, 24, 28, 4, 10, 21, 5, 47]
            },
            {
                name: 'Loss ',
                data: [0, 4, 3, 24, 9, 10, 18, 15, 44, 17, 19, 26, 31, 8, 37, 10, 24, 51]
            }
        ],
        chart: {
            height: 350,
            type: 'area',
            stacked: false,
            fontFamily: 'inherit',
            zoom: {
                enabled: false
            },
            toolbar: {
                show: false
            }
        },
        colors: ['rgba(var(--v-theme-secondary),0.9)', 'rgba(var(--v-theme-secondary),0.1)'],
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: false
        },
        markers: {
            size: 2,
            strokeColors: 'transparent',
            colors: '#26c6da'
        },
        fill: {
            type: 'solid',
            colors: ['rgba(var(--v-theme-secondary),0.9)', 'rgba(var(--v-theme-secondary),0.4)'],
            opacity: 1
        },
        grid: {
            strokeDashArray: 3,
            borderColor: 'rgba(0,0,0,0.2)'
        },
        xaxis: {
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            categories: ['0', '4', '8', '12', '16', '20', '24', '30', '16', '20', '24', '30', '34', '38', '42', '46', '50', '54'],
            labels: {
                style: {
                    colors: '#a1aab2'
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: '#a1aab2'
                }
            }
        },
        legend: {
            show: false
        },
        tooltip: {
            theme: 'dark',
            marker: {
                show: true
            }
        }
    };
});
</script>

<template>
    <VCard elevation="10" class="mt-md-3">
        <v-card-text class="py-md-0 px-0">
            <v-row class="d-sm-flex align-center justify-space-between border-b px-6 pb-md-0 pb-4">
                <v-col cols="12" lg="6">
                    <h2 class="card-title mb-1">Sales of Ample Vs Pixel</h2>
                    <h5 class="card-subtitle">Check the difference of Admin</h5>
                </v-col>
                <v-col cols="12" lg="6">
                    <v-row>
                        <v-col cols="12" lg="6" class="border-e border-m-none">
                            <div class="d-flex align-center justify-space-between pa-md-6">
                                <div>
                                    <h3 class="text-h3">$31568</h3>
                                    <p class="text-subtitle-1 textSecondary">Growth</p>
                                </div>
                                <v-avatar size="45" color="success">
                                    <ChartLineIcon size="25" />
                                </v-avatar>
                            </div>
                        </v-col>
                        <v-col cols="12" lg="6">
                            <div class="d-flex align-center justify-space-between pa-md-6">
                                <div>
                                    <h3 class="text-h3">$15478</h3>
                                    <p class="text-subtitle-1 textSecondary">Loss</p>
                                </div>
                                <v-avatar size="45" color="warning">
                                    <ChartBarIcon size="25" />
                                </v-avatar>
                            </div>
                        </v-col>
                    </v-row>
                </v-col>
            </v-row>
        </v-card-text>
        <v-card-text>
            <apexchart type="area" height="350" :options="chartOptions" :series="chartOptions.series"></apexchart>
        </v-card-text>
    </VCard>
</template>

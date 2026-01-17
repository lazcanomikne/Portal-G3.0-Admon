<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
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
            height: '50px',
            foreColor: '#adb0bb',
            toolbar: {
                show: false
            },
            sparkline: {
                enabled: true
            }
        },
        colors: ['rgba(255, 255, 255, 0.7)'],
        grid: {
            show: false
        },
        plotOptions: {
            bar: {
                horizontal: false,
                startingShape: 'flat',
                endingShape: 'flat',
                columnWidth: '60%',
                barHeight: '100%',
                borderRadius: 2
            }
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
    <VCard elevation="10" class="overflow-hidden">
        <v-card-text class="bg-secondary pa-7">
            <div class="d-flex align-center">
                <v-avatar class="bg-white-opacity" size="48">
                    <Icon icon="solar:chart-2-linear" class="text-white" height="25" />
                </v-avatar>
                <div class="mx-4">
                    <h3 class="text-h5  mb-1">Download Count</h3>
                    <div class="text-subtitle-1 opacity-80">March 2021</div>
                </div>
            </div>
            <v-row class="pt-10 d-flex justify-space-between">
                <v-col cols="5" class="d-flex align-center">
                    <h2 class="font-weight-medium text-h2">35487</h2>
                </v-col>
                <v-col cols="7" class="d-sm-flex">
                    <div  class="ms-auto">
                        <apexchart type="bar" height="80"  :options="chartOptions" :series="chartOptions.series"></apexchart>
                    </div>
                </v-col>
            </v-row>
        </v-card-text>
    </VCard>
</template>

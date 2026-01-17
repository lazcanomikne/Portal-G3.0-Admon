<script setup lang="ts">
import { computed } from 'vue';
/*Chart*/
const chartOptions = computed(() => {
    return {
        series: [
            {
                name: 'Premium ',
                data: [5, 4, 3, 7, 5, 10, 3]
            },
            {
                name: 'Free ',
                data: [3, 2, 9, 5, 4, 6, 4]
            }
        ],
        chart: {
            fontFamily: 'inherit',
            type: 'bar',
            height: 350,
            offsetY: 10,
            offsetX:-15,
            toolbar: {
                show: false
            }
        },
        grid: {
            show: true,
            strokeDashArray: 3,
            borderColor: 'rgba(0,0,0,0.2)'
        },
        colors: ['rgba(var(--v-theme-indigo))', 'rgba(var(--v-theme-secondary))'],
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '35%',
                endingShape: 'flat',
                borderRadius: 4
            }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width:5,
            colors: ['transparent']
        },
        xaxis: {
            type: 'category',
            categories: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
            tickAmount: '16',
            tickPlacement: 'on',
            axisTicks: {
                show: false
            },
            labels: {
                style: {
                    colors: '#a1aab2'
                }
            },

            axisBorder: {
                show: false
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: '#a1aab2'
                }
            }
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            theme: 'dark'
        },
        legend: {
            show: false
        }
    };
});
</script>

<template>
    <!-- ------------------------------------ -->
    <!-- html -->
    <!-- ------------------------------------ -->
    <VCard elevation="10">
        <v-card-text>
            <div class="d-sm-flex align-center">
                <div>
                    <h3 class="card-title mb-1">Download Count</h3>
                    <h5 class="card-subtitle">you can check the count</h5>
                </div>
                <div class="ms-auto">
                    <div class="d-flex align-center">
                        <div class="d-flex align-center px-2">
                            <span class="h-10 bg-secondary rounded-circle me-2"></span>
                            <span class="font-weight-regular text-subtitle-1 text-secondary">Free</span>
                        </div>
                        <div class="d-flex align-center px-2">
                            <span class="h-10 bg-purple rounded-circle me-2"></span>
                            <span class="font-weight-regular text-subtitle-1 text-purple">Premium</span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="mt-5 me-n7 rtl-me-n7">
                <apexchart type="bar" height="350" :options="chartOptions" :series="chartOptions.series"></apexchart>
            </div>
        </v-card-text>
    </VCard>
</template>

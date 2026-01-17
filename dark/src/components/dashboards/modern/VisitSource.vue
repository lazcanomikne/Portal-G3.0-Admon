<script setup lang="ts">
import { ref, computed } from 'vue';
/*Chart*/
const chartOptions = computed(() => {
    return {
        labels: ['Desktop', 'Tablet', 'Mobile', 'Other'],
        series: [100, 40, 80, 90],
        chart: {
            type: 'donut',
            height: 230,
            fontFamily: 'inherit'
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            width: 0
        },

        plotOptions: {
            pie: {
                expandOnClick: true,
                donut: {
                    size: '90%',
                    labels: {
                        show: true,
                        name: {
                            show: false,
                            offsetY: 7
                        },
                        value: {
                            show: true,
                            fontSize: '20px'
                        },
                        total: {
                            show: false,
                            color: '#a1aab2',
                            fontSize: '13px',
                            label: 'Visits'
                        }
                    }
                }
            }
        },
        colors: ['rgba(var(--v-theme-indigo))', 'rgba(var(--v-theme-error))', 'rgba(var(--v-theme-secondary))', '#dadada'],
        tooltip: {
            show: true,
            fillSeriesColor: false
        },
        legend: {
            show: false
        },
        responsive: [
            {
                breakpoint: 480,
                options: {
                    chart: {
                        width: 200
                    }
                }
            }
        ]
    };
});
const select = ref('January');
const items = ref(['January', 'Febuary', 'March', 'April']);
</script>
<template>
    <VCard elevation="10">
        <v-card-text>
            <div class="d-flex align-center justify-space-between align-center">
                <h3 class="card-title mb-1">Visit source</h3>
                <div>
                    <v-select v-model="select" :items="items" variant="outlined" elevation="0" class="text-body-1" hide-details></v-select>
                </div>
            </div>

            <div class="mt-3 text-center  w-100">
                <apexchart type="donut" height="230" :options="chartOptions" :series="chartOptions.series" class="d-flex justify-center" ></apexchart>
            </div>
            <div class="d-flex align-center justify-center ga-1 mt-3">
                <div class="d-flex align-center px-2 text-indigo">
                    <span class="h-10 bg-indigo rounded-circle me-2"></span>
                    <span class="text-subtitle-1">Desktop</span>
                </div>
                <div class="d-flex align-center px-2 text-error">
                    <span class="h-10 bg-error rounded-circle me-2"></span>
                    <span class="text-subtitle-1">Tablet</span>
                </div>
                <div class="d-flex align-center px-2 text-secondary">
                    <span class="h-10 bg-secondary rounded-circle me-2"></span>
                    <span class="text-subtitle-1">Mobile </span>
                </div>
            </div>
        </v-card-text>
    </VCard>
</template>

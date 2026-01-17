<script setup lang="ts">
import { ref, computed } from 'vue';

/*Chart*/
const chartOptions = computed(() => {
    return {
        chart: {
            height: 150,
            type: 'radialBar',
            fontFamily: 'inherit',
            sparkline: {
                enabled: true
            }
        },
        series: [100],
        colors: ['rgba(var(--v-theme-primary))'],
        plotOptions: {
            radialBar: {
                startAngle: -135,
                endAngle: 135,
                track: {
                    background: '#f2f4f8',
                    startAngle: -135,
                    endAngle: 135
                },
                hollow: {
                    size: '30%',
                    background: 'transparent'
                },
                dataLabels: {
                    show: true,
                    name: {
                        show: false
                    },
                    value: {
                        show: false
                    },
                    total: {
                        show: true,
                        fontSize: '20px',
                        color: '#000',
                        label: '91.4 %'
                    }
                }
            }
        },
        grid: {
            padding: {
                top: 20
            }
        },
        fill: {
            type: 'solid'
        },
        stroke: {
            lineCap: 'butt'
        },
        tooltip: {
            enabled: true,
            fillSeriesColor: false,
            theme: 'dark'
        },
        labels: ['Sales Prediction ']
    };
});
</script>
<template>
    <v-card elevation="10">
        <v-card-text class="pb-7">
            <h3 class="card-title mb-1">Sales Prediction</h3>
            <v-row class="mt-3">
                <v-col cols="7">
                    <span class="display-2 text-primary">$3528</span>
                    <h6 class="text-subtitle-1 my-1 textSecondary font-weight-medium">10% Increased</h6>
                    <h5 class="text-16 font-weight-medium text-no-wrap mt-1">(150-165 Sales)</h5>
                </v-col>
                <v-col cols="5">
                    <div class="ms-auto mt-n8">
                        <apexchart height="150" :options="chartOptions" :series="chartOptions.series"></apexchart>
                    </div>
                </v-col>
            </v-row>
        </v-card-text>
    </v-card>
</template>

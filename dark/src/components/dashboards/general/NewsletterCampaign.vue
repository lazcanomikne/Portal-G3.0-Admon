<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';

import Customizer from '@/layouts/full/customizer/Customizer.vue';
/*Chart*/
const chartOptions = computed(() => {
    return {
        series: [
            { name: 'Open Rate', data: [65, 80, 80, 60, 60, 45, 45, 80, 80, 70, 70, 90, 90, 80, 80, 80, 60, 60, 50] },
            { name: 'Recurring Payments', data: [90, 110, 110, 95, 95, 85, 85, 95, 95, 115, 115, 100, 100, 115, 115, 95, 95, 85, 85] }
        ],
        chart: { fontFamily: 'inherit', type: 'area', height: 300, offsetX: 0, toolbar: { show: !1 } },
        plotOptions: {},
        legend: { show: !1 },
        dataLabels: { enabled: !1 },
        fill: { type: 'solid', opacity: 0.07, colors: ['#1B84FF', '#43CED7'] },
        stroke: { curve: 'smooth', show: !0, width: 2, colors: ['rgba(var(--v-theme-primary))', 'rgba(var(--v-theme-secondary))'] },
        xaxis: {
            categories: [
                '',
                '8 AM',
                '81 AM',
                '9 AM',
                '10 AM',
                '11 AM',
                '12 PM',
                '13 PM',
                '14 PM',
                '15 PM',
                '16 PM',
                '17 PM',
                '18 PM',
                '18:20 PM',
                '18:20 PM',
                '19 PM',
                '20 PM',
                '21 PM',
                ''
            ],
            axisBorder: { show: !1 },
            axisTicks: { show: !1 },
            tickAmount: 6,
            labels: { rotate: 0, rotateAlways: !0, style: { fontSize: '12px', colors: '#a1aab2' } },
            crosshairs: { position: 'front', stroke: { color: ['rgba(var(--v-theme-primary))', 'rgba(var(--v-theme-secondary))'], width: 1, dashArray: 3 } }
        },
        yaxis: { max: 120, min: 30, tickAmount: 6, labels: { style: { fontSize: '12px', colors: '#a1aab2' } } },
        states: {
            normal: { filter: { type: 'none', value: 0 } },
            hover: { filter: { type: 'none', value: 0 } },
            active: { allowMultipleDataPointsSelection: !1, filter: { type: 'none', value: 0 } }
        },
        tooltip: {
            theme: 'dark'
        },
        colors: ['rgba(var(--v-theme-primary))', 'rgba(var(--v-theme-secondary))'],
        grid: { borderColor: 'rgba(var(--v-theme-borderColor))', strokeDashArray: 4, yaxis: { lines: { show: !0 } } },
        markers: { strokeColor: ['rgba(var(--v-theme-primary))', 'rgba(var(--v-theme-secondary))'], strokeWidth: 3 }
    };
});

const elementVisible = ref(false);
onMounted(() => {
    setTimeout(() => (elementVisible.value = true), 10);
});
</script>

<template>
    <VCard elevation="10">
        <v-card-text>
            <div class="d-sm-flex align-center">
                <div>
                    <h3 class="card-title mb-1">Newsletter Campaign</h3>
                    <h5 class="card-subtitle">Overview of Newsletter Campaign</h5>
                </div>
                <div class="ms-auto mt-sm-0 mt-3">
                    <div class="d-flex align-center">
                        <div class="d-flex align-center px-2">
                            <span class="h-10 bg-primary rounded-circle me-2"></span>
                            <span class="font-weight-regular text-subtitle-1 text-primary">Open Rate</span>
                        </div>
                        <div class="d-flex align-center px-2">
                            <span class="h-10 bg-secondary rounded-circle me-2"></span>
                            <span class="font-weight-regular text-subtitle-1 text-secondary">Recurring Payments</span>
                        </div>
                    </div>
                </div>
            </div>
            <div v-show="elementVisible" class="mt-6 ms-n3">
                <apexchart type="area" height="300" :options="chartOptions" :series="chartOptions.series"></apexchart>
            </div>
            <div class="pt-3 pb-3">
                <v-row>
                    <v-col cols="12" sm="4" class="text-center">
                        <h2 class="text-h2">5098</h2>
                        <p class="textSecondary text-subtitle-1">Total Sent</p>
                    </v-col>
                    <v-col cols="12" sm="4" class="text-center">
                        <h2 class="text-h2">4156</h2>
                        <p class="textSecondary text-subtitle-1">Mail Open Rate</p>
                    </v-col>
                    <v-col cols="12" sm="4" class="text-center">
                        <h2 class="text-h2">1369</h2>
                        <p class="textSecondary text-subtitle-1">Click Rate</p>
                    </v-col>
                </v-row>
            </div>
        </v-card-text>
    </VCard>
</template>

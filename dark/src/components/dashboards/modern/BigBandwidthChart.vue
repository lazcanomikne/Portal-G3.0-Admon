<script setup lang="ts">
import { ref,onMounted, computed } from "vue";
const chartOptions = computed(() => {
    return {
        series: [
      {
        name: "",
        labels: ["0", "4", "8", "12", "16", "20", "24", "30"],
        data: [0, 8, 12, 10, 6, 8, 15, 23],
      },
    ],
    chart: {
      height: '50px',
      type: "line",
      foreColor: "#adb0bb",
      toolbar: {
        show: false,
      },
      sparkline: {
        enabled: true,
      },
    },
    colors: ["#fff"],
    fill: {
      type: "solid",
      opacity: 1,
      colors: ["#fff"],
    },
    grid: {
      show: false,
    },
    stroke: {
      curve: "monotoneCubic",
      lineCap: "square",
      colors: ["#fff"],
      width: 2,
    },
    markers: {
      size: 0,
      colors: ["#fff"],
      strokeColors: "transparent",
      shape: "square",
      hover: {
        size: 7,
      },
    },
    xaxis: {
      axisBorder: {
        show: false,
      },
      axisTicks: {
        show: false,
      },
      labels: {
        show: false,
      },
    },
    yaxis: {
      labels: {
        show: false,
      },
    },
    tooltip: {
      theme: "dark",
      style: {
        fontSize: "10px",
        fontFamily: "inherit",
      },
      x: {
        show: false,
      },
      y: {
        formatter: undefined,
      },
      marker: {
        show: true,
      },
      followCursor: true,
    },
    }
});


</script>
<template>
    <VCard elevation="10" class="overflow-hidden"> 
        <v-card-text class="bg-purple pa-7">
            <div class="d-flex align-center">
                <v-avatar class="bg-dark" size="48">
                    <Icon icon="solar:server-square-linear" class="text-white" height="25" />
                </v-avatar>    
                <div class="mx-4">
                    <h3 class="text-h5 mb-1">Bandwidth usage</h3>
                    <div class="text-subtitle-1 opacity-80">March 2021</div>
                </div>
            </div>
            <v-row class="pt-11">
                <v-col cols="5" class="d-flex align-center">
                    <h2 class="font-weight-medium white--text text-nowrap text-h2">50 GB</h2>
                </v-col>
                <v-col cols="7">
                    <div class="float-right" >
                        <apexchart type="line" height="82" width="200" :options="chartOptions"
                            :series="chartOptions.series"></apexchart>
                    </div>
                </v-col>
            </v-row>
        </v-card-text>
    </VCard>
</template>

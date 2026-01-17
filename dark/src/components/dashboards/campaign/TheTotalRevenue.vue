<script setup lang="ts">
import { ref, computed } from "vue";

/*Chart*/
const chartOptions = computed(() => {
    return {
        series: [
      {
        name: "2024 ",
        data: [
          800000, 1200000, 1400000, 1300000, 1200000, 1400000, 1300000, 1300000,
          1200000,
        ],
      },
      {
        name: "2023 ",
        data: [
          200000, 400000, 500000, 300000, 400000, 500000, 300000, 300000,
          400000,
        ],
      },
      {
        name: "2022 ",
        data: [
          100000, 200000, 400000, 600000, 200000, 400000, 600000, 600000,
          200000,
        ],
      },
    ],
    chart: {
      fontFamily: "inherit",
      type: "bar",
      height: 335,
      stacked: true,
      offsetX:-15,
      toolbar: {
        show: false,
      },
      zoom: {
        enabled: true,
      },
    },
    grid: {
      borderColor: "rgba(0,0,0,0.1)",
      strokeDashArray: 3,
    },
    colors: ['rgba(var(--v-theme-primary))', 'rgba(var(--v-theme-secondary))', "rgba(var(--v-theme-error))"],
    responsive: [
      {
        breakpoint: 480,
        options: {
          legend: {
            position: "bottom",
            offsetX: -10,
            offsetY: 0,
          },
        },
      },
    ],
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: "15%",
        borderRadius: 6,
      },
    },
    dataLabels: {
      enabled: false,
    },
    xaxis: {
      categories: [
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sept",
      ],
      labels: {
        style: {
          colors: "#a1aab2",
        },
      },
    },
    yaxis: {
      labels: {
        style: {
          colors: "#a1aab2",
        },
      },
    },
    tooltip: {
      theme: "dark",
    },
    legend: {
      show: false,
    },
    fill: {
      opacity: 1,
    },
    }
});

</script>
<template>
    <v-card elevation="10">
        <v-card-text>
            <div class="d-sm-flex align-center">
                <div>
                    <h3 class="card-title mb-1">Total Revenue</h3>
                </div>
                <div class="ms-auto">
                    <div class="d-flex align-center">
                        <div class="d-flex align-center px-2 ">
                            <span class="h-10 bg-error rounded-circle me-2"></span>
                            <span class="font-weight-regular text-subtitle-1 text-error">2022</span>
                        </div>
                        <div class="d-flex align-center px-2 ">
                            <span class="h-10 bg-secondary rounded-circle me-2"></span>
                            <span class="font-weight-regular text-subtitle-1 text-secondary">2023</span>
                        </div>
                        <div class="d-flex align-center px-2 ">
                            <span class="h-10 bg-primary rounded-circle me-2"></span>
                            <span class="font-weight-regular text-subtitle-1 text-primary">2024</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <apexchart class="mt-3" type="bar" height="335" :options="chartOptions" :series="chartOptions.series">
            </apexchart>
        </v-card-text>
    </v-card>
</template>

  
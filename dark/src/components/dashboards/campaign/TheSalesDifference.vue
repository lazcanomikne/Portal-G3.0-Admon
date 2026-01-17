<script setup lang="ts">
import { ref, computed } from "vue";

/*Chart*/
const chartOptions = computed(() => {
    return {
        series: [25, 10],
    labels: ["A ", "B "],
    chart: {
      type: "donut",
      height: 115,
      fontFamily: "inherit",
    },
    dataLabels: {
      enabled: false,
    },
    stroke: {
      width: 0,
    },

    plotOptions: {
      pie: {
        expandOnClick: true,
        donut: {
          size: "60%",
          labels: {
            show: false,
            name: {
              show: true,
              offsetY: 7,
            },
            value: {
              show: false,
            },
            total: {
              show: false,
              color: "#a1aab2",
              fontSize: "13px",
              label: "Visits",
            },
          },
        },
      },
    },
    colors: ['rgba(var(--v-theme-secondary))', "#f2f4f8"],
    tooltip: {
      show: true,
      fillSeriesColor: false,
    },
    legend: {
      show: false,
    },
    responsive: [
      {
        breakpoint: 426,
        options: {
          chart: {
            offsetX: -35,
            width: 200,
          },
        },
      },
    ],
    }
});
</script>
<template>
    <v-card elevation="10">
        <v-card-text class="pb-7">
            <h3 class="card-title mb-1">Sales Difference</h3>
            <v-row class="mt-3">
                <v-col cols="7">
                    <span class="display-2 text-success">$4316</span>
                    <h6 class="text-subtitle-1 my-1 textSecondary font-weight-medium">10% Increased</h6>
                    <h5 class="text-16 font-weight-medium text-no-wrap mt-1">(150-165 Sales)</h5>
                </v-col>
                <v-col cols="5" class="d-flex">
                    <div class="ms-auto">
                        <apexchart type="donut" height="115" :options="chartOptions"
                            :series="chartOptions.series"></apexchart>
                    </div>
                </v-col>
            </v-row>
        </v-card-text>
    </v-card>
</template>

  
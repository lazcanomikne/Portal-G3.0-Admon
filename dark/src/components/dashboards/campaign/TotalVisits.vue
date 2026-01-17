<template>
    <VCard elevation="10" class="bigmap" style="    max-height: 475px;
    overflow: hidden;">
        <v-card-text class="position-relative">
            <div>
                <h3 class="card-title mb-1">Total Visits</h3>
            </div>
            <div id="svgMap" ref="map" class="mt-8"></div>
        </v-card-text>
    </VCard>
</template>
<script setup>
import svgMap from 'svgmap';
import 'svgmap/dist/svgMap.min.css';
import { onMounted, ref } from 'vue';

const map = ref(null);

onMounted(() => {
    new svgMap({
        targetElementID: 'svgMap',
        mouseWheelZoomEnabled: false,
        hideFlag: true,
        initialZoom: 1.23,

        data: {
            data: {
                countresidents: {
                    format: '{0}',
                    thousandSeparator: ',',
                    thresholdMax: 50000,
                    thresholdMin: 1000
                }
            },
            applyData: 'countresidents',
            values: {
                AF: { countresidents: 587, color: '#6352ce' },
                //AL: { gdp: 4583, change: 11.09 },
                IN: { countresidents: 232, color: '#0cb9c5' },
                US: { countresidents: 42393, color: '#1e88e5' }
            }
        }
    });
});
</script>

<style lang="scss" >
.bigmap .svgMap-map-wrapper {
    background: transparent;
    top: 62%;
    
}
.bigmap .svgMap-map-image{
    max-height: 372px;
}
</style>

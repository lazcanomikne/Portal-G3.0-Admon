<script setup lang="ts">

</script>
<template>
 <VCard elevation="10">
    <v-card-text>
        <h5 class="card-title">Highlights</h5>
        <div class="mt-6">
            <div class="d-flex align-center justify-space-between ">
                <h6 class="text-subtitle-1 font-weight-medium">Daily Sales</h6>
                <div class="d-flex ga-3">
                    <Icon icon="solar:arrow-right-up-linear" height="20" class="text-success" />
                    <h6 class="text-subtitle-1 font-weight-medium">Daily Sales</h6>
                </div>
            </div>
            <v-divider class="my-3" ></v-divider>
            <div class="d-flex align-center justify-space-between ">
                <h6 class="text-subtitle-1 font-weight-medium">Avg. Clients</h6>
                <div class="d-flex ga-3">
                    <Icon icon="solar:arrow-right-up-linear" height="20" class="text-error" />
                    <h6 class="text-subtitle-1 font-weight-medium">400</h6>
                </div>
            </div>
            <v-divider class="my-3" ></v-divider>
            <div class="d-flex align-center justify-space-between ">
                <h6 class="text-subtitle-1 font-weight-medium">Pending Tasks</h6>
                <div class="d-flex ga-3">
                    <Icon icon="solar:arrow-left-down-linear" height="20" class="text-success" />
                    <h6 class="text-subtitle-1 font-weight-medium">4.3 /37</h6>
                </div>
            </div>
        </div>
    </v-card-text>
 </VCard>
</template>
<template>
    <VCard elevation="10">
        <div class="pa-2">
            <div class="bg-lightprimary rounded-md py-6  px-5 d-flex justify-space-between align-center">
                <h5 class="text-h5">New Delhi</h5>
                <p class="text-15 textSecondary">Sunday 15 march</p>
            </div>
        </div>
        <v-card-text class="pt-3 pb-5">
            <div class="d-flex ga-8 align-center">
                <div class="d-flex ga-3 align-center">
                    <v-avatar class="bg-lightprimary" size="48">
                        <Icon icon="solar:cloud-sun-2-linear" class="text-primary" height="25" />
                    </v-avatar>
                    <div>
                        <h3 class="text-24 font-weight-medium text-primary">32 °C</h3>
                        <p class="text-15 textSecondary mt-n1">Sunny Rainy</p>
                    </div>
                </div>
                <div>
                    <h3 class="text-24 font-weight-medium">25 °C</h3>
                    <p class="text-15 textSecondary mt-n1">Tonight</p>
                </div>
            </div>
        </v-card-text>
    </VCard>
</template>

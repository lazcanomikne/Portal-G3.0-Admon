<script setup lang="ts">
import { CirclesRelationIcon, UserIcon } from 'vue-tabler-icons';
</script>
<template>
    <VCard elevation="0" class="bg-secondary-gradient mt-6 position-relative">
        <v-card-text>
            <p class="text-white text-15 mb-3">Grab the top deal.</p>
            <h3 class="text-h3 font-weight-semibold text-white mb-5">Upgrade Plan</h3>
            <div class="d-flex ga-5 mb-7">
                <div class="d-flex ga-2 align-center">
                    <v-avatar size="36" class="bg-white-opacity">
                        <UserIcon size="18" class="text-white" />
                    </v-avatar>
                    <div>
                        <small class="text-subtitle-2 text-white opacity-50 font-weight-regular">Team</small>
                        <div class="text-subtitle-2 text-white font-weight-semibold">Up to 240</div>
                    </div>
                </div>
                <div class="d-flex ga-2 align-center">
                    <v-avatar size="36" class="bg-white-opacity">
                        <CirclesRelationIcon size="18" class="text-white" />
                    </v-avatar>
                    <div>
                        <small class="text-subtitle-2 text-white opacity-50 font-weight-regular">Progress</small>
                        <div class="text-subtitle-2 text-white font-weight-semibold">Almost 85%</div>
                    </div>
                </div>
            </div>
            <v-btn class="bg-white-opacity text-white"  rounded="pill">Upgrade Plan</v-btn>
        </v-card-text>
        <img src="@/assets/images/backgrounds/make-social-media.png" class="position-absolute top-0 end-0 me-n5 mt-1 rtlImg d-md-block d-none" />
    </VCard>
</template>

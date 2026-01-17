<script setup lang="ts">
import img1 from '@/assets/images/backgrounds/material-pro-bg.png';
import { ref } from 'vue';
const dialog = ref(false);
</script>
<template>
    <VCard elevation="0" class="overflow-hidden position-relative">
        <v-img  aspect-ratio="16/9"  height="415" width="100%" class="rtlImg" cover :src=img1></v-img>  
        <div class="d-flex position-absolute top-0 mt-6 ms-6 ga-3 align-center">
            <v-btn rounded="circle" size="60"  @click="dialog = true">
                <Icon icon="solar:play-bold" height="18"/>
            </v-btn>
            <div>
                <h5 class="dark-card-title">Material Pro
                </h5>
                <p class="card-subtitle">The real story</p>
            </div>
        </div>  
    </VCard>

     <!---Modal Video--->
     <v-dialog v-model="dialog" max-width="800">
        <v-card>

            <XIcon size="30" class="position-absolute right-0  rounded-circle text-white pa-1 z-1 cursor-pointer" @click="dialog = false"/>

            <div class="lh-0">
                <iframe
                    width="100%"
                    height="500"
                    src="https://www.youtube.com/embed/QPbbqC-wIu8"
                    title="How to Get Started with our VueJs Admin Template? | Vue.Js Dashboard Template Installation Guide"
                    frameborder="0"
                    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                    referrerpolicy="strict-origin-when-cross-origin"
                    allowfullscreen
                ></iframe>
            </div>
        </v-card>
    </v-dialog>
</template>


<script setup lang="ts">
import { ref } from 'vue';
import { browserStatesData } from '@/_mockApis/components/dashboards/MinimalData';
</script>
<template>
    <v-card elevation="10">
        <v-card-text>
            <h3 class="card-title mb-6">Browser Stats</h3>
            <div class="d-flex align-center pb-3" v-for="item in browserStatesData" :key="item.name">
                <img :src="item.img" alt="logo" />
                <div class="ms-3 text-15 textSecondary">{{ item.name }}</div>
                <v-spacer></v-spacer>
                <div class="ml-auto">
                    <v-chip class="ma-2" rounded="pill" size="small" label :color="item.color">{{ item.visit }}</v-chip>
                </div>
            </div>
        </v-card-text>
    </v-card>
</template>

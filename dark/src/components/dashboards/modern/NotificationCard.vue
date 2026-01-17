<script setup lang="ts">
import { notifications } from '@/_mockApis/headerData';
</script>

<template>
    <VCard elevation="10" class="overflow-hidden">
        <v-card-text class="pb-4">
            <h3 class="card-title mb-1">Notification</h3>
        </v-card-text>
        <v-list class="py-0  pb-5" lines="two">
            <v-list-item v-for="item in notifications.slice(0,5)" :key="item.title" :value="item" color="primary" class="py-4 px-6 notification">
                <template v-slot:prepend>
                    <v-avatar size="40" :color="'light' + item.color" rounded="circle">
                        <Icon :icon="'solar:' + item.avatar" height="20" :class="'text-' + item.color" />
                    </v-avatar>
                </template>
                <div class="d-flex justify-space-between">
                    <h6 class="text-subtitle-1 textPrimary font-weight-medium">{{ item.title }}</h6>
                    <span class="text-12 textSecondary">{{ item.time }}</span>
                </div>
                <p class="text-subtitle-2 font-weight-regular textSecondary">{{ item.subtitle }}</p>
            </v-list-item>
        </v-list>
    </VCard>
</template>

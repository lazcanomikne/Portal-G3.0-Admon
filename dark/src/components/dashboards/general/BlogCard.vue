<script setup lang="ts">
import { ClockHour4Icon, MessageCircle2Icon, MessageCircleIcon } from 'vue-tabler-icons';
</script>

<template>
    <VCard elevation="10" class="overflow-hidden">
        <div class="pa-2">
            <img class="w-100 rounded-md" src="@/assets/images/backgrounds/blog-bg.jpg" />
        </div>

        <v-card-text class="pt-2">
            <v-chip color="primary">Technology</v-chip>
            <h5 class="my-2 text-h5">Business development new rules for 2024</h5>
            <p class="textSecondary text-15 two-line mb-3 lh-normal">
                Lorem ipsum dolor sit amet, this is a consectetur adipisicing elit, sed do eiusmod...
            </p>
            <div class="d-flex justify-space-between align-center">
                <v-btn color="primary" rounded="pill">Read More</v-btn>
                <div class="cursor-pointer">
                    <Icon icon="solar:share-outline" height="25" />
                    <v-tooltip activator="parent" location="top">Share</v-tooltip>
                </div>
            </div>
        </v-card-text>
    </VCard>
</template>

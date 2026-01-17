<script setup lang="ts">
import { ref } from 'vue';
import { recentCommentsData } from '@/_mockApis/components/dashboards/ClassicData';
const props = defineProps({
    comment: Object
});
</script>
<template>
    <VCard elevation="10">
        <v-card-text>
            <div class="d-flex justify-space-between align-center">
                <div>
                    <h2 class="card-title mb-1">Recent Comments</h2>
                    <h5 class="card-subtitle pe-3">Latest Comments on users from Material</h5>
                </div>
                <v-btn color="primary" flat rounded="pill"> View All </v-btn>
            </div>

            <div class="d-md-flex align-items-center mt-6">
                <div>
                    <div v-for="comment in recentCommentsData" :comment="comment" :key="comment.username" class="comment-box">
                        <!-- Comment Row -->
                        <div class="d-flex ga-4" :class="[comment.activeClass]">
                            <v-avatar size="44">
                                <img :src="comment.img" alt="user" width="44" />
                            </v-avatar>
                            <div class="comment-text w-100">
                                <h3 class="text-h6">{{ comment.username }}<br /></h3>
                                <div class="my-2 textSecondary text-14">{{ comment.desc }}</div>
                                <div class="d-flex">
                                    <div class="d-flex ga-3 align-center">
                                        <v-chip :color="comment.badgebg" size="small">{{ comment.status }}</v-chip>
                                        <div class="comment-action d-flex align-center ga-2">
                                            <v-btn rounded="circle" class="textPrimary hover-primary" variant="plain" size="22"><Icon icon="solar:pen-new-square-linear" height="18" /></v-btn>
                                            <v-btn rounded="circle" class="textPrimary hover-primary" variant="plain" size="22"><Icon icon="solar:check-circle-linear" height="18" /></v-btn>
                                            <v-btn rounded="circle" class="textPrimary hover-primary" variant="plain" size="22"><Icon icon="solar:heart-linear" height="18" /></v-btn>
                                        </div> 
                                    </div>
                                    <div class="textSecondary text-subtitle-1 ms-auto">{{ comment.date }}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </v-card-text>
    </VCard>
</template>

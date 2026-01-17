<script setup lang="ts">
import { ref } from 'vue';
import { myContactsData } from '@/_mockApis/components/dashboards/AnalyticalData';
function href() {
    return undefined;
}
</script>
<template>
    <VCard elevation="10" class="overflow-hidden">
        <v-card-text>
            <h2 class="card-title mb-1">My Contacts</h2>
            <h5 class="card-subtitle">Checkout my contacts here</h5>

            <div class="mt-4">
                <div v-for="(item, i) in myContactsData.slice(0,4)" :key="i" @click="href">
                    <div class="d-flex align-center py-4">
                        <div class="mr-3">
                            <v-badge bordered bottom :color="item.avatarstatus" dot offset-x="2" offset-y="2">
                                <v-avatar size="48">
                                    <img :src="item.avatar" :alt="item.title" width="48" />
                                </v-avatar>
                            </v-badge>
                        </div>
                        <div class="d-flex w-100 align-center justify-space-between">
                          <div>
                            <h4 class="text-h6 mt-n1 mb-1">{{ item.title }}</h4>
                            <div class="truncate-text text-subtitle-2 textSecondary">{{ item.desc }}</div>
                          </div>
                          <div class="d-flex align-center ms-sm-auto ga-1">
                            <v-btn density="comfortable" icon color="lighterror" size="35">
                              <Icon icon="solar:videocamera-outline"  class="text-error" height="20"></Icon>
                            </v-btn>
                            <v-btn density="comfortable" icon color="lightprimary" size="35">
                              <Icon icon="solar:incoming-call-rounded-linear"  class="text-primary" height="20"></Icon>
                            </v-btn>
                          </div>
                        </div>
                    </div>
                </div>
            </div>
        </v-card-text>
    </VCard>
</template>

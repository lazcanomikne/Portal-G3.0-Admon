<script setup lang="ts">
import { EarningData } from '@/_mockApis/components/dashboards/EcommerceData';
import { ref } from 'vue';
import { DotsVerticalIcon } from 'vue-tabler-icons';
const items = ref([{ title: 'Action' }, { title: 'Another action' }, { title: 'Something else here' }]);
</script>

<template>
    <VCard elevation="10">
        <div class="d-flex justify-space-between align-center py-3 px-6">
            <h3 class="card-title">Earning</h3>
            <v-menu bottom left >
                <template v-slot:activator="{ props }">
                    <v-btn icon color="inherit" v-bind="props" flat size="small">
                        <DotsVerticalIcon stroke-width="1.5" size="23" class="textSecondary" />
                    </v-btn>
                </template>
                <v-list density="compact"  elevation="10">
                    <v-list-item v-for="(item, i) in items" :key="i" :value="i">
                        <v-list-item-title>{{ item.title }}</v-list-item-title>
                    </v-list-item>
                </v-list>
            </v-menu>
        </div>
        <v-divider></v-divider>
        <v-card-text class="pb-0 pt-7">
            <div v-for="(item, i) in EarningData" :key="i">
                <div class="d-flex ga-4 align-center pb-6">
                    <div>
                        <v-badge bordered bottom :color="item.avatarstatus" dot offset-x="2" offset-y="2">
                            <v-avatar size="48">
                                <img :src="item.avatar" :alt="item.title" width="48" />
                            </v-avatar>
                        </v-badge>
                    </div>
                    <div class="d-flex w-100 align-center justify-space-between">
                        <div>
                            <h4 class="text-h6 mt-n1 mb-1">{{ item.title }}</h4>
                            <div class="truncate-text text-subtitle-1 textSecondary">{{ item.desc }}</div>
                        </div>
                        <div class="d-flex align-center ms-sm-auto ga-1">
                            <v-chip :color="item.color" size="small">{{ item.status }}</v-chip>
                        </div>
                    </div>
                </div>
            </div>
        </v-card-text>
    </VCard>
</template>

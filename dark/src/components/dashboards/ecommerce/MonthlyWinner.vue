<script setup lang="ts">
import { ref } from 'vue';
import { DotsVerticalIcon, ShoppingCartIcon } from 'vue-tabler-icons';
const items = ref([{ title: 'Action' }, { title: 'Another action' }, { title: 'Something else here' }]);
</script>

<template>
    <VCard elevation="10" class="overflow-hidden">
        <div class="d-flex justify-space-between align-center py-3 px-6">
            <h3 class="card-title">Monthly Winner</h3>
            <v-menu bottom left>
                <template v-slot:activator="{ props }">
                    <v-btn icon color="inherit" v-bind="props" flat size="small">
                        <DotsVerticalIcon stroke-width="1.5" size="23" class="textSecondary" />
                    </v-btn>
                </template>
                <v-list density="compact" elevation="10">
                    <v-list-item v-for="(item, i) in items" :key="i" :value="i">
                        <v-list-item-title>{{ item.title }}</v-list-item-title>
                    </v-list-item>
                </v-list>
            </v-menu>
        </div>
        <v-divider></v-divider>
        <div class="pa-2">
            <img src="@/assets/images/backgrounds/profile-bg.jpg" height="110" class="w-100 rounded-md" />
        </div>
        <div class="d-flex justify-center mt-n5">
            <v-avatar size="112" class="mt-n15">
                <img src="@/assets/images/profile/user-2.jpg" width="112" alt="user" />
            </v-avatar>
        </div>
        
        <v-card-text class="pt-3 px-6">
            <div class="text-center">
                <h2 class="mb-0 text-h3">Angela Dominic</h2>
                <div class="textSecondary text-subtitle-1">Web Designer &amp; Developer</div>
                <v-btn rounded color="primary" class="mt-3" flat>Follow</v-btn>
                <v-row class="mt-6 pt-2 border-t">
                    <v-col cols="4">
                        <h2 class="mb-0 text-h3">1099</h2>
                        <div class="text-subtitle-1 textSecondary">Articles</div>
                    </v-col>
                    <v-col cols="4">
                        <h2 class="mb-0 text-h3">23,469</h2>
                        <div class="text-subtitle-1 textSecondary">Followers</div>
                    </v-col>
                    <v-col cols="4">
                        <h2 class="mb-0 text-h3">6035</h2>
                        <div class="text-subtitle-1 textSecondary">Following</div>
                    </v-col>
                </v-row>
            </div>
        </v-card-text>
    </VCard>
</template>

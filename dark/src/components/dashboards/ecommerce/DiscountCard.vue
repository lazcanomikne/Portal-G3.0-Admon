<script setup lang="ts">

import { ref } from 'vue';
import { DotsVerticalIcon, ShoppingCartIcon } from 'vue-tabler-icons';
const items = ref([{ title: 'Action' }, { title: 'Another action' }, { title: 'Something else here' }]);
</script>

<template>
    <VCard elevation="10" class="overflow-hidden">
        <div class="d-flex justify-space-between align-center py-3 px-6">
            <h3 class="card-title">Discount</h3>
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
        <v-card-text class="bg-secondary">
            <ShoppingCartIcon size="20" class="text-white" />
            <p class="text-white text-15">25th Jan</p>
            <h3 class="text-h3 py-4 font-weight-regular text-white">Now Get 50% Off
                <br>on buy</h3>
                <p class="text-white text-15 font-italic">- Ecommerce site</p>
        </v-card-text>
    </VCard>
</template>

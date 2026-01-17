<script setup lang="ts">
import { ref } from 'vue';
import { productPerformanceData } from '@/_mockApis/components/dashboards/EcommerceData';
const select = ref('This Week');
const items = ref(['April 2024', 'May 2024', 'June 2024']);
</script>
<template>
    <VCard elevation="10">
        <v-card-text>
            <div class="d-flex justify-space-between align-center">
                <h3 class="card-title mb-1">Product Overview</h3>
                <div>
                    <v-select v-model="select" :items="items" variant="outlined" elevation="0" class="text-body-1" hide-details></v-select>
                </div>
            </div>
            <v-table class="month-table mt-4">
                <template v-slot:default>
                    <thead>
                        <tr>
                            <th class="text-subtitle-1 font-weight-medium text-15">Customer</th>
                            <th class="text-subtitle-1 font-weight-medium text-15">Photo</th>
                            <th class="text-subtitle-1 font-weight-medium text-15">Quantity</th>
                            <th class="text-subtitle-1 font-weight-medium text-15 ">Date</th>
                            <th class="text-subtitle-1 font-weight-medium text-15 ">Status</th>
                            <th class="text-subtitle-1 font-weight-medium text-15 ">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="item in productPerformanceData" :key="item.name" class="month-item-0">
                            <td>
                                <h6 class="text-subtitle-1 text-no-wrap font-weight-medium">
                                    {{ item.name }}
                                </h6>
                            </td>
                            <td>
                                <img :src="item.avatar" alt="user" width="80" />
                            </td>
                            <td>
                                <p class="text-15 textSecondary text-no-wrap">{{ item.qty }}</p>
                            </td>
                            <td>
                                <p class="text-15 textSecondary text-no-wrap">{{ item.date }}</p>
                            </td>
                            <td>
                                <v-chip class="ma-2" :class="'text-white bg-' + item.statuscolor" rounded="pill" size="small" label>{{
                                    item.status
                                }}</v-chip>
                            </td>
                            <td>
                                <div class="d-flex ga-2 text-no-wrap">
                                    <v-btn   color="lightprimary" size="30" rounded="circle">
                                        <Icon icon="solar:eye-broken" class="text-primary" height="18" />
                                    </v-btn>
                                    <v-btn   color="lighterror" size="30" rounded="circle">
                                        <Icon icon="solar:trash-bin-minimalistic-linear" class="text-error" height="18" />
                                    </v-btn>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </template>
            </v-table>
        </v-card-text>
    </VCard>

    
</template>

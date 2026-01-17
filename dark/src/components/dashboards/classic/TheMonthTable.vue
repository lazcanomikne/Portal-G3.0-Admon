<script setup lang="ts">
import { ref } from 'vue';
import { projectTableData } from '@/_mockApis/components/dashboards/ClassicData';
const select = ref('This Week');
const items = ref(['April 2024', 'May 2024', 'June 2024']);
</script>
<template>
    <VCard elevation="10">
        <v-card-text>
            <div class="d-flex justify-space-between align-center">
                <h3 class="card-title mb-1">Projects of the Month</h3>
                <div>
                    <v-select
                        v-model="select"
                        :items="items"
                        variant="outlined"
                        elevation="0"
                        class="text-body-1"
                        hide-details
                        flat
                    ></v-select>
                </div>
            </div>
            <v-table class="month-table mt-4 ">
                <template v-slot:default>
                    <thead>
                        <tr>
                            <th class="text-subtitle-1 font-weight-medium text-15 ps-0">Client</th>
                            <th class="text-subtitle-1 font-weight-medium text-15">Name</th>
                            <th class="text-subtitle-1 font-weight-medium text-15">Priority</th>
                            <th class="text-subtitle-1 font-weight-medium text-15 text-end">Budget</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="item in projectTableData" :key="item.leadname" :class="item.activestate" class="month-item-0">
                            <td class="ps-0">
                                <div class="d-flex align-center">
                                    <v-avatar size="48">
                                        <img :src="item.img" alt="user" width="48" />
                                    </v-avatar>
                                    <div class="mx-4">
                                        <h4 class="text-16 text-no-wrap font-weight-medium">
                                            {{ item.leadname }}
                                        </h4>
                                        <p class="text-subtitle-1  text-no-wrap textSecondary ">
                                            {{ item.leademail }}
                                        </p>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <h5 class="text-no-wrap text-subtitle-1 textSecondary">
                                    {{ item.projectname }}
                                </h5>
                            </td>
                            <td>
                                <v-chip class="ma-2" :color="item.statuscolor"
                            rounded="pill" size="small" label>{{ item.statustext }}</v-chip>
                            </td>
                            <td class="text-end">
                                <h4 class="text-no-wrap text-subtitle-1 textSecondary">{{ item.money }}</h4>
                            </td>
                        </tr>
                    </tbody>
                </template>
            </v-table>
        </v-card-text>
    </VCard>
</template>

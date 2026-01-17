<script setup lang="ts">
import { ref } from 'vue';

import BaseBreadcrumb from '@/components/shared/BaseBreadcrumb.vue';
import TheEarningCard from '@/components/dashboards/campaign/TheEarningCard.vue';
import TheBrowserStats from '@/components/dashboards/campaign/TheBrowserStats.vue';
import TheTotalRevenue from '@/components/dashboards/campaign/TheTotalRevenue.vue';
import TheSalesPrediction from '@/components/dashboards/campaign/TheSalesPrediction.vue';
import TheSalesDifference from '@/components/dashboards/campaign/TheSalesDifference.vue';
import TotalVisits from '@/components/dashboards/campaign/TotalVisits.vue';
import FullCalendar from '@/components/dashboards/campaign/FullCalendar.vue';
import RecentChats from '@/components/dashboards/campaign/RecentChats.vue';
import Contacts from '@/components/dashboards/campaign/Contacts.vue';

const page = ref({ title: 'Dashboard' });
const breadcrumbs = ref([
    {
        text: 'Home',
        disabled: false,
        href: '#'
    },
    {
        text: 'Dashboard',
        disabled: false,
    href: "#",
    }
]);
</script>

<template>
    <BaseBreadcrumb :title="page.title" :breadcrumbs="breadcrumbs"></BaseBreadcrumb>
    <v-row>
        <!---Earning card---->
        <v-col cols="12" lg="12">
            <TheEarningCard />
        </v-col>
        <!---Current visits---->
        <v-col cols="12" lg="8">
            <TotalVisits />
        </v-col>
        <!---Browser states---->
        <v-col cols="12" lg="4">
            <TheBrowserStats />
        </v-col>
        <!---Total revenue---->
        <v-col cols="12" lg="8">
            <TheTotalRevenue />
        </v-col>
        <v-col cols="12" lg="4">
            <TheSalesPrediction />
            <div class="mb-7"></div>
            <TheSalesDifference />
        </v-col>
        <v-col cols="12">
            <FullCalendar />
        </v-col>
        <v-col cols="12" lg="6">
            <RecentChats/>
        </v-col>
        <v-col cols="12" lg="6">
           <Contacts/>
        </v-col>
      
    </v-row>
</template>

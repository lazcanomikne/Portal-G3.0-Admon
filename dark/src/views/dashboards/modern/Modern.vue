<script setup lang="ts">
import { ref } from 'vue';
import BaseBreadcrumb from '@/components/shared/BaseBreadcrumb.vue';
import ProgressCards from '@/components/dashboards/modern/ProgressCards.vue';
import SalesChart from '@/components/dashboards/modern/SalesChart.vue';
import DownloadCount from '@/components/dashboards/modern/DownloadCount.vue';

import BigDownloadChart from '@/components/dashboards/modern/BigDownloadChart.vue';
import ProductAvaibility from '@/components/dashboards/modern/ProductAvaibility.vue';
import NotificationCard from '@/components/dashboards/modern/NotificationCard.vue';
import ReviewsCard from '@/components/dashboards/modern/ReviewsCard.vue';
import VisitSource from '@/components/dashboards/modern/VisitSource.vue';
import ExperienceCard from '@/components/dashboards/modern/ExperienceCard.vue';
import OverlayProfile from '@/components/dashboards/modern/OverlayProfile.vue';
import TheActivityTimeline from '@/components/dashboards/general/TheActivityTimeline.vue';
import ContactCard from '@/components/dashboards/modern/ContactCard.vue';
import BigBandwidthchart from '@/components/dashboards/modern/BigBandwidthChart.vue';

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
        <v-col cols="12">
            <ProgressCards />
        </v-col>
        <v-col cols="12">
            <SalesChart />
        </v-col>
        <v-col cols="12" lg="7">
            <DownloadCount />
        </v-col>
        <v-col cols="12" lg="5">
            <BigBandwidthchart />
            <BigDownloadChart class="mt-6" />
        </v-col>
        <v-col cols="12" lg="7">
            <ProductAvaibility />
        </v-col>
        <v-col cols="12" lg="5">
            <NotificationCard />
        </v-col>
        <v-col cols="12" lg="8">
            <ReviewsCard />
        </v-col>
        <v-col cols="12" lg="4">
            <VisitSource />
        </v-col>
        <v-col cols="12" lg="8">
            <ExperienceCard />
        </v-col>
        <v-col cols="12" lg="4">
            <OverlayProfile />
        </v-col>
        <v-col cols="12" lg="8">
            <TheActivityTimeline />
        </v-col>
        <v-col cols="12" lg="4">
            <ContactCard/>
        </v-col>
    </v-row>
</template>

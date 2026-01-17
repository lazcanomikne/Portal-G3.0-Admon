<script setup lang="ts">
import { ref } from 'vue';
import BaseBreadcrumb from '@/components/shared/BaseBreadcrumb.vue';
import ChartBox from '@/components/dashboards/analytical/ChartBox.vue';
import UpgradePlan from '@/components/dashboards/analytical/UpgradePlan.vue';
import VideoBox from '@/components/dashboards/analytical/VideoBox.vue';
import Highlights from '@/components/dashboards/analytical/Highlights.vue';
import Designmeet from '@/components/dashboards/analytical/Designmeet.vue';
import NewsletterCampaign from '@/components/dashboards/general/NewsletterCampaign.vue';
import MyContacts from '@/components/dashboards/analytical/MyContacts.vue';
import TheMonthTable from '@/components/dashboards/classic/TheMonthTable.vue';
import CurrentVisitors from '@/components/dashboards/analytical/CurrentVisitors.vue';
import TheBandwidthUsage from '@/components/dashboards/general/TheBandwidthUsage.vue';
import TheDownloadCount from '@/components/dashboards/general/TheDownloadCount.vue';
import OurVisitors from '@/components/dashboards/analytical/OurVisitors.vue';
import ProfileBox from '@/components/dashboards/analytical/ProfileBox.vue';


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
        <v-col col="12" md="6" lg="5">
            <ChartBox />
            <UpgradePlan />
        </v-col>

        <v-col cols="12" md="6" lg="4">
            <VideoBox />
        </v-col>
        <v-col cols="12" lg="3">
            <Highlights />
            <Designmeet class="mt-6" />
        </v-col>
        <v-col cols="12" lg="8">
            <NewsletterCampaign />
        </v-col>
        <v-col cols="12" lg="4">
            <MyContacts/>
        </v-col>
        <v-col cols="12" lg="4">
            <CurrentVisitors/>
        </v-col>
        <v-col cols="12" lg="8">
            <TheMonthTable/>
        </v-col>
        <v-col cols="12" lg="4">
            <OurVisitors/>
        </v-col>
        <v-col cols="12" lg="4">
            <TheBandwidthUsage/>
            <TheDownloadCount class="mt-6"/>
        </v-col>
        <v-col cols="12" lg="4">
            <ProfileBox/>
        </v-col>
    </v-row>
</template>

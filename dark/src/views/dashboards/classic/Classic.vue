<script setup lang="ts">
import { ref } from "vue";
import BaseBreadcrumb from "@/components/shared/BaseBreadcrumb.vue";
import TheRevenueCards from "@/components/dashboards/classic/TheRevenueCards.vue";
import TheBandwidthUsage from "@/components/dashboards/general/TheBandwidthUsage.vue";
import TheDownloadCount from "@/components/dashboards/general/TheDownloadCount.vue";
import TotalVisitors from "@/components/dashboards/classic/TotalVisitors.vue";
import CurrentVisits from "@/components/dashboards/classic/CurrentVisits.vue";
import BlogCard from "@/components/dashboards/general/BlogCard.vue";
import NewsletterCampaign from "@/components/dashboards/general/NewsletterCampaign.vue";
import TheMonthTable from "@/components/dashboards/classic/TheMonthTable.vue";
import TheProfileCard from '@/components/dashboards/classic/ProfileCard.vue';
import TheRecentComments from "@/components/dashboards/classic/TheRecentComments.vue";
import TheTodoList from "@/components/dashboards/classic/TheTodoList.vue";
const page = ref({ title: "Dashboard" });
const breadcrumbs = ref([
  {
    text: "Home",
    disabled: false,
    href: "#",
  },
  {
    text: "Dashboard",
    disabled: false,
    href: "#",
  },
]);
</script>

<template>
  <BaseBreadcrumb :title="page.title" :breadcrumbs="breadcrumbs"></BaseBreadcrumb>
  <v-row>
    <!---Revenue cards--->
    <v-col cols="12" sm="12" lg="12" class="mt-3">
      <TheRevenueCards />
    </v-col>
    <!---Bandwidth usage / Download count--->
    <v-col cols="12" sm="12" lg="4">
      <TheBandwidthUsage />
      <div class="mt-7"></div>
      <TheDownloadCount />
    </v-col>
    <!---Total visitors--->
    <v-col cols="12" sm="12" lg="4">
      <TotalVisitors/>
    </v-col>
    <!---Current visits-->
    <v-col cols="12" sm="12" lg="4">
      <CurrentVisits />
    </v-col>
    <!---Blog cards--->
    <v-col cols="12" sm="12" lg="4">
      <BlogCard />
    </v-col>
    <!---Newsletter Campaign--->
    <v-col cols="12" sm="12" lg="8">
      <NewsletterCampaign />
    </v-col>
    <!---Table--->
    <v-col cols="12" sm="12" lg="8">
      <TheMonthTable />
    </v-col>
    <!---User profile card--->
    <v-col cols="12" sm="12" lg="4">
      <TheProfileCard />
    </v-col>
    <!---Recent comments--->
    <v-col cols="12" sm="12" lg="6">
      <TheRecentComments />
    </v-col>
    <!---To do list--->
    <v-col cols="12" sm="12" lg="6">
      <TheTodoList />
    </v-col>
  </v-row>
</template>

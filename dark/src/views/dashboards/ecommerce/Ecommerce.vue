<script setup lang="ts">
import { ref } from 'vue';
import BaseBreadcrumb from '@/components/shared/BaseBreadcrumb.vue';
import SalesProductsCard from '@/components/dashboards/ecommerce/SalesProductsCard.vue';
import TheSalesOverview from '@/components/dashboards/general/TheSalesOverview.vue';
import ProductSales from '@/components/dashboards/ecommerce/ProductSales.vue';
import DownloadCountCard from '@/components/dashboards/ecommerce/DownloadCountCard.vue';
import TheWeatherCardVue from '@/components/dashboards/general/TheWeatherCardVue.vue';
import ProductOverview from '@/components/dashboards/ecommerce/ProductOverview.vue';
import EarningCards from '@/components/dashboards/ecommerce/EarningCards.vue';
import DiscountCard from '@/components/dashboards/ecommerce/DiscountCard.vue';
import MonthlyWinner from '@/components/dashboards/ecommerce/MonthlyWinner.vue';
import NewItemsCard from '@/components/dashboards/ecommerce/NewItemsCard.vue';

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
        <v-col cols="12" lg="8">
            <v-row>
                <SalesProductsCard />
                <v-col cols="12">
                    <TheSalesOverview />
                </v-col>
                <v-col cols="12" lg="6">
                    <ProductSales />
                </v-col>
                <v-col cols="12" lg="6">
                    <DownloadCountCard />
                    <TheWeatherCardVue class="mt-6" />
                </v-col>
                <v-col cols="12">
                    <ProductOverview />
                </v-col>
            </v-row>
        </v-col>
        <v-col cols="12" lg="4">
            <EarningCards />
            <DiscountCard class="mt-6" />
            <MonthlyWinner class="mt-6" />
            <NewItemsCard class="mt-6" />
        </v-col>
    </v-row>
</template>

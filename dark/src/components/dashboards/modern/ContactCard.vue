<script setup lang="ts">
import { BrandFacebookIcon, BrandTwitterIcon, BrandYoutubeIcon, PhotoIcon, UsersIcon } from 'vue-tabler-icons';
</script>

<template>
    <VCard elevation="10" class="overflow-hidden">
        <v-card-text class="pb-0">
            <div class="text-center">
                <v-avatar size="150">
                    <img src="@/assets/images/profile/user-5.jpg" width="150" alt="user" />
                </v-avatar>
                <h5 class="text-h5 mt-3">Hanna Gover</h5>
                <p class="textPrimary font-weight-medium text-15 mt-1 mb-3">Account Manager Amix corp</p>
                <div class="d-flex align-center justify-space-around px-md-8">
                    <RouterLink to="" class="d-flex ga-2 align-center text-15 text-decoration-none hover-primary textPrimary">
                        <UsersIcon size="15" />
                        254
                    </RouterLink>
                    <RouterLink to="" class="d-flex ga-2 align-center text-15 text-decoration-none hover-primary textPrimary">
                        <PhotoIcon size="15" />
                        54
                    </RouterLink>
                </div>
            </div>
        </v-card-text>
        <v-divider class="my-12"></v-divider>
        <div class="pa-6 pt-0">
            <div>
                <p class="text-13 textSecondary">Email address</p>
                <h6 class="text-subtitle-1 font-weight-medium">hannagover@gmail.com</h6>
            </div>
            <div class="mt-6">
                <p class="text-13 textSecondary">Phone</p>
                <h6 class="text-subtitle-1 font-weight-medium">+91 654 784 547</h6>
            </div>
            <div class="mt-6">
                <p class="text-13 textSecondary">Address</p>
                <h6 class="text-subtitle-1 font-weight-medium">71 Pilgrim Avenue Chevy Chase, MD 20815</h6>
            </div>
            <div class="mt-6">
                <iframe
                    src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d470029.1604841957!2d72.29955005258641!3d23.019996818380896!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x395e848aba5bd449%3A0x4fcedd11614f6516!2sAhmedabad%2C+Gujarat!5e0!3m2!1sen!2sin!4v1493204785508"
                    class="w-100"
                    height="170"
                    style="border: 0"
                   
                ></iframe>
            </div>
            <div class="mt-6">
                <p class="text-13 textSecondary mb-2">Social Profile</p>
                <div class="d-flex ga-2">
                    <v-btn icon flat color="primary" size="40" rounded="circle">
                        <BrandFacebookIcon size="18" class="text-white" />
                    </v-btn>
                    <v-btn icon flat color="primary" size="40" rounded="circle">
                        <BrandTwitterIcon size="18" class="text-white" />
                    </v-btn>
                    <v-btn icon flat color="primary" size="40" rounded="circle">
                        <BrandYoutubeIcon size="18" class="text-white" />
                    </v-btn>
                </div>
            </div>
        </div>
    </VCard>
</template>

<template>
    <VCard elevation="10" class="bg-purple">
        <v-card-text class="pa-5">
            <h2 class="text-h5 text-white font-weight-medium">Design Meetings</h2>
            <p class="text-14 text-white">2 Hours Left</p>
            <div class="mt-13">
                <div class="d-flex justify-space-between align-center">
                    <div class="d-flex justify-content-start">
                        <div class="ms-2 d-flex">
                            <v-avatar size="40" class="ms-n2 border border-md border-purple">
                                <img src="@/assets/images/profile/user-2.jpg" alt="usericon" height="40" />
                            </v-avatar>
                            <v-avatar size="40" class="ms-n2 border border-md border-purple">
                                <img src="@/assets/images/profile/user-9.jpg" alt="usericon" height="40" />
                            </v-avatar>
                            <v-avatar size="40" class="ms-n2 border border-md border-purple text-white" color="textPrimary"> +54 </v-avatar>
                        </div>
                    </div>
                    <v-avatar size="40" color="warning">
                        <Icon icon="solar:arrow-right-up-linear" height="20" />
                    </v-avatar>
                </div>
            </div>
        </v-card-text>
    </VCard>
</template>
